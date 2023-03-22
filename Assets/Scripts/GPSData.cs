using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using TMPro;
using UnityEngine.UI;

public class GPSData : MonoBehaviour
{
    public static GPSData Instance { get; set; }
    public TMP_Text GPS_Status; 
    public float longitude;
    public float latitude;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(GPSLocation());   
    }
    private void Update()
    {
        GPS_Status.text = "INside updategps fn";
        if (Input.location.status == LocationServiceStatus.Running)
        {
            GPS_Status.text = "Connection Successful";

            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
        }
    }
    IEnumerator GPSLocation()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        if (!Input.location.isEnabledByUser)
        {
            yield break;
        }
        // Starts the location service.
        Input.location.Start(0f, 0f);

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            GPS_Status.text = "Timed out";
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            GPS_Status.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            //Connection successful
            GPS_Status.text = "Invoking Repeating Fn";

            //InvokeRepeating("UpdateGPSData", 0.2f, 1.0f);
        }

        // Stops the location service if there is no need to query location updates continuously.
    }

    //private void UpdateGPSData()
    //{
      //  GPS_Status.text = "INside updategps fn";
        //if (Input.location.status == LocationServiceStatus.Running)
        //{
          //  GPS_Status.text = "Connection Successful";

            //latitude = Input.location.lastData.latitude;
            //longitude = Input.location.lastData.longitude;
        //}
        //else
        //{
          //  Input.location.Stop();
        //}
    //}
}

