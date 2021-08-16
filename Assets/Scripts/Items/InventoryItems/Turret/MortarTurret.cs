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
            if (weapon.CanShoot() && (nearestTarget.position - transform.position).sqrMagnitude <= weapon.P_BRange * weapon.P_BRange)
            {
                if (!soundPlayed)
                {
                    turretAudio.PlayOneShot(weapon.WeaponStats.WeaponSound);
                    soundPlayed = true;
                }
                Shoot(dir,nearestTarget,weapon);
            }
            else
            {
                turretAnim.SetBool("Shoot",false);
                if (soundPlayed)
                {
                  
                    soundPlayed = false;
                    turretAudio.Stop();

                }
            }
        }
    }
}
