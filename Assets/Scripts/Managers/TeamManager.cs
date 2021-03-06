using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamExtensionMethods;

public class TeamManager : MonoBehaviour
{
   private Dictionary<eTeam, List<GameObject>> differentTeams;

    public void Init()
    {
        differentTeams = new Dictionary<eTeam, List<GameObject>>();
        //https://stackoverflow.com/questions/105372/how-to-enumerate-an-enum
        foreach (eTeam teams in (eTeam[]) Enum.GetValues(typeof(eTeam)))
        {
            differentTeams.Add(teams, new List<GameObject>());
        }
    }


    public void AddToTeam(eTeam team, GameObject go)
    {
        differentTeams[team].Add(go);
    }

    public void RemoveAndDestroyFromTeam(eTeam team, GameObject go)
    {
        differentTeams[team].Remove(go);
        Destroy(go);
    }
    
    //Retourner les ennemis directs en ommetant les neutral
    public List<GameObject> GetStrictEnemies(eTeam team)
    {
        eTeam[] strictEnemies = team.GetStrictEnemies();
        List<GameObject> listToReturn = new List<GameObject>();
        foreach (eTeam t in strictEnemies)
        {
            listToReturn.AddRange(differentTeams[t]);
        }

        return listToReturn;
    }

    public List<GameObject> GetAllEnemies(eTeam team)
    {
        eTeam[] strictEnemies = team.GetStrictEnemies();
        List<GameObject> listToReturn = new List<GameObject>();
        foreach (eTeam t in strictEnemies)
        {
            listToReturn.AddRange(differentTeams[t]);
        }

        return listToReturn;
    }

    public List<GameObject> GetNeutralEnemies()
    {
        return differentTeams[eTeam.neutral];
    }

    public List<GameObject> GetAllUnits()
    {
        List<GameObject> listToReturn = new List<GameObject>();
        foreach (eTeam t in (eTeam[]) Enum.GetValues(typeof(eTeam)))
        {
            listToReturn.AddRange(differentTeams[t]);
        }

        return listToReturn;
    }

    public Transform GetNearestEnemyUnit(Vector3 pos, eTeam allyTeam)
    {
        List<GameObject> targets = GetStrictEnemies(allyTeam);
        float squaredShortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;
        Vector3 enemyTestedPos;
        float squaredDistanceToEnemy;

        foreach (GameObject target in targets)
        {
            enemyTestedPos = target.transform.position;
            squaredDistanceToEnemy = (pos.x - enemyTestedPos.x) * (pos.x - enemyTestedPos.x) + (pos.z - enemyTestedPos.z) * (pos.z - enemyTestedPos.z);

            if (squaredDistanceToEnemy < squaredShortestDistance )
            {
                squaredShortestDistance = squaredDistanceToEnemy;
                nearestEnemy = target.transform;
            }
        }
        return nearestEnemy;
    }

    public List<GameObject> GetEnemiesInRange(Vector3 pos, float range, eTeam allyTeam)
    {
        List<GameObject> targets = GetStrictEnemies(allyTeam);
        List<GameObject> enemiesToReturn = new List<GameObject>();

        for (int i = 0; i < targets.Count; i++)
        {
            if ((targets[i].transform.position - pos).sqrMagnitude <= range * range)
            {
                enemiesToReturn.Add(targets[i]);
            }
        }

        return enemiesToReturn;
    }

    public Transform GetNearestVisibleEnemyUnit(Vector3 pos, float range, eTeam allyTeam)
    {
        List<GameObject> targets = GetStrictEnemies(allyTeam);
        float squaredShortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;
        Vector3 enemyTestedPos;
        float squaredDistanceToEnemy;
        Vector3 dirToEnemy;

        foreach (GameObject target in targets)
        {
            enemyTestedPos = target.transform.position;
            squaredDistanceToEnemy = (pos.x - enemyTestedPos.x) * (pos.x - enemyTestedPos.x) + (pos.z - enemyTestedPos.z) * (pos.z - enemyTestedPos.z);
            dirToEnemy = enemyTestedPos - pos;

            // Ray
            RaycastHit hit;
            Physics.Raycast(pos, dirToEnemy, out hit, range);
            
            bool isEnemy = true;
            
            if (hit.collider == null) isEnemy = false;
            else
            {
                DestroyableUnit du = hit.collider.GetComponent<DestroyableUnit>();

                if (du == null || !allyTeam.IsEnemy(du.Team))
                {
                    isEnemy = false;
                }
            }

            if (squaredDistanceToEnemy < squaredShortestDistance && isEnemy)
            {
                squaredShortestDistance = squaredDistanceToEnemy;
                nearestEnemy = target.transform;
            }
        }
        return nearestEnemy;
    }

    public void Restart()
    {
        Init();
    }
}

namespace TeamExtensionMethods
{
    public static class TeamExtension
    {
        public static eTeam[] GetStrictEnemies(this eTeam team)
        {
            eTeam[] arrayToReturn;
            switch (team)
            {
                case eTeam.neutral:
                    arrayToReturn = new eTeam[] {eTeam.enemy, eTeam.player};
                    break;

                case eTeam.enemy:
                    arrayToReturn = new eTeam[] {eTeam.player};
                    break;
                case eTeam.player:
                    arrayToReturn = new eTeam[] {eTeam.enemy};
                    break;
                default:
                    arrayToReturn = new eTeam[] {eTeam.player};
                    break;
            }

            return arrayToReturn;
        }
        
        public static eTeam[] GetAllEnemies(this eTeam team)
        {
            eTeam[] arrayToReturn;
            switch (team)
            {
                case eTeam.neutral:
                    arrayToReturn = new eTeam[] {eTeam.enemy, eTeam.player};
                    break;

                case eTeam.enemy:
                    arrayToReturn = new eTeam[] {eTeam.neutral, eTeam.player};
                    break;
                case eTeam.player:
                    arrayToReturn = new eTeam[] {eTeam.neutral, eTeam.enemy};
                    break;
                default:
                    arrayToReturn = new eTeam[] {eTeam.neutral, eTeam.enemy};
                    break;
            }

            return arrayToReturn;
        }

        public static bool IsEnemy(this eTeam myTeam, eTeam theirTeam)
        {
            eTeam[] eTeamArray = GetAllEnemies(myTeam);

            for (int i = 0; i < eTeamArray.Length; i++)
            {
                if (eTeamArray[i] == theirTeam)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool IsFirstColliderEnemy(this eTeam myTeam, Vector3 position, Vector3 target,float attackRange, out DestroyableUnit duReturn)
        {
            RaycastHit[] hits;
            Vector3 dir = target - position;
            Vector3 myPositionGrounded = new Vector3(position.x, target.y, position.z);
            hits = Physics.RaycastAll(myPositionGrounded, dir, attackRange);
            //RaycastHit[] hits = Physics.RaycastAll(weapon.P_FirePosition.position, dir, attackRange);

            RaycastHit hit;
            if (Physics.Raycast(myPositionGrounded, dir, out hit, attackRange))
            {
                DestroyableUnit du = hit.collider.GetComponent<DestroyableUnit>();
                if(du)
                {
                    if (du.Team.IsEnemy(myTeam))
                    {
                        duReturn = du;
                        return true;
                    }
                }
            }

            duReturn = null;
            return false;
        }
    }

    public static class DistanceExtension
    {
        public static float GetSquaredDistance(Vector3 a, Vector3 b)
        {
            return (a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z);
        }
    }
}
