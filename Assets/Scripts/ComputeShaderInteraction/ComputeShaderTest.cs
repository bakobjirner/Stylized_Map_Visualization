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


    public RenderTexture GenerateTexture()
    {
        Vector2[] data = getDataFromJson();
        ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, sizeof(float)*2);
        computeBuffer.SetData(data);

        RenderTexture renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("Resolution", resolution);
        computeShader.SetFloat("Thickness", lineThickness);
        computeShader.SetFloat("NumberOfPoints", data.Length);
        computeShader.SetBuffer(0,"points", computeBuffer);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
        computeBuffer.Release();
        return renderTexture;
    }

    private Vector2[] getDataFromJson()
    {
        FeatureCollection featureCollection = JsonReader.readGeoJson(geoJson);
        List<Vector2> points = new List<Vector2>();
        int i = 1;
            for (int j = 0; j < featureCollection.features[i].polygons.Length; j++)
            {
                for (int k = 0; k < featureCollection.features[i].polygons[j].coordinates.Length; k++)
                {
                    Vector2 p = new Vector2();
                    p.x = (float)featureCollection.features[i].polygons[j].coordinates[k][0];
                    p.y = (float)featureCollection.features[i].polygons[j].coordinates[k][1];
                    points.Add(p);
                }
            }
        
        return points.ToArray(); 
    }
}


