using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowerWeapon : Weapon
{
    private Transform nearestEnemy;
    [SerializeField] private List<ParticleSystem> particleSystems;

    private bool isShooting = true;
    private bool lastIsShooting = false;
    [SerializeField] private float timeForFireToStop = 2f;
    private float remainingTimeForFireToStop = 0f;

    public override void Init()
    {
        base.Init();
        foreach (ParticleSystem particleSystem in particleSystems)
        {
            ParticleSystem.ShapeModule sm = particleSystem.shape;
            sm.angle = weaponStats.DiffusionAngle;
            ParticleSystem.MainModule mm = particleSystem.main;
            mm.startSpeed = weaponStats.Range / particleSystem.main.startLifetimeMultiplier;
            particleSystem.Stop();
        }
    }

    public override void Shoot(Vector3 direction)
    {
        isShooting = true;
        remainingTimeForFireToStop = timeForFireToStop;
        List<GameObject> allEnemies = GameManager.Instance.P_TeamManager.GetStrictEnemies(Team);

        for (int i = 0; i < allEnemies.Count; i++)
        {
            Vector3 dir = allEnemies[i].transform.position - P_FireTransform.position;

            if (dir.sqrMagnitude > weaponStats.Range * weaponStats.Range /*|| dir.sqrMagnitude <= 0.3f*/)
                continue;
            
            // Debug.Log("Player is in range!");

            Vector3 vecToEnemy = allEnemies[i].transform.position - transform.position;
            // --- Orientation check
            float deltaAngle = Vector3.Angle(vecToEnemy, direction);
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

        if (remainingTimeForFireToStop > 0)
        {
            remainingTimeForFireToStop -= Time.deltaTime;
        }
        if(remainingTimeForFireToStop <= 0f)
        {
           
            isShooting = false;
        }
        
        
        if (isShooting != lastIsShooting)
        {
            if (isShooting)
            {
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    particleSystem.Play();
                }
            }
            else
            {
                foreach (ParticleSystem particleSystem in particleSystems)
                {
                    particleSystem.Stop();
                }
            }
            lastIsShooting = isShooting;
        }

        
    }

    public override bool CanShoot()
    {
        return true;
        if (nearestEnemy == null) return true;
        
        if((nearestEnemy.position - P_FireTransform.position).sqrMagnitude <= weaponStats.Range * weaponStats.Range)
            return true;

        return false;
    }
}
