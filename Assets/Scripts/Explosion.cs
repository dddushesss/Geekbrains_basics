using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject _explosive;

    public float Radius;
    public float Force;

    private void OnTriggerEnter(Collider other)
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(_explosive.transform.position, Radius);
        for (int i = 0; i < overlappedColliders.Length; i++)
        {
            Rigidbody rigidbody = overlappedColliders[i].attachedRigidbody;
            if (rigidbody != null)
            {
                rigidbody.AddExplosionForce(Force, _explosive.transform.position, Radius);
            }
        }
    }
}