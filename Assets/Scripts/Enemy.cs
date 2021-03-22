using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public List<Atribute> Atributes;
    public float DistanceToFight = 2f;
    public GameObject FightCanvas;
    public string Name;
    [NonSerialized]
    public int CurentHealth;
    private bool _isTrigered;
    private GameObject _target;
    private NavMeshAgent _navMeshAgent;
    
    private void Start()
    {
        CurentHealth = Atributes.Find(x => x is HealthAtribute).value;
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override string ToString()
    {
        return Name;
    }

    public int Damage()
    {
        var Strength = Atributes.Find(x => x is StrengthAtribute);
        var Luck = Atributes.Find(x => x is LuckAtribute);
        var Agility = Atributes.Find(x => x is AgilityAtribute);

        if (Agility.ThrowCube() + Luck.ThrowCube() - 7 >= 0)
        {
            return Strength.ThrowCube() - 1;
        }

        return 0;
    }

    public bool ChangeHealth(int count)
    {
        int MaxHealth = Atributes.Find(x => x is HealthAtribute).value;
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
            if (_isTrigered && Vector3.Distance(this.transform.position, _target.transform.position) <= DistanceToFight)
            {
                _isTrigered = false;
                GetComponent<WaypointPatrol>().enabled = false;
                _navMeshAgent.SetDestination(transform.position);
                FightCanvas.SetActive(true);
                FightCanvas.GetComponent<Fight>().BeginFight(_target, gameObject);
            }
        }
    }
}
