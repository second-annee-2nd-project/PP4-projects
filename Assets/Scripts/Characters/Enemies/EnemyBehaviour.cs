using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TeamExtensionMethods;
using UnityEngine;

public class EnemyBehaviour : DestroyableUnit
{
    private static int id = 1;
    public int ID;
    private bool turretDetected;
    [SerializeField] protected float speed;
    [SerializeField] private float attackDamage;
    [SerializeField] protected  float detectionRange;
    [SerializeField] protected  float attackRange;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float rotationSpeed;
    protected float nextAttack;
    [SerializeField] protected Weapon weapon;

    [Header("Object prefab dropped at death")] [SerializeField]
    protected int amountToLoot;
    [SerializeField]
    protected GameObject loot;
    
    protected Transform player;
    protected Transform nearestEnemy;
    protected Transform lastNearestEnemy;

    private Node lastNearestEnemyNode;

    private PathRequestManager pathRequestManager;
    private Grid grid;
    private EnemiesManager enemiesManager;
    private List<Node> path;
    public List<Node> Path
    {
        get => path;
        set => path = value;
    }

    private Node startingNode;
    public Node StartingNode
    {
        get => startingNode;
    }

    private Node targetingNode;

    public Node TargetingNode
    {
        get => targetingNode;
    }

    private float groundY;

    private Vector3 removedPos;
  
    
    // Start is called before the first frame update
    void Awake()
    {
        ID = id;
        id++;
        name += " "+ID;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        enemiesManager = GameManager.Instance.P_EnemiesManager;
        grid = GameManager.Instance.ActualGrid;
        path = new List<Node>();


        groundY = grid.CenterPosition.y;

        if (weapon != null)
        {
            weapon.Team = Team;
            //attackRange = weapon.P_BRange;
        }
        
        
    }

    protected override void Start()
    {
        base.Start();
        Init();
    }

    public void Init()
    {
        GetNearestEnemy();
        path = new List<Node>();
        pathRequestManager = FindObjectOfType<PathRequestManager>();
        startingNode = GameManager.Instance.ActualGrid.GetNodeWithPosition(transform.position);
        StartMoving();
    }

    public IEnumerator Move()
    {
        int a = 0;
        while (healthPoints > 0)
        {
            GetNearestEnemy();
            startingNode = GameManager.Instance.ActualGrid.GetNodeWithPosition(transform.position);
            targetingNode = grid.GetNodeWithPosition(nearestEnemy.transform.position);
            if (targetingNode != lastNearestEnemyNode)
            {
                pathRequestManager.AddPath(new PathRequest(this, nearestEnemy), this);
                lastNearestEnemyNode = targetingNode;

                if (path.Count > 1)
                {

                    Vector3 dirNode1ToUnit = (transform.position - path[0].position).normalized;
                    Vector3 dirNode1ToNode2 = (path[1].position - path[0].position).normalized;
                    Vector3 dirNode2ToTarget = (targetingNode.position - path[1].position).normalized;
                    
                    Vector3 dirUnitToNode1 = (path[0].position - transform.position).normalized;
                    Vector3 dirUnitToNode2 = (path[1].position - transform.position).normalized;

                    if (dirUnitToNode1.x == 0 || dirUnitToNode1.z == 0)
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


            if (!(path == null || path.Count < 1))
            {
                Vector3 startPosition = transform.position;
                Vector3 targetPosition = new Vector3(path[0].position.x, this.transform.position.y, path[0].position.z);

                Vector3 nearestEnemyGrounded =
                    new Vector3(nearestEnemy.position.x, groundY, nearestEnemy.position.z);
                Vector3 myPositionGrounded = new Vector3(transform.position.x, groundY, transform.position.z);

                Vector3 sightDir = nearestEnemyGrounded - myPositionGrounded;

                // Debug.DrawRay(weapon.P_FirePosition.position, dir, Color.blue);

                if (Vector3.Distance(transform.position, nearestEnemyGrounded) > attackRange ||
                    !IsFirstColliderEnemy(sightDir))
                {
                    transform.position = Vector3.MoveTowards(startPosition, targetPosition, speed * Time.deltaTime);
                    transform.LookAt(targetPosition);
                }
                else
                {
                    Debug.Log("tried to attack");
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
        Debug.Log(name+" a fini une séquence Move");
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
    void Update()
    {
        if (weapon != null) return;
        if (nextAttack > 0)
        {
            nextAttack -= Time.deltaTime;
            if (nextAttack <= 0f) nextAttack = -1f;
        }
    }

    protected override void Die()
    {
        if (healthPoints <= 0)
        {
            GameManager.Instance.P_EnemiesManager.RemoveItemFromList(gameObject);
            GameObject newLoot = Instantiate(loot, transform.position, Quaternion.identity);
            newLoot.GetComponent<Loot>().AmountToLoot = amountToLoot;
            GameManager.Instance.P_LootManager.AddItemToList(newLoot);
        }
    }

    protected void GetNearestEnemy()
    {
        float minimumDistance = Mathf.Infinity;
        nearestEnemy = enemiesManager.GetNearestTarget(transform.position);
    }

    protected virtual void Attack()
    {
        nearestEnemy.gameObject.GetComponent<DestroyableUnit>().GetDamaged(attackDamage);
        nextAttack = attackSpeed;
    }

    protected virtual void ChaseEnemy()
    {
        Vector3 nearestEnemyGrounded = new Vector3(nearestEnemy.position.x, transform.position.y, nearestEnemy.position.z);
        if (Vector3.Distance(transform.position, nearestEnemyGrounded) > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, nearestEnemyGrounded, speed * Time.deltaTime);
        }
        else
        {
            TryToAttack();
        }
    }

    private void TryToAttack()
    {
        if (weapon != null)
        {
            if (weapon.CanShoot())
            {
                transform.LookAt(nearestEnemy.position);
                Vector3 shootDir = nearestEnemy.position - weapon.P_FireTransform.position;
                weapon.Shoot(shootDir);
            }
        }
        else
        {
            if (nextAttack <= 0)
            {
                Attack();
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
