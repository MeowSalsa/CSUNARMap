// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Collections.Generic;

[System.Serializable]
public class Bounds
{
    public Northeast northeast;
    public Southwest southwest;
}

[System.Serializable]
public class Distance
{
    public string text; 
    public int value;
}

[System.Serializable]
public class Duration
{
    public string text;
    public int value;
}

[System.Serializable]
public class EndLocation
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class Leg
{
    public Distance distance;
    public Duration duration;
    public string end_address;
    public EndLocation end_location;
    public string start_address;
    public StartLocation start_location;
    public List<Step> steps;
}

[System.Serializable]
public class Northeast
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class OverviewPolyline
{
    public string points;
}

[System.Serializable]
public class Polyline
{
    public string points;
}

[System.Serializable]
public class Direction
{
    public List<Route> routes;
    public string status;

    public override string ToString()
    {
        return status;
    }
}

[System.Serializable]
public class Route
{
    public Bounds bounds;
    public string copyrights;
    public List<Leg> legs;
    public OverviewPolyline overview_polyline;
    public string summary;
    public List<string> warnings;
    public List<object> waypoint_order;
}

[System.Serializable]
public class Southwest
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class StartLocation
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class Step
{
    public Distance distance;
    public Duration duration;
    public EndLocation end_location;
    public string html_instructions;
    public Polyline polyline;
    public StartLocation start_location;
    public string travel_mode;
    public string maneuver;
}

