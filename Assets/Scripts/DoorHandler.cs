using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public Transform anchor; 
    public float distance = 20;
    public bool isOpen = false; 
    public float openAngle = 120;
    public float closeAngle = 0;
    public float smooth = 2;

    private Transform target;

    void Awake () 
    {
        openAngle = Mathf.Abs(openAngle);
        closeAngle = Mathf.Abs(closeAngle);
        if(isOpen) anchor.localRotation = Quaternion.Euler(0, openAngle, 0);
        enabled = false;
    }

    void Update () 
    {
        if(isOpen)
        {
            Quaternion rotation = Quaternion.Euler(0, openAngle, 0);
            anchor.localRotation = Quaternion.Lerp(anchor.localRotation, rotation, smooth * Time.deltaTime);
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(0, closeAngle, 0);
            anchor.localRotation = Quaternion.Lerp(anchor.localRotation, rotation, smooth * Time.deltaTime);
        }

        if(target)
        {
            float dis = Vector3.Distance(transform.position, target.position);
            if(dis > distance) enabled = false;
        }
    }

    public void Invert(Transform player)
    {
        target = player;
        isOpen = !isOpen;
    }
}
