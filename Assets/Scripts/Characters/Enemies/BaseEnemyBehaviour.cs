using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TeamExtensionMethods;
using UnityEngine;

public abstract class BaseEnemyBehaviour : DestroyableUnit
{
    protected static int id = 1;
    protected int ID;

    protected float speed;
    protected float attackDamage;
    protected float detectionRange;
    protected float attackRange;
    protected float nextAttack;
    protected float rotationSpeed;

    protected float timerBeforeLookingAtPath = 2.5f;
    protected float remainingTimerBeforeLookingAtPath;
    
    [SerializeField] protected SO_BaseEnemy enemyStats;

    public SO_BaseEnemy EnemyStats => enemyStats;
    
    [Header("Object prefab dropped at death")] [SerializeField]
    protected int amountToLoot;
    [SerializeField]
    protected GameObject loot;
    
    protected Transform nearestEnemy;

    protected Node lastNearestEnemyNode;

    protected PathRequestManager pathRequestManager;
    protected Grid grid;
    protected EnemiesManager enemiesManager;
    protected List<Node> path;
    [SerializeField] private GameObject spawnEffect;
    
    public List<Node> Path
    {
        get => path;
        set => path = value;
    }

    protected Node startingNode;
    public Node StartingNode
    {
        get => startingNode;
    }

    protected Node targetingNode;

    public Node TargetingNode
    {
        get => targetingNode;
    }

    protected float groundY;

    protected Vector3 removedPos;
  
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        ID = id;
        id++;
        name += " "+ID;
        path = new List<Node>();

        enemiesManager = GameManager.Instance.P_EnemiesManager;
        pathRequestManager = GameManager.Instance.P_PathRequestManager;
        grid = GameManager.Instance.ActualGrid;
        
        groundY = grid.CenterPosition.y;
        GameObject effect = Instantiate(spawnEffect,transform.position, transform.rotation);
        effect.transform.parent = transform;
        Destroy(effect,1f);
    }

    protected override void Start()
    {
        base.Start();
       
    }

    public override void Init()
    {
        base.Init();
        bHealthPoints = enemyStats.HealthPoints;
        healthPoints = bHealthPoints;
        transform.position = new Vector3(transform.position.x, GameManager.Instance.ActualGrid.CenterPosition.y - GameManager.Instance.ActualGrid.P_GridHeight * 0.5f, transform.position.z);
        speed = enemyStats.Speed;
        attackDamage = enemyStats.AttackDamage;
        detectionRange = enemyStats.DetectionRange;
        attackRange = enemyStats.AttackRange;
        
        StartMoving();
    }
    
    // Beaucoup de RaycastAll
    protected bool IsFirstColliderEnemy(Vector3 target)
    {
        RaycastHit[] hitsArray;
        hitsArray = new RaycastHit[3];

        float radius = transform.GetComponent<Collider>().bounds.size.x * 0.5f - 0.1f;
        target = new Vector3(target.x, groundY, target.z);
        
        Vector3 myPositionCenteredGrounded = new Vector3(transform.position.x, groundY, transform.position.z);
        Vector3 myPositionLeftGrounded = new Vector3(transform.position.x - radius, groundY, transform.position.z);
        Vector3 myPositionRightGrounded = new Vector3(transform.position.x + radius, groundY, transform.position.z);

        Vector3 dirCenteredToTarget = target - myPositionCenteredGrounded;
        Vector3 dirLeftToTarget = target - myPositionLeftGrounded;
        Vector3 dirRightToTarget = target - myPositionRightGrounded;
        
        Physics.Raycast(myPositionCenteredGrounded, dirCenteredToTarget, out hitsArray[0], attackRange, ~GameManager.Instance.ActualGrid.videMask);
        Physics.Raycast(myPositionLeftGrounded, dirLeftToTarget, out hitsArray[1], attackRange, ~GameManager.Instance.ActualGrid.videMask);
        Physics.Raycast(myPositionRightGrounded, dirRightToTarget, out hitsArray[2], attackRange , ~GameManager.Instance.ActualGrid.videMask);

        bool firstCollidedIsEnemy = false;
        int numberOfHits = 0;
        for (int i = 0; i < hitsArray.Length; i++)
        {
           
            /*if (hitsArray[i].Length > 0)
            {*/
            if (hitsArray[i].collider != null)
            {
                TeamUnit tu = hitsArray[i].collider.GetComponent<TeamUnit>();
                if(tu)
                {
                    if (tu.Team.IsEnemy(this.team))
                    {
                        numberOfHits++;
                    }
                }
            }
                
            //}
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

    protected override void Die()
    {
        Node actualNode = grid.GetNodeWithPosition(transform.position);
        if (actualNode.occupiedBy == this)
        {
            actualNode.occupiedBy = null;
        }
        
        GameManager.Instance.P_EnemiesManager.RemoveItemFromList(gameObject);
        GameObject newLoot = Instantiate(loot, transform.position, Quaternion.identity);
        newLoot.GetComponent<Loot>().AmountToLoot = amountToLoot;
        GameManager.Instance.P_LootManager.AddItemToList(newLoot);
        GameManager.Instance.P_SoundManager.AudioSource.PlayOneShot(enemyStats.DeathSound);
        Destroy(Instantiate(enemyStats.DeathEffectt, transform.position, Quaternion.identity), 2);
    }

    protected void GetNearestEnemy()
    {
        nearestEnemy = enemiesManager.GetNearestTarget(transform.position);
    }

    protected virtual void Attack()
    {
        nearestEnemy.gameObject.GetComponent<DestroyableUnit>().GetDamaged(attackDamage);
    }

    protected virtual void TryToAttack()
    {
        Attack();
    }


    
    public IEnumerator Move()
    {
        int a = 0;
        Node lastNode = grid.GetNodeWithPosition(transform.position);
        CheckIfPathNeedsToChange();
        
        while (healthPoints > 0)
        {
            CheckIfPathNeedsToChange();
            Vector3 nearestEnemyGrounded =
                new Vector3(nearestEnemy.position.x, groundY, nearestEnemy.position.z);
            Vector3 myPositionGrounded = new Vector3(transform.position.x, groundY, transform.position.z);

            Vector3 sightDir = nearestEnemyGrounded - myPositionGrounded;

            if ((nearestEnemyGrounded - myPositionGrounded).sqrMagnitude <= attackRange * attackRange)
            {
                if (IsFirstColliderEnemy(nearestEnemyGrounded))
                {
                    TryToAttack();
                }
            }
            else
            {
                if (path != null && path.Count > 0 && path[0] != null)
                {
                  
                    if (remainingTimerBeforeLookingAtPath > 0f)
                    {
                        remainingTimerBeforeLookingAtPath -= Time.deltaTime;
                        transform.LookAt(nearestEnemyGrounded);
                    }
                    else
                    {
                        CallAnim(); 
                        Vector3 startPosition = transform.position;
                        Vector3 targetPosition = new Vector3(path[0].position.x, this.transform.position.y, path[0].position.z);
                      
                        Vector3Int coord = path[0].internalPosition;

                        //grid.Nodes[coord.x, coord.z].occupiedBy = this;
                        //essai de s'approprier la case

                        Node actualNode = grid.GetNodeWithPosition(transform.position);
                        if (actualNode != lastNode)
                        {
                            //lastNode.isWalkable = true;
                            lastNode.occupiedBy = null;

                            //actualNode.isWalkable = false;
                            actualNode.occupiedBy = this;

                            lastNode = actualNode;
                        }

                    
                        transform.position = Vector3.MoveTowards(startPosition, targetPosition, speed * Time.deltaTime);
                        transform.LookAt(targetPosition);
                        if (startPosition == targetPosition)
                        {
                            
                            path.RemoveAt(0);
                            //grid.Nodes[coord.x, coord.z].occupiedBy = null;
                            
                        }
                    }
                    

                    //ChoseAction(startPosition, targetPosition);

                    

                    yield return null;
                }
            }
            yield return null;
        }
        yield return null;
    }

    protected virtual void CallAnim()
    {
        
    }
    protected void CheckIfPathNeedsToChange()
    {
        GetNearestEnemy();
        startingNode = grid.GetNodeWithPosition(transform.position);
        targetingNode = grid.GetNodeWithPosition(nearestEnemy.transform.position);
            
        if (targetingNode != lastNearestEnemyNode || IsPathOccupied())
        {
            AskForPath();
            lastNearestEnemyNode = targetingNode;
                
            if(path.Count > 1 && path[0]!= null && path[1] != null)
            {
                AskForPath();
                Vector3 dirUnitToNode1 = (path[0].position - transform.position).normalized;
                Vector3 dirUnitToNode2 = (path[1].position - transform.position).normalized;

                if (Mathf.Approximately(dirUnitToNode1.x, 0f) || Mathf.Approximately(dirUnitToNode1.x, 0f))
                {
                    removedPos = path[0].position;
                    path.Remove(path[0]);
                }
                else if ((int) Mathf.Sign(dirUnitToNode1.x) != (int) Mathf.Sign(dirUnitToNode2.x) ||
                         (int) Mathf.Sign(dirUnitToNode1.z) != (int) Mathf.Sign(dirUnitToNode2.z))
                {
                    removedPos = path[0].position;
                    path.Remove(path[0]);
                }
            }

        }
    }

    protected bool IsPathOccupied()
    {
        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log(path.Count);
            Debug.Log(this.name);
            if (path[i] != null)
            {
                if (path[i].occupiedBy != this)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected void ChoseAction(Vector3 startPosition, Vector3 targetPosition)
    {
        Vector3 nearestEnemyGrounded =
            new Vector3(nearestEnemy.position.x, groundY, nearestEnemy.position.z);
        Vector3 myPositionGrounded = new Vector3(transform.position.x, groundY, transform.position.z);

        Vector3 sightDir = nearestEnemyGrounded - myPositionGrounded;

        Debug.DrawRay(transform.position, sightDir, Color.blue); 

        if (Vector3.Distance(transform.position, nearestEnemyGrounded) > attackRange ||
            !IsFirstColliderEnemy(sightDir))
        {
            Debug.Log("?");
            transform.position = Vector3.MoveTowards(startPosition, targetPosition, speed * Time.deltaTime);
            transform.LookAt(targetPosition);
        }
        else
        {
            Debug.Log("!");
            TryToAttack();
        }
    }

    protected virtual void AskForPath()
    {
        pathRequestManager.AddPath(new PathRequest(this, nearestEnemy), this);
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
