using System.Collections.Generic;

[System.Serializable]
public class Geometry
{
    public string type;
    public Polygon[] coordinates;
}


[System.Serializable]
public class Polygon
{
    public DataPoint[] points;
}

[System.Serializable]
public class DataPoint
{
    public double[] values;
}