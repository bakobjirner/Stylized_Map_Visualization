using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public TextAsset geoJson;
    public int resolution = 16;
    [Range(0,10)]
    public float lineThickness = 1;


    public void Start()
    {
        GenerateTexture();
        
    }

    private void GenerateTexture()
    {
        Point[] data = getDataFromJson();
        ComputeBuffer computeBuffer = new ComputeBuffer(data.Length, sizeof(float)*2);
        computeBuffer.SetData(data);

        renderTexture = new RenderTexture(resolution, resolution, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("Resolution", resolution);
        computeShader.SetFloat("Thickness", lineThickness);
        computeShader.SetFloat("NumberOfPoints", data.Length);
        computeShader.SetBuffer(0,"points", computeBuffer);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
        this.GetComponent<RawImage>().material.mainTexture = renderTexture;
        computeBuffer.Release();
    }
    private Point[] ReadData()
    {
        Point[] points;
        string path = Application.dataPath + "/Resources/lowResGermany.txt";
        string fileData = System.IO.File.ReadAllText(path);
        string[] lines  = fileData.Split("\n"[0]);
        points = new Point[lines.Length];
        for(int i = 0; i< lines.Length; i++)
        {
            string[] lineData  = lines[i].Split(";");
            float.TryParse(lineData[0], out points[i].x);
            float.TryParse(lineData[1], out points[i].y);
        }
        return points;
    }

    private Point[] getDataFromJson()
    {
        FeatureCollection featureCollection = JsonReader.readGeoJson(geoJson);
        List<Point> points = new List<Point>();
        for(int i = 0; i<featureCollection.features.Length; i++)
        {
            for (int j = 0; j < featureCollection.features[i].polygons.Length; j++)
            {
                for (int k = 0; k < featureCollection.features[i].polygons[j].coordinates.Length; k++)
                {
                    Point p = new Point();
                    p.x = (float)featureCollection.features[i].polygons[j].coordinates[k][0];
                    p.y = (float)featureCollection.features[i].polygons[j].coordinates[k][1];
                    points.Add(p);
                }
            }
        }
        return points.ToArray(); 
    }
}

public struct Point
{
    public float x;
    public float y;
}


