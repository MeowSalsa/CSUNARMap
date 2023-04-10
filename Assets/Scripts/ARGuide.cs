using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ARGuide : MonoBehaviour
{
    private float User_lat = 34.27744f;
    private float User_lon = -118.44037f;
    private double Dest_lat = 34.27951 ;
    private double Dest_lon = -118.44242;
    private bool destinationSelected;
    private DirectionsAPIHandler directionsAPIHandler;
    [SerializeField] private GameObject directionsHandlerObject;
    [SerializeField] private GameObject gpsHandler;

    private Camera cam;
    private GPSData gpsdata;
    private Compass compass;
    private SphereCollider sphereCollider;
    public GameObject stepPrefab;
    public GameObject interpolatePrefab;
    private List<Step> steps;
    private bool checkpointAchieved;
    private bool destinationReached;
    GameObject currCheckpoint = null;

    private float north;
    // Start is called before the first frame update
    void Start()
    {
        gpsdata = gpsHandler.GetComponent<GPSData>();
        gpsdata.Start();
        north = gpsdata.getNorth();
        Debug.Log("GPS NORTH " + north);
        directionsAPIHandler = directionsHandlerObject.GetComponent<DirectionsAPIHandler>(); //create reference to object that can perform api calls.
        cam = Camera.main;
        destinationSelected = true;

    }

    int interpolationTimer = 1000;
    int currIndex = 0;
    int totalCheckpoints;

    private void Update()
    {            //make apiCall

        //    //while (destinationSelected)
        //    if (destinationSelected)
        //    {
        //        destinationReached = false;
        Debug.Log("Fuk u");
       if(north == float.MaxValue || north == 0)
        {
            Debug.Log("CHECKING NORTH");
            north = gpsdata.getNorth();
            Debug.Log("RESETTING NORTH BECAUSE MAX VAL: " + north);
            return;
        }
      if (steps == null)
            {
                steps = directionsAPIHandler.getDirections().routes[0].legs[0].steps;
                Debug.Log("JSON OBJECT RECEIVED FROM DIRECTIONS API with stepcount" + steps.Count);
               totalCheckpoints = steps.Count;
               
           }
        Debug.Log("NORTH B4 RENDER CHECK: " + north);
        
            Debug.Log("Going to start rendering path W NORTH " + north);
            //get first checkpoint(step)
            var checkpoint = steps[currIndex];
            //while (!destinationReached)
            if (!destinationReached)
            {
                //render step (during this step, unity creates a gameobject that is placed on some position vector.
                if (currCheckpoint == null)
                {
                    currCheckpoint = PlaceCheckpoint(checkpoint);
                    InterpolatePath(currCheckpoint); //IDK
                }
                if (interpolationTimer == 0)
                {
                    InterpolatePath(currCheckpoint);
                    interpolationTimer = 1000;
                }
                //   InterpolatePath(currCheckpoint);
                //if camera.position == stepGameObject.position && checkpoint != lastStep
                if (currIndex != totalCheckpoints)
                {
                    Debug.Log("Currently at step " + currIndex);
                    if (cam.transform.position == gameObject.transform.position)
                    {
                        currIndex++;
                        checkpoint = steps[currIndex];
                        currCheckpoint = null;
                        Debug.Log("Checkpoint achieved! Moving to step " + currIndex);
                    }
                }
                else
                {
                    Debug.Log("Destination Reached");
                    destinationReached = true;
                    destinationSelected = false;
                }
                //checkpoint = nextStep
                //remove step from list of steps
                //if camera.position == stepGameObject.position && checkpoint == lastStep
                //destinationReached = true;
            }
            //destinationSelected = false;

        
            interpolationTimer--;

       // var checkpoint = steps[0];
        //angleFromCoordinate((float)checkpoint.start_location.lat, (float)checkpoint.start_location.lng, (float)checkpoint.end_location.lat, (float)checkpoint.end_location.lng);
    }
    private void InterpolatePath(GameObject checkpoint)
    {
        Debug.Log("Interpolating path");
        //consider the camera's position as a zero vector.
        //then we just have to place objects at every .3 of a meter 
        var checkpointVector = checkpoint.transform.position;
        var camPosition = cam.transform.position;
        var deltaX = checkpointVector.x - camPosition.x;
        var deltaY = checkpointVector.y - 0;
        var deltaZ = checkpointVector.z - camPosition.z;
        var distance = Mathf.Sqrt((deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ));
        distance = Mathf.Floor(distance);
        for (int i = 0; i < distance; i++)
        {
            var interpolation = Instantiate(interpolatePrefab);
            interpolation.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            interpolation.transform.position = new Vector3(0f, 0f, i);
        }
    }
    private GameObject PlaceCheckpoint(Step checkpoint)
    {
        Debug.Log("Placing checkpoint");
        //find angle between user and checkpoint
        var geographicalNorth = north;
        Debug.Log("GEOGRAPHICAL NORTH IS " + north);
        var geoNorthRight = Mathf.Cos(geographicalNorth);
        var geoNorthForward = Mathf.Sin(geographicalNorth);
        var bearing = angleFromCoordinate((float)checkpoint.start_location.lat, (float)checkpoint.start_location.lng,(float)checkpoint.end_location.lat, (float)checkpoint.end_location.lng);
        bearing = Mathf.Abs(bearing - 360) + 90;
        //xpos = cosine(angle)
        var forward = Mathf.Cos(bearing);
        //ypos = sin(angle)
        var right = Mathf.Sin(bearing);
        var magnitude = checkpoint.distance.value;
        //GameObject checkpointGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var checkpointGameObject = Instantiate(stepPrefab);
        if(checkpointGameObject!= null) { Debug.Log("Checkpoint object created??"); }
        checkpointGameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        var vector = new Vector3(magnitude * right, 0, magnitude * forward );
        checkpointGameObject.transform.position = vector;
        var q = Quaternion.Euler(0, geographicalNorth, 0); //MAKE SURE TO ROTATE AROUND WORLD SPACE NOT LOCAL SPACE
        checkpointGameObject.transform.rotation = q; //new Vector3();
        return checkpointGameObject;
    }
    void SendLatLongData()
    {
        directionsAPIHandler.CreateDirectionsCall(User_lat, User_lon, Dest_lat, Dest_lon);
    }
    private float angleFromCoordinate(float latA, float longA, float latB, float longB)//lat1 long1 is start, lat2 long2 is endlocation
    {
        Debug.Log("Finding angle with User Location: "  +latA + " "+ longA);
        Debug.Log("Destination is: " + latB + " " + longB);

        //Convert units form Degrees to Radians
        latB = latB*Mathf.Deg2Rad;
        latA=latA*Mathf.Deg2Rad;
        longA= longA*Mathf.Deg2Rad;
        longB  = longB*Mathf.Deg2Rad;

        var y = Mathf.Sin(longB - longA) * Mathf.Cos(latB);
        var x = Mathf.Cos(latA)*Mathf.Sin(latB) - Mathf.Sin(latA)*Mathf.Cos(latB)*Mathf.Cos(longB - longA);

        var bearing = Mathf.Atan2(y,x);

        var bearingDegrees = bearing * Mathf.Rad2Deg; //Mathf.Atan2(deltaLon, deltaLat)*Mathf.Rad2Deg;
        bearing = (bearing + 360) % 360;
        Debug.Log("Fucked degrees: " + bearingDegrees); 
        bearingDegrees = (bearingDegrees + 360)%360;
        Debug.Log("Bearing degrees are " + bearingDegrees);
        return bearingDegrees;
    }
}
