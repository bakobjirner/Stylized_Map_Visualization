using BAMCIS.GeoJSON;
using Newtonsoft.Json;
using UnityEngine;

public class JsonReader: MonoBehaviour
{
    /**
     * read GeoJSon from file
     * */
    public static GeoData readGeoJson(TextAsset countryJson)
    {
        Debug.Log("start reading geoJson: " + Time.realtimeSinceStartup);
        //deserialize GeoJSON object
        FeatureCollection countryData = FeatureCollection.FromJson(countryJson.text);
        Debug.Log("end reading geoJson: " + Time.realtimeSinceStartup);

        GeoData geoData = new GeoData();
        geoData.featureCollection = countryData;
        
        return geoData;
    }
}
