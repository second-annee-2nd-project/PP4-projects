using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : EnemyBehaviour
{
  
    [SerializeField] private float rateOfFire;
    [SerializeField] private GameObject bulletEnemy;
    [SerializeField] private Transform firePoint;
    private float nextFire;
  

    void Update()
    {
        // Shoot();
        GetNearestEnemy();
        
        if (nearestEnemy!= null)
        {
            transform.LookAt(nearestEnemy.position);
        }
        else
        {
            transform.LookAt(player.position);
        }
       
    }
    void Shoot()
    {

        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {
            
            if (Time.time > nextFire)
            {
                nextFire = Time.time + rateOfFire;
                Destroy(Instantiate(bulletEnemy, firePoint.position, transform.rotation), 5);
            }

        }

    }
    private void Move()
    {

        if (Vector3.Distance(transform.position, player.position) < detectionRange)
        {

            if (Vector3.Distance(transform.position, player.position) > attackRange)
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            else
            {
                Shoot();
            }
            
        }

    }

    protected override void ChasePlayer()
    {
        Move();
    }

    protected void AttackTurret()
    {
        if (Vector3.Distance(transform.position, nearestEnemy.position) < detectionRange)
        {

            if (Vector3.Distance(transform.position, nearestEnemy.position) > attackRange)
                transform.position = Vector3.MoveTowards(transform.position, nearestEnemy.transform.position, speed * Time.deltaTime);
            else
            {
                Shoot();
            }
            
        }
    }
}
