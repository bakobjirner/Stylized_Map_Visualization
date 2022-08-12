using System.Collections;
using System.Collections.Generic;
using BAMCIS.GeoJSON;
using UnityEngine;

/**
 * class to hold geo-data
 */
public class GeoData
{
    public  FeatureCollection featureCollection;
    public List<List<List<Vector2>>> coordinates = new List<List<List<Vector2>>>();
    public List<List<Vector4>> bounds = new List<List<Vector4>>();
    public List<Vector4> colors = new List<Vector4>();
}
