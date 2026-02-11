namespace Pathfinding.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class BFSPathfinder : IPathfinder
    {
        private Queue<Node> _frontier = new Queue<Node>();
        private Dictionary<Node, Node> _cameFrom = new Dictionary<Node, Node>();
        private List<Node> _pathCache = new List<Node>();

        public List<Node> FindPath(Node start, Node end)
        {
            if (start == null || end == null) return null;
            
            _frontier.Clear();
            _cameFrom.Clear();

            _frontier.Enqueue(start);
            _cameFrom[start] = null;

            while (_frontier.Count > 0)
            {
                Node current = _frontier.Dequeue();

                if (current == end) break;

                foreach (Node next in current.Neighbors)
                {
                    if (!_cameFrom.ContainsKey(next))
                    {
                        _cameFrom[next] = current;
                        _frontier.Enqueue(next);
                    }
                }
            }
            return ReconstructPath(start, end);
        }

        public IEnumerator FindPathStepByStep(Node start, Node end, float delay, 
            Action<Node> onProcessing, Action<Node> onFrontier, Action<List<Node>> onFinished)
        {
            if (start == null || end == null) yield break;

            _frontier.Clear();
            _cameFrom.Clear();

            _frontier.Enqueue(start);
            _cameFrom[start] = null;

            var wait = new WaitForSeconds(delay);

            while (_frontier.Count > 0)
            {
                Node current = _frontier.Dequeue();
                
                if (current != start && current != end) onProcessing?.Invoke(current);
                if (current == end) break;

                yield return wait;

                foreach (Node next in current.Neighbors)
                {
                    if (!_cameFrom.ContainsKey(next))
                    {
                        _cameFrom[next] = current;
                        _frontier.Enqueue(next);
                        if (next != end) onFrontier?.Invoke(next);
                    }
                }
            }
            onFinished?.Invoke(ReconstructPath(start, end));
        }

        private List<Node> ReconstructPath(Node start, Node end)
        {
            if (!_cameFrom.ContainsKey(end)) return null;
            _pathCache.Clear();
            Node curr = end;
            while (curr != start)
            {
                _pathCache.Add(curr);
                curr = _cameFrom[curr];
            }
            _pathCache.Add(start);
            _pathCache.Reverse();
            return new List<Node>(_pathCache);
        }
    }
}