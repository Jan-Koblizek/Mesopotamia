using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TrainingPlacesManager : MonoBehaviour
{
    public UIDocument document;
    public List<TrainingPlace> trainingPlaces = new List<TrainingPlace>();
    private List<GroupBox> trainingPlacesUI = new List<GroupBox>();
    private Camera cam;

    private void OnEnable()
    {
        cam = Camera.main;
    }

    public void DisplayPlaces(List<TrainingPlace> trainingPlaces)
    {
        document.rootVisualElement.Clear();
        trainingPlacesUI.Clear();
        this.trainingPlaces = trainingPlaces;
        foreach (TrainingPlace trainingPlace in trainingPlaces)
        {
            CreateTrainingPlaceVisual(trainingPlace, document.rootVisualElement);
        }
    }

    public void HidePlaces()
    {
        trainingPlaces.Clear();
        document.rootVisualElement.Clear();
    }

    private void CreateTrainingPlaceVisual(TrainingPlace TrainingPlace, VisualElement root)
    {
        if (TrainingPlace.cell != null) {
            GroupBox groupBox = new GroupBox();
            groupBox.AddToClassList("TrainingPlaceBox");
            Vector3 viewPortPoint = cam.WorldToViewportPoint(TrainingPlace.cell.transform.position);
            groupBox.style.left = root.resolvedStyle.width * viewPortPoint.x - 50;
            groupBox.style.top = root.resolvedStyle.height * viewPortPoint.y - 50;

            groupBox.style.left = root.resolvedStyle.width * viewPortPoint.x - 50;
            groupBox.style.top = root.resolvedStyle.height * viewPortPoint.y - 50;
            
            Button button = new Button();
            button.AddToClassList("TrainingPlaceButton");
            button.clicked += () => { GameManager.instance.Train(TrainingPlace.unit, TrainingPlace.cell); };
            button.clicked += () => ButtonEffects.ButtonClick(button);
            button.RegisterCallback<MouseEnterEvent>(e =>
            {
                ButtonEffects.ButtonMouseEnter(button);
            });
            button.RegisterCallback<MouseLeaveEvent>(e =>
            {
                ButtonEffects.ButtonMouseExit(button);
            });
            button.style.backgroundImage = (StyleBackground)TrainingPlace.unit.icon.texture;
            groupBox.Add(button);
            groupBox.style.display = DisplayStyle.None;
            root.Add(groupBox);
            trainingPlacesUI.Add(groupBox);
        }
    }

    private GroupBox CreateGroupBoxLabel(string text)
    {
        string[] textParts = text.Split('>');
        GroupBox groupBoxLabel = new GroupBox();
        groupBoxLabel.AddToClassList("TrainingPlaceLabelBox");
        for (int i = 0; i < textParts.Length; i++)
        {
            string textPart = textParts[i];
            string[] textPartSplit = textPart.Split('<');
            Label label = new Label();
            label.AddToClassList("TrainingPlaceLabel");
            label.text = textPartSplit[0];
            groupBoxLabel.Add(label);
            if (textPartSplit.Length > 1)
            {
                Image icon = new Image();
                icon.AddToClassList("TrainingPlaceLabelImage");
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
        for (int i = 0; i < trainingPlaces.Count; i++)
        {
            Vector3 viewPortPoint = cam.WorldToViewportPoint(trainingPlaces[i].cell.transform.position);
            trainingPlacesUI[i].style.left = root.resolvedStyle.width * viewPortPoint.x - 50;
            trainingPlacesUI[i].style.top = root.resolvedStyle.height * (1.0f - viewPortPoint.y) - 50;
            trainingPlacesUI[i].style.display = DisplayStyle.Flex;
        }
    }
}

public class TrainingPlace
{
    public UnitDefinition unit;
    public HexCell cell;

    public TrainingPlace(UnitDefinition unit, HexCell cell)
    {
        this.unit = unit;
        this.cell = cell;
    }
}
