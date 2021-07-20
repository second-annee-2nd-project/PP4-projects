﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eWeaponType
{
    Pistol,
    Rifle,
    Shotgun,
    Launcher,
    Bonus
}

public class Weapon : MonoBehaviour
{
    [Header("Name")]
    private string name;
    private eWeaponType weaponType;

    [Header("Base Stats")] 
    [SerializeField] private SO_Weapon weaponStats;

    public SO_Weapon WeaponStats => weaponStats;
    private int bAmmo;
    private float bReloadTimer;
    private float bFireRate;

    private int bNumberOfBullets;
    private float bDiffusionAngle;

    private float bDamage;
    private float bRange;

    public float P_BRange => bRange;
    // [SerializeField] private float bDiffusionAngle;

    [Header("Actual Stats")]
    private int ammo;
    private float reloadTimer;
    private float fireRate;
    private float nextFire;

    private int numberOfBullets;
    private float diffusionAngle;
    
    private bool reloading;

    [Header("Bullet")]
    [SerializeField]  private Transform firePosition;

    public Transform P_FirePosition => firePosition;
    [SerializeField]  private GameObject bulletPrefab;
    [SerializeField] private BulletsPool bulletsPool;
    private eTeam team;

    public eTeam Team
    {
        get => team;
        set => team = value;
    }

    private Bullet bulletPrefabScript;
   

    private Transform _tr;
    private SpriteRenderer _sr;

    void Awake()
    {
        _tr = transform;
       // _sr = GetComponent<SpriteRenderer>();
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bRange);
    }

    void Start()
    {
        
        Init();
    }

    public void Init()
    {
        //b Stats
        bAmmo = weaponStats.MaxAmmo;
        bReloadTimer = 0f;
        bFireRate = weaponStats.FireRate;
        bNumberOfBullets = weaponStats.NumberOfBulletsPerShot;
        bDiffusionAngle = weaponStats.DiffusionAngle;
        bDamage = weaponStats.Damage;
        bRange = weaponStats.Range;
    
        //actual stats    
        ammo = bAmmo;
        reloadTimer = bReloadTimer;
        fireRate = bFireRate;
        nextFire = 0f;

        if(bNumberOfBullets == 0)
        {
            bNumberOfBullets = 1;
        }
        numberOfBullets = bNumberOfBullets;

        bulletPrefabScript = bulletPrefab.GetComponent<Bullet>();
        bulletsPool = FindObjectOfType<BulletsPool>();
        // diffusionAngle = bDiffusionAngle;
    }

    void Update()
    {
        if (nextFire > 0f)
            nextFire -= Time.deltaTime;

        if(reloading)
        {
            
            if(reloadTimer > 0f)
                reloadTimer -= Time.deltaTime;
            else
            {
                reloading = false;

                reloadTimer = bReloadTimer;
                ammo = bAmmo;
            }
        }
    }

    public void Shoot(Vector3 direction)
    {
        if (ammo == 0)
        {
            Reload();
            return;
        }

        float newAngle = bDiffusionAngle % 360f;
        float bHalfAngle = newAngle / 2f;
        float angleToAdd = bDiffusionAngle / numberOfBullets;
        for (int i = 0; i < numberOfBullets; i++)
        {
            float nextAngle = -bHalfAngle + i * angleToAdd * Mathf.Deg2Rad;

            float newDirectionX = direction.x * Mathf.Cos(nextAngle) - direction.z * Mathf.Sin(nextAngle);
            float newDirectionZ = direction.x * Mathf.Sin(nextAngle) - direction.z * Mathf.Cos(nextAngle);
            
           GameObject newBullet = bulletsPool.GetNextBulletInstance(bulletPrefabScript.BulletType);
           newBullet.SetActive(true);
           Bullet newBulletScript = newBullet.GetComponent<Bullet>();
           newBullet.transform.position = firePosition.position;
           newBulletScript.Team = Team;
           newBulletScript.MaxRange = bRange;
           newBulletScript.Damage = bDamage;
           
           newBulletScript.Shoot(direction);
           ammo--;
        }
        nextFire = fireRate;
    }

    public void Shoot(Vector3 direction, Transform target)
    {
        if (ammo == 0)
        {
            Reload();
            return;
        }
        
        for (int i = 0; i < numberOfBullets; i++)
        {
            GameObject newBullet = bulletsPool.GetNextBulletInstance(bulletPrefabScript.BulletType);
            newBullet.SetActive(true);
            Bullet newBulletScript = newBullet.GetComponent<Bullet>();
            newBullet.transform.position = firePosition.position;
            newBulletScript.Team = Team;
            newBulletScript.MaxRange = bRange;
            newBulletScript.Damage = bDamage;
            newBulletScript.Target = target;
            newBulletScript.Shoot(direction);
            ammo--;
        }

        nextFire = fireRate;
    }

    private void Reload()
    {
        reloading = true;
        //_sr.color = Color.red;
    }

    public bool CanShoot()
    {
        if (nextFire <= 0f)
        {
            return true;
        }

        return false;
    }

}