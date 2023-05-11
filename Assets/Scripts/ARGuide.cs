using System;
using System.Collections.Generic;
using UnityEngine;

public class ARGuide : MonoBehaviour
{
    private bool destinationSelected;
    private DirectionsAPIHandler directionsAPIHandler;
    [SerializeField] private GameObject directionsHandlerObject;
    private Camera cam;
    public GameObject stepPrefab;
    public GameObject interpolatePrefab;
    private List<Step> steps;
    private bool destinationReached;
    GameObject currCheckpoint = null;
    private List<GameObject> interpolationObjectsList;
    private float north;

    float angle;
    float offsetAngle;
    Vector3 targetDot;
    Vector3 forward;

    public GameObject RightArrow;
    public GameObject LeftArrow;

    void Start()
    {
        north = GPSData.getNorth();
        directionsAPIHandler = directionsHandlerObject.GetComponent<DirectionsAPIHandler>();
        cam = Camera.main;
        destinationSelected = false;
    }

    int currIndex = 0;
    int totalCheckpoints;

    private void Update()
    {
        //Keep trying to get north until it's not the preset MaxValue or 0.
        if (north == float.MaxValue || north == 0)
        {
            north = GPSData.getNorth();
            return;
        }
        //destinationSelected should be set to true when the User selects a destination and presses start
        if (destinationSelected)
        {    // if steps doesnt contain any steps yet, get steps from directionsAPIHandler.
             //returns in case DirectionsAPIHandler isnt ready. This if statement stops working
             //when steps is not null meaning APIHandler has returned steps.
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
                    offsetAngle = getAngle(currCheckpoint);
                    
                }

                if (currIndex != totalCheckpoints)
                {
                    //check if user is going the right direction
                    checkDirection();
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
            }
        }
    }
    private void InterpolatePath(GameObject checkpoint)
    {
        List<Vector3> spherePositions = new List<Vector3>();
        Debug.Log("Interpolating path");
        var checkpointVector = checkpoint.transform.position;
        var camPosition = cam.transform.position;
        var distance = Vector3.Distance(camPosition, checkpointVector);
        distance = Mathf.Floor(distance);
        int numSpheres = (int) distance;
        //linear interpolation between checkpoint and camera position
        for (int i = 0; i <= numSpheres; i++)
        {
            Vector3 spherePosition = Vector3.Lerp(checkpointVector, camPosition, (float)i / (float)numSpheres);
            spherePositions.Add(spherePosition);
        }
        //Create the objects in the World Space and add them to interpolation object list
        foreach (Vector3 position in spherePositions)
        {
            var interpolation = Instantiate(interpolatePrefab);
            interpolation.transform.position = position;
            interpolation.transform.localScale= Vector3.one*0.1f;
            interpolationObjectsList.Add(interpolation);
        }
    }
    private GameObject PlaceCheckpoint(Step checkpoint)
    {
        Debug.Log("Placing checkpoint");
        //find angle between user and checkpoint
        var geographicalNorth = GPSData.getNorth();
        var bearing = angleFromCoordinate(checkpoint.start_location.lat, checkpoint.start_location.lng,checkpoint.end_location.lat,checkpoint.end_location.lng);
        //bearing = (bearing +360) % 360;
        bearing *= Mathf.Deg2Rad;
        Debug.Log("Bearing after deg2rad " + bearing);
        var forward = (float)Math.Cos(bearing);
        var right = (float)Math.Sin(bearing);
        Debug.Log("FORWARD AND RIGHT ARE " + forward + " " + right);
        //Magnitude (distance) is stored in the step's json data.
        var magnitude = checkpoint.distance.value;
        var checkpointGameObject = Instantiate(stepPrefab);
        checkpointGameObject.transform.localScale = Vector3.one * 0.1f;
        //unrotatedVector is GameObject's calculated position from cos(bearing), sin(bearing) NOT rotated to true north yet.
        var unrotatedVector = new Vector3( right, 0,forward ) * magnitude; 
        checkpointGameObject.transform.position = unrotatedVector;
        Debug.Log("Magnitude of step was " + magnitude);
        Debug.Log("Unrotated vector " + unrotatedVector.ToString());
        checkpointGameObject.transform.RotateAround(cam.transform.position, Vector3.up, -GPSData.getNorth() );
        Debug.Log("Rotated vector ends up being " + checkpointGameObject.transform.position.ToString() + "\nRotation is " + checkpointGameObject.transform.rotation);
        
        //NORTH INDICATOR -- Just creates an object wherever North is.
       /* var test = Instantiate(interpolatePrefab);
        var positionvector = new Vector3(0,0,1);
        test.transform.position = positionvector + cam.transform.position; //????
        test.transform.RotateAround(cam.transform.position, Vector3.up, -GPSData.getNorth());*/
        
        return checkpointGameObject;
    }
    private double angleFromCoordinate(double startLat, double startLong, double endLat, double endLong)
    {
        Debug.Log("Finding angle with User Location: "  +startLat + " "+ startLong);
        Debug.Log("Destination is: " + endLat + " " + endLong);

        //Convert units from Degrees to Radians
        endLat =Deg2Rad(endLat);
        startLat=Deg2Rad(startLat);
        startLong= Deg2Rad(startLong);
        endLong  = Deg2Rad(endLong);

        var deltaLong = endLong - startLong;
        var y = Math.Sin(deltaLong) * Math.Cos(endLat);
        var x = Math.Cos(startLat)*Math.Sin(endLat) - Math.Sin(startLat)*Math.Cos(endLat)*Math.Cos(deltaLong);
        var bearing = Math.Atan2(y,x);
        bearing = Rad2Deg(bearing);
        bearing = (bearing + 360) % 360;
        return bearing;
    }
    //Deg2Rad and Rad2Deg are custom functions to keep the precision in double as opposed to Unity's functions that return floats.
    private double Deg2Rad(double degrees)
    {
        double radians = (Math.PI/180)*degrees;
        return radians;
    }
    private double Rad2Deg(double radians) { 
        double degrees = (180/Math.PI)*radians;
        return degrees;
    }
    private bool isVisible(GameObject path)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Vector3 point = path.transform.position;

        foreach(var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }

    private float getAngle(GameObject currDot)
    {
        targetDot = currDot.transform.position - cam.transform.position;
        forward = cam.transform.forward;
        return Vector3.SignedAngle(targetDot, forward, Vector3.up);
    }

    private void checkDirection()
    {
        angle = getAngle(currCheckpoint) - offsetAngle;
        Debug.Log("angle " + angle);
        if (!isVisible(currCheckpoint) && angle < -8.0F)
        {
            Debug.Log("Turn Right");
            RightArrow.transform.position = new Vector3(100f, 2000f, 0f);
            LeftArrow.transform.position = new Vector3(-1207f, 526.27f, 0f);
        }
        else if (!isVisible(currCheckpoint) && angle > 8.0F)
        {
            Debug.Log("Turn Left");
            LeftArrow.transform.position = new Vector3(1340f, 2000f, 0f);
            RightArrow.transform.position = new Vector3(-1207f, 526.27f, 0f);
        }
        else
        {
            Debug.Log("Forward");
            RightArrow.transform.position = new Vector3(-1207f, 526.27f, 0f);
            LeftArrow.transform.position = new Vector3(-1207f, 526.27f, 0f);
        }
    }
}
