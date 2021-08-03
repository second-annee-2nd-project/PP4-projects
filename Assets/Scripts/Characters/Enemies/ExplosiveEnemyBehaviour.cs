using System;
using System.Collections;
using System.Collections.Generic;
using TeamExtensionMethods;
using UnityEngine;

public class ExplosiveEnemyBehaviour : BaseEnemyBehaviour
{
    private float explosionRadius;

    private SO_ExplosiveEnemy enemyRealStats;

    private void Update()
    {
        UpdateLifeBar();
    }

    public override void Init()
    {
        enemyRealStats = (SO_ExplosiveEnemy) enemyStats;
        
        speed = enemyRealStats.Speed;
        attackDamage = enemyRealStats.AttackDamage;
        detectionRange = enemyRealStats.DetectionRange;
        attackRange = enemyRealStats.AttackRange;
        explosionRadius = enemyRealStats.ExplosionRadius; 
        rotationSpeed = enemyRealStats.RotationSpeed;
        bHealthPoints = enemyRealStats.HealthPoints;
        healthPoints = bHealthPoints;

        StartMoving();
    }
    /*
    protected override void ChoseAction(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3 nearestEnemyGrounded =
            new Vector3(nearestEnemy.position.x, groundY, nearestEnemy.position.z);
        Vector3 myPositionGrounded = new Vector3(transform.position.x, groundY, transform.position.z);

        Vector3 sightDir = nearestEnemyGrounded - myPositionGrounded;

        Debug.DrawRay(transform.position, sightDir, Color.blue); 

        if (Vector3.Distance(transform.position, nearestEnemyGrounded) > attackRange ||
            !IsFirstColliderEnemy(sightDir))
        {
            transform.position = Vector3.MoveTowards(startPosition, targetPosition, speed * Time.deltaTime);
            transform.LookAt(targetPosition);
        }
        else
        {
            TryToAttack();
        }
    }*/
    
    protected override void Attack()
    {
        Collider[] colliders = new Collider[20];
        Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, colliders);
        Debug.Log("J'ia attaqué : "+colliders.Length);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i])
            {
                DestroyableUnit du = colliders[i].GetComponent <DestroyableUnit>();
                if (du)
                {
                    if (this.team.IsEnemy(du.Team))
                    {
                        du.GetDamaged(attackDamage);
                    }
                }
            }
        }

        Die();
    }

    protected void OnCollisionEnter(Collision col)
    {
        DestroyableUnit du = col.gameObject.GetComponent<DestroyableUnit>();
        if (du)
        {
            if (this.team.IsEnemy(du.Team))
            {
                TryToAttack();
            }
        }
    }

    protected override void TryToAttack()
    {
        Attack();
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(removedPos, 0.5f);
        if (path.Count < 1) return;
        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.color = Color.blue;
            Vector3 uppedPosition = path[i].position;
            uppedPosition.y += 3; 
            Gizmos.DrawWireSphere(path[i].position, 0.5f);
        }
    }
}
