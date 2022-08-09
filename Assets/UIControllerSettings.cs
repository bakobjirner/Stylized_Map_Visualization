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
    public Button btReset;
    public Button btExit;
    
    private int resolution;
    private float height;
    
    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        sliderHeight = root.Q<Slider>("height_slider");
        sliderResolution = root.Q<SliderInt>("resolution_slider");
        btExit = root.Q<Button>("exit_button");
        btReset = root.Q<Button>("reset_button");
        resolution = PlayerPrefs.GetInt("resolution");
        height = PlayerPrefs.GetFloat("height");
        if (resolution == 0 || resolution == null)
        {
            Reset();
        }

        sliderHeight.value = height;
        sliderResolution.value = resolution;

        btReset.clicked += Reset;
        btExit.clicked += Exit;
    }

    private void Reset()
    {
        resolution = 7;
        height = 0.05f;
        sliderHeight.value = height;
        sliderResolution.value = resolution;
    }

    private void Exit()
    {
        height = sliderHeight.value;
        resolution = sliderResolution.value;
        PlayerPrefs.SetInt("resolution",resolution);
        PlayerPrefs.SetFloat("height", height);
        SceneManager.LoadScene(0);
    }
}
