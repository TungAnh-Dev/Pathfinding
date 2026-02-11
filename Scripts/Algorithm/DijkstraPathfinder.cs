using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace Pathfinding.Scripts
{
    public class DijkstraPathfinder : IPathfinder
    {
        private PriorityQueue<Node> priortyQueue = new PriorityQueue<Node>();
        private Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        private Dictionary<Node, float> costSoFar = new Dictionary<Node, float>();
        private List<Node> pathCache = new List<Node>();

        public List<Node> FindPath(Node start, Node end)
        {
            RunDijkstra(start, end);
            return ReconstructPath(start, end);
        }

        public IEnumerator FindPathStepByStep(Node start, Node end, float delay, 
            Action<Node> onProcessing, 
            Action<Node> onFrontier, 
            Action<List<Node>> onFinished)
        {
            if (start == null || end == null) yield break;

            ClearData();

            priortyQueue.Enqueue(start, 0);
            cameFrom[start] = null;
            costSoFar[start] = 0;

            var wait = new WaitForSeconds(delay);

            while (priortyQueue.Count > 0)
            {
                Node current = priortyQueue.Dequeue();

                if (current == end) break;

                if (current != start) onProcessing?.Invoke(current);

                yield return wait;

                foreach (Node next in current.Neighbors)
                {
                    float dist = Vector3.Distance(current.Position, next.Position);
                    float newCost = costSoFar[current] + (dist * next.Penalty);

                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        cameFrom[next] = current;
                        priortyQueue.Enqueue(next, newCost);

                        if (next != end) onFrontier?.Invoke(next);
                    }
                }
            }

            onFinished?.Invoke(ReconstructPath(start, end));
        }


        private void RunDijkstra(Node start, Node end)
        {
            if (start == null || end == null) return;
            ClearData();
            
            priortyQueue.Enqueue(start, 0);
            cameFrom[start] = null;
            costSoFar[start] = 0;

            while (priortyQueue.Count > 0)
            {
                Node current = priortyQueue.Dequeue();
                if (current == end) break;

                foreach (Node next in current.Neighbors)
                {
                    float dist = Vector3.Distance(current.Position, next.Position);
                    float newCost = costSoFar[current] + (dist * next.Penalty);

                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        cameFrom[next] = current;
                        priortyQueue.Enqueue(next, newCost);
                    }
                }
            }
        }

        private List<Node> ReconstructPath(Node start, Node end)
        {
            if (!cameFrom.ContainsKey(end)) return null;

            pathCache.Clear();
            Node curr = end;

            while (curr != start)
            {
                pathCache.Add(curr);
                curr = cameFrom[curr];
            }
            pathCache.Add(start);
            pathCache.Reverse();

            return new List<Node>(pathCache);
        }

        private void ClearData()
        {
            priortyQueue.Clear();
            cameFrom.Clear();
            costSoFar.Clear();
        }
    }
}