using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol,
        Rifle,
        Shotgun,
        Launcher,
        Bonus
    }

    [Header("Name")]
    private string name;
    private WeaponType weaponType;

    [Header("Base Stats")]
    [SerializeField] private int bAmmo;
    [SerializeField] private float bReloadTimer;
    [SerializeField] private float bFireRate;

    [SerializeField] private int bNumberOfBullets;
    [SerializeField] private float bDiffusionAngle;

    [SerializeField] private float bDamage;
    [SerializeField] private float bRange;
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
        _sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
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
        

        for (int i = 0; i < numberOfBullets; i++)
        {
            /*
            Angle de diffusion 
            float halfDiffusionAngle = diffusionAngle / 2f;
            float addAngle = diffusionAngle / numberOfBullets;
            float nextAngle = halfDiffusionAngle - diffusionAngle + i * addAngle;
            float nextAngleInRadians = nextAngle * Mathf.PI / 180f;
            
            Vector3 playerPlus1 = 
            
            */
            
           GameObject newBullet = bulletsPool.GetNextBulletInstance(bulletPrefabScript.BulletType);
           newBullet.SetActive(true);
           Bullet newBulletScript = newBullet.GetComponent<Bullet>();
           newBullet.transform.position = firePosition.position;
           newBulletScript.MaxRange = bRange;
           newBulletScript.Damage = bDamage;
           newBulletScript.Shoot(direction);
           
           

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
            /*
            Angle de diffusion 
            float halfDiffusionAngle = diffusionAngle / 2f;
            float addAngle = diffusionAngle / numberOfBullets;
            float nextAngle = halfDiffusionAngle - diffusionAngle + i * addAngle;
            float nextAngleInRadians = nextAngle * Mathf.PI / 180f;
            
            Vector3 playerPlus1 = 
            
            */
            
            GameObject newBullet = bulletsPool.GetNextBulletInstance(bulletPrefabScript.BulletType);
            newBullet.SetActive(true);
            Bullet newBulletScript = newBullet.GetComponent<Bullet>();
            newBullet.transform.position = firePosition.position;
            newBulletScript.MaxRange = bRange;
            newBulletScript.Damage = bDamage;
            newBulletScript.Target = target;
            newBulletScript.Shoot(direction);
        }

        nextFire = fireRate;
    }

    public void Reload()
    {
        reloading = true;
        _sr.color = Color.red;
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