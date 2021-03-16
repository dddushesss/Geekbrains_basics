using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    private static Tooltip instance;

    [SerializeField]
    private Camera uiCamera;

    private Text tooltipText;
    private RectTransform backgroundRectTransform;

    private void Awake() {
        instance = this;
        backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        tooltipText = transform.GetChild(0).Find("text").GetComponent<Text>();

        HideTooltip();
        
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera,
            out localPoint);
        transform.localPosition = localPoint + new Vector2(backgroundRectTransform.sizeDelta.x + 25f, -backgroundRectTransform.sizeDelta.y - 25f)/2;
    }

    private void ShowTooltip(string tooltipString) {
        gameObject.SetActive(true);
        tooltipText.text = tooltipString;
        float textPaddingSize = 4f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 4f);
        
        backgroundRectTransform.sizeDelta = backgroundSize;
        Update();
    }

    private void HideTooltip() {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString) {
        instance.ShowTooltip(tooltipString);
    }

    public static void HideTooltip_Static() {
        instance.HideTooltip();
    }

}
