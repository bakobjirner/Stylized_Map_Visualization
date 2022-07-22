using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class FlyingRoutes : MonoBehaviour
{
    public TextAsset flightRoutesJson;
    Dictionary<string, Airport> airportMap = new Dictionary<string, Airport>();
    private List<Flight> flights = new List<Flight>();

    // Start is called before the first frame update
    void Start()
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
                flights[i].start = airportMap[flights[i].airport1];
                flights[i].destination = airportMap[flights[i].airport2];
            }
            catch (Exception e)
            {
                //delete flight with error from List
                flights.RemoveAt(i);
            }
        }

        Debug.Log(JsonConvert.SerializeObject(flights));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
