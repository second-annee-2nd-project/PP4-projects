using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerBehaviour : DestroyableUnit
{
   private Rigidbody rb;
   [SerializeField] private float speed;
   public float Speed => speed;
   private Vector3 movement;
   [SerializeField] private GameObject weaponTr;
   [SerializeField] private Weapon weapon;

   private GameObject weaponGO;

   [SerializeField] private int dropAmount;
   [SerializeField] private float gazeHoldTimer = 2f;
   [SerializeField] float animationBarSpeed;
  

   private float lastBulletShot;

   void Awake()
   {
      rb = GetComponent<Rigidbody>();
   }
   protected override void Start()
   {
      base.Start();
      
      PickUpWeapon(weapon.gameObject);
      if (weapon != null) weapon.Team = Team;
      
      GameManager.Instance.P_UiManager.Life_Img.fillAmount = healthPoints / bHealthPoints;
      GameManager.Instance.P_TeamManager.AddToTeam(team, gameObject);
   }

   void Update()
   {
      healthPoints = Mathf.Clamp(healthPoints, 0, bHealthPoints);
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
            weapon.Shoot(nearestTarget.position - weapon.P_FirePosition.position);
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

   public void DropWeapon()
   {
      Destroy(weaponGO);
      weaponGO = null;
      weapon = null;
   }

   private void UpdateLifeBar()
   {
      float actualHealth = healthPoints / bHealthPoints;

      if (GameManager.Instance.P_UiManager.Life_Img_Damage_Heal.fillAmount > actualHealth)
      {
         GameManager.Instance.P_UiManager.Life_Img.fillAmount = actualHealth;
         GameManager.Instance.P_UiManager.Life_Img_Damage_Heal.color = Color.red;
         GameManager.Instance.P_UiManager.Life_Img_Damage_Heal.fillAmount  -= animationBarSpeed * Time.deltaTime;

      }
      else
      {
         GameManager.Instance.P_UiManager.Life_Img_Damage_Heal.fillAmount = actualHealth;
      }

      if (actualHealth > GameManager.Instance.P_UiManager.Life_Img.fillAmount)
      {
         GameManager.Instance.P_UiManager.Life_Img_Damage_Heal.color = Color.green;
         GameManager.Instance.P_UiManager.Life_Img_Damage_Heal.fillAmount = actualHealth;
         GameManager.Instance.P_UiManager.Life_Img.fillAmount  += animationBarSpeed * Time.deltaTime;
         
      }
      else
      {
         GameManager.Instance.P_UiManager.Life_Img.fillAmount = actualHealth;
      }

   }

   public void GainHealth()
   {
      healthPoints += bHealthPoints / 10;
   }
}
