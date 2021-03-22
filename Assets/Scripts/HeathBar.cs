using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class HeathBar : MonoBehaviour
{
    public Image Bar;
    private Atributes _playerAtributes;
    private Text healthText;
    void Start()
    {
        _playerAtributes = GetComponent<Atributes>();
        healthText = Bar.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Bar.fillAmount = (float)(_playerAtributes.CurentHealth) /
                         (float)_playerAtributes.PlayerAtributes.Find(x => x is HealthAtribute).value;
        healthText.text = _playerAtributes.CurentHealth + "/" +
                          _playerAtributes.PlayerAtributes.Find(x => x is HealthAtribute).value;
    }
}
