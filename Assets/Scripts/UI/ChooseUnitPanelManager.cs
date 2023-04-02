using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChooseUnitPanelManager : MonoBehaviour
{
    public UIDocument document;
    private List<(Button, UnitDefinition)> buttonList = new List<(Button, UnitDefinition)>();
    private GroupBox unitInfo;
    private GroupBox resources;
    private Label unitName;
    private Label unitDescription;

    private Label attack;
    private Label armor;
    private Label health;
    private Label movement;

    private VisualElement unitClass;
    void OnEnable()
    {
        unitInfo = document.rootVisualElement.Q<GroupBox>("UnitInfo");
        resources = document.rootVisualElement.Q<GroupBox>("Resources");
        unitName = document.rootVisualElement.Q<Label>("UnitName");
        unitDescription = document.rootVisualElement.Q<Label>("Description");

        attack = document.rootVisualElement.Q<Label>("Attack");
        armor = document.rootVisualElement.Q<Label>("Armor");
        health = document.rootVisualElement.Q<Label>("Health");
        movement = document.rootVisualElement.Q<Label>("Movement");

        unitClass = document.rootVisualElement.Q<VisualElement>("UnitClassIcon");
        HideUnitInfo();

        foreach (UnitDefinition unit in GameManager.instance.availableUnits)
        {
            Button button = new Button();
            button.AddToClassList("buildingButton");
            button.style.backgroundImage = unit.icon.texture;
            button.clicked += () => { GameManager.instance.UnitSelect(unit); };
            button.clicked += () => ButtonEffects.ButtonClick(button);
            button.RegisterCallback<MouseEnterEvent>(e =>
            {
                ButtonEffects.ButtonMouseEnter(button);
            });
            button.RegisterCallback<MouseLeaveEvent>(e =>
            {
                ButtonEffects.ButtonMouseExit(button);
            });
            button.RegisterCallback<MouseEnterEvent>(e =>
            {
                ShowUnitInfo(unit);
            });
            button.RegisterCallback<MouseLeaveEvent>(e =>
            {
                HideUnitInfo();
            });
            button.SetEnabled(false);
            button.style.borderBottomColor = button.style.borderRightColor = button.style.borderLeftColor = button.style.borderTopColor = Color.gray;
            document.rootVisualElement.Q<GroupBox>("Units").Add(button);
            buttonList.Add((button, unit));
        }
        UpdateButtonAvailability();
    }

    public void UpdateButtonAvailability()
    {
        foreach ((Button, UnitDefinition) button in buttonList)
        {
            bool canTrain = GameManager.instance.thisPlayer.EnoughResources(button.Item2.cost);
            canTrain = canTrain && GameManager.instance.players[GameManager.instance.playerTurn] == GameManager.instance.thisPlayer;
            button.Item1.SetEnabled(canTrain);
            if (canTrain)
            {
                button.Item1.style.borderBottomColor = button.Item1.style.borderRightColor = button.Item1.style.borderLeftColor = button.Item1.style.borderTopColor = new Color(0.7372549f, 0.6784314f, 0.1843137f);
            }
            else
            {
                button.Item1.style.borderBottomColor = button.Item1.style.borderRightColor = button.Item1.style.borderLeftColor = button.Item1.style.borderTopColor = Color.gray;
            }
        }
    }

    private void HideUnitInfo()
    {
        unitInfo.style.display = DisplayStyle.None;
    }

    private void ShowUnitInfo(UnitDefinition unit)
    {
        unitName.text = unit.unitName;
        unitDescription.text = unit.description;
        unitInfo.style.display = DisplayStyle.Flex;
        resources.Clear();
        foreach (ResourceAmount ra in unit.cost.resources)
        {
            Label label = new Label();
            label.name = ra.resource.name + "Label";
            label.style.fontSize = 20;
            label.style.width = new StyleLength(StyleKeyword.Auto);
            label.style.marginLeft = 10;
            label.style.marginRight = 0;
            label.style.paddingBottom = label.style.paddingTop = label.style.paddingLeft = label.style.paddingRight = 0;
            label.text = ra.Amount.ToString();
            int playerResourceAmount = 0;
            foreach (ResourceAmount pra in GameManager.instance.thisPlayer.startingResources)
            {
                if (pra.resource == ra.resource) playerResourceAmount = pra.Amount;
            }
            if (ra.Amount <= playerResourceAmount)
            {
                label.style.color = Color.white;
            }
            else
            {
                label.style.color = Color.red;
            }
            label.AddToClassList("ResourceAmount");
            Image image = new Image();
            image.name = $"{ra.resource.name}Icon";
            image.AddToClassList("ResourceIcon");
            image.sprite = ra.resource.icon;
            image.style.height = 30;
            image.style.alignSelf = Align.Center;

            attack.text = unit.baseAttack.ToString();
            armor.text = unit.baseArmor.ToString();
            health.text = unit.maxHealth.ToString();
            movement.text = unit.movementPerTurn.ToString();

            unitClass.style.backgroundImage = (StyleBackground)unit.unitClass.icon;

            resources.Add(label);
            resources.Add(image);
        }
    }
}
