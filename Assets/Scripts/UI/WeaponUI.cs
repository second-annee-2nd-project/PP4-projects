using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
   // [SerializeField] private Text costText, nameText;
    [SerializeField] private Button buyWeaponBtn;
   // public void SetWeaponInfo(GameObject weaponGo)
   // {
   //    Weapon wp = weaponGo.GetComponent<Weapon>();
   //    costText.text =  "Prix : " + wp.WeaponStats.Price;
   //    nameText.text =  wp.WeaponStats.Name;
   //    buyWeaponBtn.onClick.RemoveAllListeners();
   //    buyWeaponBtn.onClick.AddListener(delegate { ShopManager.Instance.BuyWeapon(weaponGo); });
   // }
   [SerializeField] private List<Button> buttonList;
   [SerializeField] private float weaponPriceButton;
   // [SerializeField] private Text noMoney;
   
   [SerializeField] private Text soldText;
   public Text SoldText => soldText;
   private bool isBough;
   
   void Update()
   {
      foreach (var btn in buttonList )
      {
         if(btn.GetComponent<WeaponUI>().weaponPriceButton <= ShopManager.Instance.Coins && btn.GetComponent<WeaponUI>().isBough == false )
         {
            btn.GetComponent<WeaponUI>().buyWeaponBtn.interactable = true;
         }
         else
         {
            btn.GetComponent<WeaponUI>().buyWeaponBtn.interactable = false;
           
         }
      }
   }

   public void WeaponBuyableOrNot()
   {
      if(weaponPriceButton <= ShopManager.Instance.Coins && !isBough )
      {
         buyWeaponBtn.interactable = true;
         // noMoney.gameObject.SetActive(false);
      }
      else
      {
         buyWeaponBtn.interactable = false;
         // noMoney.gameObject.SetActive(true);
      }
   }
   public void SetInteractableButton(Button button)
   {
      GameObject pos = button.GetComponent<WeaponUI>().soldText.gameObject;
    
      foreach (var btn in buttonList )
      {
         btn.GetComponent<WeaponUI>().soldText.gameObject.SetActive(false);
         btn.interactable = true;
         btn.GetComponent<WeaponUI>().isBough = false;

      }
      GameManager.Instance.P_UiManager.FloatingTextInstantiate(pos.transform.parent.position, pos.transform,
            GameManager.Instance.P_UiManager.FloatingTextPrefab, 30f, weaponPriceButton);
         pos.SetActive(true);
         button.interactable = false;
         button.GetComponent<WeaponUI>().isBough = true;
         GameManager.Instance.P_UpgradeWeapon.IsBough = false;
      
   }

   public void SetSoldToFalse()
   {
      foreach (var btn in buttonList )
      {
         btn.GetComponent<WeaponUI>().soldText.gameObject.SetActive(false);
         btn.GetComponent<WeaponUI>().isBough = false;
         btn.interactable = true;
      }
   }
}

