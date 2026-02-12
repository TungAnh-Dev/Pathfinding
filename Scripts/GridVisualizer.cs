namespace Pathfinding.Scripts
{
    using UnityEngine;
    using System.Collections.Generic;

    public class GridVisualizer : MonoBehaviour
    {
        public GameObject nodePrefab;
        public Transform container;

        private Dictionary<Node, NodeObj> _visualMap = new Dictionary<Node, NodeObj>();
        private List<GameObject> _spawned = new List<GameObject>();

        public void Visualize(Graph graph)
        {
            Clear();
            foreach (var node in graph.AllNodes)
            {
                GameObject obj = Instantiate(nodePrefab, node.Position, Quaternion.identity, container);
                _spawned.Add(obj);
                
                NodeObj visual = obj.GetComponent<NodeObj>();
                if (visual != null)
                {
                    _visualMap.Add(node, visual);
                    visual.ResetNode(node.isWalkable);
                }
            }
        }

        public void Clear()
        {
            foreach (var o in _spawned) if (o) DestroyImmediate(o);
            _spawned.Clear();
            _visualMap.Clear();
        }

        public void ResetColors() { foreach (var kvp in _visualMap) kvp.Value.ResetNode(kvp.Key.isWalkable); }
        public void UpdateNode(Node n, System.Action<NodeObj> action) { if (n != null && _visualMap.TryGetValue(n, out var v)) action(v); }
    }
}