using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : UnitManager
{
    [SerializeField] private TurretDictionary turretPrefabs;

    public TurretDictionary TurretPrefabs => turretPrefabs;

    private Grid actualGrid;
    
    
    protected override void Start()
    {
        base.Start();
        team = eTeam.player;
        actualGrid = GameManager.Instance.ActualGrid;
    }

    public override void Restart()
    {
        base.Restart();
        actualGrid = GameManager.Instance.ActualGrid;
    }

    public void ResetAnimTurret()
    {
        foreach (var turretGO in InstantiatedItems)
        {
            turretGO.GetComponent<Turret>().TurretAnim.SetBool("Shoot",false);
        }
    }


}
