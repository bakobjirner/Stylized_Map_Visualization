using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{

    public Button exitButton;
    public Button modeButton;
    public Label countryLabel;
    public DropdownField dropdownField;

    public PlanetGenerator planetGenerator;


    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        exitButton = root.Q<Button>("exit");
        countryLabel = root.Q<Label>("country_name");
        dropdownField = root.Q<DropdownField>("dropdown");
        modeButton = root.Q<Button>("set_mode");
        List<string> choices = new List<string>();
        choices.Add("heigth");
        choices.Add("gdp");
        choices.Add("gdp per capita");
        choices.Add("population");
        choices.Add("night");
        dropdownField.choices = choices;
        exitButton.clicked += Exit;
        modeButton.clicked += SetMode;
    }

    void Exit()
    {
        Debug.Log("exit");
        Application.Quit();
    }

    void SetMode()
    {
        planetGenerator.setMode(dropdownField.value);
    }

    public void SetCountryName(string name)
    {
        countryLabel.text = name;
    }
}
