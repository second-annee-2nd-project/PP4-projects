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
  
   
   void Awake()
   {
      // weaponUIList = new List<WeaponUI>(FindObjectsOfType<WeaponUI>());
   }
   public void PossibleToBuyWeapon()
   {
      foreach (var btn in buttonList )
      {
         btn.GetComponent<WeaponUI>().WeaponBuyableOrNot();
      }
   }

   public void WeaponBuyableOrNot()
   {
      if(weaponPriceButton <= ShopManager.Instance.Coins)
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
   public void SetInteractableButton()
   {
      foreach (var btn in buttonList )
      {
         btn.GetComponent<WeaponUI>().soldText.gameObject.SetActive(false);
      }
      soldText.gameObject.SetActive(true);
      GameManager.Instance.P_UiManager.FloatingTextInstantiate(soldText.gameObject.transform.parent.position, soldText.gameObject.transform,
         GameManager.Instance.P_UiManager.FloatingTextPrefab, 30f, weaponPriceButton);
   }

   public void SetSoldToFalse()
   {
      foreach (var btn in buttonList )
      {
         btn.GetComponent<WeaponUI>().soldText.gameObject.SetActive(false);
      }
   }
}

