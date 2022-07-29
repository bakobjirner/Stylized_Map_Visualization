using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlanetMovement : MonoBehaviour
{
    public float speed = 1;
    public float scrollSpeed = 100;
    private bool global = true;
    public GameObject airplaneHolder;

    public Transform cameraHolder;

    public Transform myCamera;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * Time.deltaTime * speed);
        cameraHolder.Rotate(Vector3.right, Input.GetAxis("Vertical") * Time.deltaTime * speed);
        myCamera.transform.Translate(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scrollSpeed * Vector3.forward);

        if (global)
        {
            if (Input.GetMouseButtonDown(0))
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
                        cameraHolder.rotation = Quaternion.Euler(90,0,0);
                        transform.rotation = Quaternion.identity;
                        this.GetComponent<PlanetGenerator>().ShowDetailView(featureIndex);
                        global = false;
                        airplaneHolder.SetActive(false);
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
                airplaneHolder.SetActive(true);
            }
        }
    }
}