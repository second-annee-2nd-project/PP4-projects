using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public Vector3Int internalPosition;
    public float nodeRadius;
    public bool isWalkable;
    public bool isTurretable;
    
    public Node(Vector3 position, Vector3Int internalPosition, float nodeRadius, bool isWalkable, bool isTurretable)
    {
        this.position = position;
        this.internalPosition = internalPosition;
        this.nodeRadius = nodeRadius;
        this.isWalkable = isWalkable;
        this.isTurretable = isTurretable;
    }
}
