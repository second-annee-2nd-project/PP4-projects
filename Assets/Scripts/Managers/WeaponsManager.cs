using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WeaponsManager : MonoBehaviour
{
    private eWeaponType weaponType;
    
    [SerializeField] private WeaponDictionary weaponPrefabs;

    private int lastWeaponChose;
    
    private List<GameObject> weaponsList;

    private void Awake()
    {
        weaponsList = new List<GameObject>();
        foreach (var weapon in weaponPrefabs)
        {
            weaponsList.Add(weapon.Value);
        }
    }

    public List<GameObject> GetRandomWeapons(int weaponChose)
    {
        List<GameObject> weaponNewList = new List<GameObject>(weaponsList);
        List<GameObject> returnWeaponList = new List<GameObject>();
        GameObject actualWeapon = GameManager.Instance.Player.P_Weapon.gameObject;
        for (int i = 0; i < weaponNewList.Count; i++)
        {
            if (weaponNewList[i].GetComponent<Weapon>().WeaponStats.WeaponType ==
                actualWeapon.GetComponent<Weapon>().WeaponStats.WeaponType)
            {
                weaponNewList.RemoveAt(i);
                break;
            }
        }

        for(int i = 0; i < weaponChose; i++)
        {
            int r = Random.Range (0, weaponNewList.Count);
            returnWeaponList.Add(weaponNewList[r]);
            weaponNewList.RemoveAt(r);
        }

        return returnWeaponList;
    }
}
