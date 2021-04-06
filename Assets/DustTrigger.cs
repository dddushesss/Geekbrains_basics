using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustTrigger : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    void Start()
    {
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _particleSystem.Play();
    }

   
}
