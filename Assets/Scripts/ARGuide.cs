using UnityEngine;

public class ARGuide : MonoBehaviour
{
    private double User_lat;
    private double User_lon;
    private double Dest_lat;
    private double Dest_lon;

    private DirectionsAPIHandler directionsAPIHandler;
    [SerializeField] private GameObject directionsHandlerObject;

   
    private Direction CurrentDirection;
    // Start is called before the first frame update
    void Start()
    {
        directionsAPIHandler = directionsHandlerObject.GetComponent<DirectionsAPIHandler>(); //create reference to object that can perform api calls.
    }
    private void Update()
    {

    }

    void SendLatLongData()
    {
        directionsAPIHandler.CreateDirectionsCall(User_lat, User_lon, Dest_lat, Dest_lon);
    }

}
