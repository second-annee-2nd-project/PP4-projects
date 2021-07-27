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
        
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }

    public virtual void Init()
    {
        speed = enemyStats.Speed;
        attackDamage = enemyStats.AttackDamage;
        detectionRange = enemyStats.DetectionRange;
        attackRange = enemyStats.AttackRange;
        
        StartMoving();
    }
    
    // Beaucoup de RaycastAll
    protected bool IsFirstColliderEnemy(Vector3 dir)
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

    protected override void Die()
    {
        GameManager.Instance.P_EnemiesManager.RemoveItemFromList(gameObject);
        GameObject newLoot = Instantiate(loot, transform.position, Quaternion.identity);
        newLoot.GetComponent<Loot>().AmountToLoot = amountToLoot;
        GameManager.Instance.P_LootManager.AddItemToList(newLoot);
    }

    protected void GetNearestEnemy()
    {
        float minimumDistance = Mathf.Infinity;
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
        Node lastNode = grid.GetNodeWithPosition(transform.position);;
        while (healthPoints > 0)
        {
            GetNearestEnemy();
            startingNode = grid.GetNodeWithPosition(transform.position);
            targetingNode = grid.GetNodeWithPosition(nearestEnemy.transform.position);
            if (targetingNode != lastNearestEnemyNode)
            {
                AskForPath();
                lastNearestEnemyNode = targetingNode;
                
                if(path.Count > 1 && path[0]!= null && path[1] != null)
                {
                    
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


            
            if (!(path == null || path.Count < 1) && path[0] != null)
            {
                Vector3 startPosition = transform.position;
                Vector3 targetPosition = new Vector3(path[0].position.x, this.transform.position.y, path[0].position.z);
                
                //essai de s'approprier la case

                /*Node actualNode = grid.GetNodeWithPosition(transform.position);
                if (actualNode != lastNode)
                {
                    lastNode.isWalkable = true;

                    actualNode.isWalkable = false;

                    lastNode = actualNode;
                }*/
                
                

                Vector3 nearestEnemyGrounded =
                    new Vector3(nearestEnemy.position.x, groundY, nearestEnemy.position.z);
                Vector3 myPositionGrounded = new Vector3(transform.position.x, groundY, transform.position.z);

                Vector3 sightDir = nearestEnemyGrounded - myPositionGrounded;

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

                if (startPosition == targetPosition)
                {
                    path.RemoveAt(0);
                }
                
                yield return null;
            }
            yield return null;
        }
        yield return null;
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
