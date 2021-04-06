using Scripts;
using UnityEngine;
using UnityEngine.UI;

public class QuestContainer : MonoBehaviour
{
    public Quest Quest;

    private void Awake()
    {
        GetComponentInChildren<Text>().text = $"{Quest.QuestText} - Опыта: {Quest.ExpValue}";
    }
}
