using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ExplosiveEnemy", menuName = "ScriptableObjects/Destroyable/Movable/Enemies/ExplosiveEnemy", order = 2)]
public class SO_ExplosiveEnemy : SO_BaseEnemy
{
    [SerializeField] private float explosionRadius;
    public float ExplosionRadius => explosionRadius;
}
