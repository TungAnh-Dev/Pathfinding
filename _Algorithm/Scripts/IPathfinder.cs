using System;
using System.Collections;
using System.Collections.Generic;
using _DijkstraAlgorithm.Scripts;

public interface IPathfinder
{
    List<Node> FindPath(Node start, Node end);

    IEnumerator FindPathStepByStep(Node start, Node end, float delay, 
        Action<Node> onProcessing, 
        Action<Node> onFrontier, 
        Action<List<Node>> onFinished);
}