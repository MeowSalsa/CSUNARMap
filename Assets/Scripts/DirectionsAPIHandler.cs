using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;


public class DirectionsAPIHandler : MonoBehaviour
{
    public TextAsset testJSONData;
    private const string API_KEY = "AIzaSyAX_31pghGvv0axcnsP_OR7filS4 - NuJN4";

    //public string url = string.Format("https://maps.googleapis.com/maps/api/directions/json?origin={0}{1}&destination={2}{3}&mode=walking&key={4}",user_lat, user_long, dest_lat,dest_long, API_KEY);
    public string uri = "https://maps.googleapis.com/maps/api/directions/json?origin=Disneyland&destination=Universal+Studios+Hollywood&mode=walking&key=AIzaSyAX_31pghGvv0axcnsP_OR7filS4 - NuJN4";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRoute(uri));

    }

    private IEnumerator GetRoute(string uri)
    {
        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending WebRequest: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
        }
        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("https://maps.googleapis.com/maps/api/directions/json?origin=Disneyland&destination=Universal+Studios+Hollywood&key={0}", API_KEY));
        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //StreamReader reader = new StreamReader(response.GetResponseStream());
        //string json = reader.ReadToEnd();

        var myDeserializedClass = JsonUtility.FromJson<Direction>(request.downloadHandler.text);


    }
}

//This is how API calls should look
//https://maps.googleapis.com/maps/api/directions/json?origin=Disneyland&destination=Universal+Studios+Hollywood&mode=walking&key=AIzaSyAX_31pghGvv0axcnsP_OR7filS4 - NuJN4

//Documentation on making API calls with Directions
//https://developers.google.com/maps/documentation/directions/get-directions