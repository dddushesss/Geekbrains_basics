using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using UnityEngine.UI;

public class QuestContainer : MonoBehaviour
{
    public Quest Quest;

    private void Start()
    {
        GetComponentInChildren<Text>().text = $"{Quest.QuestText} - Опыта: {Quest.ExpValue}";
    }
}
