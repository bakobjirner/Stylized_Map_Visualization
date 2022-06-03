using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader computeShader;
    public TextAsset geoJson;
    public int resolution = 16;
    [Range(0,10)]
    public float lineThickness = 1;

    public RenderTexture generateByPolygons()
    {
        Debug.Log("start Shader setup: " + Time.realtimeSinceStartup);
        RenderTexture renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        FeatureCollection featureCollection = JsonReader.readGeoJson(geoJson);
        List<List<List<Vector2>>> coordinates = new List<List<List<Vector2>>>();
        List<List<Vector4>> bounds = new List<List<Vector4>>();
        List<Vector4> colors = new List<Vector4>();
        for (int i = 0; i < featureCollection.features.Length; i++)
        {
            coordinates.Add(new List<List<Vector2>>());
            bounds.Add(new List<Vector4>());
            for (int j = 0; j < featureCollection.features[i].polygons.Length; j++)
            {
                coordinates[i].Add(new List<Vector2>());
                //the highest and lowest points of the polygon
                float xMin = resolution;
                float xMax = 0;
                float yMin = resolution;
                float yMax = 0;

                for (int k = 0; k < featureCollection.features[i].polygons[j].coordinates.Length; k++)
                {
                    Vector2 p = new Vector2();
                    p.x = (float)featureCollection.features[i].polygons[j].coordinates[k][0];
                    p.y = (float)featureCollection.features[i].polygons[j].coordinates[k][1];
                    coordinates[i][j].Add(p);

                    //set border points
                    if (p.x > xMax)
                    {
                        xMax = p.x;
                    }else if(p.x < xMin)
                    {
                        xMin = p.x;
                    }
                    if (p.y > yMax)
                    {
                        yMax = p.y;
                    }
                    else if (p.y < yMin)
                    {
                        yMin = p.y;
                    }
                }
                bounds[i].Add(new Vector4(xMin, xMax, yMin, yMax));
            }
            colors.Add(MapColor.color7[featureCollection.features[i].properties.MAPCOLOR7-1]);
        }

        featureCollection.bounds = bounds;
        featureCollection.colors = colors;
        featureCollection.coordinates = coordinates;
        Debug.Log("end Shader setup: " + Time.realtimeSinceStartup);
        
        for (int i = 0; i < featureCollection.features.Length; i++)
        {
            for (int j = 0; j < featureCollection.features[i].polygons.Length; j++)
            {
                renderTexture = addPolygonToTexture(coordinates[i][j].ToArray(), renderTexture, bounds[i][j],colors[i]);
            }
        }
        Debug.Log("end Shader calculations: " + Time.realtimeSinceStartup);
        CheckInPolygon.featureCollection = featureCollection;
        return renderTexture;
    }



    private RenderTexture addPolygonToTexture(Vector2[] polygonData, RenderTexture texture, Vector4 bounds, Vector4 color)
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
}


