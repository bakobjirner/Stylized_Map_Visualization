using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [Range(1,7)]
    public int details = 5;

    public bool sphere = true;

    public void Start()
    {
        Debug.Log("start mesh creation: " + Time.realtimeSinceStartup);
        CreateSphere();
        Debug.Log("end mesh creation: " + Time.realtimeSinceStartup);
        drawData();
    }

    public void CreateSphere()
    {

        
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
             data = SphereGenerator.GetSphere(details);
            //list to array
            
        }
        else
        {
            data = PlaneGenerator.getPlane(details*500, .45f,.55f,.75f,.85f);
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
        ComputeShaderTest computeShaderTest =  this.GetComponent<ComputeShaderTest>();

        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_dataTexture", computeShaderTest.generateByPolygons());

        int featureIndex = CheckInPolygon.GetFeatureByCoordiantes(new Vector2(9, 49));
        if (featureIndex == -1)
        {
            Debug.Log("no feature");
        }
        else
        {
            Debug.Log(CheckInPolygon.featureCollection.features[featureIndex].properties.NAME);
        }
    }

}