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
       
       [SerializeField] private CanvasGroup floatingText;
       public CanvasGroup FloatingText => floatingText;
       
       [SerializeField] private float timeToFade;
       
       [SerializeField] private GameObject floatingTextPrefab;
        public GameObject FloatingTextPrefab => floatingTextPrefab;
        
       [SerializeField] private GameObject floatingTextPrefabWorld;
       public GameObject FloatingTextPrefabWorld => floatingTextPrefabWorld;
       
       private GameObject floatTextInstance;
       
       [SerializeField] private Transform canvasContainerWorld;
       public Transform CanvasContainerWorld => canvasContainerWorld;

       private float speedFloatText;
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
       
       public void FloatingTextInstantiate(Vector3 flotingTextPos, Transform transformParent,GameObject prefabToSpawn, float speed, float PriceInText)
       {
          
           speedFloatText = speed;
           floatTextInstance = Instantiate(prefabToSpawn,flotingTextPos,Quaternion.identity);
           floatTextInstance.transform.parent = transformParent;
           floatTextInstance.GetComponentInChildren<Text>().text = "-" + PriceInText;
           StopCoroutine(nameof(FadeOutFloatingText));
           StartCoroutine(nameof(FadeOutFloatingText));
           Destroy(floatTextInstance,3f);
       
       }
       public IEnumerator FadeOutFloatingText()
       {
           float time = 0;
           while (time < timeToFade)
           {
               time += Time.deltaTime;
               floatTextInstance.transform.position += transform.up* speedFloatText * Time.deltaTime;
               floatTextInstance.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f,0f, time / timeToFade);
               yield return null;
           }
       }

    
}
