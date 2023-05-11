using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public RectTransform panel;
    public Button button;

    private bool isPanelActive = false;
    private float panelWidth;

    void Start()
    {
        panelWidth = panel.rect.width;
        button.onClick.AddListener(TogglePanel);
    }

    void TogglePanel()
    {
        if (isPanelActive)
        {
            panel.anchoredPosition = new Vector2(-panelWidth, 0);
            isPanelActive = false;
        }
        else
        {
            panel.anchoredPosition = new Vector2(0, 0);
            isPanelActive = true;
        }
    }
}