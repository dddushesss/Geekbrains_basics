using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPoints : MonoBehaviour
{
    public int SkillPointsValue = 0;

    public void ChangeSkillPointsValue(int count)
    {
        SkillPointsValue += count;
        gameObject.GetComponentInChildren<Text>().text = SkillPointsValue.ToString();
    }
}
