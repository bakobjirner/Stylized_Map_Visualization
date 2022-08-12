using System;
using System.Collections.Generic;

/**
 * class to hold airport informaition deserialized from json
 */
[Serializable]
public class Airport 
{
    public string city_name { get; set; }
    public string continent { get; set; }
    public string country { get; set; }
    public string country_code { get; set; }
    public string display_name { get; set; }
    public int elevation { get; set; }
    public string iata { get; set; }
    public string icao { get; set; }
    public string latitude { get; set; }
    public string longitude { get; set; }
    public string name { get; set; }
    public object[] routes { get; set; }
    public string timezone { get; set; }
}
