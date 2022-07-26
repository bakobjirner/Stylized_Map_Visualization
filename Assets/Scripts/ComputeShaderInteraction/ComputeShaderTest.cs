using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BAMCIS.GeoJSON;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader computeShader;
    public TextAsset geoJson;
    public TextAsset cityOutlineJson;
    public int resolution = 16;
    private float maxGDP = 21000000;
    private float maxPop = 1400000000;
    public Texture cachedCityTexture;
    [Range(0, 10)] public float lineThickness = 1;

    public RenderTexture[] generateByPolygons()
    {
        RenderTexture[] textures;
        Debug.Log("start Shader setup: " + Time.realtimeSinceStartup);
        RenderTexture colorTexture = new RenderTexture(resolution, resolution, 0);
        RenderTexture oceanTexture = new RenderTexture(resolution, resolution, 0);
        RenderTexture gdpTexture = new RenderTexture(resolution, resolution, 0);
        RenderTexture gdppcTexture = new RenderTexture(resolution, resolution, 0);
        RenderTexture populationTexture = new RenderTexture(resolution, resolution, 0);
        oceanTexture.enableRandomWrite = true;
        gdpTexture.enableRandomWrite = true;
        gdppcTexture.enableRandomWrite = true;
        populationTexture.enableRandomWrite = true;
        colorTexture.enableRandomWrite = true;
        GeoData geoData = JsonReader.readGeoJson(geoJson);
        List<Vector4> colors = new List<Vector4>();
        List<List<Vector4>> bounds = new List<List<Vector4>>();
        List<List<List<Vector2>>> coordinates = new List<List<List<Vector2>>>();
        Debug.Log("start calculating minMax: " + Time.realtimeSinceStartup);
        List<Feature> features = geoData.featureCollection.Features.ToList();
        List<List<Polygon>> allPolygons = new List<List<Polygon>>();
        for (int i = 0; i < features.Count; i++)
        {
            int a = (int)features[i].Properties["MAPCOLOR7"];
            colors.Add(MapColor.color7[a - 1]);

            List<Polygon> polygons = new List<Polygon>();

            if (features[i].Geometry.Type.Equals(GeoJsonType.MultiPolygon))
            {
                polygons = ((MultiPolygon)features[i].Geometry).Coordinates.ToList();
            }
            else if (features[i].Geometry.Type.Equals(GeoJsonType.Polygon))
            {
                polygons.Add((Polygon)features[i].Geometry);
            }

            allPolygons.Add(polygons);
        }

        //featureCollection.bounds = bounds;
        //featureCollection.colors = colors;
        //featureCollection.coordinates = coordinates;
        Debug.Log("end Shader setup: " + Time.realtimeSinceStartup);

        for (int i = 0; i < allPolygons.Count; i++)
        {
            bounds.Add(new List<Vector4>());
            coordinates.Add(new List<List<Vector2>>());
            for (int j = 0; j < allPolygons[i].Count; j++)
            {
                coordinates[i].Add(new List<Vector2>());
                Polygon p = allPolygons[i][j];
                Position[] polygonData = p.Coordinates.First().Coordinates.ToArray();

                // calc bounds
                float xMin = resolution;
                float xMax = 0;
                float yMin = resolution;
                float yMax = 0;

                Vector2[] pData = new Vector2[polygonData.Length];


                for (int k = 0; k < polygonData.Length; k++)
                {
                    Vector2 vector2 = new Vector2((float)polygonData[k].Longitude, (float)polygonData[k].Latitude);
                    coordinates[i][j].Add(vector2);
                    pData[k] = vector2;
                    //set border points
                    if (vector2.x > xMax)
                    {
                        xMax = vector2.x;
                    }
                    else if (vector2.x < xMin)
                    {
                        xMin = vector2.x;
                    }

                    if (vector2.y > yMax)
                    {
                        yMax = vector2.y;
                    }
                    else if (vector2.y < yMin)
                    {
                        yMin = vector2.y;
                    }
                }

                Vector4 colorGDP;
                Vector4 colorGDPPC;
                Vector4 colorPop;

                float gdp = (float)features[i].Properties["GDP_MD"] / maxGDP;
                colorGDP = new Vector4(gdp, gdp, gdp, 1);

                float pop = (float)features[i].Properties["POP_EST"] / maxPop;
                colorPop = new Vector4(pop, pop, pop, 1);

                float gdppc = (gdp / pop) / 10;
                colorGDPPC = new Vector4(gdppc, gdppc, gdppc, 1);

                Vector4 myBounds = new Vector4(xMin, xMax, yMin, yMax);
                bounds[i].Add(myBounds);
                colorTexture = addPolygonToTexture(pData, colorTexture, myBounds, colors[i],resolution);
                oceanTexture = addPolygonToTexture(pData, oceanTexture, myBounds, Vector4.one,resolution);
                gdpTexture = addPolygonToTexture(pData, gdpTexture, myBounds, colorGDP,resolution);
                gdppcTexture = addPolygonToTexture(pData, gdppcTexture, myBounds, colorGDPPC,resolution);
                populationTexture = addPolygonToTexture(pData, populationTexture, myBounds, colorPop,resolution);
            }
        }

        RenderTexture cityTexture;
        if (cachedCityTexture == null)
        {
            cityTexture = getCityOutlineTexture();
        }
        else
        {
            RenderTexture renderTexture = new RenderTexture(cachedCityTexture.width, cachedCityTexture.height, 0);
            RenderTexture.active = renderTexture;
            Graphics.Blit(cachedCityTexture, renderTexture);
            cityTexture = renderTexture;
        }
        Debug.Log("end Shader calculations: " + Time.realtimeSinceStartup);
        geoData.bounds = bounds;
        geoData.coordinates = coordinates;
        CheckInPolygon.geoData = geoData;
        textures = new[] { colorTexture, oceanTexture, gdpTexture, gdppcTexture, populationTexture, cityTexture };
        return textures;
    }

    private RenderTexture addPolygonToTexture(Vector2[] polygonData, RenderTexture texture, Vector4 bounds,
        Vector4 color, float resolution)
    {
        ComputeBuffer computeBuffer = new ComputeBuffer(polygonData.Length, sizeof(float) * 2);
        computeBuffer.SetData(polygonData);
        computeShader.SetTexture(0, "Result", texture);
        computeShader.SetFloat("Resolution", resolution);
        computeShader.SetFloat("Thickness", lineThickness);
        computeShader.SetVector("Bounds", bounds);
        computeShader.SetVector("Color", color);
        computeShader.SetFloat("NumberOfPoints", polygonData.Length);
        computeShader.SetBuffer(0, "points", computeBuffer);
        computeShader.Dispatch(0, texture.width / 8, texture.height / 8, 1);
        computeBuffer.Release();
        return texture;
    }

    public RenderTexture getFeatureMask(int featureIndex)
    {
        RenderTexture renderTexture = new RenderTexture(resolution, resolution, 0);
        renderTexture.enableRandomWrite = true;

        List<Polygon> polygons = new List<Polygon>();
        Feature feature = CheckInPolygon.geoData.featureCollection.Features.ToList()[featureIndex];

        if (feature.Geometry.Type.Equals(GeoJsonType.MultiPolygon))
        {
            polygons = ((MultiPolygon)feature.Geometry).Coordinates.ToList();
        }
        else if (feature.Geometry.Type.Equals(GeoJsonType.Polygon))
        {
            polygons.Add((Polygon)feature.Geometry);
        }

        for (int j = 0; j < polygons.Count; j++)
        {
            renderTexture = addPolygonToTexture(CheckInPolygon.geoData.coordinates[featureIndex][j].ToArray(),
                renderTexture, CheckInPolygon.geoData.bounds[featureIndex][j], Color.white,resolution);
        }

        return renderTexture;
    }

    private RenderTexture getCityOutlineTexture()
    {
        int resolution = this.resolution * 2;
        FeatureCollection cityData = FeatureCollection.FromJson(cityOutlineJson.text);
        RenderTexture cityTexture = new RenderTexture(resolution, resolution, 0);
        cityTexture.enableRandomWrite = true;
        List<Feature> features = cityData.Features.ToList();
        for (int i = 0; i < features.Count; i++)
        {
            Polygon p = (Polygon)features[i].Geometry;

            // calc bounds
            float xMin = resolution;
            float xMax = 0;
            float yMin = resolution;
            float yMax = 0;
            Position[] polygonData = p.Coordinates.First().Coordinates.ToArray();
            Vector2[] pData = new Vector2[polygonData.Length];
            for (int k = 0; k < polygonData.Length; k++)
            {
                Vector2 vector2 = new Vector2((float)polygonData[k].Longitude, (float)polygonData[k].Latitude);
                pData[k] = vector2;
                //set border points
                if (vector2.x > xMax)
                {
                    xMax = vector2.x;
                }
                else if (vector2.x < xMin)
                {
                    xMin = vector2.x;
                }

                if (vector2.y > yMax)
                {
                    yMax = vector2.y;
                }
                else if (vector2.y < yMin)
                {
                    yMin = vector2.y;
                }
            }

            Vector4 myBounds = new Vector4(xMin, xMax, yMin, yMax);
            cityTexture = addPolygonToTexture(pData, cityTexture, myBounds, Vector4.one,resolution);
        }
        // storage of texture based on: https://answers.unity.com/questions/37134/is-it-possible-to-save-rendertextures-into-png-fil.html
        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
        RenderTexture.active = cityTexture;
        tex.ReadPixels(new Rect(0, 0, cityTexture.width, cityTexture.height), 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets/Resources/cityTexture.png", bytes);

        return cityTexture;
    }
}