using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    public UIDocument document;

    private List<(Button, Building)> buttonList = new List<(Button, Building)>();
    private GroupBox buildingInfo;
    private GroupBox resources;
    private Label buildingName;
    private Label buildingDescription;
    void OnEnable()
    {
        Button playButton = document.rootVisualElement.Q<Button>("Play");
        playButton.clicked += () => ButtonEffects.ButtonClick(playButton);
        playButton.clicked += () => SceneManager.LoadScene("Game");
        playButton.RegisterCallback<MouseEnterEvent>(e =>
        {
            ButtonEffects.MainMenuButtonMouseEnter(playButton);
        });
        playButton.RegisterCallback<MouseLeaveEvent>(e =>
        {
            ButtonEffects.MainMenuButtonMouseExit(playButton);
        });

        /*
        Button settingsButton = document.rootVisualElement.Q<Button>("Settings");
        settingsButton.clicked += () => ButtonEffects.ButtonClick(settingsButton);
        settingsButton.RegisterCallback<MouseEnterEvent>(e =>
        {
            ButtonEffects.MainMenuButtonMouseEnter(settingsButton);
        });
        settingsButton.RegisterCallback<MouseLeaveEvent>(e =>
        {
            ButtonEffects.MainMenuButtonMouseExit(settingsButton);
        });
        */

        Button exitButton = document.rootVisualElement.Q<Button>("Exit");
        exitButton.clicked += () => ButtonEffects.ButtonClick(exitButton);
        exitButton.clicked += () => Application.Quit();
        exitButton.RegisterCallback<MouseEnterEvent>(e =>
        {
            ButtonEffects.MainMenuButtonMouseEnter(exitButton);
        });
        exitButton.RegisterCallback<MouseLeaveEvent>(e =>
        {
            ButtonEffects.MainMenuButtonMouseExit(exitButton);
        });
    }
}
