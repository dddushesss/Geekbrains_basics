using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class IteamHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    private Iteam _iteam;
    private float distaceToInteract = 2.5f;
    private GameObject player;
    private bool IsToTalk = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        _iteam = GetComponentInParent<Iteam>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0) && Vector3.Distance(transform.position, player.transform.position) <= distaceToInteract)
        {
            _iteam.PickUp(_iteam.playerInv);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<NavMeshAgent>().SetDestination(transform.position);
            IsToTalk = true;
        }

        Tooltip.HideTooltip_Static();
    }

    private void Update()
    {
        if (IsToTalk
            && (Vector3.Distance(transform.position, player.transform.position) <= distaceToInteract))
        {
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
            _iteam.PickUp(_iteam.playerInv);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.ShowTooltip_Static($"{_iteam.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.HideTooltip_Static();
    }
}