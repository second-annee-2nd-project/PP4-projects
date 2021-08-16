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
     // public Button UpgradeBtn => upgradeBtn;
     public Text SoldText => soldText;
     bool isBough;

     public bool IsBough
     {
         get => isBough;
         set => isBough = value;
     }
    void Start()
    {
        costText.text =  "Price : 40";
        nameText.text = "Upgrade\nGun";
    }

    void Update()
    {
        if (isBough)
        {
            upgradeBtn.interactable = false;
        }
        else
        {
            upgradeBtn.interactable = true;
        }

        NonInteractable();
    }
    public void SetUpgradedWeaponInfo()
    {
        wp = GameManager.Instance.Player.P_Weapon;
        if (wp != null)
        {
            if(wp.UpgradeWeaponStats != null)
                costText.text =  "Price : " + wp.UpgradeWeaponStats.Price;
            nameText.text = "Upgrade\n" + wp.WeaponStats.Name;
        }
        soldText.gameObject.SetActive(false);
    }
    public void BuyUpgradeWeapon()
    {
        //même arme feedback
        wp = GameManager.Instance.Player.P_Weapon;
       
        if (ShopManager.Instance.Coins >= wp.UpgradeWeaponStats.Price && !isBough)
        {
            GameManager.Instance.Player.WeaponGo.GetComponent<Weapon>().WeaponStats = wp.UpgradeWeaponStats;
            ShopManager.Instance.UpdateCoins(-wp.UpgradeWeaponStats.Price);
            GameManager.Instance.Player.WeaponGo.GetComponent<Weapon>().Init();
            soldText.gameObject.SetActive(true);
            isBough = true;
            GameManager.Instance.P_UiManager.FloatingTextInstantiate(upgradeBtn.transform.position,upgradeBtn.transform, GameManager.Instance.P_UiManager.FloatingTextPrefab, 30f,wp.UpgradeWeaponStats.Price);
            
            wp.GetComponent<Renderer>().material = wp.UpgradeWeaponStats.UpgradedTexture;

        }
        
    }

    public void NonInteractable()
    {
        wp = GameManager.Instance.Player.P_Weapon;
        if (wp != null)
        {
            if(ShopManager.Instance.Coins >= wp.UpgradeWeaponStats.Price )
                upgradeBtn.interactable = true;
            else if(ShopManager.Instance.Coins < wp.UpgradeWeaponStats.Price)
                upgradeBtn.interactable = false;
        }
      
        
    }
}
