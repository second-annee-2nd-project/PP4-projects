using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
   
    private Rigidbody rb;

    [SerializeField] private float speed;
    [SerializeField] private float damage;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.velocity = Vector3.back * speed * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == ("Turret"))
        {
            col.GetComponent<Turret>().TakeDamage(damage);
        }
    }

}
