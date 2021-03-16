using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using Dialoge_Editor.Editor.Nodes;
using Dialoge_Editor.Runtime;
using Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogeBegin : MonoBehaviour
{
    [SerializeField] private DialogueContainer dialogAsset;
    [SerializeField] private GameObject dialogeManager;

    private DialogueNodeData _data;
    private GiveQuestNodeData _giveQuestNodeData;
    private CheckQuestNodeData _checkQuestNodeData;
    private GameObject _dialogeCanvas;
    private GameObject _player;
    private Button[] _options;

    private void Start()
    {
        _data = dialogAsset.DialogueNodeData.Find(x => x.DialogueText.Equals("ENTRYPOINT"));
        var choise = dialogAsset.NodeLinks.Find(x => x.BaseNodeGUID == _data.NodeGUID);
        _data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID.Equals(choise.TargetNodeGUID));
        _player = GameObject.FindWithTag("Player");
    }

    public void beginDialoge(GameObject dialogeCanvas)
    {
        _player.GetComponent<PlayerMovement>().enabled = false;
        _dialogeCanvas = dialogeCanvas;
        _options ??= _dialogeCanvas.GetComponentsInChildren<Button>();
        ProceedDialoge();
    }

    private void ProceedDialoge()
    {
        _dialogeCanvas.SetActive(true);
        var choises = dialogAsset.NodeLinks.FindAll(x => x.BaseNodeGUID == _data.NodeGUID);
        var i = 0;
        foreach (var option in _options)
        {
            option.gameObject.SetActive(false);
            option.onClick.RemoveAllListeners();
        }

        foreach (var choise in choises)
        {
            var quest = dialogAsset.IfDialogueNodeData.Find(x => x.NodeGUID.Equals(choise.TargetNodeGUID));
            var i1 = i;
            if (quest != null && _player.GetComponent<QuestsList>().quests.Contains((Quest) quest.Quest))
            {
                i++;
                continue;
            }

            _options[i].gameObject.SetActive(true);
            _options[i].onClick.AddListener(() => ContinueDialog(i1));
            _options[i++].GetComponentInChildren<Text>().text = choise.PortName;
        }


        if (i == 0)
        {
            _options[0].gameObject.SetActive(true);
            _options[0].onClick.AddListener(EndDialoge);
            _options[0].GetComponentInChildren<Text>().text = "Выход";
        }

        _dialogeCanvas.transform.Find("DialogeTextObj").GetComponentInChildren<Text>().text = _data.DialogueText;
    }

// ReSharper disable Unity.PerformanceAnalysis
    public void ContinueDialog(int choise)
    {
        var choises = dialogAsset.NodeLinks.FindAll(x => x.BaseNodeGUID == _data.NodeGUID);
        if (choises.Count != 0)
        {
            _data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID == choises[choise].TargetNodeGUID);


            if (_data != null)
            {
                ProceedDialoge();
            }
            else
            {
                _giveQuestNodeData =
                    dialogAsset.IfDialogueNodeData.Find(x => x.NodeGUID == choises[choise].TargetNodeGUID);
                if (_giveQuestNodeData != null)
                    GiveQuest();
                else
                {
                    _checkQuestNodeData =
                        dialogAsset.CheckQuestNodeData.Find(x => x.NodeGUID == choises[choise].TargetNodeGUID);
                    CheckQuest();
                }
            }
        }
        else
        {
            ProceedDialoge();
        }
    }

    private void CheckQuest()
    {
        var checkQuest = _checkQuestNodeData.Quest as Quest;

        for (int j = 1; j < _options.Length; j++)
        {
            _options[j].gameObject.SetActive(false);
            _options[j].onClick.RemoveAllListeners();
        }
        
        if (_player.GetComponent<Inventory>()._iteams.Contains(checkQuest.condition))
        {
            checkQuest.Complete();
            _options[0].gameObject.SetActive(true);
            _options[0].GetComponentInChildren<Text>().text = "Сдать квест";
            _options[0].onClick.AddListener(() => ContinueDialog(0));
            _dialogeCanvas.transform.Find("DialogeTextObj").GetComponentInChildren<Text>().text =
                checkQuest.QuestText;
        }
        else
        {
            _options[0].gameObject.SetActive(true);
            _options[0].GetComponentInChildren<Text>().text = "Далее";
            _dialogeCanvas.transform.Find("DialogeTextObj").GetComponentInChildren<Text>().text = 
                $"Не выполнены условия квеста! Вам нужно принести {checkQuest.condition.name}";
        }
        
        var choise = dialogAsset.NodeLinks.Find(x => x.BaseNodeGUID.Equals(_checkQuestNodeData.NodeGUID));
        _data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID.Equals(choise.TargetNodeGUID));
    }

    private void GiveQuest()
    {
        var quest = _giveQuestNodeData.Quest as Quest;

        _player.GetComponent<QuestsList>().AddQuest(quest);
        for (int j = 1; j < _options.Length; j++)
        {
            _options[j].gameObject.SetActive(false);
            _options[j].onClick.RemoveAllListeners();
        }

        _options[0].gameObject.SetActive(true);
        _options[0].GetComponentInChildren<Text>().text = "Получить квест";
        _options[0].onClick.AddListener(() => ContinueDialog(0));
        _dialogeCanvas.transform.Find("DialogeTextObj").GetComponentInChildren<Text>().text =
            _giveQuestNodeData.FunctionName;
        var choise = dialogAsset.NodeLinks.Find(x => x.BaseNodeGUID.Equals(_giveQuestNodeData.NodeGUID));
        _data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID.Equals(choise.TargetNodeGUID));
       
    }

    private void EndDialoge()
    {
        _data = dialogAsset.DialogueNodeData.Find(x => x.DialogueText.Equals("ENTRYPOINT"));
        var choise = dialogAsset.NodeLinks.Find(x => x.BaseNodeGUID == _data.NodeGUID);
        _data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID.Equals(choise.TargetNodeGUID));
        dialogeManager.GetComponent<DialogeManager>().EndDialoge();
        _options[0].onClick.RemoveListener(EndDialoge);
        foreach (var button in _options)
        {
            button.gameObject.SetActive(true);
        }

        _dialogeCanvas.SetActive(false);
        _player.GetComponent<PlayerMovement>().enabled = true;
    }
}