using System;
using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuestsList : MonoBehaviour
{
    public GameObject questCanvas;
    public GameObject questPrefab;
    [NonSerialized] public List<Quest> quests;

    private void Start()
    {
        quests = new List<Quest>();
    }

    public void AddQuest(Quest quest)
    {
        quests.Add(quest);
        var newStr = "";
        foreach (var c in quest.QuestText)
        {
            newStr += "" + c + '\u0336';
        }

        quest.doneText = newStr;
        Instantiate(questPrefab, questCanvas.transform).GetComponentInChildren<QuestContainer>().Quest = quest;
    }

    public void RemoveQuest(Quest quest)
    {
        quests.Remove(quest);
        for (int i = 0; i < questCanvas.transform.childCount; i++)
        {
            if (questCanvas.transform.GetChild(i).GetComponent<QuestContainer>().Quest.Equals(quest))
            {
                questCanvas.transform.GetChild(i).GetComponent<QuestContainer>().Quest.IsDone = true;
                
                questCanvas.transform.GetChild(i).GetComponentInChildren<Text>().text = quest.doneText;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            questCanvas.SetActive(!questCanvas.activeSelf);
        }
    }
}