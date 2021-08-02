using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBehaviour : DestroyableUnit
{
   private Rigidbody rb;

   [Header("Can die & Respawn : ")] 
   [SerializeField] private bool canDie;
   [SerializeField] private Transform spawnTransform;
   [Header("Stats")]
   [SerializeField] private float speed;
   public float Speed => speed;
   private Vector3 movement;
   [SerializeField] private GameObject weaponTr;
   [SerializeField] private Weapon weapon;

   private GameObject weaponGO;
   [SerializeField] private float gazeHoldTimer = 2f;

   [SerializeField] private float maxInvincibilityTimer;
   private bool frozen = false;
   private float invincibilityTimer = 0;
  

   private float lastBulletShot;

   void Awake()
   {
      rb = GetComponent<Rigidbody>();
   }
   protected override void Start()
   {
      base.Start();
      Init();

   }

   public override void Init()
   {
      base.Init();

      if (weapon != null)
      {
         InstantiateWeapon(weapon.gameObject);
         weapon.Team = Team;
         
      }

      if(spawnTransform != null)
         transform.position = spawnTransform.position;
      
      GameManager.Instance.P_UiManager.Life_Img.fillAmount = healthPoints / bHealthPoints;
      GameManager.Instance.P_TeamManager.AddToTeam(team, gameObject);
   }
   
   public override void Restart()
   {
      Init();
   }

   void Update()
   {
      healthPoints = Mathf.Clamp(healthPoints, 0, bHealthPoints);
      if (invincibilityTimer > 0)
      {
         invincibilityTimer -= Time.deltaTime;
         // Changer de couleur ici j'imagine
      }
      UpdateLifeBar();
      if (GameManager.Instance.EGameState != eGameState.Wave)
      {
         movement = Vector3.zero;
         return;
      }
        
      movement = new Vector3(GameManager.Instance.Joystick.Horizontal(), 0, GameManager.Instance.Joystick.Vertical());

      nearestTarget = GameManager.Instance.P_TeamManager.GetNearestEnemyUnit(transform.position, team);
      
      if (nearestTarget != null && lastBulletShot > 0)
      {
         transform.LookAt(nearestTarget);
         lastBulletShot -= Time.deltaTime;
         if (lastBulletShot <= 0f) lastBulletShot = 0;
      }
      else
      {
         transform.LookAt((transform.position + movement * speed * Time.fixedDeltaTime));
      }

      if (Input.GetKey(KeyCode.Space) || GameManager.Instance.FireButton.FirePressed)
      {
        Shoot();
      }
   }

   public void Shoot()
   {
      if (weapon.CanShoot())
      {
         if (nearestTarget != null)
         {
            transform.LookAt(nearestTarget.position);
            weapon.Shoot(nearestTarget.position - weapon.P_FireTransform.position);
            lastBulletShot = gazeHoldTimer;
            //weapon.Shoot(transform.forward);
         }
         else
         {
            weapon.Shoot(transform.forward);
         }
      }
   }
   void FixedUpdate()
   {
      MoveCharacter(movement);
   }

   void MoveCharacter(Vector3 direction)
   {
      rb.velocity = direction * speed;
   }

   void OnTriggerEnter(Collider col)
   {
      if (col.gameObject.tag == ("CoinsLoot"))
      {
         col.GetComponent<Loot>().Looted(this);
      }
      else if (col.gameObject.tag == ("Enemy"))
      {
         
      }
   }

   public void Loot(int dropAmount)
   {
      ShopManager.Instance.UpdateCoins(dropAmount);
   }

   public void PickUpWeapon(GameObject go)
   {
      if (weapon != null) DropWeapon();
      go.transform.parent = weaponTr.transform;
      go.transform.localPosition = Vector3.zero;
      go.transform.localRotation = Quaternion.identity;
      
      weaponGO = go;
      weapon = weaponGO.GetComponent<Weapon>();
      weapon.Team = team;
   }

   private void InstantiateWeapon(GameObject go)
   {
      GameObject newGO = Instantiate(go);
      PickUpWeapon(newGO);
   }

   public void DropWeapon()
   {
      Destroy(weaponGO);
      weaponGO = null;
      weapon = null;
   }

   public override void GetDamaged(float damage)
   {
      if (invincibilityTimer <= 0)
      {
         base.GetDamaged(damage);
         invincibilityTimer = maxInvincibilityTimer;
      }
   }

   protected override void Die()
   {
      if (canDie)
      {
         GameManager.Instance.P_UI_Manager.RenderRetryButton(true);
      }
   }

   public void GainHealth()
   {
      if (ShopManager.Instance.Coins >= ShopManager.Instance.HealPrice)
      {
         healthPoints += bHealthPoints / 10;
         ShopManager.Instance.UpdateCoins(-ShopManager.Instance.HealPrice);

      }
      
   }
   
   protected override void UpdateTarget()
   {
      nearestTarget = GameManager.Instance.P_TeamManager.GetNearestVisibleEnemyUnit(transform.position, weapon.P_BRange, team);
   }
}
