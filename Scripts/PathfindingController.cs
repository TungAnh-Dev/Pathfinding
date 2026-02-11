namespace Pathfinding.Scripts
{
    using UnityEngine;

    [RequireComponent(typeof(GridVisualizer))]
    [RequireComponent(typeof(GridInput))] 
    public class PathfindingController : MonoBehaviour
    {
        [Header("Generator Module")]
        public GridGenerator generator;
        [Header("Grid Config")]
        public int width = 15;
        public int height = 15;
        public float cellSize = 1.2f;
        public Vector3 originOffset;
        public bool allowDiagonal = true;

        [Header("Generation")]
        [Range(0, 1)] public float obstaclePercent = 0.25f;
        public int seed = 0;

        [Header("Algorithm")]
        public float stepDelay = 0.05f;
        private GridVisualizer gridVisualizer;
        private GridInput input;
        private IPathfinder pathfinder;

        private Node startNode, endNode;
        private Coroutine pathRoutine;

        void Awake()
        {
            gridVisualizer = GetComponent<GridVisualizer>();
            input = GetComponent<GridInput>();
            pathfinder = new DFSPathfinder();
        }

        void Start()
        {
            input.OnLeftClick += HandleLeftClick;
            input.OnRightClick += HandleRightClick;
            input.OnClearInput += ResetPath;
            
            input.gridHeight = transform.position.y + originOffset.y;

            Generate();
        }
        
        void OnDestroy()
        {
            if (input != null)
            {
                input.OnLeftClick -= HandleLeftClick;
                input.OnRightClick -= HandleRightClick;
                input.OnClearInput -= ResetPath;
            }
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            generator.Generate();
            gridVisualizer.Visualize(generator.Graph);
        }


        private void HandleLeftClick(Vector3 worldPos)
        {
            Node n = generator.GetNodeFromWorldPos(worldPos);

            if (IsValidNode(n))
            {
                SetEndAndRun(n);
            }
        }

        private void HandleRightClick(Vector3 worldPos)
        {
            Node n = generator.GetNodeFromWorldPos(worldPos);
            
            if (IsValidNode(n))
            {
                SetStart(n);
            }
        }

        private bool IsValidNode(Node n)
        {
            return n != null && n.isWalkable;
        }

        
        private void SetStart(Node n) 
        { 
            ResetPath(); 
            startNode = n; 
            gridVisualizer.UpdateNode(n, v => v.SetAsStart()); 
        }

        private void SetEndAndRun(Node n)
        {
            if (startNode == null) return;
            if (pathRoutine != null) StopCoroutine(pathRoutine);
            
            gridVisualizer.ResetColors();
            gridVisualizer.UpdateNode(startNode, nodeObj => nodeObj.SetAsStart()); 
            
            endNode = n;
            gridVisualizer.UpdateNode(n, nodeObj => nodeObj.SetAsEnd());

            pathRoutine = StartCoroutine(pathfinder.FindPathStepByStep(startNode, endNode, stepDelay,
                onProcessing: node => gridVisualizer.UpdateNode(node, nodeObj => nodeObj.SetAsProcessing()),
                onFrontier: node => gridVisualizer.UpdateNode(node, nodeObj => nodeObj.SetAsFrontier()),
                onFinished: path => {
                    if (path != null) 
                        foreach (var node in path) 
                            if (node != startNode && node != endNode) 
                                gridVisualizer.UpdateNode(node, nodeObj => nodeObj.SetAsPath());
                }));
        }

        private void ResetPath() 
        { 
            if (pathRoutine != null) StopCoroutine(pathRoutine); 
            startNode = endNode = null; 
            gridVisualizer.ResetColors(); 
        }
    }
}