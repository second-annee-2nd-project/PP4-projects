using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTeam
{
    neutral,
    player,
    enemy
}
public abstract class TeamUnit : MonoBehaviour
{
    [Header("Team : ")] [SerializeField] private eTeam team;
    public eTeam  Team
    {
        get => team;
        set => team = value;
    }
}
