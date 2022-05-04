using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator
{
    private static Dictionary<long, int> middlePointIndexCache;
    private static List<Vector3> vertList;
    private static SphereMeshData[] spheres = new SphereMeshData[7];


    public static SphereMeshData GetSphere(int subdivisions)
    {
        if (spheres[subdivisions] == null)
        {
            spheres[subdivisions] = CreateIco(subdivisions);
        }
        return spheres[subdivisions];
    }



    private static SphereMeshData CreateIco(int subdivisions)
    {
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

        //subdivide to desired level
        faces = SubdivideFaces(faces, subdivisions);

        //vertice-list to array
        Vector3[] verticeArray = vertList.ToArray();
        

        //create the list of triangles (always 3 consecutive points in list are one triangle)
        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++)
        {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }
        int[] triangleArray = triList.ToArray();

        //generate uvs
        Vector2[] uvs = new Vector2[vertList.Count];

        //set normals, simply use position of the point so all normals point outward
        Vector3[] normales = new Vector3[vertList.Count];
        for (int i = 0; i < normales.Length; i++)
        {
            normales[i] = vertList[i].normalized;

            uvs[i] = PointToCoordinate(verticeArray[i]);
        }

        return new SphereMeshData(verticeArray, triangleArray, normales,uvs);
    }

    private static Vector2 PointToCoordinate(Vector3 pointOnSphere)
    {
        float latitude = Mathf.Asin(pointOnSphere.y*.5f+0.5f);
        float longitude = Mathf.Atan2(pointOnSphere.x * .5f + 0.5f, -pointOnSphere.z * .5f + 0.5f);
        return new Vector2(longitude, latitude);
    } 


    private static List<Triangle> SubdivideFaces(List<Triangle> faces, int numberOfSubdivisions)
    {

        //in case somebody enters a value that is to high. do not remove if you aren't absolutely sure what youre doing. Will 
        if (numberOfSubdivisions > 8)
        {
            numberOfSubdivisions = 2;
        }

        // refine triangles
        for (int i = 0; i < numberOfSubdivisions; i++)
        {
            List<Triangle> facesDivided = new List<Triangle>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = MiddlePoint(tri.v1, tri.v2);
                int b = MiddlePoint(tri.v2, tri.v3);
                int c = MiddlePoint(tri.v3, tri.v1);

                facesDivided.Add(new Triangle(tri.v1, a, c));
                facesDivided.Add(new Triangle(tri.v2, b, a));
                facesDivided.Add(new Triangle(tri.v3, c, b));
                facesDivided.Add(new Triangle(a, b, c));
            }
            faces = facesDivided;
        }
        return faces;
    }



    // return index of vertice in the middle of p1 and p2, creates new vertice if it doesn't exist yet
    private static int MiddlePoint(int p1, int p2)
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
        Vector3 point1 = vertList[p1];
        Vector3 point2 = vertList[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        //normalize point to make sure its on the sphere
        middle = middle.normalized;

        // add vertex to list
        int i = vertList.Count;
        vertList.Add(middle);

        // store it to dictonary, return index
        middlePointIndexCache.Add(key, i);

        return i;
    }

}
