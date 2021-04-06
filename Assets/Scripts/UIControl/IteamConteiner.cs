using System;
using _Scripts;
using UnityEngine;
using UnityEngine.Serialization;


public class IteamConteiner : MonoBehaviour
{
    public Iteam iteam;

    private void Start()
    {
        iteam.playerInv = GameObject.FindWithTag("Player").GetComponent<Inventory>();
    }
}