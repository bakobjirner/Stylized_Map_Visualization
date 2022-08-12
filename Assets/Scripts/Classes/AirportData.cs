using System;
using System.Collections.Generic;

/**
 * class to hold a list of airports deserialized from json
 */
[Serializable]
public class AirportData
{
    public object[] airports { get; set; }
}
