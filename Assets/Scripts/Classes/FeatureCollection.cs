
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeatureCollection 
{
    public string type;
    public string name;
    public object crs;
    public Feature[] features;
    public List<List<List<Vector2>>> coordinates = new List<List<List<Vector2>>>();
    public List<List<Vector4>> bounds = new List<List<Vector4>>();
    public List<Vector4> colors = new List<Vector4>();
}
