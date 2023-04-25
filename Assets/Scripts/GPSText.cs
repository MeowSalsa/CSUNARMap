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
        coordinates.text = "Latitude: " + GPSData.Instance.latitude.ToString("R") + "\n Longitude: " + GPSData.Instance.longitude.ToString("R") + "\n Time stamp: " + GPSData.Instance.timeStamp.ToString() + "\n\n North: " + GPSData.northForText.ToString() + "\n\n Heading Accuracy: " + GPSData.headingAccuracy.ToString();
    }
}
