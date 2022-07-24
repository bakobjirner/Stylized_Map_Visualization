using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{

public Vector3 start;
public Vector3 destination;
public float height = 5;
private float pos;
public Transform plane;
private float distance;


    // Start is called before the first frame update
    void Start()
    {
        //set middle of parabola between start and destination
        transform.position = (destination + start) / 2;
        
        //point parabola towards destination
        transform.LookAt(destination);
        
        //set up direction to point outwards
        //transform.Rotate(transform.forward,180);
        
        //set distance 
        distance = Vector3.Distance(start, destination);
        pos = -distance/2;
    }

    // Update is called once per frame
    void Update()
    {
        pos += Time.deltaTime;
        if (pos > distance/2)
        {
            pos = -distance/2;
        }

        plane.transform.localPosition = new Vector3(0, getPointOnParabola(pos, distance,height), pos);
    }

    /*
     *returns the value of the parabola at a certain position. position should be between -1 ana 1
     */
    private float getPointOnParabola(float position, float distance, float height)
    {
        //normal parabola thats downwards open and moved up by one, so the nullPoints are at x= 1 and -1 : y = -x^2+1
        //stretch the parabola by .5 so the nullPoints are at x= -.5 and .5: y = -(2x)^2+1
        //stretch again in x dir by desired distance y = -((2x/distance))^2+1
        //stretch in y dir by desired height y = (-((2x/distance))^2+1)*height
        //to simplify calculation: a = ((2x/distance))
        float a = (2 * position / distance);
        return (-a*a+1)*height;
    }
}
