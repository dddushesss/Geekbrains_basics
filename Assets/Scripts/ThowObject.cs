using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThowObject : MonoBehaviour
{
    [SerializeField] private GameObject thowableGameObject;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var instanceThowable = Instantiate(thowableGameObject, transform.position,
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 10 * Time.deltaTime));
            instanceThowable.GetComponent<Rigidbody>().AddForce(transform.forward * 5000f);
        }
    }
}