using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
   // [SerializeField] private Text costText, nameText;
   // [SerializeField] private Button buyWeaponBtn;
   // public void SetWeaponInfo(GameObject weaponGo)
   // {
   //    Weapon wp = weaponGo.GetComponent<Weapon>();
   //    costText.text =  "Prix : " + wp.WeaponStats.Price;
   //    nameText.text =  wp.WeaponStats.Name;
   //    buyWeaponBtn.onClick.RemoveAllListeners();
   //    buyWeaponBtn.onClick.AddListener(delegate { ShopManager.Instance.BuyWeapon(weaponGo); });
   // }
   [SerializeField] private Text soldText;
   [SerializeField] private Button buyWeaponBtn;

   public void WeaponSoldedUI()
   {
      buyWeaponBtn.interactable = false;
      soldText.gameObject.SetActive(true);
   }
}
