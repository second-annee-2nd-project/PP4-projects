using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPiercingWalls : Bullet
{
    
    protected override void OnTriggerEnter(Collider col)
    {
        this.TestCollider(col);
    }
    protected override void TestCollider(Collider col)
    {
        // S'il ne faut pas détruire en fonction de ce que la balle touche
        Debug.Log("??????????");
        
        if (col.tag == "Bullet" || col.tag == "CoinsLoot") return;
        DestroyableUnit du = col.GetComponent<DestroyableUnit>();
        if (du)
        {
            if (du.Team != this.Team)
            {
                du.GetDamaged(damage);
                DestroyBullet();
            }
            else
            {
                DestroyBullet();
                return;
            }
        }
    }
}
