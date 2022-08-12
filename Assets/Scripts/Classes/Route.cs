using System;
using System.Collections.Generic;

/**
 * class to hold route information deserialized from json
 */
[Serializable]
public class Route
{
    public List <string> carriers { get; set; }
    public string iata { get; set; }
    public int km { get; set; }
    public int minutes { get; set; }
}
