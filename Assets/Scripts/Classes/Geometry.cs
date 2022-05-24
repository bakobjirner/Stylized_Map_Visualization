

using UnityEngine;

[System.Serializable]
public class Geometry
{
    public string type;
    public object coordinates;
}

[System.Serializable]
public class Polygon
{
    public double[][] coordinates;
}