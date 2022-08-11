using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class PlanetGenerator : MonoBehaviour
{
    [Range(1, 8)] public int details = 5;

    public bool sphere = true;
    public RenderTexture featureMask;
    private Vector4 featureBounds = new Vector4(0, 0, .5f, .5f);
    public GameObject ocean;
    public Mesh[] sphereMeshes = new Mesh[8];
    public Mesh[] planeMeshes = new Mesh[8];
    public GameObject sun;
    public GameObject sunDetail;
    public float heightMultiplier = 0.05f;
    public Mesh oceanSphereMesh;
    public Mesh oceanPlaneMesh;
    public UIController ui;
    public GameObject airplaneHolder;
    public int mode = 0;


    public void Start()
    {
       init();
    }

    private void init()
    {
        if (PlayerPrefs.GetInt("resolution") == 0)
        {
            PlayerPrefs.SetInt("resolution",7);
            PlayerPrefs.SetInt("planeRate",5);
            PlayerPrefs.SetFloat("height", .05f);
        }
        Debug.Log("start Planet Generation: " + Time.realtimeSinceStartup);
        heightMultiplier = PlayerPrefs.GetFloat("height");
        details = PlayerPrefs.GetInt("resolution");
        this.GetComponent<FlyingRoutes>().airPlaneRate = PlayerPrefs.GetInt("planeRate");
       
        
        ShowGlobalView();
        drawData();
    }

    public Mesh CreateMesh()
    {
        Debug.Log("start mesh setup: " + Time.realtimeSinceStartup);
        //get mesh of Gameobject
        Mesh mesh = new Mesh();
        //set indexformat to allow for meshes with more than 65536 Vertices
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        MeshData data;
        if (sphere)
        {
            data = SphereGenerator.GetSphere(details);
            //list to array
        }
        else
        {
            data = PlaneGenerator.getPlane(details, featureBounds);
        }

        mesh.vertices = data.getVerticeArray();
        mesh.triangles = data.getTriangleArray();
        mesh.normals = data.getNormalArray();
        mesh.uv = data.getUVArray();

        //recalculate mesh
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        return mesh;
    }


    private void drawData()
    {
        ComputeShaderTest computeShaderTest = this.GetComponent<ComputeShaderTest>();
        RenderTexture[] textures = computeShaderTest.generateByPolygons();

        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_dataTexture", textures[0]);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_oceanTexture", textures[1]);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_gdpTexture", textures[2]);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_gdppcTexture", textures[3]);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_populationTexture", textures[4]);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_cityTexture", textures[5]);
    }

    public void ShowGlobalView()
    {
        sunDetail.SetActive(false);
        sun.SetActive(true);
        airplaneHolder.SetActive(true);
        Mesh mesh;
        ui.SetCountryName("");
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spherical", 1);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_heightMultiplier", heightMultiplier);
        sphere = true;
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_useFeatureMask", 0);
        if (sphereMeshes[details - 1] == null)
        {
            Debug.Log("no mesh set, new one will be created");
            mesh = CreateMesh();
        }
        else
        {
            Debug.Log("load precalculated mesh-data");
            mesh = sphereMeshes[details - 1];
        }

        this.GetComponent<MeshFilter>().sharedMesh = mesh;
        ocean.GetComponent<MeshFilter>().sharedMesh = oceanSphereMesh;
        ocean.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showWaves", 0);
    }

    public void ShowDetailView(int featureIndex)
    {
        //set to standard mode
        setMode(0);
        //set correct light
        sun.SetActive(false);
        sunDetail.SetActive(true);
        //disable airplanes
        airplaneHolder.SetActive(false);
        ui.SetCountryName(CheckInPolygon.geoData.featureCollection.Features.ToList()[featureIndex].Properties["NAME"]);
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

        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_spherical", 0);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showData", 0);
        sphere = false;
        featureMask = this.GetComponent<ComputeShaderTest>().getFeatureMask(featureIndex);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_featureMask", featureMask);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_useFeatureMask", 1);
        this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_heightMultiplier", heightMultiplier * 100/(featureBounds.y-featureBounds.x));
        Mesh mesh = CreateMesh();
        this.GetComponent<MeshFilter>().sharedMesh = mesh;
        ocean.GetComponent<MeshFilter>().sharedMesh = oceanPlaneMesh;
        ocean.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showWaves", 1);
    }

    /*
     * Sets the display-mode
     * 0 is standard
     * 1 is night
     * 2 is gdp
     * 3 is population
     */
    public void setMode(int mode)
    {
        this.mode = mode;
        switch (mode)
        {
            case 0:
            {
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDP", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDPPC", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showPopulation", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showCityEmission", 0);
                sun.SetActive(true);
                airplaneHolder.SetActive(true);
                break;
            }
            case 1:
            {
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDP", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDPPC", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showPopulation", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showCityEmission", 1);
                sun.SetActive(false);
                airplaneHolder.SetActive(false);
                break;
            }
            case 2:
            {
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDP", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDPPC", 1);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showPopulation", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showCityEmission", 0);
                sun.SetActive(true);
                airplaneHolder.SetActive(false);
                break;
            }
            case 3:
            {
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDP", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDPPC", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showPopulation", 1);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showCityEmission", 0);
                airplaneHolder.SetActive(false);
                sun.SetActive(true);
                break;
            }
            default:
            {
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDP", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showGDPPC", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showPopulation", 0);
                this.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_showCityEmission", 0);
                airplaneHolder.SetActive(true);
                sun.SetActive(true);
                break;
            }
        }
    }
}