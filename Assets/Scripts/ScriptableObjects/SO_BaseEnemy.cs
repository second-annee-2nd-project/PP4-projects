using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class SO_BaseEnemy : SO_Character
{
    // Start is called before the first frame update
    [SerializeField] private float attackDamage;
    public float AttackDamage => attackDamage;

    [SerializeField] private float attackRange;
    public float AttackRange => attackRange;

    [SerializeField] private float detectionRange;
    public float DetectionRange => detectionRange;
    
    [SerializeField] private float rotationSpeed;
    public float  RotationSpeed =>  rotationSpeed;
    
    [SerializeField] private AudioClip deathSound;
    public AudioClip DeathSound => deathSound;
    
    [SerializeField] private GameObject deathEffectt;
    public GameObject DeathEffectt =>  deathEffectt;
}