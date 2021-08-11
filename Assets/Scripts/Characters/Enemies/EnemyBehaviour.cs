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

    public override void Init()
    {
        enemyRealStats = (SO_Enemy) enemyStats;
        
        speed = enemyRealStats.Speed;
        attackDamage = enemyRealStats.AttackDamage;
        detectionRange = enemyRealStats.DetectionRange;
        attackRange = enemyRealStats.AttackRange;
        attackSpeed = enemyRealStats.AttackSpeed; 
        rotationSpeed = enemyRealStats.RotationSpeed;
        
        StartMoving();
    }

    

    
    // Beaucoup de RaycastAll
    private bool IsFirstColliderEnemy(Vector3 dir)
    {
        RaycastHit[][] hitsArray;
        hitsArray = new RaycastHit[3][];
        
        
        float radius = transform.GetComponent<Collider>().bounds.size.x;
        
        Vector3 myPositionCenteredGrounded = new Vector3(transform.position.x, groundY, transform.position.z);
        Vector3 myPositionLeftGrounded = new Vector3(transform.position.x - radius, groundY, transform.position.z);
        Vector3 myPositionRightGrounded = new Vector3(transform.position.x + radius, groundY, transform.position.z);
        hitsArray[0] = Physics.RaycastAll(myPositionCenteredGrounded, dir, attackRange);
        hitsArray[1] = Physics.RaycastAll(myPositionLeftGrounded, dir, attackRange);
        hitsArray[2] = Physics.RaycastAll(myPositionRightGrounded, dir, attackRange);
        
        Debug.DrawRay(myPositionCenteredGrounded, dir, Color.blue);
        Debug.DrawRay(myPositionLeftGrounded, dir, Color.blue);
        Debug.DrawRay(myPositionRightGrounded, dir, Color.blue);
        //RaycastHit[] hits = Physics.RaycastAll(weapon.P_FirePosition.position, dir, attackRange);
        
        
        //hitsArray[i][0] = premier hit de chaque hitsArray
        
        bool firstCollidedIsEnemy = false;
        int numberOfHits = 0;
        for (int i = 0; i < hitsArray.Length; i++)
        {
            if (hitsArray[i].Length > 0)
            {
                TeamUnit tu = hitsArray[i][0].collider.GetComponent<TeamUnit>();
                if(tu)
                {
                    if (tu.Team.IsEnemy(this.team))
                    {
                        numberOfHits++;
                    }
                }
            }
        }

        if (numberOfHits == hitsArray.Length)
        {
            return true;
        }
        
        return false;
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
                remainingTimerBeforeLookingAtPath = timerBeforeLookingAtPath;
                transform.LookAt(nearestEnemy.position);
                Vector3 shootDir = nearestEnemy.position - weapon.P_FireTransform.position;
                weapon.Shoot(shootDir);
            }
            else
            {
                enemyAnim.SetBool("Attacking",false);
            }
        }
        else
        {
            if (nextAttack <= 0)
            {
                Attack();
                enemyAnim.SetBool("Attacking",true);
            }
            else
            {
                enemyAnim.SetBool("Attacking",false);
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
