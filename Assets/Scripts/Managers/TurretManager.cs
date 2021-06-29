using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
  public List<Transform> turretList;

    // public List<Transform> TurretList => turretList;
    public void GetTurrets()
    {
        foreach (var turret in GameObject.FindGameObjectsWithTag("Turret") )
        {
            turretList.Add(turret.transform);
        }
    }
}
