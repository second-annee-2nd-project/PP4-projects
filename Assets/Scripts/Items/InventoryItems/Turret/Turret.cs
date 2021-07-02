using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public enum eTurretType
{
    slowFire,
    rapidFire
}
public class Turret : DestroyableUnit
{
    [SerializeField] private Transform partToRotate;
    [SerializeField] private Weapon weapon;
    private float health;

    //[SerializeField] private GameObject turretPrefab;
   //[SerializeField] private GameObject bulletPrefab;
   [SerializeField] private Transform firePoint;
   [SerializeField] private SO_Turret soTurret;
   public SO_Turret SoTurret => soTurret;
   private TurretManager turretManager;
   private void Start()
    {
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
        if (weapon.CanShoot() && Vector3.Distance(nearestTarget.position, transform.position) < weapon.P_BRange)
        {
            RaycastHit[] hits = Physics.RaycastAll(weapon.P_FirePosition.position, dir, weapon.P_BRange);
            if (hits.Length > 0)
            {
                Debug.DrawRay(weapon.P_FirePosition.position, dir, Color.blue);
                Debug.Log("Le nom du raycast touché : " + hits[0].collider.name + "\nPosition : "+hits[0].collider.gameObject.transform.position);
                if (hits[0].collider.tag == "Enemy")
                {
                    Shoot(dir, nearestTarget);
                }
            }
            
        }
            
    }

    public void Deploy(Vector3 position, Quaternion rotation)
    {
        GetComponent<MeshRenderer>().enabled = true;
        transform.position = position;
        transform.rotation = rotation;
        //GameObject newTurret = GameObject.Instantiate(turretPrefab, position, rotation);
    }

    public void Shoot(Vector3 direction, Transform _target)
    {
        Debug.Log(gameObject.name);
        weapon.Shoot(direction, _target);
    }

    public void TakeDamage(float amount)
    {
        health-= amount;
       TurretDeath();
    }

    void TurretDeath()
    {
        if (health<= 0)
        {
            GameManager.Instance.P_TurretManager.RemoveItemFromList(gameObject);
            Destroy(gameObject);
        }
    }
    
}
