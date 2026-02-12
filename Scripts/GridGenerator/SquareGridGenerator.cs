namespace Pathfinding.Scripts
{
    using UnityEngine;

    public class SquareGridGenerator : GridGenerator
    {
        [Header("Square Specific")]
        public bool allowDiagonal = true; 

        protected override void OnGenerateGrid()
        {
            Vector3 startPos = transform.position + originOffset;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 pos = startPos + new Vector3(x * cellSize, 0, z * cellSize);
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
                    Node current = nodeGrid[x, z];
                    if (!current.isWalkable) continue;

                    if (x > 0) TryConnect(current, nodeGrid[x - 1, z]);
                    if (z > 0) TryConnect(current, nodeGrid[x, z - 1]);

                    if (allowDiagonal)
                    {
                        if (x > 0 && z > 0) TryConnect(current, nodeGrid[x - 1, z - 1]);
                        if (x > 0 && z < height - 1) TryConnect(current, nodeGrid[x - 1, z + 1]);
                    }
                }
            }
        }

        public override Node GetNodeFromWorldPos(Vector3 worldPos)
        {
            Vector3 local = worldPos - (transform.position + originOffset);
            int x = Mathf.RoundToInt(local.x / cellSize);
            int z = Mathf.RoundToInt(local.z / cellSize);

            if (x >= 0 && x < width && z >= 0 && z < height) return nodeGrid[x, z];
            return null;
        }

        private void TryConnect(Node a, Node b)
        {
            if (a.isWalkable && b.isWalkable) Graph.ConnectNodes(a, b);
        }

        protected override void OnDrawGizmos()
        {
            if (!showGrid) return;
            Gizmos.color = Color.cyan;
            Vector3 startPos = transform.position + originOffset;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 pos = startPos + new Vector3(x * cellSize, 0, z * cellSize);
                    Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.1f, cellSize));
                }
            }
        }
    }
}