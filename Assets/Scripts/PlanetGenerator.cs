using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{
    [Range(1,6)]
    public int details = 5;


    public void OnValidate()
    {
        CreateSphere();
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

        Vector3[] vertices = data.vertices;
        //list to array
        mesh.vertices = vertices;
        mesh.triangles = data.faces;
        mesh.normals = data.normals;
        mesh.uv = data.uvs;
        

        //recalculate mesh
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();

    }

}