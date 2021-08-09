using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParableBullet : Bullet
{
    private const float gravity = 9.8f;
    // [SerializeField] private GameObject target; //on la deja
    // [SerializeField] private float speed = 10;  //on la deja
    private float verticalSpeed;
    private Vector3 moveDirection;

    protected override IEnumerator DestroyOnMaxRange()
    {
         float time = 0;
         Vector3 lastPos = Vector3.zero;
         
        while (lastPos != transform.position)
        {
            if(target != null)
                lastPos = target.transform.position;
            moveDirection = lastPos - transform.position;
            //Vector3.up pour donner l effet parabole et ajout des variable de temps, gravite et speed pour que la balle retombe en fonction de la distance qu elle doit faire 
            time += Time.deltaTime;
            float fallAcrossTime = verticalSpeed - gravity * 5 * time;
            transform.Translate(Vector3.up  * fallAcrossTime * Time.deltaTime ); 
            transform.Translate(moveDirection.normalized * speed * Time.deltaTime);
            
            
            yield return null;
        }
       
      
    }

    public override void Shoot(Vector3 direction)
    {
        
        Debug.Log("ffrfriufn");
        float distance = Vector3.Distance(transform.position, target.transform.position); //on la deja
        float tempTime = distance/speed; //remplacer par ce qu on a deja 
        float riseTime = tempTime/2;
        verticalSpeed = gravity * riseTime *5;
        
        if (cor == null)
            cor = StartCoroutine(DestroyOnMaxRange());
       
    }
}
