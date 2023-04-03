using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GPSText : MonoBehaviour
{
    public TMP_Text coordinates;

    // Update is called once per frame
    void Update()
    {
        coordinates.text = "Latitude: " + GPSData.Instance.latitude.ToString() + "\n\n Longitude: " + GPSData.Instance.longitude.ToString() + "\n\n Time stamp: " + GPSData.Instance.timeStamp.ToString();
    }
}
