using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Atributes : MonoBehaviour
{
    public List<Atribute> PlayerAtributes;
    [NonSerialized]
    public int CurentHealth;
    
    public int StartSkillPoints;
    public GameObject atributeUIPrefab;
    public GameObject atributeUICanvas;
    
    public Image Bar;
    private Text healthText;

    public override string ToString()
    {
        return "Игрок";
    }

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
        healthText = Bar.GetComponentInChildren<Text>();
        Bar.fillAmount = (float)(CurentHealth) /
                         (float)PlayerAtributes.Find(x => x is HealthAtribute).value;
        healthText.text = CurentHealth + "/" +
                          PlayerAtributes.Find(x => x is HealthAtribute).value;
        
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

    private void Death()
    {
        Destroy(gameObject);
    }
    
    public bool ChangeHealth(int count)
    {
        int MaxHealth = PlayerAtributes.Find(x => x is HealthAtribute).value;
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
        
        Bar.fillAmount = (float)(CurentHealth) /
                         (float)PlayerAtributes.Find(x => x is HealthAtribute).value;
        healthText.text = CurentHealth + "/" +
                          PlayerAtributes.Find(x => x is HealthAtribute).value;
        
        return false;
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            atributeUICanvas.SetActive(!atributeUICanvas.activeSelf);
        }
    }
}