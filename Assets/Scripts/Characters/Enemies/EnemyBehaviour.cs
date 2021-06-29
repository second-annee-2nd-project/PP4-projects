using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviour : DestroyableUnit
{
    private bool turretDetected;
    [SerializeField] protected float speed;
    [SerializeField] private float attackDamage;
    [SerializeField] protected  float detectionRange;
    [SerializeField] protected  float attackRange;
    [SerializeField] protected float attackSpeed;
    protected float nextAttack;
    [SerializeField] protected Weapon weapon;
    
    protected Transform player;
    protected Transform nearestEnemy;
    private TurretManager turretManager;
    private CoinsLoot coinsLoot;

    private PathRequestManager pathRequestManager;
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
        turretManager = GameObject.FindObjectOfType<TurretManager>().GetComponent<TurretManager>();
        coinsLoot = GameObject.FindGameObjectWithTag("CoinsLoot").GetComponent<CoinsLoot>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pathRequestManager = FindObjectOfType<PathRequestManager>();

        if (weapon != null) weapon.Team = Team;

    }

    public void Init()
    {
        pathRequestManager = FindObjectOfType<PathRequestManager>();
        startingNode = GameManager.Instance.ActualGrid.GetNode(transform.position);
        pathRequestManager.AddPath(new PathRequest(this));
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
        // if(!turretDetected)
        //     ChasePlayer();
        // FindTurret();
        
        GetNearestEnemy();
        if (weapon != null) return;
        if (nextAttack > 0)
        {
            nextAttack -= Time.deltaTime;
            if (nextAttack <= 0f) nextAttack = -1f;
        }
        
       
    }

    // void FindTurret()
    // {
    //     List <GameObject> go = new List<GameObject>(GameManager.Instance.P_EnemiesManager.Targets);
    //     go.Add(player.gameObject);
    //     float distance = 1000000f;
    //     Transform nearestEnemy = player.transform;
    //     if (!turretDetected)
    //     {
    //         for (int i = 0; i < go.Count; i++)
    //         {
    //             if (Vector3.Distance(transform.position, go[i].transform.position) < rangeDetectTurret)
    //             {
    //                 turretDetected = true;
    //                 nearestEnemy = go[i].transform;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         if (Vector3.Distance(transform.position, nearestEnemy.position) > rangeAttack)
    //             transform.position = Vector3.MoveTowards(transform.position, 
    //                 new Vector3(nearestEnemy.position.x,transform.position.y,nearestEnemy.position.z),speed * Time.deltaTime);
    //     }
    // }

    void OnTriggerEnter(Collider col)
    {

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
            GameManager.Instance.P_EnemiesManager.RemoveEnemyFromList(gameObject);
            coinsLoot.LootCoins(transform.position);
            Destroy(this.gameObject);
        }
      
        
    }

    protected void GetNearestEnemy()
    {
        float minimumDistance = Mathf.Infinity;

        nearestEnemy = null;
      
        if (turretManager.turretList.Count == 0)
        {
            ChasePlayer();
            
        }
        else
        {
            foreach(Transform enemy in turretManager.turretList)
            {

                float distance = Vector3.Distance(transform.position, enemy.position);
            

                if ( distance < minimumDistance)
                {

                    minimumDistance = distance;

                    nearestEnemy = enemy;

                }
            }
            float distancePlayer = Vector3.Distance(transform.position, player.position);
            if (Vector3.Distance(transform.position, nearestEnemy.position) < distancePlayer)
            {
                ChaseEnemy();
            }
            else
            { 
                ChasePlayer();
            }
        }
       
    }

    protected virtual void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    protected virtual void Attack()
    {
        nearestEnemy.gameObject.GetComponent<DestroyableUnit>().GetDamaged(attackDamage);
        
        Debug.Log("L'ennemi a pris un coup");

    }

    protected virtual void ChaseEnemy()
    {
        Vector3 nearestEnemyGrounded =
            new Vector3(nearestEnemy.position.x, transform.position.y, nearestEnemy.position.z);
        if (Vector3.Distance(transform.position, nearestEnemyGrounded) < attackRange)
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
                weapon.Shoot(nearestEnemy.position);
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
