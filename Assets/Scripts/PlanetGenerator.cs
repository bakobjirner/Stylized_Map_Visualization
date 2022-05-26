using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [Range(1,7)]
    public int details = 5;


    public void Start()
    {
        CreateSphere();
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

        SphereMeshData data = SphereGenerator.GetSphere(details);
        //list to array
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
        this.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_dataTexture", computeShaderTest.GenerateTexture());
    }

}