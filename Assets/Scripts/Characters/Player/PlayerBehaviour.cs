using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBehaviour : DestroyableUnit
{
   private Rigidbody rb;
   [SerializeField] private float speed;
   public float Speed => speed;
   private Vector3 movement;
   [SerializeField] private Weapon weapon;
   [SerializeField] private int dropAmount;
   [SerializeField] private float gazeHoldTimer = 2f;

   private float lastBulletShot;
  
   private Transform nearestTarget;
 

   void Start()
   {
      rb = GetComponent<Rigidbody>();
      if (weapon != null) weapon.Team = Team;
   }

   void Update()
   {
      movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

      nearestTarget = GameManager.Instance.P_EnemiesManager.GetNearestTarget(transform.position);
      if (nearestTarget != null && lastBulletShot > 0)
      {
         transform.LookAt(nearestTarget.position);
         lastBulletShot -= Time.deltaTime;
         if (lastBulletShot <= 0f) lastBulletShot = 0;
      }
      else
      {
         transform.LookAt((transform.position + movement * speed * Time.fixedDeltaTime));
      }

      if (Input.GetKeyDown(KeyCode.Space))
      {
         if (weapon.CanShoot() && nearestTarget != null)
         {
            transform.LookAt(nearestTarget.position);
            weapon.Shoot(nearestTarget.position - transform.position);
            lastBulletShot = gazeHoldTimer;
            //weapon.Shoot(transform.forward);
         }
      }
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
         Destroy(col.gameObject);
         ShopManager.Instance.Coins += dropAmount;

      }
   }
}
