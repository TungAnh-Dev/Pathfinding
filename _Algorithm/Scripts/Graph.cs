using System.Collections.Generic;

namespace _DijkstraAlgorithm.Scripts
{
    public class Graph
    {
        public List<Node> AllNodes;

        public Graph()
        {
            AllNodes = new List<Node>();
        }

        public void ConnectNodes(Node a, Node b)
        {
            if (!a.Neighbors.Contains(b)) a.Neighbors.Add(b);
            if (!b.Neighbors.Contains(a)) b.Neighbors.Add(a);
        }
        
        public void DisconnectNodes(Node a, Node b)
        {
            if (a.Neighbors.Contains(b)) a.Neighbors.Remove(b);
            if (b.Neighbors.Contains(a)) b.Neighbors.Remove(a);
        }

        public void Clear()
        {
            AllNodes.Clear();
        }

        public void TryConnectNodes(Node a, Node b)
        {
            if (a.isWalkable && b.isWalkable)
            {
                ConnectNodes(a, b);
            }
        }
    }
}