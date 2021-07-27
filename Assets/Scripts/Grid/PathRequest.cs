using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequest
{
    public BaseEnemyBehaviour requestedFrom;
    public Transform target;
    public List<Node> path;

    public PathRequest(BaseEnemyBehaviour enemy, Transform target)
    {
        target = target;
        requestedFrom = enemy;
        path = new List<Node>();
    }

    public void ReturnPath()
    {
        requestedFrom.Path = this.path;
    }
}