using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TeamExtensionMethods;
using UnityEngine;

public class EnemyBehaviour : BaseEnemyBehaviour
{
   [SerializeField] protected Weapon weapon;

    protected float attackSpeed;

    private SO_Enemy enemyRealStats;

    private Animator enemyAnim;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        enemyAnim = GetComponent<Animator>();
        if (weapon != null)
        {
            weapon.Team = Team;
            //attackRange = weapon.P_BRange;
        }
    }

    protected override void CallAnim()
    {
        base.CallAnim();
        enemyAnim.SetBool("Attacking",false);
    }

    public override void Init()
    {
        base.Init();
        enemyRealStats = (SO_Enemy) enemyStats;
        
        speed = enemyRealStats.Speed;
        attackDamage = enemyRealStats.AttackDamage;
        detectionRange = enemyRealStats.DetectionRange;
        attackRange = enemyRealStats.AttackRange;
        attackSpeed = enemyRealStats.AttackSpeed; 
        rotationSpeed = enemyRealStats.RotationSpeed;
        
        StartMoving();
    }
    
    public void StartMoving()
    {
        StartCoroutine(Move());
    }

    // Update is called once per frame
    protected void Update()
    {
        UpdateLifeBar();
        if (weapon != null) return;
        if (nextAttack > 0)
        {
            nextAttack -= Time.deltaTime;
            if (nextAttack <= 0f) nextAttack = -1f;
        }
    }

    protected override void Attack()
    {
        nearestEnemy.gameObject.GetComponent<DestroyableUnit>().GetDamaged(attackDamage);
        nextAttack = attackSpeed;
    }

    protected override void TryToAttack()
    {
        if (weapon != null)
        {
            if (weapon.CanShoot())
            {
                enemyAnim.SetBool("Attacking",true);
                remainingTimerBeforeLookingAtPath = attackSpeed;
                transform.LookAt(nearestEnemy.position);
                Vector3 shootDir = nearestEnemy.position - weapon.P_FireTransform.position;
                weapon.Shoot(shootDir);
            }
            // else if (remainingTimerBeforeLookingAtPath <= 0)
            // {
            //     enemyAnim.SetBool("Attacking",false);
            // }
        }
        else
        {
            if (nextAttack <= 0)
            {
                Attack();
                enemyAnim.SetBool("Attacking",true);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(removedPos, 0.5f);
        if (path == null || path.Count < 1) return;
        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.color = Color.blue;
            Vector3 uppedPosition = path[i].position;
            uppedPosition.y += 3; 
            Gizmos.DrawWireSphere(path[i].position, 0.5f);
        }
    }
}
