using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeTextScript : MonoBehaviour
{
    public TextMeshProUGUI myText;
    private bool isTextOne = true;

    void Start()
    {
        myText.text = "Menu";
    }

    public void ToggleText()
    {
        isTextOne = !isTextOne;
        if (isTextOne)
        {
            myText.text = "Stop";
        }
        else
        {
            myText.text = "Stop";
        }
    }
        public GameObject targetButton;

    public void ToggleTargetButton()
    {
        if (myText.text == "Stop")
        {
            myText.text = "Menu";
        }
        else
        {
        }
    }
}

