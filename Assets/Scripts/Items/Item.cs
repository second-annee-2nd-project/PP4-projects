using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Item : MonoBehaviour
{
    protected Transform nearestVisibleEnemy;
    public Transform NearestVisibleEnemy => nearestVisibleEnemy;
}
