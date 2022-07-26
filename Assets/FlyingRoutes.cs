using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class FlyingRoutes : MonoBehaviour
{
    public TextAsset flightRoutesJson;
    public TextAsset cleanFlightDataJson;
    string path = "Assets/Resources/cleanFlightData.json";
    Dictionary<string, Airport> airportMap = new Dictionary<string, Airport>();
    private List<Flight> flights = new List<Flight>();
    CleanFlightData flightData;

    public Airplane airplanePrefab;

    //how many airplanes get spawned per second
    public float airPlaneRate = 1;
    public float counter;
    public float speed = .1f;
    public float height = .5f;

    // Start is called before the first frame update
    void Start()
    {
        readFlightData();
        startPlane();
    }

    public void readFlightData()
    {
        if (cleanFlightDataJson != null)
        {
            flightData = readCleanFile();
        }
        else
        {
            flightData = readOriginalFile();
        }
    }

    private CleanFlightData readCleanFile()
    {
        return JsonUtility.FromJson<CleanFlightData>(cleanFlightDataJson.text);
    }

    /**
     * this function reads and cleans up the original flight-data-file from https://raw.githubusercontent.com/Jonty/airline-route-data/main/airline_routes.json
     * Only use if there is no clean file present
     */
    private CleanFlightData readOriginalFile()
    {
        //needed for float-parsing https://www.codeproject.com/articles/3940/parsing-floating-point-strings-with-specified-deci
        System.Globalization.NumberFormatInfo ni = null;
        System.Globalization.CultureInfo ci = 
            System.Globalization.CultureInfo.InstalledUICulture;
        ni = (System.Globalization.NumberFormatInfo)
            ci.NumberFormat.Clone();
        ni.NumberDecimalSeparator = ".";
        
        string jsonText = flightRoutesJson.text;
        AirportData airportData = JsonConvert.DeserializeObject<AirportData>(jsonText);

        Airport a = JsonConvert.DeserializeObject<Airport>(airportData.airports[0].ToString());
        for (int i = 0; i < airportData.airports.Length; i++)
        {
            try
            {
                Airport airport = JsonConvert.DeserializeObject<Airport>(airportData.airports[i].ToString());
                airportMap.Add(airport.iata, airport);

                for (int j = 0; j < airport.routes.Length; j++)
                {
                    Route route = JsonConvert.DeserializeObject<Route>(airport.routes[j].ToString());
                    flights.Add(new Flight(airport.iata, route.iata, route.minutes, route.km));
                }
            }
            catch (Exception e)
            {
                //Debug.Log("airport with no routes detected");
            }
        }

        for (int i = 0; i < flights.Count; i++)
        {
            try
            {
                Airport start = airportMap[flights[i].startCode];
                Airport destination = airportMap[flights[i].destinationCode];
                flights[i].startName = start.name;
                flights[i].destinationName = destination.name;
                flights[i].startCountry = start.country;
                flights[i].destinationCountry = destination.country;
                //additional parse arguments to accept decimal point
                flights[i].startLocation = new Vector2(float.Parse(start.latitude, ni), float.Parse(start.longitude,ni));
                flights[i].destinationLocation = new Vector2(float.Parse(destination.latitude,ni), float.Parse(destination.longitude,ni));
            }
            catch (Exception e)
            {
                //delete flight with error from List
                flights.RemoveAt(i);
            }
        }

        CleanFlightData cleanFlightData = new CleanFlightData();
        cleanFlightData.flights = flights.ToArray();
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(JsonUtility.ToJson(cleanFlightData));
        writer.Close();
        return cleanFlightData;
    }

    private void startPlane()
    {
        int flightNumber = UnityEngine.Random.Range(0, flightData.flights.Length - 1);
        Flight flight = flightData.flights[flightNumber];
        Airplane airplane = Instantiate(airplanePrefab, Vector3.zero, Quaternion.identity, this.transform);
        Vector3 start = transform.TransformPoint(getPointOnSphere(flight.startLocation.x, flight.startLocation.y));
        Vector3 destination = transform.TransformPoint(getPointOnSphere(flight.destinationLocation.x, flight.destinationLocation.y));
        //make sure no flight with the same start and destination gets called
        if (flight.startLocation ==  flight.destinationLocation|| flight.startName.Equals("")||flight.destinationName.Equals(""))
        {
            return;
        }
        airplane.Init(start, destination, height, speed);
        Debug.Log("started airplane from: " + flight.startName + " " + flight.startLocation + " to " +
                  flight.destinationName + " " + flight.destinationLocation);
    }

    public void Update()
    {
        counter += Time.deltaTime;
        if (counter >= 1 / airPlaneRate)
        {
            counter = 0;
            startPlane();
        }
    }

    //based on sebastian lagues project https://www.youtube.com/watch?v=sLqXFF8mlEU&t=820s Minute: 3
    private Vector3 getPointOnSphere(float lat, float lon)
    {
        //use radians
        lat *= Mathf.PI / 180;
        lon *= Mathf.PI / 180;
        float y = Mathf.Sin(lat);
        float r = Mathf.Cos(lat);
        float x = Mathf.Sin(lon) * r;
        float z = -Mathf.Cos(lon) * r;
        return new Vector3(x, y, z).normalized;
    }
}