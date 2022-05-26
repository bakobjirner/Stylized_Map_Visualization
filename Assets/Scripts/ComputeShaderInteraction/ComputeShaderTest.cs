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
        RenderTexture renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        FeatureCollection featureCollection = JsonReader.readGeoJson(geoJson);
        for (int i = 0; i < featureCollection.features.Length; i++)
        {
            for (int j = 0; j < featureCollection.features[i].polygons.Length; j++)
            {
                //the highest and lowest points of the polygon
                float xMin = resolution;
                float xMax = 0;
                float yMin = resolution;
                float yMax = 0;


                List<Vector2> coordinates = new List<Vector2>();
                for (int k = 0; k < featureCollection.features[i].polygons[j].coordinates.Length; k++)
                {
                    Vector2 p = new Vector2();
                    p.x = (float)featureCollection.features[i].polygons[j].coordinates[k][0];
                    p.y = (float)featureCollection.features[i].polygons[j].coordinates[k][1];
                    coordinates.Add(p);

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

                renderTexture = addPolygonToTexture(coordinates.ToArray(), renderTexture, xMin, xMax, yMin, yMax);
            }
        }
        return renderTexture;
    }



    private RenderTexture addPolygonToTexture(Vector2[] polygonData, RenderTexture texture, float xMin, float xMax, float yMin, float yMax)
    {
        Vector4 bounds = new Vector4(xMin, xMax, yMin, yMax);
        ComputeBuffer computeBuffer = new ComputeBuffer(polygonData.Length, sizeof(float) * 2);
        computeBuffer.SetData(polygonData);
        computeShader.SetTexture(0, "Result", texture);
        computeShader.SetFloat("Resolution", resolution);
        computeShader.SetFloat("Thickness", lineThickness);
        computeShader.SetVector("Bounds", bounds);
        computeShader.SetFloat("NumberOfPoints", polygonData.Length);
        computeShader.SetBuffer(0, "points", computeBuffer);
        computeShader.Dispatch(0, texture.width / 8, texture.height / 8, 1);
        computeBuffer.Release();
        return texture;
    }
}


