using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerAtributeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Atribute atribute;
    
    private void Start()
    {
        var parent = gameObject.transform;
        var skillPoints = parent.parent.parent.Find("SkillPoints").GetComponent<SkillPoints>();
        
        parent.Find("AtributeName").gameObject.GetComponent<Text>().text =
            atribute.ToString();
        
        parent.Find("Count").gameObject.GetComponent<Text>().text =
            atribute.value.ToString();
        
        parent.Find("Minus").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (skillPoints.SkillPointsValue != 0 && atribute.value >= 0)
            {
                atribute.ChangeAtributeValue(-1);
                skillPoints.ChangeSkillPointsValue(1);
                UpdateValueText();
            }
        });
        

        parent.Find("Plus").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (skillPoints.SkillPointsValue != 0 && atribute.value < atribute.MaxValue)
            {
                atribute.ChangeAtributeValue(1);
                skillPoints.ChangeSkillPointsValue(-1);
                UpdateValueText();
            }
        });
     

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltip_Static(atribute.GetDesctiption());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip_Static();
    }
    
    private void UpdateValueText()
    {
        
        gameObject.transform.Find("Count").gameObject.GetComponent<Text>().text =
            atribute.value.ToString();
    }
}
