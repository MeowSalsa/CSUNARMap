using TMPro;
using UnityEngine;

public class ARGuide : MonoBehaviour
{
    private float User_lat;
    private float User_lon;
    private float Dest_lat;
    private float Dest_lon;

    int stepLength = 0;
    int stepCount = 0;
    double nextLat;
    double nextLon;
    double startLat;
    double startLon;

    private DirectionsAPIHandler directionsAPIHandler;
    [SerializeField] private GameObject directionsHandlerObject;

    private Step currStep;
    private Direction directions;
    public TMP_Text directionText;

    private Direction CurrentDirection;
    // Start is called before the first frame update
    void Start()
    {
        directionsAPIHandler = directionsHandlerObject.GetComponent<DirectionsAPIHandler>(); //create reference to object that can perform api calls.
    }
    int index = 0;
    private void Update()
    {
        //GetDirections();

        DotTest();
    }
    

    public void GoButtonClick()
    {
        SendLatLongData();
        GetDirections();
        directionText.text = distanceFromCoordinates((float)User_lat, (float)User_lon, (float)nextLat, (float)nextLon).ToString();
        //directionText.text = angleFromCoordinate((float)User_lat, (float)User_lon, (float)nextLat, (float)nextLon).ToString();
    }

    public void DotTest()
    {
        if (nextLon != 0 && index == 0)
        {
            var dot = GameObject.CreatePrimitive(0);
            dot.name = string.Format("myDot{0}", index.ToString());
            index++;
            
            //dot.transform.position = Quaternion.AngleAxis((float)nextLon, -Vector3.up) * Quaternion.AngleAxis((float)nextLat, -Vector3.right) * new Vector3(0, 0, 1) * 10;
        }
    }

    void SendLatLongData()
    {
        //get user lat and lon get next step
        User_lat = 34.236631f;//GPSData.Instance.latitude;
        User_lon = -118.4689255f;//GPSData.Instance.longitude;
        
        //pass through destination lat and long
        Dest_lat = 34.2366232f;
        Dest_lon = -118.4667621f;
        directionsAPIHandler.CreateDirectionsCall(User_lat, User_lon, Dest_lat, Dest_lon);
        
    }

    void GetDirections()
    {
        directions = directionsAPIHandler.getDirections();
        //init current step
        
        currStep = directions.routes[0].legs[0].steps[stepCount];

        stepLength = directions.routes[0].legs[0].steps.Count;//size of list

        nextLat = currStep.end_location.lat;
        nextLon = currStep.end_location.lng;

        Debug.Log(nextLon);
        Debug.Log(nextLat);
    }
    void NextStep()
    {
        stepCount++;
        if (stepCount < stepLength)
        {
            currStep = directions.routes[0].legs[0].steps[stepCount];
        }
    }
    //get distance between to points
    private float distanceFromCoordinates(float lat1, float long1, float lat2, float long2)
    {
        float dist = 0;
        int R = 6371;

        var latRad1 = Mathf.Deg2Rad * lat1;
        var latRad2 = Mathf.Deg2Rad * lat2;
        var dLatRad = Mathf.Deg2Rad * (lat2 - lat1);
        var dLongRad = Mathf.Deg2Rad * (long2 - long1);

        var a = Mathf.Pow(Mathf.Sin(dLatRad / 2),2) + Mathf.Cos(latRad1) * Mathf.Cos(latRad2) * Mathf.Pow(Mathf.Sin(dLongRad/2),2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        dist = R * c * 1000; // convert to meters

        Debug.Log(dist);
        return dist;
    }

    //get bearing of two points
    private float angleFromCoordinate(float lat1, float long1, float lat2, float long2)//lat1 long1 is start, lat2 long2 is endlocation
    {
        float dLon = (long2 - long1);

        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2);
        float x = (Mathf.Cos(lat1) * Mathf.Sin(lat2)) - (Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(dLon));

        float brng = Mathf.Atan2(y, x);

        brng = Mathf.Rad2Deg * brng;
        brng = (brng + 360) % 360;

        Debug.Log(brng);
        return brng;

    }
}
