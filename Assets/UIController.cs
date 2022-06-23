using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{

    public Button exitButton;
    public Label countryLabel;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        exitButton = root.Q<Button>("exit");
        countryLabel = root.Q<Label>("country_name");

        exitButton.clicked += Exit;
    }

    void Exit()
    {
        Debug.Log("exit");
        Application.Quit();
    }

    public void SetCountryName(string name)
    {
        countryLabel.text = name;
    }
}
