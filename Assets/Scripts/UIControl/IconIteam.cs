using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class IconIteam : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Iteam _iteam;


    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(1))
            _iteam.Drop(_iteam.CurInventory);
        else if(Input.GetMouseButtonUp(0))
        {
            _iteam.Usage();
            _iteam.DestroyIteam(this.gameObject);
        }
        Tooltip.HideTooltip_Static();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltip_Static($"{_iteam.name}\n{_iteam.description}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip_Static();
    }
}