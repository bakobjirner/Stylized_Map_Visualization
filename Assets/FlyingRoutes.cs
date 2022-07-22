using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class FlyingRoutes : MonoBehaviour
{
    public TextAsset flightRoutesJson;
    public TextAsset cleanFlightDataJson;
    string path = "Assets/Resources/cleanFlightData.json";
    Dictionary<string, Airport> airportMap = new Dictionary<string, Airport>();
    private List<Flight> flights = new List<Flight>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void readFlightData()
    {
        CleanFlightData flightData;
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
        string jsonText = flightRoutesJson.text;
        AirportData airportData = JsonConvert.DeserializeObject<AirportData>(jsonText);
        
        Airport a = JsonConvert.DeserializeObject<Airport>(airportData.airports[0].ToString());
        for (int i = 0; i < airportData.airports.Length; i++)
        {
            try
            {
                Airport airport = JsonConvert.DeserializeObject<Airport>(airportData.airports[i].ToString());
                airportMap.Add(airport.iata,airport);
                
                for (int j = 0; j < airport.routes.Length; j++)
                {
                    Route route = JsonConvert.DeserializeObject<Route>(airport.routes[j].ToString());
                    flights.Add(new Flight(airport.iata, route.iata,route.minutes,route.km));
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
                flights[i].startLocation = new Vector2(float.Parse(start.latitude),float.Parse(start.longitude));
                flights[i].destinationLocation = new Vector2(float.Parse(destination.latitude),float.Parse(destination.longitude));
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
}
