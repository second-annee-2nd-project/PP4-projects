using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using TeamExtensionMethods;

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
   private bool isWall;
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
       
        RaycastHit[] hitsArray = new RaycastHit[3];
        float radius = transform.GetComponent<Collider>().bounds.size.x * 0.5f - 0.3f;

        Vector3 myPositionCenteredGrounded = new Vector3(transform.position.x, groundY, transform.position.z);
        Vector3 myPositionLeftGrounded = new Vector3(transform.position.x - radius, groundY, transform.position.z);
        Vector3 myPositionRightGrounded = new Vector3(transform.position.x + radius, groundY, transform.position.z);

        Vector3 dirCenteredToTarget = dir- myPositionCenteredGrounded;
        Vector3 dirLeftToTarget = dir- myPositionLeftGrounded;
        Vector3 dirRightToTarget = dir - myPositionRightGrounded;

        Physics.Raycast(myPositionCenteredGrounded, dir, out hitsArray[0], attackRange, ~GameManager.Instance.ActualGrid.videMask);
        Physics.Raycast(myPositionLeftGrounded, dir, out hitsArray[1], attackRange, ~GameManager.Instance.ActualGrid.videMask);
        Physics.Raycast(myPositionRightGrounded, dir, out hitsArray[2], attackRange , ~GameManager.Instance.ActualGrid.videMask);

       
        int numberOfHits = 0;
        for (int i = 0; i < hitsArray.Length; i++)
        {
           
                if (hitsArray[i].transform.collider.tag == "Obstacle")
                {
                    return false;
                }
                else if(hitsArray[i].collider.tag =="Enemy")
                {
                    TeamUnit tu = hitsArray[i].collider.GetComponent<TeamUnit>();
                    if(tu)
                    {
                        if (tu.Team == eTeam.player)
                        {
                            Debug.Log("fck");
                            numberOfHits++;
                           
                        }
                    }
                }
              
            
        }

        if (numberOfHits == hitsArray.Length)
        {
            Debug.Log("yay");
            return true;
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
