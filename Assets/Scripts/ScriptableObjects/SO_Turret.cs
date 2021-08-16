using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Turret", menuName = "ScriptableObjects/Destroyable/Turret", order = 3)]
public class SO_Turret : SO_Destroyable
{
 [SerializeField] private float attackSpeed;
 public float AttackSpeed =>  attackSpeed; 
 
 [SerializeField] private float range;
 public float  Range =>  range; 
 
 [SerializeField] private int price;
 public int Price =>  price;

 [SerializeField] private float turnSpeed = 5f;
 public float TurnSpeed => turnSpeed;
 
}
