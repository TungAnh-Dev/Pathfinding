namespace Pathfinding.Scripts
{
    using UnityEngine;

    public class GridGenerator
    {
        public Graph Graph { get; private set; }
        public Node[,] NodeGrid { get; private set; }

        public void Generate(int width, int height, float cellSize, float obstaclePercent, int seed, bool allowDiagonal, Vector3 origin)
        {
            Graph = new Graph();
            NodeGrid = new Node[width, height];

            if (seed != 0) Random.InitState(seed);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 worldPos = origin + new Vector3(x * cellSize, 0, z * cellSize);
                    bool isWalkable = Random.value >= obstaclePercent;
                    
                    Node newNode = new Node(worldPos, isWalkable);
                    NodeGrid[x, z] = newNode;
                    Graph.AllNodes.Add(newNode);
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Node current = NodeGrid[x, z];
                    if (!current.isWalkable) continue;

                    if (x > 0) TryConnect(current, NodeGrid[x - 1, z]);
                    if (z > 0) TryConnect(current, NodeGrid[x, z - 1]);

                    if (allowDiagonal)
                    {
                        if (x > 0 && z > 0) TryConnect(current, NodeGrid[x - 1, z - 1]);
                        if (x > 0 && z < height - 1) TryConnect(current, NodeGrid[x - 1, z + 1]);
                    }
                }
            }
        }

        private void TryConnect(Node a, Node b)
        {
            if (a.isWalkable && b.isWalkable) Graph.ConnectNodes(a, b);
        }

        public Node GetNodeFromWorldPos(Vector3 worldPos, Vector3 origin, float cellSize, int width, int height)
        {
            Vector3 localPos = worldPos - origin;
            int x = Mathf.RoundToInt(localPos.x / cellSize);
            int z = Mathf.RoundToInt(localPos.z / cellSize);

            if (x >= 0 && x < width && z >= 0 && z < height) return NodeGrid[x, z];
            return null;
        }
    }
}