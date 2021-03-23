using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    public List<Atribute> atributes;
    public float distanceToFight = 2f;
    public GameObject fightCanvas;
    public string name;
    private int CurentHealth;
    private bool _isTrigered;
    private GameObject _target;
    private NavMeshAgent _navMeshAgent;
    
    private void Start()
    {
        CurentHealth = atributes.Find(x => x is HealthAtribute).value;
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override string ToString()
    {
        return name;
    }

    public int Damage()
    {
        var Strength = atributes.Find(x => x is StrengthAtribute);
        var Luck = atributes.Find(x => x is LuckAtribute);
        var Agility = atributes.Find(x => x is AgilityAtribute);

        if (Agility.ThrowCube() + Luck.ThrowCube() - 7 >= 0)
        {
            return Strength.ThrowCube() - 1;
        }

        return 0;
    }

    public bool ChangeHealth(int count)
    {
        int MaxHealth = atributes.Find(x => x is HealthAtribute).value;
        if (CurentHealth + count > MaxHealth)
        {
            CurentHealth = MaxHealth;
        }
        else if (CurentHealth + count <= 0)
        {
            Death();
            return true;
        }
        else
        {
            CurentHealth += count;
        }

        return false;
    }

    private void Death()
    {
        gameObject.GetComponent<Inventory>().DropAll();
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            _isTrigered = true;
            _target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            _isTrigered = false;
            
        }
    }

    private void Update()
    {
        if (_isTrigered)
        {
            _navMeshAgent.SetDestination(_target.transform.position);
            if (_isTrigered && Vector3.Distance(this.transform.position, _target.transform.position) <= distanceToFight)
            {
                _isTrigered = false;
                GetComponent<WaypointPatrol>().enabled = false;
                _navMeshAgent.SetDestination(transform.position);
                fightCanvas.SetActive(true);
                fightCanvas.GetComponent<Fight>().BeginFight(_target, gameObject);
            }
        }
    }
}
