using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropdownButton : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public RectTransform panel;
    private float panelWidth;

    private void Start()
    {
        // Hide the dropdown when the game starts
        //dropdown.gameObject.SetActive(false);
        panelWidth = panel.rect.width;
    }

    public void ToggleDropdown()
    {
        // Get the currently selected option from the dropdown menu
        int selectedOption = dropdown.value;

        // Trigger an event based on the selected option
        switch (selectedOption)
        {
            case 0:
                // Call a method or trigger an event for the first option
                Debug.Log("First option selected");
                break;
            case 1:
                // Call a method or trigger an event for the second option
                Debug.Log("Second option selected");
                break;
            case 2:
                // Call a method or trigger an event for the third option
                Debug.Log("Third option selected");
                break;
            // Add more cases for additional options as needed
            default:
                // Do nothing if no option is selected
                break;
        }
        panel.anchoredPosition = new Vector2(-panelWidth, 0);
    }

}
