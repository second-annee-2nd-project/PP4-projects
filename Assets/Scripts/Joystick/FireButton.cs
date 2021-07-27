using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool firePressed;
    public bool FirePressed => firePressed;
    public void OnPointerDown(PointerEventData data)
    {
        if(GameManager.Instance.EGameState==eGameState.Wave)
            firePressed = true;
    }
 
    public void OnPointerUp(PointerEventData data)
    {
        firePressed = false;
    }
}

