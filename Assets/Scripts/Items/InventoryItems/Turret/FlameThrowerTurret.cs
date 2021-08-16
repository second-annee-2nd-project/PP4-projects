using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerTurret : Turret
{
    protected override void TryToShoot(Vector3 dir)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.CanShoot() && (nearestTarget.position - weapon.P_FireTransform.position).sqrMagnitude <= weapon.WeaponStats.Range * weapon.WeaponStats.Range)
            {
                if (!soundPlayed)
                {
                    turretAudio.PlayOneShot(weapon.WeaponStats.WeaponSound);
                    soundPlayed = true;
                    Debug.Log("marche");
                }
                Shoot(dir, weapon);
                turretAnim.SetBool("canDeploy",false);
                turretAnim.SetBool("Shoot",true);
                
                
                
            }
            else
            {
                turretAnim.SetBool("Shoot",false);
                if (soundPlayed)
                {
                    Debug.Log("marchePas");
                    soundPlayed = false;

                }
            }
        }
    }
}
