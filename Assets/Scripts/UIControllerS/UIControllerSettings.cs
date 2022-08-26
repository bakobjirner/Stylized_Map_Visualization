using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIControllerSettings : MonoBehaviour
{

    public Slider sliderHeight;
    public SliderInt sliderResolution;
    public SliderInt sliderPlanes;

    public Button btReset;
    public Button btExit;
    
    private int resolution;
    private float height;
    private int planeRate;
    
    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        sliderHeight = root.Q<Slider>("height_slider");
        sliderResolution = root.Q<SliderInt>("resolution_slider");
        sliderPlanes = root.Q<SliderInt>("plane_slider");
        btExit = root.Q<Button>("exit_button");
        btReset = root.Q<Button>("reset_button");
        resolution = PlayerPrefs.GetInt("resolution");
        height = PlayerPrefs.GetFloat("height");
        planeRate = PlayerPrefs.GetInt("planeRate");
        if (resolution == 0 || resolution == null)
        {
            Reset();
        }

        sliderHeight.value = height;
        sliderResolution.value = resolution;
        sliderPlanes.value = planeRate;
        btReset.clicked += Reset;
        btExit.clicked += Exit;
    }

    private void Reset()
    {
        resolution = 7;
        height = 0.05f;
        planeRate = 5;
        sliderPlanes.value = planeRate;
        sliderHeight.value = height;
        sliderResolution.value = resolution;
    }

    private void Exit()
    {
        height = sliderHeight.value;
        resolution = sliderResolution.value;
        planeRate = sliderPlanes.value;
        PlayerPrefs.SetInt("resolution",resolution);
        PlayerPrefs.SetInt("planeRate",planeRate);
        PlayerPrefs.SetFloat("height", height);
        SceneManager.LoadScene(0);
    }
}
