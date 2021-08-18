using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using TeamExtensionMethods;
using Vector2 = System.Numerics.Vector2;

public enum eTurretType
{
    slowFire,
    rapidFire
}
public class Turret : DestroyableUnit
{
    [SerializeField] protected Transform partToRotate;
    [SerializeField] protected Weapon[] weapons;
    protected float health;

     protected AudioSource turretAudio;
    //[SerializeField] private GameObject turretPrefab;
   //[SerializeField] private GameObject bulletPrefab;
  
   [SerializeField] protected SO_Turret soTurret;
   public SO_Turret SoTurret => soTurret;
   protected TurretManager turretManager;
   protected Animator turretAnim;
   public Animator TurretAnim => turretAnim;

   private Vector3Int innerPos;
   public Vector3Int InnerPos => innerPos;
   private float groundY;
   protected bool soundPlayed;
   [SerializeField] private AudioClip deploySound;
   public AudioClip DeploySound => deploySound;
   [SerializeField] private bool isWall;
   [SerializeField] private bool isWall2;
   void Awake()
   {
       turretAnim = FindObjectOfType<Animator>();
   }
   protected override void Start()
   {
       base.Start();
       // InvokeRepeating("UpdateTarget", 0f, 0.5f);
       turretManager = FindObjectOfType<TurretManager>();
       bHealthPoints = soTurret.HealthPoints;
       healthPoints = bHealthPoints;
       groundY = GameManager.Instance.ActualGrid.CenterPosition.y;
       turretAudio = GetComponent<AudioSource>();

   }
   void Update()
    {
        Debug.Log(healthPoints);
        UpdateTarget();
        UpdateLifeBar();
        if (Object.ReferenceEquals(nearestTarget, null))
            return;
        Vector3 nearestTargetGrounded = new Vector3(nearestTarget.position.x, groundY, nearestTarget.position.z);
        Vector3 dir = nearestTargetGrounded - transform.position;
        //Vector3 dir = nearestTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * soTurret.TurnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        TryToShoot(dir);
    }

    
    // CALCUL DE DISTANCE AVEC UNE RACINE CARREE
    protected virtual void TryToShoot(Vector3 dir)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.CanShoot() && (nearestTarget.position - transform.position).sqrMagnitude <= weapon.P_BRange * weapon.P_BRange)
            {

                if (IsFirstColliderEnemy(dir,weapon.P_BRange))
                {
                    
                    if (!soundPlayed)
                    {
                        turretAudio.PlayOneShot(weapon.WeaponStats.WeaponSound);
                        soundPlayed = true;
                      
                    }
                    Shoot(dir,nearestTarget,weapon);

                }
            }
            else
            {
                turretAnim.SetBool("Shoot",false);
                if (soundPlayed)
                {
                   
                    soundPlayed = false;

                }
            }
        }
    }
    protected bool IsFirstColliderEnemy(Vector3 dir,float attackRange)
    {
       
        RaycastHit[] hitsArray = new RaycastHit[2];
        float radius = GetComponent<Collider>().bounds.size.x * 0.2f -0.1f;

        // Vector3 myPositionCenteredGrounded = new Vector3(transform.position.x, groundY, transform.position.z);
        Vector3 myPositionLeftGrounded = new Vector3(transform.position.x + radius, groundY, transform.position.z);
        Vector3 myPositionRightGrounded = new Vector3(transform.position.x - radius, groundY,transform.position.z);
        
Vector3 target =new Vector3( nearestTarget.transform.position.x,groundY,nearestTarget.transform.position.z);
        // Vector3 dirCenteredToTarget = dir- myPositionCenteredGrounded;
        Vector3 dirLeftToTarget = target- myPositionLeftGrounded;
        Vector3 dirRightToTarget = target - myPositionRightGrounded;

        // Physics.Raycast(myPositionCenteredGrounded, dir, out hitsArray[0], attackRange, ~GameManager.Instance.ActualGrid.videMask);
        bool ray2 = Physics.Raycast(myPositionLeftGrounded, dirLeftToTarget, out hitsArray[0], attackRange, ~GameManager.Instance.ActualGrid.videMask);
        bool ray3 =Physics.Raycast(myPositionRightGrounded, dirRightToTarget, out hitsArray[1], attackRange , ~GameManager.Instance.ActualGrid.videMask);

        if (ray2)
        {
            if (hitsArray[0].transform.GetComponent<Collider>().tag == "Obstacle")
            {
                Debug.DrawRay(myPositionLeftGrounded, dir,Color.green);
                Debug.DrawRay(myPositionRightGrounded, dir,Color.green);
               
                isWall = true;
                return false;
               
            }
            else
            {
                isWall = false;
            }
        }
        else if (ray3)
        {
            if (hitsArray[1].transform.GetComponent<Collider>().tag == "Obstacle")
            {
                Debug.DrawRay(myPositionLeftGrounded, dir,Color.green);
                Debug.DrawRay(myPositionRightGrounded, dir,Color.green);
              
                isWall2 = true;
                return false;
               
            }
            else
            {
                isWall2 = false;
            }
        }
        if (isWall || isWall2)
        {
            return false;
        }
        if (ray2 && !isWall && !isWall2)
        {   
           
            TeamUnit tu = hitsArray[0].collider.GetComponent<TeamUnit>();
            if(tu)
            {
                if (tu.Team.IsEnemy(this.team))
                {
                    Debug.DrawRay(myPositionLeftGrounded, dir,Color.green);
                    Debug.DrawRay(myPositionRightGrounded, dir,Color.green);
                    return true;
                }
            }
        } 
        if (ray3&& !isWall && !isWall2)
        {  
         
            TeamUnit tu = hitsArray[1].collider.GetComponent<TeamUnit>();
            if(tu)
            {
                if (tu.Team.IsEnemy(this.team))
                {
                    Debug.DrawRay(myPositionLeftGrounded, dir,Color.green);
                    Debug.DrawRay(myPositionRightGrounded, dir,Color.green);
                    return true;
                }
            }
        }
       
        return false;
    }
    public void Deploy(Vector3 position, Vector3Int innerPos, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        
        GameManager.Instance.ActualGrid.Nodes[innerPos.x, innerPos.z].isWalkable = false;
        GameManager.Instance.ActualGrid.Nodes[innerPos.x, innerPos.z].isTurretable = false;

        this.innerPos = innerPos;
        turretAnim.SetBool("canDeploy",true);
       
    }
    public void Shoot(Vector3 direction, Transform _target,Weapon weapon)
    {
        weapon.Team = team;
        turretAnim.SetBool("canDeploy",false);
        turretAnim.SetBool("Shoot",true);
        weapon.Shoot(direction, _target);
      
    }

    public void Shoot(Vector3 direction, Weapon weapon)
    {
        weapon.Team = team;
        weapon.Shoot(direction);
       
        
    }

    protected override void Die()
    {
        GameManager.Instance.ActualGrid.Nodes[innerPos.x, innerPos.z].isWalkable = true;
        GameManager.Instance.ActualGrid.Nodes[innerPos.x, innerPos.z].isTurretable = true;
        turretManager.RemoveItemFromList(gameObject);
        Destroy(gameObject);
    }
        
    
}
