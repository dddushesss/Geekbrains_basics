using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogeManager : MonoBehaviour
{
    public GameObject _target; 
    public GameObject dialogeCanvas;

    public void beginDialoge(GameObject target)
    {
        _target = target;
        target.GetComponent<DialogeBegin>().beginDialoge(dialogeCanvas);
    }

    public void choiseProceed(int choise)
    {
        _target.GetComponent<DialogeBegin>().ContinueDialog(choise);
    }

    public void EndDialoge()
    {
        _target = null;
    }
}
