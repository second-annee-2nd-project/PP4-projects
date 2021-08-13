using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWeapon : MonoBehaviour
{
    [SerializeField] private Text costText, nameText, soldText;
     private Weapon wp;
     [SerializeField] private Button upgradeBtn;
    void Start()
    {

        SetUpgradedWeaponInfo();
    }
    public void SetUpgradedWeaponInfo()
    {
        wp = GameManager.Instance.Player.P_Weapon;
        costText.text =  "Prix : " + wp.UpgradeWeaponStats.Price;
        nameText.text = "Upgrade : " + wp.WeaponStats.Name;
        soldText.gameObject.SetActive(false);
        upgradeBtn.interactable = true;
    }
    public void BuyUpgradeWeapon()
    {
        //même arme feedback
        wp = GameManager.Instance.Player.P_Weapon;
       
        if (ShopManager.Instance.Coins >= wp.UpgradeWeaponStats.Price)
        {
            GameManager.Instance.Player.WeaponGo.GetComponent<Weapon>().WeaponStats = wp.UpgradeWeaponStats;
            ShopManager.Instance.UpdateCoins(-wp.UpgradeWeaponStats.Price);
            GameManager.Instance.Player.WeaponGo.GetComponent<Weapon>().Init();
            soldText.gameObject.SetActive(true);
            upgradeBtn.interactable = false;
            GameManager.Instance.P_UiManager.FloatingTextInstantiate(upgradeBtn.transform.position,upgradeBtn.transform, GameManager.Instance.P_UiManager.FloatingTextPrefab, 30f);

        }
        
    }
}
