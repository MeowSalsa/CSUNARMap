using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    public delegate void Demo(bool demoOn);
    public static event Demo OnDemoOn;

    public void buttonClick(bool demoOn)
    {
        OnDemoOn(demoOn);
    }
}
