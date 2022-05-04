using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Mesh mesh;
    MeshFilter meshFilter;
    public int resolutionX = 100;
    //resolutionZ will be set based on ratio of inputTexture
    private int resolutionZ;

    private void setValues()
    {


        //fixed ratio for now
        resolutionZ = resolutionX/2;
        meshFilter = this.GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.mesh = mesh;
        CreatePlane();
        CreateTriangles();
        UpdateMesh();
    }




    private void OnValidate()
    {
        setValues();
    }


    //create a basic plane
    private void CreatePlane()
    {

        vertices = new Vector3[(resolutionX + 1) * (resolutionZ + 1)];
        uvs = new Vector2[vertices.Length];
        //set verticePositions
        int index = 0;
        for (int x = 0; x <= resolutionX; x++)
        {
            for (int z = 0; z <= resolutionZ; z++)
        {
            
                vertices[index] = new Vector3(x, 0, z);
                uvs[index] = new Vector2(x * 1.0f / resolutionX, z * 1.0f / resolutionZ);
                index++;
            }
        }
    }

    private void CreateTriangles()
    {


        //create one quad cosisting out of two triangles per loop
        int quad = 0;
        int tris = 0;
        triangles = new int[resolutionX * resolutionZ * 6];
        for (int x = 0; x < resolutionX; x++)
        {
            for (int z = 0; z < resolutionZ; z++)
        {
            
                triangles[0 + tris] = quad + 0;
                triangles[2 + tris] = quad + resolutionZ + 1;
                triangles[1 + tris] = quad + 1;
                triangles[5 + tris] = quad + resolutionZ + 1;
                triangles[4 + tris] = quad + resolutionZ + 2;
                triangles[3 + tris] = quad + 1;
                quad++;
                tris += 6;
            }
            //prevent creation of triangles between rows
            quad++;
       }

    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}
