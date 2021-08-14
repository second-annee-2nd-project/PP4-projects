using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretBtnUI : MonoBehaviour
{
  
       [SerializeField] private Button buyWeaponBtn;
       [SerializeField] private List<Button> buttonList;
      [SerializeField] private float weaponPriceButton;

      public void PossibleToBuyTurret()
      {
         foreach (var btn in buttonList )
         {
            btn.GetComponent<TurretBtnUI>().TurretBuyableOrNot();
         }
      }
   
      public void TurretBuyableOrNot()
      {
         if(weaponPriceButton <= ShopManager.Instance.Coins)
         {
            buyWeaponBtn.interactable = true;
         }
         else
         {
            buyWeaponBtn.interactable = false;
         }
      }
}
