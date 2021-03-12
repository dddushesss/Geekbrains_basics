using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Iteam : MonoBehaviour
{
    public string name;
    public string description;
    public Sprite Icon;
    public GameObject prefab;
    public Inventory CurInventory;
    public Inventory playerInv;
    private void Start()
    {
        var iteam = Instantiate(this.prefab, transform);
        playerInv = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        iteam.tag = "Iteam";
        
    }
    public virtual void PickUp(Inventory inventory)
    {
        inventory.AddIteam(this);
        Destroy(this.transform.GetChild(0).gameObject);
    }

    public virtual void Drop(Inventory inventory)
    {
        CurInventory.RemoveIteam(this);
        this.transform.position = CurInventory.transform.position;
        var icon = Instantiate(this.prefab, transform).tag = "Iteam";
    }
    public abstract void Usage();
    
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        PickUp(playerInv);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltip_Static(this.name);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip_Static();
    }
}
