using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BAMCIS.GeoJSON;
using UnityEngine;

public class CheckInPolygon
{

    public static GeoData geoData;
    
    private static bool PointInPolygon(Vector2 p, List<Vector2> polygon, Vector4 bounds)
    {
        //check if in country bounds
        if (p.x < bounds.x || p.x > bounds.y || p.y < bounds.z || p.y > bounds.w)
        {
            return false;
        }
        
        //A point is in a polygon if a line from the point to infinity crosses the polygon an odd number of times
        bool odd = false; // Starting with the edge from the last to the first node
        //For each edge (In this case for each point of the polygon and the previous one)
        int j = polygon.Count - 1;
        for (int i = 0; i < polygon.Count; i++) { 
            //If a line from the point into infinity crosses this edge
            // One point needs to be above, one below our y coordinate
            if (((polygon[i].y > p.y) != (polygon[j].y > p.y))
                // ...and the edge doesn't cross our Y corrdinate before our x coordinate (but between our x coordinate and infinity)
                && (p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)) {
                // Invert odd
                odd = !odd;
            }
            j = i;
        }
        //If the number of crossings was odd, the point is in the polygon
        return odd;
        
    }

    public static int GetFeatureByCoordiantes(Vector2 p)
    {
        for (int i = 0; i < geoData.coordinates.Count; i++)
        {
            for (int j = 0; j < geoData.coordinates[i].Count; j++)
            {
                if (PointInPolygon(p, geoData.coordinates[i][j], geoData.bounds[i][j]))
                {
                    return i;
                }
            }
        }
        Debug.Log("coordinates out of bounds");
        return -1;
    }
    
    
    
   
}