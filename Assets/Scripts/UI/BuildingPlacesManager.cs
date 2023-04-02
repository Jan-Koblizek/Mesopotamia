using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingPlacesManager : MonoBehaviour
{
    public UIDocument document;
    public List<BuildingPlace> buildingPlaces = new List<BuildingPlace>();
    private List<GroupBox> buildingPlacesUI = new List<GroupBox>();
    private Camera cam;

    private void OnEnable()
    {
        cam = Camera.main;
    }

    public void DisplayPlaces(List<BuildingPlace> buildingPlaces)
    {
        document.rootVisualElement.Clear();
        buildingPlacesUI.Clear();
        this.buildingPlaces = buildingPlaces;
        foreach (BuildingPlace buildingPlace in buildingPlaces)
        {
            CreateBuildingPlaceVisual(buildingPlace, document.rootVisualElement);
        }
    }

    public void HidePlaces()
    {
        buildingPlaces.Clear();
        document.rootVisualElement.Clear();
    }

    private void CreateBuildingPlaceVisual(BuildingPlace buildingPlace, VisualElement root)
    {
        if (buildingPlace.cell != null) {
            GroupBox groupBox = new GroupBox();
            groupBox.AddToClassList("BuildingPlaceBox");
            Vector3 viewPortPoint = cam.WorldToViewportPoint(buildingPlace.cell.transform.position);
            groupBox.style.left = root.resolvedStyle.width * viewPortPoint.x - 50;
            groupBox.style.top = root.resolvedStyle.height * viewPortPoint.y - 50;
            string info = buildingPlace.building.GetPlaceInfo(buildingPlace.cell);
            if (info != "")
            {
                GroupBox groupBoxLabel = CreateGroupBoxLabel(info);
                groupBox.Add(groupBoxLabel);
            }
            groupBox.style.left = root.resolvedStyle.width * viewPortPoint.x - 50;
            groupBox.style.top = root.resolvedStyle.height * viewPortPoint.y - 50;
            
            Button button = new Button();
            button.AddToClassList("BuildingPlaceButton");
            button.clicked += () => { GameManager.instance.Build(buildingPlace.building, buildingPlace.cell); };
            button.clicked += () => ButtonEffects.ButtonClick(button);
            button.RegisterCallback<MouseEnterEvent>(e =>
            {
                ButtonEffects.ButtonMouseEnter(button);
            });
            button.RegisterCallback<MouseLeaveEvent>(e =>
            {
                ButtonEffects.ButtonMouseExit(button);
            });
            groupBox.Add(button);
            groupBox.style.display = DisplayStyle.None;
            root.Add(groupBox);
            buildingPlacesUI.Add(groupBox);
        }
    }

    private GroupBox CreateGroupBoxLabel(string text)
    {
        string[] textParts = text.Split('>');
        GroupBox groupBoxLabel = new GroupBox();
        groupBoxLabel.AddToClassList("BuildingPlaceLabelBox");
        for (int i = 0; i < textParts.Length; i++)
        {
            string textPart = textParts[i];
            string[] textPartSplit = textPart.Split('<');
            Label label = new Label();
            label.AddToClassList("BuildingPlaceLabel");
            label.text = textPartSplit[0];
            groupBoxLabel.Add(label);
            if (textPartSplit.Length > 1)
            {
                Image icon = new Image();
                icon.AddToClassList("BuildingPlaceLabelImage");
                string iconIdentifier = textPartSplit[1];
                ResourceAmount resource = GameManager.instance.thisPlayer.startingResources.Find(x => x.resource.nameOfTheResource == iconIdentifier);
                if (iconIdentifier == "population") icon.sprite = GameManager.instance.thisPlayer.population.icon;
                else if (resource != null)
                {
                    icon.sprite = resource.resource.icon;
                }
                groupBoxLabel.Add(icon);
            }
        }
        return groupBoxLabel;
    }

    private void Update()
    {
        VisualElement root = document.rootVisualElement;
        for (int i = 0; i < buildingPlaces.Count; i++)
        {
            Vector3 viewPortPoint = cam.WorldToViewportPoint(buildingPlaces[i].cell.transform.position);
            buildingPlacesUI[i].style.left = root.resolvedStyle.width * viewPortPoint.x - 50;
            buildingPlacesUI[i].style.top = root.resolvedStyle.height * (1.0f - viewPortPoint.y) - 50;
            buildingPlacesUI[i].style.display = DisplayStyle.Flex;
        }
    }
}

public class BuildingPlace
{
    public Building building;
    public HexCell cell;

    public BuildingPlace(Building building, HexCell cell)
    {
        this.building = building;
        this.cell = cell;
    }
}
