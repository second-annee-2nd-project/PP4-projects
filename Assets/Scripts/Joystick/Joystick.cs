using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events ;
using UnityEngine.EventSystems ;

  public class Joystick : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler {

      private RectTransform container ;
      private RectTransform handle ;
      
      public bool ArrowKeysSimulationEnabled = false ;

      private Vector2 point ;
      private Vector2 normalizedPoint ;

      private float maxLength ;

      private bool _isTouching = false ;
      public bool _IsTouching => _isTouching;

      public UnityAction OnJoystickDownAction ;
      public UnityAction OnJoystickUpAction ;

      private PointerEventData pointerEventData ;
      private Camera cam ;
      
      private void OnEnable () {
         OnPointerUp (null) ;
      }

      private void Awake () {
         container = transform.GetComponent <RectTransform> () ;
         handle = container.GetChild (0).GetComponent <RectTransform> () ;

         maxLength = (container.sizeDelta.x / 2f) - (handle.sizeDelta.x / 2f) - 5f ;
      }

      public void OnPointerDown (PointerEventData e) {
         if (OnJoystickDownAction != null)
            OnJoystickDownAction.Invoke () ;

         _isTouching = true ;
         cam = e.pressEventCamera ;
         OnDrag (e) ;
      }

      public void OnDrag (PointerEventData e) {
         pointerEventData = e ;
      }

      void Update () {
         if (_isTouching && RectTransformUtility.ScreenPointToLocalPointInRectangle (container, pointerEventData.position, cam, out point)) {
            point = Vector2.ClampMagnitude (point, maxLength) ;
            handle.anchoredPosition = point ;

            float length = Mathf.InverseLerp (0f, maxLength, point.magnitude) ;
            normalizedPoint = Vector2.ClampMagnitude (point, length) ;
         }
      }

      public void OnPointerUp (PointerEventData e) {
         if (OnJoystickUpAction != null)
            OnJoystickUpAction.Invoke () ;

         _isTouching = false ;
         normalizedPoint = Vector3.zero ;
         handle.anchoredPosition = Vector3.zero ;
      }
      
      public float Horizontal () {
         if (ArrowKeysSimulationEnabled)
            return (normalizedPoint.x != 0) ? normalizedPoint.x : Input.GetAxis ("Horizontal") ;

         return normalizedPoint.x ;
      }
      public float Vertical () {
         if (ArrowKeysSimulationEnabled)
            return (normalizedPoint.y != 0) ? normalizedPoint.y : Input.GetAxis ("Vertical") ;

         return normalizedPoint.y ;
      }

   }
