using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParableBullet : Bullet
{
    [SerializeField] private float parableMaxHeight;
    public override void Shoot(Vector3 direction)
    {
        
        base.Shoot(direction);
    }

    private IEnumerator ShootParable(Vector3 direction)
    {
        float x = 0;
        float y = x * x - x;

        yield return null;
    }
}
