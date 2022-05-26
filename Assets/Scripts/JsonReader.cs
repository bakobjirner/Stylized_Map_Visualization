using Newtonsoft.Json;
using UnityEngine;

public class JsonReader: MonoBehaviour
{
    /**
     * read GeoJSon from file
     * */
    public static FeatureCollection readGeoJson(TextAsset countryJson)
    {
        //deserialize GeoJSON object
        FeatureCollection countryData = JsonConvert.DeserializeObject<FeatureCollection>(countryJson.text);
        for(int i = 0; i< countryData.features.Length; i++)
        {
            //since the geometry might be either Polygon or Multipolygon, this has to be done manually. TODO: look for libary
            if(countryData.features[i].geometry.type == "Polygon")
            {
                double[][][] coordinates = JsonConvert.DeserializeObject<double[][][]>(countryData.features[i].geometry.coordinates.ToString());
                countryData.features[i].polygons = new Polygon[1];
                countryData.features[i].polygons[0] = new Polygon();
                countryData.features[i].polygons[0].coordinates = coordinates[0];
            }
            else if (countryData.features[i].geometry.type == "MultiPolygon")
            {
                double[][][][] coordinates = JsonConvert.DeserializeObject<double[][][][]>(countryData.features[i].geometry.coordinates.ToString());
                countryData.features[i].polygons = new Polygon[coordinates.Length];
                for(int j = 0; j< coordinates.Length; j++)
                {
                    countryData.features[i].polygons[j] = new Polygon();
                    countryData.features[i].polygons[j].coordinates = coordinates[j][0];
                }
            }
            else
            {
                Debug.Log("unexpected geometry type");
            }
        }

        return countryData;
    }
}
