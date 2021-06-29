using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Destroyable : ScriptableObject
{
    [SerializeField] private string name;
    public string Name => name;
    
    [SerializeField] private float healthPoints;

    public float HealthPoints
    {
        get => healthPoints;
        set => healthPoints = value;
    }
}