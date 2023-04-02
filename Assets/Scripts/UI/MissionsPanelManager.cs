using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MissionsPanelManager : MonoBehaviour
{
    public UIDocument document;
    [Tooltip("Icon indicator, that the mission is finished (for example a checkmark)")]
    public Texture finished;
    [Tooltip("Icon indicator, that the mission isn't finished yet (for example a red cross)")]
    public Texture unfinished;


    private List<(GameEndCondition, GameEndConditionDisplay)> conditions = new List<(GameEndCondition, GameEndConditionDisplay)>();

    private void Start()
    {
        GroupBox victoryConditions = document.rootVisualElement.Q<GroupBox>("VictoryConditions");
        victoryConditions.Clear();
        foreach (PlayerConditionPair pair in GameManager.instance.gameEndConditions.victoryConditions)
        {
            GameEndConditionDisplay display = CreateGameEndConditionDisplay(pair.condition, pair.player);
            victoryConditions.Add(display.groupBox);
            conditions.Add((pair.condition, display));
        }

        GroupBox defeatConditions = document.rootVisualElement.Q<GroupBox>("DefeatConditions");
        defeatConditions.Clear();
        foreach (PlayerConditionPair pair in GameManager.instance.gameEndConditions.defeatConditions)
        {
            GameEndConditionDisplay display = CreateGameEndConditionDisplay(pair.condition, pair.player);
            defeatConditions.Add(display.groupBox);
            conditions.Add((pair.condition, display));
        }
    }

    private void Update()
    {
        foreach ((GameEndCondition, GameEndConditionDisplay) condition in conditions)
        {
            condition.Item2.label.text = condition.Item1.text;
            condition.Item2.checkBox.style.backgroundImage = ((StyleBackground)(condition.Item1.satisfied ? finished : unfinished));
        }
    }

    private GameEndConditionDisplay CreateGameEndConditionDisplay(GameEndCondition condition, Player player)
    {
        GroupBox groupBox = new GroupBox();
        groupBox.AddToClassList("Condition");

        VisualElement check = new VisualElement();
        check.AddToClassList("ConditionCheck");
        check.style.backgroundImage = ((StyleBackground)(condition.satisfied ? finished : unfinished));

        Label label = new Label();
        label.AddToClassList("ConditionText");
        label.text = condition.text;

        VisualElement playerIndicator = new VisualElement();
        playerIndicator.AddToClassList("ConditionPlayerIndicator");
        playerIndicator.style.backgroundColor = player.color;

        groupBox.Add(check);
        groupBox.Add(label);
        groupBox.Add(playerIndicator);

        GameEndConditionDisplay result = new GameEndConditionDisplay();
        result.groupBox = groupBox;
        result.checkBox = check;
        result.label = label;
        result.playerIndicator = playerIndicator;

        return result;
    }

    private class GameEndConditionDisplay
    {
        public GroupBox groupBox;
        public Label label;
        public VisualElement checkBox;
        public VisualElement playerIndicator;
    }
}