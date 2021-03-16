using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    public NavMeshAgent _navMeshAgent;
    private Animator _animator;
    public GameObject Tutorial;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                _animator.SetBool("IsWalking", true);
                _navMeshAgent.SetDestination(hit.point);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Tutorial.SetActive(!Tutorial.activeSelf);
        }
        
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _animator.SetBool("IsWalking", false);
        }
        
    }
}