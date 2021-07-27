using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy", menuName = "ScriptableObjects/Destroyable/Movable/Enemies/Enemy", order = 1)]
public class SO_Enemy : SO_BaseEnemy
{
    [SerializeField] private float attackSpeed;
    public float AttackSpeed => attackSpeed;
}
