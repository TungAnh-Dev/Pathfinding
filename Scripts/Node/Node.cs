using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Scripts
{
    public class Node
    {
        public Vector3 Position;
        public float Penalty;
        public bool isWalkable;
        public List<Node> Neighbors;

        public Node(Vector3 pos, bool walkable = true, float penalty = 1f)
        {
            Position = pos;
            isWalkable = walkable;
            Penalty = penalty;
            Neighbors = new List<Node>();
        }
    }
}

