using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PlaneGenerator : MonoBehaviour
{
    //create a basic plane
    public static MeshData getPlane(int details, Vector4 bounds)
    {
       
        float xStart = bounds.x / 360 + .5f;
        float xEnd = bounds.y / 360 + .5f;
        float yStart = bounds.z / 180 + .5f;
        float yEnd = bounds.w / 180 + .5f;


        float ratio = (yEnd - yStart) / (xEnd - xStart);
        int resolutionX = (int)(details*200*(xEnd - xStart));
        Debug.Log(xEnd - xStart);
        Debug.Log(resolutionX);
        int resolutionZ = (int)(resolutionX*ratio);
        MeshData data = new MeshData();

        Vector3[] vertices = new Vector3[(resolutionX + 1) * (resolutionZ + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        //set verticePositions
        int index = 0;
        for (float x = 0; x <= resolutionX; x++)
        {
            for (float z = 0; z <= resolutionZ; z++)
            {
                vertices[index] = new Vector3(x / resolutionX - .5f, 0, (z - (float)resolutionZ/2) / resolutionX);
                
                
                uvs[index] = new Vector2(xStart + x * (xEnd-xStart) / resolutionX, yStart + z * (yEnd-yStart) / resolutionZ);
                
                
                normals[index] = Vector3.up;
                index++;
            }
        }

        //create one quad comsisting out of two triangles per loop
        int quad = 0;
        int tris = 0;
        Triangle[] triangles = new Triangle[resolutionX * resolutionZ * 2];
        for (int x = 0; x < resolutionX; x++)
        {
            for (int z = 0; z < resolutionZ; z++)
            {
                triangles[0 + tris] = new Triangle(quad + 0, quad + 1, quad + resolutionZ + 1);
                triangles[1 + tris] = new Triangle(quad + resolutionZ + 1, quad + 1, quad + resolutionZ + 2);
                quad++;
                tris += 2;
            }

            //prevent creation of triangles between rows
            quad++;
        }

        data.vertices = vertices.ToList();
        data.uvs = uvs.ToList();
        data.faces = triangles.ToList();
        data.normals = normals.ToList();
        return data;
    }
}