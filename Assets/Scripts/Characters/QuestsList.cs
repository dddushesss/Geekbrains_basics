using System;
using System.Collections.Generic;
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
        Instantiate(questPrefab, questCanvas.transform).GetComponentInChildren<QuestContainer>().Quest = quest;
    }

    public void RemoveQuest(Quest quest)
    {
        quests.Remove(quest);
        for (int i = 0; i < questCanvas.transform.childCount; i++)
        {
            if (questCanvas.transform.GetChild(i).GetComponent<QuestContainer>().Quest.Equals(quest))
            {
                Destroy(questCanvas.transform.GetChild(i).gameObject);
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
