using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Luck", menuName = "Atributes/Luck")]
public class LuckAtribute : Atribute
{
    public override string ToString()
    {
        return "Удача";
    }

    public override string GetDesctiption()
    {
        return "Дополнение к атрибутам при броске (0-8)";
    }

    public override int ThrowCube()
    {
        return value - Random.Range(1, 6);
    }
}