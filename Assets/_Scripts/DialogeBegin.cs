using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialoge_Editor.Runtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogeBegin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private DialogueContainer dialogAsset;
    [SerializeField] private GameObject dialogeManager;
    [SerializeField] private float distaceToInteract = 2.5f;
    
    private bool isToTalk = false;
    private DialogueNodeData data;
    private GameObject _dialogeCanvas;
    private GameObject player;
    private Button[] _options;

    private void Start()
    {
        data = dialogAsset.DialogueNodeData.Find(x => x.DialogueText.Equals("ENTRYPOINT"));
        var choise = dialogAsset.NodeLinks.Find(x => x.BaseNodeGUID == data.NodeGUID);
        data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID.Equals(choise.TargetNodeGUID));
        player = GameObject.FindWithTag("Player");
    }

    public void beginDialoge(GameObject dialogeCanvas)
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        _dialogeCanvas = dialogeCanvas;
        _options ??= _dialogeCanvas.GetComponentsInChildren<Button>();
        ProceedDialoge();
    }

    public void ProceedDialoge()
    {
        _dialogeCanvas.SetActive(true);
        var choises = dialogAsset.NodeLinks.FindAll(x => x.BaseNodeGUID == data.NodeGUID);
        var i = 0;
        foreach (var choise in choises)
        {
            _options[i].gameObject.SetActive(true);
            _options[i++].GetComponentInChildren<Text>().text = choise.PortName;
        }

        for (int j = i; j < _options.Length; j++)
        {
            _options[j].gameObject.SetActive(false);
        }

        if (choises.Count == 0)
        {
            _options[0].gameObject.SetActive(true);
            _options[0].onClick.AddListener(EndDialoge);
            _options[0].GetComponentInChildren<Text>().text = "Выход";
        }
        _dialogeCanvas.transform.Find("DialogeTextObj").GetComponentInChildren<Text>().text = data.DialogueText;
    }

// ReSharper disable Unity.PerformanceAnalysis
    public void ContinueDialog(int choise)
    {
        var choises = dialogAsset.NodeLinks.FindAll(x => x.BaseNodeGUID == data.NodeGUID);
        if (choises.Count != 0)
        {
            data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID == choises[choise].TargetNodeGUID);
            ProceedDialoge();
        }
    }

    private void EndDialoge()
    {
        data = dialogAsset.DialogueNodeData.Find(x => x.DialogueText.Equals("ENTRYPOINT"));
        var choise = dialogAsset.NodeLinks.Find(x => x.BaseNodeGUID == data.NodeGUID);
        data = dialogAsset.DialogueNodeData.Find(x => x.NodeGUID.Equals(choise.TargetNodeGUID));
        dialogeManager.GetComponent<DialogeManager>().EndDialoge();
        _options[0].onClick.RemoveListener(EndDialoge);
        foreach (var button in _options)
        {
            button.gameObject.SetActive(true);
        }
        _dialogeCanvas.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltip_Static("Поговорить");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip_Static();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0) &&
            Vector3.Distance(transform.position, player.transform.position) <= distaceToInteract)
        {
            dialogeManager.GetComponent<DialogeManager>().beginDialoge(this.gameObject);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            isToTalk = true;
            player.GetComponent<NavMeshAgent>().SetDestination(transform.position);
            player.GetComponent<PlayerMovement>().enabled = false;
        }
    }

    private void Update()
    {
        if (isToTalk
            && (Vector3.Distance(transform.position, player.transform.position) <= distaceToInteract))
        {
            player.GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
            dialogeManager.GetComponent<DialogeManager>().beginDialoge(this.gameObject);
            isToTalk = false;
        }
    }
}