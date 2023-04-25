using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ARGuide : MonoBehaviour
{
    private double Demo_Dest_lat = 34.27934545966688;
    private double Demo_Dest_lon = -118.43802427080684;
    private bool destinationSelected;
    private DirectionsAPIHandler directionsAPIHandler;
    [SerializeField] private GameObject directionsHandlerObject;
    [SerializeField] private GameObject gpsHandler;
    private LineRenderer lineRenderer;
    private Camera cam;
    private SphereCollider sphereCollider;
    public GameObject stepPrefab;
    public GameObject interpolatePrefab;
    private List<Step> steps;
    private bool checkpointAchieved;
    private bool destinationReached;
    GameObject currCheckpoint = null;

    private float north;
    FileStream fs;
    bool isDemoOn;
    private void OnEnable()
    {
        DemoManager.OnDemoOn += DemoManager_OnDemoOn;
    }

    private void DemoManager_OnDemoOn(bool demoOn)
    {
        Debug.Log("Demo Event Handler");
        isDemoOn = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        north = GPSData.getNorth();
        Debug.Log("North In START is " + north);    
        directionsAPIHandler = directionsHandlerObject.GetComponent<DirectionsAPIHandler>(); //create reference to object that can perform api calls.
        cam = Camera.main;
        destinationSelected = false;
        fs = File.OpenWrite("northData.csv");

    }

    int interpolationTimer = 1000;
    int currIndex = 0;
    int totalCheckpoints;

    private void Update()
    {
        if (isDemoOn && destinationSelected == false) 
        {
            Debug.Log("Making an API call");
            directionsAPIHandler.CreateDirectionsCall(GPSData.Instance.latitude, GPSData.Instance.longitude , Demo_Dest_lat, Demo_Dest_lon);
            destinationSelected = true;
        }
        if (north == float.MaxValue || north == 0)
        {
            north = GPSData.getNorth();
            // Debug.Log("North was " + north + " " + "Now it is " + GPSData.getNorth());
            return;
        }
      
        //USING THIS TO GET NORTH DATA FOR VARIANCE
        if (true)
        {
             GPSData.getNorth();
            return;
        }


        // if steps doesnt contain any steps yet, get steps from directionsAPIHandler.
        //returns in case DirectionsAPIHandler isnt ready. This if statement stops working
        //when steps is not null meaning APIHandler has returned steps.
        if (destinationSelected)
        {
            if (steps == null)
            {

                steps = directionsAPIHandler.getDirections().routes[0].legs[0].steps;
                totalCheckpoints = steps.Count;
                Debug.Log("Getting steps " + totalCheckpoints);
                return;
            }

            //get first step
            var checkpoint = steps[currIndex];
            if (!destinationReached)
            {
                //place a step (during this step, unity creates a gameobject that is placed on some position vector.
                if (currCheckpoint == null)
                {
                    currCheckpoint = PlaceCheckpoint(checkpoint);
                    InterpolatePath(currCheckpoint);

                }

                if (currIndex != totalCheckpoints)
                {
                    if (cam.transform.position == gameObject.transform.position)
                    {
                        currIndex++;
                        checkpoint = steps[currIndex];
                        Destroy(currCheckpoint);
                        currCheckpoint = null;
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
        }
    }
    private void InterpolatePath(GameObject checkpoint)
    {
        List<Vector3> spherePositions = new List<Vector3>();
        Debug.Log("Interpolating path");
        //consider the camera's position as a zero vector.
        //then we just have to place objects at every .3 of a meter 
        var checkpointVector = checkpoint.transform.position;
        var camPosition = cam.transform.position;
        var distance = Vector3.Distance(camPosition, checkpointVector);
        distance = Mathf.Floor(distance);
        int numSpheres = (int) distance;
        for (int i = 0; i <= numSpheres; i++)
        {
            Vector3 spherePosition = Vector3.Lerp(checkpointVector, camPosition, (float)i / (float)numSpheres);
            spherePositions.Add(spherePosition);
        }
        foreach (Vector3 position in spherePositions)
        {
            var interpolation = Instantiate(interpolatePrefab);
            interpolation.transform.position = position;
            interpolation.transform.localScale= Vector3.one*0.1f;
        }
    }
    private void debugLine(GameObject checkPoint)
    {
         var targetPos = checkPoint.transform.position;
        lineRenderer.SetPosition(1,targetPos);
    }


   // transform.rotation= worldRotation;
    private GameObject PlaceCheckpoint(Step checkpoint)
    {
        Debug.Log("Placing checkpoint");
        //find angle between user and checkpoint
        var geographicalNorth = GPSData.getNorth();
        var bearing = angleFromCoordinate(checkpoint.start_location.lat, checkpoint.start_location.lng,checkpoint.end_location.lat,checkpoint.end_location.lng);
        Debug.Log("Bearing  " + bearing + "\n North " + geographicalNorth);
        //bearing = (bearing +360) % 360;
        Debug.Log("Bearing after +90 " + bearing);
        //bearing += GPSData.getNorth();
        bearing *= Mathf.Deg2Rad;
        Debug.Log("Bearing after deg2rad " + bearing);
        var forward = (float)Math.Cos(bearing);
        var right = (float)Math.Sin(bearing);
        Debug.Log("FORWARD AND RIGHT ARE " + forward + " " + right);
        var magnitude = checkpoint.distance.value;
        var checkpointGameObject = Instantiate(stepPrefab);
        checkpointGameObject.transform.localScale = Vector3.one * 0.1f;
        var unrotatedVector = new Vector3( right, 0,forward ) * magnitude; //GameObject's calculated position from cos(bearing), sin(bearing) NOT rotated to true north yet.
        checkpointGameObject.transform.position = unrotatedVector;
        Debug.Log("Magnitude of step was " + magnitude);
        Debug.Log("Unrotated vector " + unrotatedVector.ToString());
        //float rotationfudge = -180f;
        checkpointGameObject.transform.RotateAround(cam.transform.position, Vector3.up, -GPSData.getNorth() );// -GPSData.getNorth() + rotationfudge);

        Debug.Log("Rotated vector ends up being " + checkpointGameObject.transform.position.ToString() + "\nRotation is " + checkpointGameObject.transform.rotation);
        
        //NORTH INDICATOR
        var test = Instantiate(interpolatePrefab);
        var positionvector = new Vector3(0,0,1);
        test.transform.position = positionvector + cam.transform.position; //????
        test.transform.RotateAround(cam.transform.position, Vector3.up, -GPSData.getNorth());
        
        return checkpointGameObject;
    }
    private double angleFromCoordinate(double latA, double longA, double latB, double longB)//lat1 long1 is start, lat2 long2 is endlocation
    {
        Debug.Log("Finding angle with User Location: "  +latA + " "+ longA);
        Debug.Log("Destination is: " + latB + " " + longB);

        //Convert units from Degrees to Radians
        latB =Deg2Rad(latB);
        latA=Deg2Rad(latA);
        longA= Deg2Rad(longA);
        longB  = Deg2Rad(longB);

        var deltaLong = longB - longA;
        var y = Math.Sin(deltaLong) * Math.Cos(latB);
        var x = Math.Cos(latA)*Math.Sin(latB) - Math.Sin(latA)*Math.Cos(latB)*Math.Cos(deltaLong);
        var bearing = Math.Atan2(y,x);
        bearing = Rad2Deg(bearing);
        bearing = (bearing + 360) % 360;
        return bearing;
    }

    private double Deg2Rad(double degrees)
    {
        double radians = (Math.PI/180)*degrees;
        return radians;
    }
    private double Rad2Deg(double radians) { 
        double degrees = (180/Math.PI)*radians;
        return degrees;
    }
}
