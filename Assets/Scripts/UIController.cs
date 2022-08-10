using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{

    public Button exitButton;
    public Label countryLabel;
    public RadioButtonGroup radioButtonGroup;
    public RadioButton rbStandard;
    public RadioButton rbNight;
    public RadioButton rbGDP;
    public RadioButton rbPopulation;

    private int mode;


    public PlanetGenerator planetGenerator;


    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        exitButton = root.Q<Button>("exit");
        countryLabel = root.Q<Label>("country_name");
        radioButtonGroup = root.Q<RadioButtonGroup>("rb_group");
        rbStandard = radioButtonGroup.Q<RadioButton>("rb_standard");
        rbNight = radioButtonGroup.Q<RadioButton>("rb_night");
        rbGDP = radioButtonGroup.Q<RadioButton>("rb_gdp");
        rbPopulation = radioButtonGroup.Q<RadioButton>("rb_population");
        exitButton.clicked += Exit;
    }

    private void Update()
    {
        int oldMode = mode;
        if (rbStandard.value)
        {
            mode = 0;
        }else if(rbNight.value)
        {
            mode = 1;
        }else if(rbGDP.value)
        {
            mode = 2;
        }else if(rbPopulation.value)
        {
            mode = 3;
        }

        if (mode != oldMode)
        {
            planetGenerator.setMode(mode);
        }
        //disable option to change mode in detail view
        if (!planetGenerator.sphere && radioButtonGroup.visible)
        {
            radioButtonGroup.visible = false;
        }else if (planetGenerator.sphere && !radioButtonGroup.visible)
        {
            radioButtonGroup.visible = true;
            planetGenerator.setMode(mode);
        }
    }

    void Exit()
    {
        SceneManager.LoadScene(0);
    }

    public void SetCountryName(string name)
    {
        countryLabel.text = name;
    }
}
