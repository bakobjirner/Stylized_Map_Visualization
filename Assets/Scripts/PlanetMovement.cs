using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlanetMovement : MonoBehaviour
{
    public float rotationSpeed = 1;
    public float scrollSpeed = 100;
    public float speed = .1f;
    private bool global = true;
    public float rotLimitGlobal = 80;
    public float zoomUpperLimit = -2;
    public float zoomLowerLimit = -1.2f;
    public float zoomLowerLimitDetail = 0;

    public Transform cameraHolder;

    public Transform myCamera;

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Zoom();
        Move();

        if (global)
        {
            if (Input.GetMouseButtonDown(0)&&this.GetComponent<PlanetGenerator>().mode == 0)
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    Vector2 pos = new Vector2((hit.textureCoord.x - .5f) * 360, (hit.textureCoord.y - .5f) * 180);

                    int featureIndex = CheckInPolygon.GetFeatureByCoordiantes(pos);
                    if (featureIndex == -1)
                    {
                        Debug.Log("no feature");
                    }
                    else
                    {
                        Debug.Log(hit.textureCoord);
                        Debug.Log(pos);
                        cameraHolder.rotation = Quaternion.Euler(70,0,0);
                        transform.rotation = Quaternion.identity;
                        this.GetComponent<PlanetGenerator>().ShowDetailView(featureIndex);
                        global = false;
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cameraHolder.rotation = Quaternion.Euler(45,0,0);
                transform.rotation = Quaternion.identity;
                myCamera.transform.localPosition = new Vector3(0, 0, -2);
                this.GetComponent<PlanetGenerator>().ShowGlobalView();
                global = true;
            }
        }
    }

    private void Rotate()
    {
        if (global)
        {
            Quaternion oldRot = cameraHolder.rotation;
            transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * Time.deltaTime * rotationSpeed);
            cameraHolder.Rotate(Vector3.right, Input.GetAxis("Vertical") * Time.deltaTime * rotationSpeed);
            if ((cameraHolder.rotation.eulerAngles.x >= rotLimitGlobal && cameraHolder.rotation.eulerAngles.x <= 180) ||
                (cameraHolder.rotation.eulerAngles.x <= 360 - rotLimitGlobal && cameraHolder.rotation.eulerAngles.x >= 180) ||
                cameraHolder.rotation.eulerAngles.x <= -rotLimitGlobal) {
                cameraHolder.rotation = oldRot;
            }
        }
    }

    private void Zoom()
    {
        if (global)
        {
            Vector3 oldZoom = myCamera.transform.localPosition;
            myCamera.transform.Translate(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scrollSpeed * Vector3.forward);
            if (myCamera.transform.localPosition.z < zoomUpperLimit || myCamera.transform.localPosition.z > zoomLowerLimit )
            {
                myCamera.transform.localPosition = oldZoom;
            }
        }
        else
        {
            Vector3 oldZoom = myCamera.transform.localPosition;
            myCamera.transform.Translate(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scrollSpeed * Vector3.forward);
            if (myCamera.transform.localPosition.z < zoomUpperLimit || myCamera.transform.localPosition.z > zoomLowerLimitDetail )
            {
                myCamera.transform.localPosition = oldZoom;
            }
        }
       
    }

    private void Move()
    {
        if (!global)
        {
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime * speed, Input.GetAxis("Vertical") * Time.deltaTime * speed,
                0);
            myCamera.transform.Translate(movement);
        }
    }
}