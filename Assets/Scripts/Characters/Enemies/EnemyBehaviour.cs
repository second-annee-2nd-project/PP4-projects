using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyBehaviour : DestroyableUnit
{
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
    protected GameObject loot;
    
    protected Transform player;
    protected Transform nearestEnemy;

    private PathRequestManager pathRequestManager;
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

  
    
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        enemiesManager = GameManager.Instance.P_EnemiesManager;
        

        if (weapon != null)
        {
            weapon.Team = Team;
            attackRange = weapon.P_BRange;
        }
        

    }

    public void Init()
    {
        /*pathRequestManager = FindObjectOfType<PathRequestManager>();
        startingNode = GameManager.Instance.ActualGrid.GetNode(transform.position);
        pathRequestManager.AddPath(new PathRequest(this));*/
        
    }

    public IEnumerator Move()
    {
        while (healthPoints > 0 && path.Any())
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = new Vector3(path[0].position.x, this.transform.position.y, path[0].position.z);
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startPosition, targetPosition);

            while (healthPoints > 0 && transform.position != targetPosition)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
                // FindTurret();
                yield return null;
            }
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        GetNearestEnemy();
        if (nearestEnemy != null)
        {
            transform.LookAt(nearestEnemy.transform.position);
        }
        if (weapon != null) return;
        if (nextAttack > 0)
        {
            nextAttack -= Time.deltaTime;
            if (nextAttack <= 0f) nextAttack = -1f;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected override void Die()
    {
        if (healthPoints <= 0)
        {
            GameManager.Instance.P_EnemiesManager.RemoveItemFromList(gameObject);
            Instantiate(loot, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    protected void GetNearestEnemy()
    {
        float minimumDistance = Mathf.Infinity;
        nearestEnemy = enemiesManager.GetNearestTarget(transform.position);
        if (nearestEnemy == null) return;
        ChaseEnemy();
       
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
                Vector3 shootDir = nearestEnemy.position - weapon.P_FirePosition.position;
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

}
