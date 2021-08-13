using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
     [SerializeField] private Image life_Img;
     public Image Life_Img => life_Img;
     
       [SerializeField] private Image life_Img_Damage_Heal;
       public Image Life_Img_Damage_Heal => life_Img_Damage_Heal;

       [SerializeField] private CanvasGroup retryButton;
       public CanvasGroup RetryButton => RetryButton;

       [SerializeField] private CanvasGroup pause;
       public CanvasGroup Pause => pause;
       [SerializeField] private CanvasGroup joystick;
       public CanvasGroup Joystick => joystick;
       
       public void RenderRetryButton(bool state)
       {
           if (state)
           {
               retryButton.alpha = 1;
           }
           else
           {
               retryButton.alpha = 0;
           }
           retryButton.interactable = state;
           retryButton.blocksRaycasts = state;
       }
       public void RenderJoystick(bool state)
       {
           if (state)
           {
               joystick.alpha = 1;
           }
           else
           {
               joystick.alpha = 0;
           }
           joystick.interactable = state;
           joystick.blocksRaycasts = state;
       }
       public void RenderPause(bool state)
       {
           if (state)
           {
               pause.alpha = 1;
           }
           else
           {
              pause.alpha = 0;
           }
           pause.interactable = state;
           pause.blocksRaycasts = state;
       }


}
