using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMeshData
{
    public List<Vector3> vertices;
    public List<Triangle> faces;
    public List<Vector3> normals;
    public List<Vector2> uvs;
    public Dictionary<long, int> middlePointIndexCache;

    public SphereMeshData(List<Vector3> vertices, List<Triangle> faces, List<Vector3> normals, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.faces = faces;
        this.normals = normals;
        this.uvs = uvs;
    }
    public SphereMeshData()
    {
    }


        public Vector3[] getVerticeArray()
    {
        return vertices.ToArray();
    }
    public Vector3[] getNormalArray()
    {
        return normals.ToArray();
    }
    public Vector2[] getUVArray()
    {
        return uvs.ToArray();
    }
    public int[] getTriangleArray()
    {
        //create the list of triangles (always 3 consecutive points in list are one triangle)
        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++)
        {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }
        return triList.ToArray();
    }
}