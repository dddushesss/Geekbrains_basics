using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerAtributeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Atribute Atribute;
    
    private void Start()
    {
        var parent = gameObject.transform;
        var skillPoints = parent.parent.parent.Find("SkillPoints").GetComponent<SkillPoints>();
        
        parent.Find("AtributeName").gameObject.GetComponent<Text>().text =
            Atribute.ToString();
        
        parent.Find("Count").gameObject.GetComponent<Text>().text =
            Atribute.value.ToString();
        
        parent.Find("Minus").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (skillPoints.SkillPointsValue != 0 && Atribute.value >= 0)
            {
                Atribute.ChangeAtributeValue(-1);
                skillPoints.ChangeSkillPointsValue(1);
                UpdateValueText();
            }
        });
        

        parent.Find("Plus").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (skillPoints.SkillPointsValue != 0 && Atribute.value < Atribute.MaxValue)
            {
                Atribute.ChangeAtributeValue(1);
                skillPoints.ChangeSkillPointsValue(-1);
                UpdateValueText();
            }
        });
     

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltip_Static(Atribute.GetDesctiption());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip_Static();
    }
    
    private void UpdateValueText()
    {
        
        gameObject.transform.Find("Count").gameObject.GetComponent<Text>().text =
            Atribute.value.ToString();
    }
}
