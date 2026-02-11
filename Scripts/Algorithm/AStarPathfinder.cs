namespace Pathfinding.Scripts
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    public class AStarPathfinder : IPathfinder
    {
        private PriorityQueue<Node> _frontier = new PriorityQueue<Node>();
        private Dictionary<Node, Node> _cameFrom = new Dictionary<Node, Node>();
        private Dictionary<Node, float> _costSoFar = new Dictionary<Node, float>(); 
        private List<Node> _pathCache = new List<Node>();

        private float Heuristic(Node a, Node b)
        {
            return Vector3.Distance(a.Position, b.Position);
        }

        public List<Node> FindPath(Node start, Node end)
        {
            if (start == null || end == null) return null;

            ClearData();

            _frontier.Enqueue(start, 0);
            _cameFrom[start] = null;
            _costSoFar[start] = 0;

            while (_frontier.Count > 0)
            {
                Node current = _frontier.Dequeue();

                if (current == end) break;

                foreach (Node next in current.Neighbors)
                {
                    float dist = Vector3.Distance(current.Position, next.Position);
                    float newCost = _costSoFar[current] + (dist * next.Penalty);

                    if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                    {
                        _costSoFar[next] = newCost; 
                        _cameFrom[next] = current;
                        
                        float priority = newCost + Heuristic(next, end);
                        
                        _frontier.Enqueue(next, priority);
                    }
                }
            }

            return ReconstructPath(start, end);
        }

        public IEnumerator FindPathStepByStep(Node start, Node end, float delay, 
            Action<Node> onProcessing, 
            Action<Node> onFrontier, 
            Action<List<Node>> onFinished)
        {
            if (start == null || end == null) yield break;

            ClearData();

            _frontier.Enqueue(start, 0);
            _cameFrom[start] = null;
            _costSoFar[start] = 0;

            var wait = new WaitForSeconds(delay);

            while (_frontier.Count > 0)
            {
                Node current = _frontier.Dequeue();

                if (current != start && current != end) onProcessing?.Invoke(current);

                if (current == end) break;

                yield return wait;

                foreach (Node next in current.Neighbors)
                {
                    float dist = Vector3.Distance(current.Position, next.Position);
                    float newCost = _costSoFar[current] + (dist * next.Penalty);

                    if (!_costSoFar.ContainsKey(next) || newCost < _costSoFar[next])
                    {
                        _costSoFar[next] = newCost;
                        _cameFrom[next] = current;
                        
                        float priority = newCost + Heuristic(next, end);
                        
                        _frontier.Enqueue(next, priority);

                        if (next != end) onFrontier?.Invoke(next);
                    }
                }
            }

            onFinished?.Invoke(ReconstructPath(start, end));
        }

        private void ClearData()
        {
            _frontier.Clear();
            _cameFrom.Clear();
            _costSoFar.Clear();
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