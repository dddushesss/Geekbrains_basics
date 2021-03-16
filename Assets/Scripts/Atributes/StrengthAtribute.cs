using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Strength", menuName = "Atributes/Strength")]
public class StrengthAtribute : Atribute
{
    public override string ToString()
    {
        return "Сила";
    }

    public override string GetDesctiption()
    {
        return "Влияет на урон (0-6)";
    }
}
