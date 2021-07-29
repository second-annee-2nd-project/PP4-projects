using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamExtensionMethods;

public class DestroyableUnit : TeamUnit
{
    [Header("Health Points")]
    [SerializeField] protected float bHealthPoints;
    
    protected float healthPoints;
    
    protected virtual void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        healthPoints = bHealthPoints;

        transform.position = new Vector3(transform.position.x, GameManager.Instance.ActualGrid.CenterPosition.y, 
            transform.position.z);
        GameManager.Instance.P_SoundManager.AudioSource = FindObjectOfType<AudioSource>();
    }

    public virtual void Restart()
    {
        Init();
    }
    public virtual void GetDamaged(float damage)
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
