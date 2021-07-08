using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : UnitManager
{
    [SerializeField] private TurretDictionary turretPrefabs;

    public TurretDictionary TurretPrefabs => turretPrefabs;
    
    protected override void Start()
    {
        base.Start();
        team = eTeam.player;

    }

}
