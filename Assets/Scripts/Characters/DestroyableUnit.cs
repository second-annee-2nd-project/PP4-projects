using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableUnit : TeamUnit
{
    [Header("Health Points")]
    [SerializeField] protected float bHealthPoints;
    
    protected float healthPoints;


    public void GetDamaged(float damage)
    {
        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        
    }
}
