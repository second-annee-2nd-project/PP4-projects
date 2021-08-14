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
   [SerializeField] private Transform weaponTr; 
  private Weapon weapon;
   public Weapon P_Weapon => weapon;

   private GameObject weaponGO;
   [SerializeField] private GameObject weaponPrefab;
   public GameObject WeaponGo => weaponGO;
   [SerializeField] private float gazeHoldTimer = 2f;

   [SerializeField] private float maxInvincibilityTimer;
   private bool frozen = false;
   private float invincibilityTimer = 0;
   [SerializeField] private Joystick shootJoystick;
   private Vector3 shotDir;
   [SerializeField] protected SO_Character characterStats;

   public SO_Character CharacterStats => characterStats;

   private float lastBulletShot;
   private Animator playerAnim;
   void Awake()
   {
      rb = GetComponent<Rigidbody>();
      playerAnim = GetComponent<Animator>();
   }
   protected override void Start()
   {
      base.Start();
      
   }

   public override void Init()
   {
      base.Init();
      transform.position = new Vector3(transform.position.x, GameManager.Instance.ActualGrid.CenterPosition.y - GameManager.Instance.ActualGrid.P_GridHeight *0.5f , transform.position.z);
      bHealthPoints = characterStats.HealthPoints;
      healthPoints = bHealthPoints;
      if (weaponPrefab != null)
      {
        
         weaponGO =  InstantiateWeapon(weaponPrefab);
         weapon = weaponGO.GetComponent<Weapon>();
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
      CheckWeaponToAnim();
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
      shotDir =  new Vector3(shootJoystick.Horizontal(), 0, shootJoystick.Vertical());

      if(movement.z == 0 && shootJoystick._IsTouching)
      {
         SetAnimDirection("Arriere",true);
      } 
      if (Mathf.Sign(movement.z * shotDir.z) < 0)
      {
         SetAnimDirection("Arriere",true);
      } 
      if(Mathf.Sign(movement.z * shotDir.z) > 0)
      {
         SetAnimDirection("Avant",true);
      }
      if(movement.x > 0 && shootJoystick._IsTouching)
      {
         SetAnimDirection("Droite",true);
      } 
      if(movement.x < 0 && shootJoystick._IsTouching )
      {
         SetAnimDirection("Gauche",true);
      }
      
      if (movement == Vector3.zero)
      {
         playerAnim.SetBool("Avant",false);
         playerAnim.SetBool("Gauche",false);
         playerAnim.SetBool("Arriere",false);
         playerAnim.SetBool("Droite",false);
      }
      if (Input.GetKey(KeyCode.Space) || shootJoystick._IsTouching)
      {
        Shoot();
        transform.LookAt((transform.position + shotDir * speed * Time.fixedDeltaTime));
      }
      else
      {
         transform.LookAt((transform.position + movement * speed * Time.fixedDeltaTime));
      }
      PlayAnim();
    
   }
   void SetAnimDirection(string n, bool state)
   {
      switch (n)
      {
         case "Avant":
            playerAnim.SetBool("Avant", state);
            playerAnim.SetBool("Arriere", !state);
            playerAnim.SetBool("Droite", !state);
            playerAnim.SetBool("Gauche", !state);
            break;

         case "Arriere":
            playerAnim.SetBool("Arriere", state);
            playerAnim.SetBool("Avant", !state);
            playerAnim.SetBool("Droite", !state);
            playerAnim.SetBool("Avant", !state);
            break;

         case "Droite":
            playerAnim.SetBool("Droite", state);
            playerAnim.SetBool("Avant", !state);
            playerAnim.SetBool("Arriere", !state);
            playerAnim.SetBool("Gauche", !state);
            break;

         case "Gauche":
            playerAnim.SetBool("Gauche", state);
            playerAnim.SetBool("Avant", !state);
            playerAnim.SetBool("Arriere", !state);
            playerAnim.SetBool("Droite", !state);
            break;
      }
   }
   private GameObject InstantiateWeapon(GameObject go)
   {
      GameObject newGO = Instantiate(go);
      PickUpWeapon(newGO);
      return newGO;
   }
   public void Shoot()
   {
      if (weapon.CanShoot())
      {
         weapon.Shoot(transform.forward + shotDir);
      }
   }
   void FixedUpdate()
   {
      MoveCharacter(movement.normalized);
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
      go.transform.parent = weaponTr;
      go.transform.localPosition = Vector3.zero;
      go.transform.localRotation = Quaternion.identity;
      
      weaponGO = go;
      weapon = weaponGO.GetComponent<Weapon>();
      weapon.Team = team;
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
         playerAnim.SetBool("IsDead",true);
         GameManager.Instance.P_UI_Manager.RenderRetryButton(true);
      }
   }

   public void GainHealth()
   {
      if (ShopManager.Instance.Coins >= ShopManager.Instance.HealPrice && GameManager.Instance.P_UiManager.Life_Img.fillAmount < 1 )
      {
         healthPoints += bHealthPoints / 10;
         ShopManager.Instance.UpdateCoins(-ShopManager.Instance.HealPrice);

      }
      
   }
   
   protected override void UpdateTarget()
   {
      nearestTarget = GameManager.Instance.P_TeamManager.GetNearestVisibleEnemyUnit(transform.position, weapon.P_BRange, team);
   }

   void PlayAnim()
   {
      if(shootJoystick._IsTouching)
         playerAnim.SetBool("IsShooting",true);
      else
         playerAnim.SetBool("IsShooting",false);
       if (movement.x > 0 || movement.x < 0 || movement.z < 0 || movement.z > 0 && !shootJoystick._IsTouching)
      {
         playerAnim.SetBool("IsSafe",true);
      }
      else if (shootJoystick._IsTouching)
      {
         playerAnim.SetBool("IsSafe",false);
      }
       
   }

   void CheckWeaponToAnim()
   {
      
      if (weapon.WeaponStats.Name == "Shotgun")
      {
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Top"),0);
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Weapon"),1);
         
         if(shootJoystick._IsTouching)
            playerAnim.SetBool("IsShotgun",true);
         else
            playerAnim.SetBool("IsShotgun",false);
         if (movement.x > 0 || movement.x < 0 || movement.z < 0 || movement.z > 0 && !shootJoystick._IsTouching)
         {
            playerAnim.SetBool("IsSafe",true);
         }
         else if (shootJoystick._IsTouching)
         {
            playerAnim.SetBool("IsSafe",false);
         }
      }
      if (weaponPrefab.name == "Flame")
      {
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Top"),0);
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Weapon"),1);
         
         if(shootJoystick._IsTouching)
            playerAnim.SetBool("IsFlame",true);
         else
            playerAnim.SetBool("IsFlame",false);
         if (movement.x > 0 || movement.x < 0 || movement.z < 0 || movement.z > 0 && !shootJoystick._IsTouching)
         {
            playerAnim.SetBool("IsSafe",true);
         }
         else if (shootJoystick._IsTouching)
         {
            playerAnim.SetBool("IsSafe",false);
         }
      } 
      if (weaponPrefab.name == "Assaut")
      {
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Top"),0);
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Weapon"),1);
         
         if(shootJoystick._IsTouching)
            playerAnim.SetBool("Assaut",true);
         else
            playerAnim.SetBool("Assaut",false);
         if (movement.x > 0 || movement.x < 0 || movement.z < 0 || movement.z > 0 && !shootJoystick._IsTouching)
         {
            playerAnim.SetBool("IsSafe",true);
         }
         else if (shootJoystick._IsTouching)
         {
            playerAnim.SetBool("IsSafe",false);
         }
      }
   }
}
