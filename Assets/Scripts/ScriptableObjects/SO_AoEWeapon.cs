using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    
[CreateAssetMenu(fileName ="AoE_Weapon", menuName = "ScriptableObjects/AoEWeapon", order = 1)]
public class SO_AoEWeapon : SO_Weapon
{    
    [SerializeField] private float damageRadius;
    public float DamageRadius => damageRadius;
    
}
