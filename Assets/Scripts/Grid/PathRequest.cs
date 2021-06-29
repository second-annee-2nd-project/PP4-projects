using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequest
{
    public EnemyBehaviour requestedFrom;
    public List<Node> path;

    public PathRequest(EnemyBehaviour enemy)
    {
        requestedFrom = enemy;
        path = new List<Node>();
    }

    public void ReturnPath()
    {
        requestedFrom.Path = this.path;
    }
}