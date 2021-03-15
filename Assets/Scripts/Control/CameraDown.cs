using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDown : MonoBehaviour
{
   private bool IsInside = false;
   [SerializeField]
   private GameObject mainCamera;
   private float _defaultHeight;
   private float _defaultDistance;
   private CameraControl _cameraControl;
   
   public float insideHeight;
   public float insideDistance;
   
   

   private void Start()
   {
      _cameraControl = mainCamera.GetComponent<CameraControl>();
      _defaultHeight = _cameraControl.height;
      _defaultDistance = _cameraControl.distance;
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.tag.Equals("Player"))
      {
         if (!IsInside)
         {
            _cameraControl.height = insideHeight;
            _cameraControl.distance = insideDistance;
            IsInside = true;
         }
         else
         {
            _cameraControl.height = _defaultHeight;
            _cameraControl.distance = _defaultDistance;
            IsInside = false;
         }
      }
   }
}
