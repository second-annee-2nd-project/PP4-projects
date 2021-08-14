using System.Collections;
using System.Collections.Generic;
using TeamExtensionMethods;
using UnityEditor;
using UnityEngine;

public class UnitManager : ItemManager
{
    protected List<GameObject> targets;
    public List<GameObject> Targets => targets;

    private TeamManager teamUnitManager;
    
    protected float timerBeforeNextSpawn;
    
    protected int itemsLeftToSpawn;
    public int ItemsLeftToSpawn
    {
        get => itemsLeftToSpawn;
        set => itemsLeftToSpawn = value;
    }

    protected override void Start()
    {
        base.Start();
        targets = new List<GameObject>();
        team = eTeam.neutral;
        teamUnitManager = GameManager.Instance.P_TeamManager;
    }

    public void GetTargets()
    {
        if(teamUnitManager == null) teamUnitManager = GameManager.Instance.P_TeamManager;
        targets = teamUnitManager.GetStrictEnemies(team);
    }
    
    public virtual Transform GetNearestTarget(Vector3 pos)
    {
        GetTargets();
        if (targets.Count <= 0) return null;
        Transform nearest = targets[0].transform;
        float minSqrDist = (nearest.position - pos).sqrMagnitude;
        for (int i = 1; i < targets.Count; i++)
        {
            float sqrDist = (targets[i].transform.position - pos).sqrMagnitude;
            if (minSqrDist > sqrDist)
            {
                nearest = targets[i].transform;
                minSqrDist = sqrDist;
            }
        }

        return nearest;
    }
    
    public override void AddItemToList(GameObject item)
    {
        base.AddItemToList(item);
        teamUnitManager.AddToTeam(team, item);
    }
    
    public override void RemoveItemFromList(GameObject item)
    {
        base.RemoveItemFromList(item);
        teamUnitManager.RemoveAndDestroyFromTeam(team, item);
    }

    public override void Restart()
    {
        base.Restart();
        teamUnitManager.Restart();
    }
        
}
