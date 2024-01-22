using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class DirectionsAPIHandler : MonoBehaviour 
{
    [SerializeField] private TextAsset testJSONData;

    private const string API_KEY = "AIzaSyAX_31pghGvv0axcnsP_OR7filS4 - NuJN4";
    protected Direction directions;

    private IEnumerator GetRoute(string uri)
    {
        //Debug.Log("Inside GetRoute IENumerator");
        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending WebRequest: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            directions = JsonUtility.FromJson<Direction>(request.downloadHandler.text);
            Debug.Log("Current direction status: " + directions.status);
        }
    }
    public void CreateDirectionsCall(double user_lat, double user_long, double destination_lat, double destination_long)
    {
        Debug.Log("Making API Call in API handler");
        var uri = string.Format("https://maps.googleapis.com/maps/api/directions/json?origin={0},{1}&destination={2},{3}&mode=walking&key={4}", user_lat, user_long, destination_lat, destination_long, API_KEY);
        StartCoroutine(GetRoute(uri));
    }
    public Direction getDirections() { 
        //directions = JsonUtility.FromJson<Direction>(testJSONData.text);
        return directions; }
}

//This is how API calls should look
//https://maps.googleapis.com/maps/api/directions/json?origin=Disneyland&destination=Universal+Studios+Hollywood&mode=walking&key=AIzaSyAX_31pghGvv0axcnsP_OR7filS4 - NuJN4

//Documentation on making API calls with Directions
//https://developers.google.com/maps/documentation/directions/get-directions