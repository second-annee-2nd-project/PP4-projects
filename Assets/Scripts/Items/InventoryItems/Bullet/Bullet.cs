using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : TeamUnit
{
    private BulletsPool bulletsPool;
    private Rigidbody rb;

    private Vector3 dir;
    
    private float damage;
    public float Damage
    {
        get => damage;
        set => damage = value;
    }
    private float maxRange;
    public float MaxRange
    {
        get => maxRange;
        set => maxRange = value;
    }

    private float distance;

    [SerializeField] private eBulletType bulletType;
    public eBulletType BulletType
    {
        get => bulletType;
        set => bulletType = value;
    }

    private Coroutine cor;
    
    [SerializeField] private float speed;
    [SerializeField] private bool isSeekingBullet;
    private Transform target;

    public Transform Target
    {
        get => target;
        set => target = value;
    }
    public BulletsPool P_BulletsPool 
    {
        get => bulletsPool;
        set => bulletsPool = value;
    }
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void GetTarget(Transform _target)
    {
        target = _target;
    }
    
    void UpdateTarget()
    {
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortsDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in ennemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortsDistance)
            {
                shortsDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }

        }

        if (nearestEnemy != null && shortsDistance <= distance)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        Team = eTeam.neutral;
        bulletsPool.ReleaseBulletInstance(gameObject, bulletType);
    }
    public void Shoot(Vector3 direction)
    {
        dir = direction;
        if (cor != null)
        {
            StopAllCoroutines();
            cor = null;
        }
        
        if(!isSeekingBullet)
            rb.velocity = dir.normalized * speed;

        if (cor == null)
            cor = StartCoroutine(DestroyOnMaxRange());
    }

    private IEnumerator DestroyOnMaxRange()
    {
        distance = maxRange;
        Vector3 lastPos = Vector3.zero;
        if (isSeekingBullet)
        {
            while (distance > 0f)
            {
                if(target != null)
                    lastPos = target.transform.position;

                
                dir = lastPos - transform.position;
                
                rb.velocity = dir.normalized * speed;
                
                float distanceThisFrame = speed * Time.fixedDeltaTime;
                distance -= distanceThisFrame;
                

                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (distance > 0f)
            {
                float distanceThisFrame = speed * Time.fixedDeltaTime;
                
                
                distance -= distanceThisFrame;

                yield return new WaitForFixedUpdate();
            }
        }
        

        cor = null;
        DestroyBullet();
    }
    

    private void OnTriggerEnter(Collider col)
    {
        // S'il ne faut pas détruire en fonction de ce que la balle touche
        if (col.tag == "Bullet" || col.tag == "CoinsLoot") return;
        DestroyableUnit du = col.GetComponent<DestroyableUnit>();
        if (du)
        {
            if (du.Team != this.Team)
            {
                du.GetDamaged(damage);
            }
        }
        
        DestroyBullet();
    }
    
}

