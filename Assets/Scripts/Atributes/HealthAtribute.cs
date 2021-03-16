using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Atributes/Health")]
public class HealthAtribute : Atribute
{
    public override string ToString()
    {
        return "Здоровье";
    }

    public override void ChangeAtributeValue(int count)
    {
        value += count;
        CharacterAtributes.CurentHealth = value;
    }

    public override string GetDesctiption()
    {
        return "Максимальное количество здоровья (0-12)";
    }
}
