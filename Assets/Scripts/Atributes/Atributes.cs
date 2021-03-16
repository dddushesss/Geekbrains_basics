using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Serialization;

public class Atributes : MonoBehaviour
{
    public List<Atribute> PlayerAtributes;
    [NonSerialized]
    public int CurentHealth;
    
    public int StartSkillPoints;
    public GameObject atributeUIPrefab;
    public GameObject atributeUICanvas;

    private void Start()
    {
        foreach (var atribute in PlayerAtributes)
        {
            var prefab = Instantiate(atributeUIPrefab, atributeUICanvas.transform.GetChild(0));
            prefab.GetComponentInChildren<PlayerAtributeUI>().atribute = atribute;
            atribute.CharacterAtributes = this;
        }
        atributeUICanvas.transform.GetChild(0).GetChild(1).GetComponent<SkillPoints>().ChangeSkillPointsValue(StartSkillPoints);
        CurentHealth = PlayerAtributes.Find(x => x is HealthAtribute).value;
        atributeUICanvas.SetActive(false);
    }

    public int Damage()
    {
        var Strength = PlayerAtributes.Find(x => x is StrengthAtribute);
        var Luck = PlayerAtributes.Find(x => x is LuckAtribute);
        var Agility = PlayerAtributes.Find(x => x is AgilityAtribute);

        if (Agility.ThrowCube() + Luck.ThrowCube() - 7 >= 0)
        {
            return Strength.ThrowCube() - 1;
        }

        return 0;
    }

    public void ChangeHealth(int count)
    {
        int MaxHealth = PlayerAtributes.Find(x => x is HealthAtribute).value;
        if (CurentHealth + count > MaxHealth)
        {
            CurentHealth = MaxHealth;
        }
        else if (CurentHealth + count <= 0)
        {
            gameObject.GetComponent<Character>().Death();
        }
        else
        {
            CurentHealth += count;
        }
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            atributeUICanvas.SetActive(!atributeUICanvas.activeSelf);
        }
    }
}