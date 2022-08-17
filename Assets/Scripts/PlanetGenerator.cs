using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetGenerator : MonoBehaviour
{
    [Range(1, 7)] public int details = 5;

    public bool sphere = true;
    public RenderTexture featureMask;
    private Vector4 featureBounds = new Vector4(0, 0, .5f, .5f);
    public bool useStorage;
    public GameObject waterPlane;
    public GameObject waterSphere;

    public UIController ui;


    public void Start()
    {
        Debug.Log("start Planet Generation: " + Time.realtimeSinceStartup);
        CreateMesh();
        drawData();
    }

    public void CreateMesh()
    {
        Debug.Log("start mesh setup: " + Time.realtimeSinceStartup);
        //get mesh of Gameobject
        MeshFilter filter = this.GetComponent<MeshFilter>();
        Mesh mesh;
        if (filter.sharedMesh != null)
        {
            mesh = filter.sharedMesh;
        }
        else
        {
            mesh = new Mesh();
            filter.sharedMesh = mesh;
        }

        //clear mesh if not empty
        mesh.Clear();
        //set indexformat to allow for meshes with more than 65536 Vertices
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        MeshData data;

        if (sphere)
        {
            data = SphereGenerator.GetSphere(details, useStorage);
            //list to array
        }
        else
        {
            data = PlaneGenerator.getPlane(details*5, featureBounds);
        }

        mesh.vertices = data.getVerticeArray();
        mesh.triangles = data.getTriangleArray();
        mesh.normals = data.getNormalArray();
        mesh.uv = data.getUVArray();

        //recalculate mesh
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
    }


    private void drawData()
    {
        ComputeShaderTest computeShaderTest = this.GetComponent<ComputeShaderTest>();

        this.GetComponent<MeshRenderer>().sharedMaterial
            .SetTexture("_dataTexture", computeShaderTest.generateByPolygons());
    }

    public void ShowGlobalView()
    {
        ui.SetCountryName("World");
        waterPlane.SetActive(false);
        waterSphere.SetActive(true);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spherical",1);
        sphere = true;
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_useFeatureMask",0);
        CreateMesh();
    }

    public void ShowDetailView(int featureIndex)
    {
        ui.SetCountryName(CheckInPolygon.geoData.featureCollection.Features.ToList()[featureIndex].Properties["name"]);
         waterPlane.SetActive(true);
         waterSphere.SetActive(false);
        featureBounds = CheckInPolygon.geoData.bounds[featureIndex][0];

        //calculate total bounds for features with multiple subfeatures
        for (int i = 0; i < CheckInPolygon.geoData.bounds[featureIndex].Count; i++)
        {
            Vector4 b = CheckInPolygon.geoData.bounds[featureIndex][i];
            if (b.x < featureBounds.x)
            {
                featureBounds.x = b.x;
            }

            if (b.y > featureBounds.y)
            {
                featureBounds.y = b.y;
            }

            if (b.z < featureBounds.z)
            {
                featureBounds.z = b.z;
            }

            if (b.w > featureBounds.w)
            {
                featureBounds.w = b.w;
            }
        }
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spherical",0);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showData",0);
        sphere = false;
        featureMask = this.GetComponent<ComputeShaderTest>().getFeatureMask(featureIndex);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_featureMask",featureMask);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_useFeatureMask",1);
        CreateMesh();
    }
}