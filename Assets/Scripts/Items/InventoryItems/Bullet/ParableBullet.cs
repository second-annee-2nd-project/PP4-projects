using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParableBullet : Bullet
{
    [SerializeField] private float height = 1f;
    private const float gravity = 9.8f;
    // [SerializeField] private GameObject target; //on la deja
    // [SerializeField] private float speed = 10;  //on la deja
    private float verticalSpeed;
    private Vector3 moveDirection;
    private bool hitTarget;

    private float damageRadius;

    [SerializeField] private GameObject deathEffect;

    public float DamageRadius
    {
        get => damageRadius;
        set => damageRadius = value;
    }
    

    protected override IEnumerator DestroyOnMaxRange()
    {
         float time = 0;
         Vector3 lastPos = Vector3.zero;
         if(target != null)
             lastPos = target.transform.position;
         Vector3 positionGrounded = new Vector3(transform.position.x, lastPos.y, transform.position.z);
         
        while (lastPos != positionGrounded)
        {
            if(target != null)
                lastPos = target.transform.position;
            positionGrounded = new Vector3(transform.position.x, lastPos.y, transform.position.z);
            moveDirection = lastPos - positionGrounded;
            //Vector3.up pour donner l effet parabole et ajout des variable de temps, gravite et speed pour que la balle retombe en fonction de la distance qu elle doit faire 
            time += Time.deltaTime;
            float fallAcrossTime = verticalSpeed - gravity * height * time;
            transform.Translate(Vector3.up  * fallAcrossTime * height * Time.deltaTime); 
            transform.Translate(moveDirection.normalized * speed * Time.deltaTime);

            if (transform.position.y < lastPos.y)
            {
                transform.position = positionGrounded;
                if (target == null)
                {
                    transform.position = lastPos;
                }
            }
            
            
            yield return null;
        }
       
        cor = null;
        DestroyBullet();
    }

    public override void Shoot(Vector3 direction)
    {
        
        if (height <= 0) height = 1f;

        hitTarget = false;
        Vector3 positionGrounded = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
        float distance = Vector3.Distance(positionGrounded, target.transform.position); //on la deja
        float tempTime = distance/speed; //remplacer par ce qu on a deja 
        float riseTime = tempTime/2;
        verticalSpeed = gravity * riseTime *height;
        
        if (cor == null)
            cor = StartCoroutine(DestroyOnMaxRange());
       
    }
/*protected override void TestCollider(Collider col)
    {
        if (col.tag == "Bullet" || col.tag == "CoinsLoot") return;
        Collider[] hitColliders = new Collider[20];
        int size = Physics.OverlapSphereNonAlloc(transform.position, damageRadius, hitColliders);
        
        //play anim

        for (int i = 0; i < size; i++)
        {
            DestroyableUnit du = hitColliders[i].GetComponent<DestroyableUnit>();
            if (du)
            {
                if (du.Team != this.Team)
                {
                    du.GetDamaged(damage);
                }
            }
        }

        DestroyBullet();
    }*/

    
    
    protected override void DestroyBullet()
    {
        // coûte sûrement moins que l'overlapSphere
        
        /*List<GameObject> allEnemies = GameManager.Instance.P_TeamManager.GetAllEnemies(Team);

        allEnemies = GameManager.Instance.P_TeamManager.GetEnemiesInRange(transform.position, damageRadius, Team);
        
        for (int i = 0; i < allEnemies.Count; i++)
        {
            DestroyableUnit du = allEnemies[i].GetComponent<DestroyableUnit>();
            du.GetDamaged(damage);
        }*/
      
        Collider[] hitColliders = new Collider[20];
        int size = Physics.OverlapSphereNonAlloc(transform.position, damageRadius, hitColliders);
        
        //play anim

        for (int i = 0; i < size; i++)
        {
            DestroyableUnit du = hitColliders[i].GetComponent<DestroyableUnit>();
            if (du)
            {
                if (du.Team != this.Team)
                {
                    du.GetDamaged(damage);
                }
            }
        }
        
        Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity), 2);
        
        cor = null;
        Team = eTeam.neutral;
        bulletsPool.ReleaseBulletInstance(gameObject, bulletType);
    }
}
