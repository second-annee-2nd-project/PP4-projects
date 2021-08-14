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

    //[SerializeField] private GameObject turretPrefab;
   //[SerializeField] private GameObject bulletPrefab;
  
   [SerializeField] protected SO_Turret soTurret;
   public SO_Turret SoTurret => soTurret;
   protected TurretManager turretManager;
   protected Animator turretAnim;
   public Animator TurretAnim => turretAnim;

   private Vector3Int innerPos;
   public Vector3Int InnerPos => innerPos;

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
    
     
   }
   void Update()
    {
        UpdateTarget();
        UpdateLifeBar();
        if (nearestTarget == null)
            return;

        Vector3 dir = nearestTarget.position - transform.position;
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
            Debug.DrawRay(transform.position, dir, Color.blue);
            if (weapon.CanShoot() && Vector3.Distance(nearestTarget.position, transform.position) <= weapon.P_BRange)
            {

                if (IsFirstColliderEnemy(dir,weapon.P_BRange))
                {
                    Shoot(dir,nearestTarget,weapon);
                }
            }
            else
            {
                turretAnim.SetBool("Shoot",false);
            }
        }
    }
    protected bool IsFirstColliderEnemy(Vector3 dir,float attackRange)
    {
        RaycastHit[] hits;
        Vector3 myPositionGrounded = new Vector3(transform.position.x, GameManager.Instance.ActualGrid.CenterPosition.y, transform.position.z);
        hits = Physics.RaycastAll(myPositionGrounded, dir, attackRange);
        //RaycastHit[] hits = Physics.RaycastAll(weapon.P_FirePosition.position, dir, attackRange);

        RaycastHit hit;
        if (Physics.Raycast(myPositionGrounded, dir, out hit, attackRange))
        {
            TeamUnit tu = hit.collider.GetComponent<TeamUnit>();
            if(tu)
            {
                if (tu.Team.IsEnemy(this.team))
                {
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
