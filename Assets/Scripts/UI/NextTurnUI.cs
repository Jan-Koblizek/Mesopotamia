using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NextTurnUI : MonoBehaviour
{
    public UIDocument document;
    private Button nextTurnButton;
    // Start is called before the first frame update
    void Start()
    {
        Button button = document.rootVisualElement.Q<Button>("NextTurn");
        button.clicked -= GameManager.instance.NextTurnCurrentPlayer;
        button.clicked += GameManager.instance.NextTurnCurrentPlayer;
        button.clicked += () => ButtonEffects.ButtonClick(button);
        button.RegisterCallback<MouseEnterEvent>(e =>
        {
            ButtonEffects.NextTurnMouseEnter(button);
        });
        button.RegisterCallback<MouseLeaveEvent>(e =>
        {
            ButtonEffects.NextTurnMouseExit(button);
        });
        nextTurnButton = button;
    }

    private void Update()
    {
        if (GameManager.instance.thisPlayer != GameManager.instance.players[GameManager.instance.playerTurn])
        {
            nextTurnButton.SetEnabled(false);
        }
        else
        {
            nextTurnButton.SetEnabled(true);
        }
    }
}
