using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMeshData
{
    public Vector3[] vertices;
    public int[] faces;
    public Vector3[] normals;
    public Vector2[] uvs;

    public SphereMeshData(Vector3[] vertices, int[] faces, Vector3[] normals, Vector2[] uvs)
    {
        this.vertices = vertices;
        this.faces = faces;
        this.normals = normals;
        this.uvs = uvs;
    }
}