using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Agility", menuName = "Atributes/Agility")]
public class AgilityAtribute : Atribute
{
    public override string ToString()
    {
        return "Ловкость";
    }

    public override string GetDesctiption()
    {
        return "Шанс попасть по врагу (0-6)";
    }
}
