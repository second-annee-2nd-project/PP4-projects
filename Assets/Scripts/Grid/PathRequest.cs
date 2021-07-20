using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequest
{
    public EnemyBehaviour requestedFrom;
    public Transform target;
    public List<Node> path;

    public PathRequest(EnemyBehaviour enemy, Transform target)
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