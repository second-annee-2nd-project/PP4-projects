using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy", menuName = "ScriptableObjects/Destroyable/Movable/Enemy", order = 2)]
public class SO_Enemy : SO_Character
{
    // Start is called before the first frame update
    [SerializeField] private float attackDamage;
    public float AttackDamage => attackDamage;

    [SerializeField] private float attackSpeed;
    public float AttackSpeed => attackSpeed;

    //public float AttackSpeed => 1f / attackCooldown;

    [SerializeField] private float attackRange;
    public float AttackRange => attackRange;

    [SerializeField] private float detectionRange;
    public float DetectionRange => detectionRange;
}
