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
     public Button UpgradeBtn => upgradeBtn;
     public Text SoldText => soldText;
    void Start()
    {

        SetUpgradedWeaponInfo();
    }
    public void SetUpgradedWeaponInfo()
    {
        wp = GameManager.Instance.Player.P_Weapon;
        if (wp != null)
        {
            if(wp.UpgradeWeaponStats != null)
                costText.text =  "Prix : " + wp.UpgradeWeaponStats.Price;
            nameText.text = "Upgrade : " + wp.WeaponStats.Name;
        }
        soldText.gameObject.SetActive(false);
      
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
            GameManager.Instance.P_UiManager.FloatingTextInstantiate(upgradeBtn.transform.position,upgradeBtn.transform, GameManager.Instance.P_UiManager.FloatingTextPrefab, 30f,wp.UpgradeWeaponStats.Price);

        }
        
    }

    public void NonInteractable()
    {
        if (wp != null && wp.UpgradeWeaponStats != null)
        {
            if(ShopManager.Instance.Coins >= wp.UpgradeWeaponStats.Price)
                upgradeBtn.interactable = true;
            else 
                upgradeBtn.interactable = false;
        }
    }
}
