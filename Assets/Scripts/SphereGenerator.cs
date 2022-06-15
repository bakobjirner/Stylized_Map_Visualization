using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SphereGenerator
{

    public static Dictionary<long, int> middlePointIndexCache;

    public static MeshData GetSphere(int subdivisions)
    {
        return ReadFile(subdivisions);
    }

    public static void WriteFile(MeshData data,int subdivisions)
    {
        XmlSerializer xmls = new XmlSerializer(typeof(MeshData));

        using (var stream = File.OpenWrite("mesh"+subdivisions+".xml"))
        {
            xmls.Serialize(stream, data);
        }
    }

    public static MeshData ReadFile(int subdivisions)
    {

        MeshData data;
        XmlSerializer xmls = new XmlSerializer(typeof(MeshData));
        try
        {
            using (var stream = File.OpenRead("mesh" + subdivisions + ".xml"))
            {
                data = xmls.Deserialize(stream) as MeshData;
            }
        }
        catch
        {
            data = CreateIco(subdivisions);
            WriteFile(data,subdivisions);
        }
        return data;
    }



    private static MeshData CreateIco(int subdivisions)
    {

        MeshData data = new MeshData();
        List<Vector3> vertList;

        middlePointIndexCache = new Dictionary<long, int>();
        vertList = new List<Vector3>();

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized);
        vertList.Add(new Vector3(1f, t, 0f).normalized);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized);
        vertList.Add(new Vector3(1f, -t, 0f).normalized);

        vertList.Add(new Vector3(0f, -1f, t).normalized);
        vertList.Add(new Vector3(0f, 1f, t).normalized);
        vertList.Add(new Vector3(0f, -1f, -t).normalized);
        vertList.Add(new Vector3(0f, 1f, -t).normalized);

        vertList.Add(new Vector3(t, 0f, -1f).normalized);
        vertList.Add(new Vector3(t, 0f, 1f).normalized);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized);

        data.vertices = vertList;

        // create 20 triangles of the icosahedron
        List<Triangle> faces = new List<Triangle>();

        // 5 faces around point 0
        faces.Add(new Triangle(0, 11, 5));
        faces.Add(new Triangle(0, 5, 1));
        faces.Add(new Triangle(0, 1, 7));
        faces.Add(new Triangle(0, 7, 10));
        faces.Add(new Triangle(0, 10, 11));

        // 5 adjacent faces
        faces.Add(new Triangle(1, 5, 9));
        faces.Add(new Triangle(5, 11, 4));
        faces.Add(new Triangle(11, 10, 2));
        faces.Add(new Triangle(10, 7, 6));
        faces.Add(new Triangle(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new Triangle(3, 9, 4));
        faces.Add(new Triangle(3, 4, 2));
        faces.Add(new Triangle(3, 2, 6));
        faces.Add(new Triangle(3, 6, 8));
        faces.Add(new Triangle(3, 8, 9));

        // 5 adjacent faces
        faces.Add(new Triangle(4, 9, 5));
        faces.Add(new Triangle(2, 4, 11));
        faces.Add(new Triangle(6, 2, 10));
        faces.Add(new Triangle(8, 6, 7));
        faces.Add(new Triangle(9, 8, 1));

        data.faces = faces;

        //subdivide to desired level
        SubdivideFaces(data, subdivisions);

        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < data.vertices.Count; i++)
        {
            float u;
            float v;
            float lat = Mathf.Asin(data.vertices[i].y);
            float lng = Mathf.Atan2(data.vertices[i].x, -data.vertices[i].z);
            v = lat / (Mathf.PI) + 0.5f;
            u = lng / (2 * Mathf.PI) + 0.5f;
            uvs.Add(new Vector2(u, v));
        }

        data.uvs = uvs;

        int[] wrapped = DetectWrappedUVCoordinates(data);
        Debug.Log(wrapped.Length);
        FixWrappedUV(wrapped, data);

        //set normals, simply use position of the point so all normals point outward
        data.normals = new List<Vector3>();
        for (int i = 0; i < data.vertices.Count; i++)
        {
            data.normals.Add(vertList[i].normalized);
        }


        return data;
    }


    private static void SubdivideFaces(MeshData data, int numberOfSubdivisions)
    {

        //in case somebody enters a value that is to high. do not remove if you aren't absolutely sure what youre doing. Will slow down computer
        if (numberOfSubdivisions > 8)
        {
            numberOfSubdivisions = 2;
        }

        // refine triangles
        for (int i = 0; i < numberOfSubdivisions; i++)
        {
            List<Triangle> facesDivided = new List<Triangle>();
            foreach (var tri in data.faces)
            {
                // replace triangle by 4 triangles
                int a = MiddlePoint(tri.v1, tri.v2, data);
                int b = MiddlePoint(tri.v2, tri.v3, data);
                int c = MiddlePoint(tri.v3, tri.v1, data);

                facesDivided.Add(new Triangle(tri.v1, a, c));
                facesDivided.Add(new Triangle(tri.v2, b, a));
                facesDivided.Add(new Triangle(tri.v3, c, b));
                facesDivided.Add(new Triangle(a, b, c));
            }
            data.faces = facesDivided;
        }
    }



    // return index of vertice in the middle of p1 and p2, creates new vertice if it doesn't exist yet
    private static int MiddlePoint(int p1, int p2, MeshData data)
    {
        // get key of searched point in dictionary by combining the indexes of the two points
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        //key is smaller index followed by larger index
        long key = (smallerIndex << 32) + greaterIndex;

        //check if point exists
        int indexMiddle;
        if (middlePointIndexCache.TryGetValue(key, out indexMiddle))
        {
            //if the point already exists, return its index
            return indexMiddle;
        }

        // if it doesn't exist, calculate it
        Vector3 point1 = data.vertices[p1];
        Vector3 point2 = data.vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        //normalize point to make sure its on the sphere
        middle = middle.normalized;

        // add vertex to list
        int i = data.vertices.Count;
        data.vertices.Add(middle);

        // store it to dictonary, return index
        middlePointIndexCache.Add(key, i);

        return i;
    }

    /**
     * detect which triangles sit on the seem. find them by looking for inverted normals
     * */
    private static int[] DetectWrappedUVCoordinates(MeshData data)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < data.faces.Count; ++i)
        {
            int a = data.faces[i].v1;
            int b = data.faces[i].v2;
            int c = data.faces[i].v3;
            Vector3 texA = new Vector3(data.uvs[a].x, data.uvs[a].y, 0);
            Vector3 texB = new Vector3(data.uvs[b].x, data.uvs[b].y, 0);
            Vector3 texC = new Vector3(data.uvs[c].x, data.uvs[c].y, 0);
            Vector3 texNormal = Vector3.Cross(texB - texA, texC - texA);
            if (texNormal.z > 0)
            {
                indices.Add(i);
            }

        }
        return indices.ToArray();
    }

    /**
     * duplicate vertices that cause seem disfraction
     * */
    private static void FixWrappedUV(int[] wrapped, MeshData data)
    {
        int verticeIndex = data.vertices.Count - 1;
        Dictionary<int, int> visited = new Dictionary<int, int>();
        foreach (int i in wrapped)
        {
            int a = data.faces[i].v1;
            int b = data.faces[i].v2;
            int c = data.faces[i].v3;
            Vector2 A = data.uvs[a];
            Vector2 B = data.uvs[b];
            Vector2 C = data.uvs[c];
            if (A.x < 0.25f)
            {
                int tempA = a;
                if (!visited.TryGetValue(a, out tempA))
                {
                    A.x += 1;
                    data.uvs.Add(A);
                    data.vertices.Add(data.vertices[a]);
                    verticeIndex++;
                    visited[a] = verticeIndex;
                    tempA = verticeIndex;
                }
                a = tempA;
                Debug.Log(i);
            }
            if (B.x < 0.25f)
            {
                int tempB = b;
                if (!visited.TryGetValue(b, out tempB))
                {
                    B.x += 1;
                    data.uvs.Add(B);
                    data.vertices.Add(data.vertices[b]);
                    verticeIndex++;
                    visited[b] = verticeIndex;
                    tempB = verticeIndex;
                }
                b = tempB;
                Debug.Log(i);
            }
            if (C.x < 0.25f)
            {
                int tempC = c;
                if (!visited.TryGetValue(c, out tempC))
                {
                    C.x += 1;
                    data.uvs.Add(C);
                    data.vertices.Add(data.vertices[c]);
                    verticeIndex++;
                    visited[c] = verticeIndex;
                    tempC = verticeIndex;
                }
                c = tempC;
                Debug.Log(i);
            }
            data.faces[i].v1 = a;
            data.faces[i].v2 = b;
            data.faces[i].v3 = c;
        }
    }

}
