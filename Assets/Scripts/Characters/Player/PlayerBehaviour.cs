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
   [SerializeField] private Weapon weapon;
   [SerializeField] private int dropAmount;
   [SerializeField] private float gazeHoldTimer = 2f;
   [SerializeField] private Image life_Img;

   private float lastBulletShot;
  

   protected override void Start()
   {
      base.Start();
      rb = GetComponent<Rigidbody>();
      if (weapon != null) weapon.Team = Team;
      
      life_Img.fillAmount = healthPoints / bHealthPoints;
      GameManager.Instance.P_TeamManager.AddToTeam(team, gameObject);
   }

   void Update()
   {
      life_Img.fillAmount = healthPoints / bHealthPoints;
      movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

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

      if (Input.GetKey(KeyCode.Space))
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

     // healthPoints -= Time.deltaTime;
   }

   void FixedUpdate()
   {
      MoveCharacter(movement);
   }

   void MoveCharacter(Vector3 direction)
   {
      rb.velocity = direction * speed * Time.fixedDeltaTime;
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
}
