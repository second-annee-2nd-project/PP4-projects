using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private Transform target;
   
    [SerializeField] private Transform partToRotate;
    [SerializeField] private Weapon weapon;
    private float health;

    //[SerializeField] private GameObject turretPrefab;
   //[SerializeField] private GameObject bulletPrefab;
   [SerializeField] private Transform firePoint;
   [SerializeField] private SO_Turret soTurret;
   public SO_Turret SoTurret => soTurret;
   [SerializeField] private TurretManager turretManager;
   private void Start()
    {
       // InvokeRepeating("UpdateTarget", 0f, 0.5f);
       turretManager = FindObjectOfType<TurretManager>();
       health = soTurret.HealthPoints;
    }

    void UpdateTarget()
    {
        
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        float shortsDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in ennemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortsDistance)
            {
                shortsDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }

        }

        if (nearestEnemy != null && shortsDistance <= soTurret.Range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        UpdateTarget();
        if (target == null)
            return;

        // SHOOT TOURELLE
            
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * soTurret.TurnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        if(weapon.CanShoot())
            Shoot(dir, target);
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
            turretManager.turretList.Remove(transform);
            Destroy(gameObject);
        }
    }
    
}
