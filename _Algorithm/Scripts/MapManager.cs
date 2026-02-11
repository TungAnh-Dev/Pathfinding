using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace _DijkstraAlgorithm.Scripts
{
    public class MapManager : MonoBehaviour
    {
        [Header("Grid Configuration")]
        public Vector3 originPos = Vector3.zero;
        public int width = 10;
        public int height = 10;
        public float cellSize = 1.1f;
        public bool allowDiagonal = true;

        [Header("Obstacles Generation")]
        [Range(0, 1)] public float obstaclePercent = 0.2f; 
        public int seed = 0; 

        [Header("Visual Settings")]
        public GameObject nodePrefab;
        public Transform mapContainer;
        [Range(0.01f, 1f)] public float stepDelay = 0.05f;

        [Header("Debug")]
        public bool showGizmos = true;

        // --- Core ---
        private IPathfinder _pathfinder;
        private Graph _graph;
        private Node[,] _nodeGrid;
        
        // --- Mapping ---
        private Dictionary<Node, NodeObj> _nodeToVisualMap = new Dictionary<Node, NodeObj>();
        private List<GameObject> _spawnedTiles = new List<GameObject>();

        // --- State ---
        private Node _startNode;
        private Node _endNode;
        private Coroutine _runningRoutine;

        void Start()
        {
            _pathfinder = new DijkstraPathfinder();
            GenerateMap();
        }

        void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ResetAllVisuals();
                _startNode = null; _endNode = null;
                if (_runningRoutine != null) StopCoroutine(_runningRoutine);
                return;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float gridY = transform.position.y + originPos.y;
                Plane gridPlane = new Plane(Vector3.up, new Vector3(0, gridY, 0));

                if (gridPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    Node clickedNode = GetNodeFromWorldPos(hitPoint);

                    if (clickedNode != null && clickedNode.isWalkable)
                    {
                        if (Input.GetMouseButtonDown(1)) SetStartNode(clickedNode);
                        else if (Input.GetMouseButtonDown(0)) SetEndNodeAndRun(clickedNode);
                    }
                }
            }
        }


        private void ResetAllVisuals()
        {
            foreach (var kvp in _nodeToVisualMap)
            {
                kvp.Value.ResetNode(kvp.Key.isWalkable);
            }
        }

        [ContextMenu("Generate Map")]
        public void GenerateMap()
        {
            ClearMap();

            _graph = new Graph();
            _nodeGrid = new Node[width, height];
            if (_spawnedTiles == null) _spawnedTiles = new List<GameObject>();

            Vector3 startPos = transform.position + originPos;
            Transform parent = mapContainer != null ? mapContainer : transform;

            if (seed != 0) Random.InitState(seed);

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 worldPos = startPos + new Vector3(x * cellSize, 0, z * cellSize);
                    
                    bool isWalkable = true;
                    if (Random.value < obstaclePercent)
                    {
                        isWalkable = false;
                    }

                    Node newNode = new Node(worldPos, isWalkable);
                    _nodeGrid[x, z] = newNode;
                    _graph.AllNodes.Add(newNode);

                    if (nodePrefab != null)
                    {
                        GameObject obj = Instantiate(nodePrefab, worldPos, Quaternion.identity, parent);
                        obj.name = $"Node_{x}_{z}";
                        _spawnedTiles.Add(obj);

                        NodeObj nodeVisual = obj.GetComponent<NodeObj>();
                        if (nodeVisual != null) 
                        {
                            _nodeToVisualMap.Add(newNode, nodeVisual);
                            nodeVisual.ResetNode(isWalkable);
                        }
                    }
                }
            }

            // Connect nodes
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Node currentNode = _nodeGrid[x, z];
                    
                    if (!currentNode.isWalkable) continue;

                    // Row - Col
                    if (x > 0) _graph.TryConnectNodes(currentNode, _nodeGrid[x - 1, z]);
                    if (z > 0) _graph.TryConnectNodes(currentNode, _nodeGrid[x, z - 1]);

                    // Diagonal
                    if (allowDiagonal)
                    {
                        if (x > 0 && z > 0) 
                            _graph.TryConnectNodes(currentNode, _nodeGrid[x - 1, z - 1]);
                        if (x > 0 && z < height - 1) 
                            _graph.TryConnectNodes(currentNode, _nodeGrid[x - 1, z + 1]);
                    }
                }
            }
        }
        
        private void SetStartNode(Node node)
        {
            ResetAllVisuals();
            if (_runningRoutine != null) StopCoroutine(_runningRoutine);
            _startNode = node;
            _endNode = null;
            if (_nodeToVisualMap.ContainsKey(_startNode)) _nodeToVisualMap[_startNode].SetAsStart();
        }

        private void SetEndNodeAndRun(Node node)
        {
            if (_startNode == null) return;
            _endNode = node;
            if (_nodeToVisualMap.ContainsKey(_endNode)) _nodeToVisualMap[_endNode].SetAsEnd();
            if (_runningRoutine != null) StopCoroutine(_runningRoutine);
            
            _runningRoutine = StartCoroutine(_pathfinder.FindPathStepByStep(
                _startNode, _endNode, stepDelay,
                n => { if (_nodeToVisualMap.TryGetValue(n, out var v)) v.SetAsProcessing(); },
                n => { if (_nodeToVisualMap.TryGetValue(n, out var v)) v.SetAsFrontier(); },
                path => { 
                    if (path != null) 
                        foreach (var n in path) 
                            if (n != _startNode && n != _endNode && _nodeToVisualMap.TryGetValue(n, out var v)) 
                                v.SetAsPath(); 
                }
            ));
        }
        
        [ContextMenu("Clear Map")]
        public void ClearMap()
        {
            foreach (var obj in _spawnedTiles)
            {
                if (obj)
                {
                    if (Application.isPlaying) Destroy(obj);
                    else DestroyImmediate(obj);
                }
            }
            _spawnedTiles.Clear();
            _nodeToVisualMap.Clear();
            _nodeGrid = null;
        }

        private Node GetNodeFromWorldPos(Vector3 worldPos)
        {
            Vector3 localPos = worldPos - (transform.position + originPos);
            int x = Mathf.RoundToInt(localPos.x / cellSize);
            int z = Mathf.RoundToInt(localPos.z / cellSize);

            if (x >= 0 && x < width && z >= 0 && z < height)
                return _nodeGrid[x, z];
            return null;
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            Gizmos.color = Color.cyan;
            Vector3 startPos = transform.position + originPos;
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