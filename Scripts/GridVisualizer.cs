namespace Pathfinding.Scripts
{
    using UnityEngine;
    using System.Collections.Generic;

    public class GridVisualizer : MonoBehaviour
    {
        public GameObject nodePrefab;
        public Transform container;

        private Dictionary<Node, NodeObj> visualMap = new Dictionary<Node, NodeObj>();
        private List<GameObject> spawned = new List<GameObject>();

        public void Visualize(Graph graph)
        {
            Clear();
            foreach (var node in graph.AllNodes)
            {
                GameObject obj = Instantiate(nodePrefab, node.Position, Quaternion.identity, container);
                spawned.Add(obj);
                
                NodeObj visual = obj.GetComponent<NodeObj>();
                if (visual != null)
                {
                    visualMap.Add(node, visual);
                    visual.ResetNode(node.isWalkable);
                }
            }
        }

        public void Clear()
        {
            foreach (var o in spawned) if (o) DestroyImmediate(o);
            spawned.Clear();
            visualMap.Clear();
        }

        public void ResetColors() { foreach (var kvp in visualMap) kvp.Value.ResetNode(kvp.Key.isWalkable); }
        public void UpdateNode(Node n, System.Action<NodeObj> action) { if (n != null && visualMap.TryGetValue(n, out var v)) action(v); }
    }
}