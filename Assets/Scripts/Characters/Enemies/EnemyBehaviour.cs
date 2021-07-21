﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        startingNode = GameManager.Instance.ActualGrid.GetNode(transform.position);
        StartMoving();
    }

    public IEnumerator Move()
    {
        while (healthPoints > 0)
        {
            GetNearestEnemy();
            startingNode = GameManager.Instance.ActualGrid.GetNode(transform.position);
            targetingNode = grid.GetNodeWithPosition(nearestEnemy.transform.position);
            if (targetingNode != lastNearestEnemyNode)
            {
                pathRequestManager.AddPath(new PathRequest(this, nearestEnemy), this);
                lastNearestEnemyNode = targetingNode;

                if (path.Count > 1)
                {

                    Vector3 dirNode1ToUnit = (transform.position - path[0].position).normalized;
                    Vector3 dirUnitToNode2 = (path[1].position - transform.position).normalized;

/*
                    float uX = dirNode1ToUnit.x;
                    float uY = dirNode1ToUnit.z;
                    float vX = dirUnitToNode2.x;
                    float vY = dirUnitToNode2.z;

                    
                    // produit scalaire élevé au carré pour le dénominateur
                    float num = uX * vX + uY * vY;
                    float numSqr = num * num;
                    
                    // norme U & V sans racine carrée
                    float den = (uX * uX + uY * uY) * (vX * vX + vY * vY);


                    float Mathf.Cos(num / den);*/




/*
                    float a = Vector3.Distance(transform.position, path[0].position);
                    float b = Vector3.Distance(transform.position, path[1].position);

                    if (b < a)
                    {

                    }*/

                    if (dirNode1ToUnit == dirUnitToNode2)
                    {

                        path.RemoveAt(0);
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

    private bool IsFirstColliderEnemy(Vector3 dir)
    {
        RaycastHit[] hits;
        Vector3 myPositionGrounded = new Vector3(transform.position.x, groundY, transform.position.z);
        hits = Physics.RaycastAll(myPositionGrounded, dir, attackRange);
        //RaycastHit[] hits = Physics.RaycastAll(weapon.P_FirePosition.position, dir, attackRange);
        if (hits.Length > 0)
        {
            Debug.DrawRay(myPositionGrounded, dir, Color.blue);
            //Debug.DrawRay(weapon.P_FirePosition.position, dir, Color.blue);
            if(hits[0].collider.tag == "Player")
            {
                return true;
            }
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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (path.Count < 1) return;
        for (int i = 0; i < path.Count; i++)
        {
                    Gizmos.color = Color.blue;
                    Vector3 uppedPosition = path[i].position;
                    uppedPosition.y += 3; 
                    Gizmos.DrawWireCube(path[i].position, new Vector3(1, 1, 1));
        }
    }

}
