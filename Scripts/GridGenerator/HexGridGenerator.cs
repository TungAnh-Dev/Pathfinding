namespace Pathfinding.Scripts
{
    using UnityEngine;

    public class HexGridGenerator : GridGenerator
    {
        protected override void OnGenerateGrid()
        {
            Vector3 startPos = transform.position + originOffset;
            float xOffset = cellSize;
            float zOffset = cellSize * 0.866f; 

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    float xPos = x * xOffset;
                    if (z % 2 != 0) xPos += xOffset * 0.5f; 
                    
                    Vector3 pos = startPos + new Vector3(xPos, 0, z * zOffset);
                    bool walkable = Random.value >= obstaclePercent;

                    Node node = new Node(pos, walkable);
                    nodeGrid[x, z] = node;
                    Graph.AllNodes.Add(node);
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    if (!nodeGrid[x, z].isWalkable) continue;
                    ConnectHexNeighbors(x, z);
                }
            }
        }

        public override Node GetNodeFromWorldPos(Vector3 worldPos)
        {
            Node bestNode = null;
            float minSqrDist = cellSize * cellSize * 0.5f;

            foreach (var node in Graph.AllNodes)
            {
                float sqrDist = (node.Position - worldPos).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    bestNode = node;
                }
            }
            return bestNode;
        }

        private void ConnectHexNeighbors(int x, int z)
        {
            int[,] offsets = (z % 2 == 0) 
                ? new int[,] { {-1,0}, {1,0}, {-1,-1}, {0,-1}, {-1,1}, {0,1} }
                : new int[,] { {-1,0}, {1,0}, {0,-1}, {1,-1}, {0,1}, {1,1} };

            for (int i = 0; i < 6; i++)
            {
                int nx = x + offsets[i, 0];
                int nz = z + offsets[i, 1];

                if (nx >= 0 && nx < width && nz >= 0 && nz < height)
                {
                    Node neighbor = nodeGrid[nx, nz];
                    if (neighbor.isWalkable) Graph.ConnectNodes(nodeGrid[x, z], neighbor);
                }
            }
        }

        protected override void OnDrawGizmos()
        {
            if (!showGrid) return;

            Gizmos.color = Color.red;

            Vector3 startPos = transform.position + originOffset;
            
            float xOffset = cellSize;
            float zOffset = cellSize * 0.866f; 

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    float xPos = x * xOffset;
                    if (z % 2 != 0) xPos += xOffset * 0.5f; 
                    
                    Vector3 pos = startPos + new Vector3(xPos, 0, z * zOffset);

                    Gizmos.DrawWireSphere(pos, cellSize * 0.2f);
                }
            }
        }
    }
}