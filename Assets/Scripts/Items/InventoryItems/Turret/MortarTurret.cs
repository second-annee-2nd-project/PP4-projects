using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTurret : Turret
{
    // CALCUL DE DISTANCE AVEC UNE RACINE CARREE
    protected override void TryToShoot(Vector3 dir)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.CanShoot() && Vector3.Distance(nearestTarget.position, transform.position) <= weapon.P_BRange)
            {
                    Shoot(dir,nearestTarget,weapon);
            }
        }
    }
}
