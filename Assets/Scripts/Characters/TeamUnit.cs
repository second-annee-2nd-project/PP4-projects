using System.Collections;
using System.Collections.Generic;
using TeamExtensionMethods;
using UnityEngine;

public enum eTeam
{
    neutral,
    player,
    enemy
}
public abstract class TeamUnit : Item
{
    [Header("Team : ")] [SerializeField] protected eTeam team;

    protected Transform nearestTarget;
    
    public eTeam  Team
    {
        get => team;
        set => team = value;
    }
    
    

    protected virtual void UpdateTarget()
    {
        nearestTarget = GameManager.Instance.P_TeamManager.GetNearestEnemyUnit(transform.position, team);
    }

}
