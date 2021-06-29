using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{ 
    public Node node;
    public int priority;

    public PriorityQueue(Node n, int p)
    {
        this.node = n;
        this.priority = p;
    }
}