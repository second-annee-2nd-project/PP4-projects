using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWeapon : MonoBehaviour
{
    [SerializeField] private Text costText, nameText;
     private Weapon wp;
  
    void Start()
    {

        SetUpgradedWeaponInfo();
    }
    public void SetUpgradedWeaponInfo()
    {
        wp = GameManager.Instance.Player.P_Weapon.GetComponent<Weapon>();
        costText.text =  "Prix : " + wp.UpgradeWeaponStats.Price;
        nameText.text = "Upgrade : " + wp.WeaponStats.Name;
    }
    public void BuyUpgradeWeapon()
    {
        if (ShopManager.Instance.Coins >= wp.UpgradeWeaponStats.Price)
        {
            GameManager.Instance.Player.WeaponGo.GetComponent<Weapon>().WeaponStats = wp.UpgradeWeaponStats;
            ShopManager.Instance.UpdateCoins(-wp.UpgradeWeaponStats.Price);
            GameManager.Instance.Player.WeaponGo.GetComponent<Weapon>().Init();

        }
        
    }
}
