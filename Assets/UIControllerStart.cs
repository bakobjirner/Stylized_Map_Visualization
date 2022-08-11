using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class UIControllerStart : MonoBehaviour
{
    
    public Button btStart;
    public Button btExit;
    public Button btSettings;
    public Label loadingIndicator;
    
    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        btStart = root.Q<Button>("start_button");
        btExit = root.Q<Button>("exit_button");
        btSettings = root.Q<Button>("settings_button");
        loadingIndicator = root.Q<Label>("loading_indicator");
        btExit.clicked += Exit;
        btStart.clicked += StartGame;
        btSettings.clicked += OpenSettings;

    }

    private void Exit()
    {
        Application.Quit();
    }

    private void StartGame()
    {
        btStart.style.display = DisplayStyle.None;
        btExit.style.display = DisplayStyle.None;
        btSettings.style.display = DisplayStyle.None;
        loadingIndicator.style.display = DisplayStyle.Flex;
        SceneManager.LoadScene(1);
    }

    private void OpenSettings()
    {
        SceneManager.LoadScene(2);
    }
}
