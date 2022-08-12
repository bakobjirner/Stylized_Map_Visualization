using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * class to hold triangle information for mesh generation
 */
public class Triangle
{
    public int v1;
    public int v2;
    public int v3;

    public Triangle(int v1, int v2, int v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }
    public Triangle()
    {
    }
}