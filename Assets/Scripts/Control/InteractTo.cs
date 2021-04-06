using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class InteractTo : MonoBehaviour
{
    private bool IsToTake;
    private bool isToTalk;
    private bool IsToHideTooltip;
    private float distaceToInteract = 2.5f;
    private Iteam _iteam;
    private RaycastHit hit;
    private DialogeManager dialogeManager;
    private GameObject targetToTalk;

    private void Start()
    {
        dialogeManager = GameObject.Find("DialogeManager").GetComponent<DialogeManager>();
    }

    private void Update()
    {
        if ((!IsToTake) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.collider.GetComponent<IteamConteiner>())
            {
                IsToHideTooltip = true;
                _iteam = hit.collider.GetComponent<IteamConteiner>().iteam;
                Tooltip.ShowTooltip_Static($"{_iteam.name}");
                if (Input.GetMouseButtonUp(0) && Vector3.Distance(hit.point, gameObject.transform.position) <=
                    distaceToInteract)
                {
                    _iteam.PickUp(_iteam.playerInv, hit.collider.gameObject);
                    Tooltip.HideTooltip_Static();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    gameObject.GetComponent<PlayerMovement>().enabled = false;
                    gameObject.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                    IsToTake = true;
                    Tooltip.HideTooltip_Static();
                }
            }
            else if ((!isToTalk) && hit.collider.GetComponent<DialogeBegin>())
            {
                IsToHideTooltip = true;
                targetToTalk = hit.collider.gameObject;
                Tooltip.ShowTooltip_Static("Поговорить");
                if (Input.GetMouseButtonUp(0) &&
                    Vector3.Distance(hit.point, gameObject.transform.position) <= distaceToInteract)
                {
                    dialogeManager.GetComponent<DialogeManager>().beginDialoge(targetToTalk);
                    Tooltip.HideTooltip_Static();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    isToTalk = true;
                    gameObject.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                    gameObject.GetComponent<PlayerMovement>().enabled = false;
                    Tooltip.HideTooltip_Static();
                }
            }
            else if (hit.collider.GetComponent<DoorHandler>())
            {
                IsToHideTooltip = true;
                Tooltip.ShowTooltip_Static(hit.transform.GetComponent<DoorHandler>().isOpen
                    ? "Закрыть дверь"
                    : "Открыть дверь");
                if (Input.GetMouseButtonDown(0))
                {
                    hit.transform.GetComponent<DoorHandler>().enabled = true;
                    hit.transform.GetComponent<DoorHandler>().Invert(transform);
                    Tooltip.HideTooltip_Static();
                }
            }
            else if (IsToHideTooltip)
            {
                Tooltip.HideTooltip_Static();
                IsToHideTooltip = false;
            }
        }

        if (IsToTake
            && (Vector3.Distance(hit.point, gameObject.transform.position) <= distaceToInteract))
        {
            gameObject.GetComponent<PlayerMovement>().enabled = true;
            gameObject.GetComponent<NavMeshAgent>().SetDestination(gameObject.transform.position);
            _iteam.PickUp(_iteam.playerInv, hit.collider.gameObject);
            IsToTake = false;
        }

        if (isToTalk
            && (Vector3.Distance(targetToTalk.transform.position, gameObject.transform.position) <= distaceToInteract))
        {
            gameObject.GetComponent<NavMeshAgent>().SetDestination(gameObject.transform.position);
            dialogeManager.GetComponent<DialogeManager>().beginDialoge(targetToTalk);
            isToTalk = false;
        }
    }
}