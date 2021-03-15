using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private IfNodeData _ifNodeData;
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
            _options[i].onClick.AddListener(()=>ContinueDialog(i1));
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
                _ifNodeData = dialogAsset.IfDialogueNodeData.Find(x => x.NodeGUID == choises[choise].TargetNodeGUID);
                GiveQuest();
            }
        }
        else
        {
            ProceedDialoge();
        }
    }

    private void GiveQuest()
    {
        var quest = _ifNodeData.Quest as Quest;

        _player.GetComponent<QuestsList>().AddQuest(quest);
        for (int j = 1; j < _options.Length; j++)
        {
            _options[j].gameObject.SetActive(false);
        }

        _options[0].gameObject.SetActive(true);
        _options[0].GetComponentInChildren<Text>().text = "Получить квест";
        _dialogeCanvas.transform.Find("DialogeTextObj").GetComponentInChildren<Text>().text =
            _ifNodeData.FunctionName;
        var choise = dialogAsset.NodeLinks.Find(x => x.BaseNodeGUID.Equals(_ifNodeData.NodeGUID));
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