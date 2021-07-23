﻿using System.Collections;
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
    [SerializeField] private Transform partToRotate;
    [SerializeField] private Weapon[] weapons;
    private float health;

    //[SerializeField] private GameObject turretPrefab;
   //[SerializeField] private GameObject bulletPrefab;
  
   [SerializeField] private SO_Turret soTurret;
   public SO_Turret SoTurret => soTurret;
   private TurretManager turretManager;
   protected override void Start()
   {
       base.Start();
       // InvokeRepeating("UpdateTarget", 0f, 0.5f);
       turretManager = FindObjectOfType<TurretManager>();
       health = soTurret.HealthPoints;
    }

    

    void Update()
    {
        UpdateTarget();
        if (nearestTarget == null)
            return;

        // SHOOT TOURELLE
            
        Vector3 dir = nearestTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * soTurret.TurnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        foreach (var weapon in weapons)
        {
            Debug.DrawRay(weapon.P_FireTransform.position, dir, Color.blue);
            if (weapon.CanShoot() && Vector3.Distance(nearestTarget.position, transform.position) < weapon.P_BRange)
            {

                if (IsFirstColliderEnemy(dir,weapon.P_BRange))
                {
                    Shoot(dir,nearestTarget,weapon);
                }
                

            }
        }
       
            
    }
    
    protected bool IsFirstColliderEnemy(Vector3 dir,float attackRange)
    {
        RaycastHit[] hits;
        Vector3 myPositionGrounded = new Vector3(transform.position.x, GameManager.Instance.ActualGrid.CenterPosition.y ,transform.position.z);
        hits = Physics.RaycastAll(myPositionGrounded, dir, attackRange);
        //RaycastHit[] hits = Physics.RaycastAll(weapon.P_FirePosition.position, dir, attackRange);
        if (hits.Length > 0)
        {
            Debug.DrawRay(myPositionGrounded, dir, Color.blue);
            //Debug.DrawRay(weapon.P_FirePosition.position, dir, Color.blue);
            TeamUnit tu = hits[0].collider.GetComponent<TeamUnit>();
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
    public void Deploy(Vector3 position, Quaternion rotation)
    {
        GetComponent<MeshRenderer>().enabled = true;
        transform.position = position;
        transform.rotation = rotation;
        //GameObject newTurret = GameObject.Instantiate(turretPrefab, position, rotation);
    }

    public void Shoot(Vector3 direction, Transform _target,Weapon weapon)
    {
        weapon.Team = team;
        weapon.Shoot(direction, _target);
    }

    protected override void Die()
    {
        turretManager.RemoveItemFromList(gameObject);
        Destroy(gameObject);
    }
        
    
}
