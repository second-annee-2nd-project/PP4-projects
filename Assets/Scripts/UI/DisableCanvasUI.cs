using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCanvasUI : MonoBehaviour
{
  [SerializeField] private CanvasGroup[] pauseCanvasGroup;

  public void DisableCG()
  {
    foreach (var cg in pauseCanvasGroup)
    {
      if (cg.gameObject.activeInHierarchy)
      {
        cg.interactable = true;
        cg.blocksRaycasts = true;
        cg.alpha = 1;
      }
      else
      {
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.alpha = 0;
      }
    }
  }
}
