using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChooseBuildingPanelManager : MonoBehaviour
{
    public UIDocument document;

    private List<(Button, Building)> buttonList = new List<(Button, Building)>();
    private GroupBox buildingInfo;
    private GroupBox resources;
    private Label buildingName;
    private Label buildingDescription;
    void OnEnable()
    {
        buildingInfo = document.rootVisualElement.Q<GroupBox>("BuildingInfo");
        resources = document.rootVisualElement.Q<GroupBox>("Resources");
        buildingName = document.rootVisualElement.Q<Label>("BuildingName");
        buildingDescription = document.rootVisualElement.Q<Label>("Description");
        HideBuildingInfo();

        foreach (Building building in GameManager.instance.availableBuildings)
        {
            Button button = new Button();
            button.AddToClassList("buildingButton");
            button.style.backgroundImage = building.buildingData.sprite.texture;
            button.clicked += () => { GameManager.instance.BuildingSelect(building); };
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
                ShowBuildingInfo(building);
            });
            button.RegisterCallback<MouseLeaveEvent>(e =>
            {
                HideBuildingInfo();
            });
            button.SetEnabled(false);
            button.style.borderBottomColor = button.style.borderRightColor = button.style.borderLeftColor = button.style.borderTopColor = Color.gray;
            document.rootVisualElement.Q<GroupBox>("Buildings").Add(button);
            buttonList.Add((button, building));
        }
        UpdateButtonAvailability();
    }

    public void UpdateButtonAvailability()
    {
        foreach ((Button, Building) button in buttonList)
        {
            bool canBuild = GameManager.instance.thisPlayer.EnoughResources(button.Item2.buildingData.cost) && GameManager.instance.HasAvailableCell(button.Item2);
            canBuild = canBuild && GameManager.instance.players[GameManager.instance.playerTurn] == GameManager.instance.thisPlayer;
            button.Item1.SetEnabled(canBuild);
            if (canBuild)
            {
                button.Item1.style.borderBottomColor = button.Item1.style.borderRightColor = button.Item1.style.borderLeftColor = button.Item1.style.borderTopColor = new Color(0.7372549f, 0.6784314f, 0.1843137f);
            }
            else
            {
                button.Item1.style.borderBottomColor = button.Item1.style.borderRightColor = button.Item1.style.borderLeftColor = button.Item1.style.borderTopColor = Color.gray;
            }
        }
    }

    private void HideBuildingInfo()
    {
        buildingInfo.style.display = DisplayStyle.None;
    }

    private void ShowBuildingInfo(Building building)
    {
        buildingName.text = building.buildingData.name;
        buildingDescription.text = building.buildingData.buildingDescription;
        buildingInfo.style.display = DisplayStyle.Flex;
        resources.Clear();
        foreach (ResourceAmount ra in building.buildingData.cost.resources)
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
            resources.Add(label);
            resources.Add(image);
        }
    }
}
