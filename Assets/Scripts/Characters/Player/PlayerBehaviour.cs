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

   [SerializeField] private GameObject weaponGO;
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
      if (healthPoints <= 0)
      {
         movement =Vector3.zero;
         return;
      }
        
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
         
         playerAnim.SetBool("Avant",false);
         playerAnim.SetBool("Gauche",false);
         playerAnim.SetBool("Arriere",false);
         playerAnim.SetBool("Droite",false);
         playerAnim.SetBool("IsShooting",false);
         playerAnim.SetBool("IsSafe",false);
         return;
      }
        
      movement = new Vector3(GameManager.Instance.Joystick.Horizontal(), 0, GameManager.Instance.Joystick.Vertical());
      shotDir =  new Vector3(shootJoystick.Horizontal(), 0, shootJoystick.Vertical());

      Animate(movement, shotDir);


      if (Input.GetKey(KeyCode.Space) || shootJoystick._IsTouching)
      {
        Shoot();
        transform.LookAt((transform.position + shotDir * speed * Time.fixedDeltaTime));
      }
      else
      {
         transform.LookAt((transform.position + movement * speed * Time.fixedDeltaTime));
      }
   }

   void Animate(Vector3 movement, Vector3 shotDir)
   {
      //le joueur ne bouge pas
      if (movement == Vector3.zero)
      {
         playerAnim.SetBool("Avant",false);
         playerAnim.SetBool("Gauche",false);
         playerAnim.SetBool("Arriere",false);
         playerAnim.SetBool("Droite",false);
      }
      else
      {
         //Le joueur est en train de bouger et de tirer
   
         //Direction
         //up
         //1
         //droite
         //2
         //bas
         //4
         //gauche
         //8
   
         //Tir
         //up
         //16
         //droite
         //32
         //bas
         //64
         //haut
         //128
   
         if (shootJoystick._IsTouching)
         {
            int a = 0;
            string[] baseDirections = {"Avant", "Gauche", "Arriere", "Droite"};
            string[] finalDirections = new string[4];
            if (Mathf.Abs(movement.x) >= Mathf.Abs(movement.z))
            {
               if (movement.x <= 0)
               {
                  for (int i = 0; i < finalDirections.Length; i++)
                  {
                     finalDirections[i] = baseDirections[(i + 1) % finalDirections.Length];
                  }
   
                  a += 1;
               }
               else if (movement.x > 0)
               {
                  for (int i = 0; i < finalDirections.Length; i++)
                  {
                     finalDirections[i] = baseDirections[(i + 3) % finalDirections.Length];
                  }
   
                  a += 2;
               }
            }
            else
            {
               if (movement.z <= 0)
               {
                  for (int i = 0; i < finalDirections.Length; i++)
                  {
                     finalDirections[i] = baseDirections[(i + 2) % finalDirections.Length];
                  }
   
                  a += 4;
               }
   
               else if (movement.z > 0)
               {
                  for (int i = 0; i < finalDirections.Length; i++)
                  {
                     finalDirections[i] = baseDirections[i];
                  }
   
                  a += 8;
               }
            }
   
            string s = "";
            for (int i = 0; i < finalDirections.Length; i++)
            {
               s += finalDirections[i];
            }
   
            // introduire un if == alors ça prend l'arrière
            if (Mathf.Abs(shotDir.x) > Mathf.Abs(shotDir.z))
            {
               if (shotDir.x <= 0)
               {
                  SetAnimDirection(finalDirections[3], true);
                  a += 16;
               }
   
               else if (shotDir.x > 0)
               {
                  SetAnimDirection(finalDirections[1], true);
                  a += 32;
               }
            }
            else if (Mathf.Abs(shotDir.x) < Mathf.Abs(shotDir.z))
            {
               if (shotDir.z <= 0)
               {
                  SetAnimDirection(finalDirections[2], true);
                  a += 64;
               }
               else if (shotDir.z > 0)
               {
                  SetAnimDirection(finalDirections[0], true);
                  a += 128;
               }
            }
            else
            {
               SetAnimDirection("Arriere", true);
            }
         }
         else
         {
            SetAnimDirection("Avant", true);
         }
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

      weaponGO = go;
      Vector3 pos = weaponGO.transform.position;
      Quaternion rot = WeaponGo.transform.rotation;
      weapon = weaponGO.GetComponent<Weapon>();
      weapon.Team = team;
     
      go.transform.parent = weaponTr;
      go.transform.localPosition = pos;
      go.transform.localRotation = rot;
     
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
      
      
      if ((movement.x > 0 || movement.x < 0 || movement.z < 0 || movement.z > 0) && !shootJoystick._IsTouching)
      {
         playerAnim.SetBool("IsSafe",true);
         
      }
      else if (shootJoystick._IsTouching && movement.x == 0 || movement.x == 0 || movement.z == 0 || movement.z == 0 || shootJoystick._IsTouching)
      {
         playerAnim.SetBool("IsSafe",false);
      }
   }

   void CheckWeaponToAnim()
   {
      
      if (weapon.WeaponStats.WeaponType == eWeaponType.Shotgun)
      {
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Top"),0);
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Weapon"),1);
         
         if(shootJoystick._IsTouching)
            playerAnim.SetBool("IsShotgun",true);
         else
            playerAnim.SetBool("IsShotgun",false);
      }
      if (weapon.WeaponStats.WeaponType == eWeaponType.FlameThrower)
      {
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Top"),0);
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Weapon"),1);
         
         if(shootJoystick._IsTouching)
            playerAnim.SetBool("IsFlame",true);
         else
            playerAnim.SetBool("IsFlame",false);
      } 
      if (weapon.WeaponStats.WeaponType == eWeaponType.Assault)
      {
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Top"),0);
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Weapon"),1);
         
         if(shootJoystick._IsTouching)
            playerAnim.SetBool("IsAssaut",true);
         else
            playerAnim.SetBool("IsAssaut",false);
      }
      if (weapon.WeaponStats.WeaponType == eWeaponType.Pistol)
      {
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Top"),1);
         playerAnim.SetLayerWeight(playerAnim.GetLayerIndex("Weapon"),0);
      }
   }
}
