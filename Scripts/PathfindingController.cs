namespace Pathfinding.Scripts
{
    using UnityEngine;
    using _DijkstraAlgorithm.Scripts;

    [RequireComponent(typeof(GridVisualizer))]
    [RequireComponent(typeof(GridInput))] 
    public class PathfindingController : MonoBehaviour
    {
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

        // Modules
        private GridGenerator _generator = new GridGenerator();
        private GridVisualizer _visualizer;
        private GridInput _input;
        private IPathfinder _pathfinder;

        private Node _startNode, _endNode;
        private Coroutine _pathRoutine;

        void Awake()
        {
            _visualizer = GetComponent<GridVisualizer>();
            _input = GetComponent<GridInput>();
            _pathfinder = new DijkstraPathfinder();
        }

        void Start()
        {
            _input.OnLeftClick += HandleLeftClick;
            _input.OnRightClick += HandleRightClick;
            _input.OnClearInput += ResetPath;
            
            _input.gridHeight = transform.position.y + originOffset.y;

            Generate();
        }
        
        void OnDestroy()
        {
            if (_input != null)
            {
                _input.OnLeftClick -= HandleLeftClick;
                _input.OnRightClick -= HandleRightClick;
                _input.OnClearInput -= ResetPath;
            }
        }

        [ContextMenu("Generate")]
        public void Generate()
        {
            _generator.Generate(width, height, cellSize, obstaclePercent, seed, allowDiagonal, transform.position + originOffset);
            _visualizer.Visualize(_generator.Graph);
        }


        private void HandleLeftClick(Vector3 worldPos)
        {
            Node n = _generator.GetNodeFromWorldPos(worldPos, transform.position + originOffset, cellSize, width, height);
            
            if (IsValidNode(n))
            {
                SetEndAndRun(n);
            }
        }

        private void HandleRightClick(Vector3 worldPos)
        {
            Node n = _generator.GetNodeFromWorldPos(worldPos, transform.position + originOffset, cellSize, width, height);
            
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
            _startNode = n; 
            _visualizer.UpdateNode(n, v => v.SetAsStart()); 
        }

        private void SetEndAndRun(Node n)
        {
            if (_startNode == null) return;
            if (_pathRoutine != null) StopCoroutine(_pathRoutine);
            
            _visualizer.ResetColors();
            _visualizer.UpdateNode(_startNode, v => v.SetAsStart()); 
            
            _endNode = n;
            _visualizer.UpdateNode(n, v => v.SetAsEnd());

            _pathRoutine = StartCoroutine(_pathfinder.FindPathStepByStep(_startNode, _endNode, stepDelay,
                onProcessing: node => _visualizer.UpdateNode(node, v => v.SetAsProcessing()),
                onFrontier: node => _visualizer.UpdateNode(node, v => v.SetAsFrontier()),
                onFinished: path => {
                    if (path != null) 
                        foreach (var node in path) 
                            if (node != _startNode && node != _endNode) 
                                _visualizer.UpdateNode(node, v => v.SetAsPath());
                }));
        }

        private void ResetPath() 
        { 
            if (_pathRoutine != null) StopCoroutine(_pathRoutine); 
            _startNode = _endNode = null; 
            _visualizer.ResetColors(); 
        }
    }
}