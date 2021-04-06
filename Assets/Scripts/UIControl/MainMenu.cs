using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _continue;
    [SerializeField] private Slider _volumeSlider;

    void Awake()
    {
        _continue.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        _volumeSlider.onValueChanged.AddListener((val => AudioListener.volume = val));
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
        
    }
}
