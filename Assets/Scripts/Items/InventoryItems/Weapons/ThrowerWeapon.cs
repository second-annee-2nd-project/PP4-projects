using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowerWeapon : Weapon
{
    private Transform nearestEnemy;
    
    public override void Shoot(Vector3 direction)
    {
        List<GameObject> allEnemies = GameManager.Instance.P_TeamManager.GetStrictEnemies(Team);
        

        for (int i = 0; i < allEnemies.Count; i++)
        {
            Vector3 dir = allEnemies[i].transform.position - P_FireTransform.position;

            if (dir.sqrMagnitude > weaponStats.Range * weaponStats.Range)
                continue;
            
            // Debug.Log("Player is in range!");

            // --- Orientation check
            float deltaAngle = Vector3.Angle(transform.forward, direction);
            //if (deltaAngle > weaponStats.DiffusionAngle/2f || deltaAngle < -weaponStats.DiffusionAngle/2f)
            if (deltaAngle > weaponStats.DiffusionAngle * .5f)
                continue;

            // Debug.Log("Player is in front of me!");

           /* DestroyableUnit du;
            // --- Raycast check
            if (Team.IsFirstColliderEnemy(transform.position, direction + transform.position, weaponStats.Range,
                out du))
                return;*/
            /*
                if (!Physics.Raycast(transform.position, direction.normalized, out RaycastHit hitInfo, weaponStats.Range))
                return false;
          
            if (!hitInfo.transform.CompareTag("Player"))
                return false;*/

            // Debug.Log("Raycast against Player successful!");
            DestroyableUnit du = allEnemies[i].GetComponent<DestroyableUnit>();
            if (du)
            {
                du.GetDamaged(weaponStats.Damage);
            }
        }

    }

    protected override void Update()
    {
        nearestEnemy = GameManager.Instance.P_TeamManager.GetNearestEnemyUnit(transform.position, team);
    }

    public override bool CanShoot()
    {
        if((nearestEnemy.position - P_FireTransform.position).sqrMagnitude <= weaponStats.Range * weaponStats.Range)
            return true;

        return false;
    }
}
