namespace Pathfinding.Scripts
{
    using UnityEngine;

    public abstract class GridGenerator : MonoBehaviour
    {
        [Header("Common Config")]
        public int width = 15;
        public int height = 15;
        public float cellSize = 1.2f;
        public Vector3 originOffset = Vector3.zero;

        [Header("Obstacles")]
        [Range(0, 1)] public float obstaclePercent = 0.2f;
        public int seed = 0;

        [Header("Debug")]
        public bool showGrid = true;

        public Graph Graph { get; protected set; }
        
        protected Node[,] nodeGrid;

        public void Generate()
        {
            if (seed != 0) Random.InitState(seed);
            
            Graph = new Graph();
            nodeGrid = new Node[width, height];

            OnGenerateGrid(); 
        }

        protected abstract void OnGenerateGrid();

        public abstract Node GetNodeFromWorldPos(Vector3 worldPos);

        protected abstract void OnDrawGizmos();
    }
}