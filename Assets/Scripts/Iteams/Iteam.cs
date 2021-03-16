using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using _Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Iteam : ScriptableObject
{
    public string name;
    public string description;
    public Sprite Icon;
    public GameObject prefab;
    public Inventory CurInventory;
    public Inventory playerInv;
    
    public void PickUp(Inventory inventory, GameObject IteamGO)
    {
        inventory.AddIteam(this);
        Destroy(IteamGO);
    }

    public void DestroyIteam(GameObject IteamGO)
    {
        CurInventory.RemoveIteam(this);
        Destroy(IteamGO);
    }
    
    public void Drop(Inventory inventory)
    {
        inventory.RemoveIteam(this);
        Instantiate(prefab, playerInv.transform.position, Quaternion.identity);
    }
    public abstract void Usage();
    
}
