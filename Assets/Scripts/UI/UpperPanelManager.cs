using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UpperPanelManager : MonoBehaviour
{
    public UIDocument document;
    // Start is called before the first frame update
    void Start()
    {
        foreach (ResourceAmount ra in GameManager.instance.thisPlayer.startingResources)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.name = ra.resource.name;
            groupBox.AddToClassList("ResourceBox");
            Label label = new Label();
            label.name = ra.resource.name + "Label";
            ra.LinkToLabel(label);
            label.AddToClassList("ResourceAmount");
            Image image = new Image();
            image.name = $"{ra.resource.name}Icon";
            image.AddToClassList("ResourceIcon");
            image.sprite = ra.resource.icon;
            groupBox.Add(label);
            groupBox.Add(image);
            document.rootVisualElement.Q<GroupBox>("Resources").Add(groupBox);
        }

        GroupBox groupBox2 = new GroupBox();
        groupBox2.name = "Population";
        groupBox2.AddToClassList("ResourceBox");
        Label label2 = new Label();
        label2.name = "Population" + "Label";
        GameManager.instance.thisPlayer.population.LinkToLabel(label2);
        label2.AddToClassList("ResourceAmount");
        Image image2 = new Image();
        image2.name = $"PopulationIcon";
        image2.AddToClassList("ResourceIcon");
        image2.sprite = GameManager.instance.thisPlayer.population.icon;
        groupBox2.Add(label2);
        groupBox2.Add(image2);
        document.rootVisualElement.Q<GroupBox>("Resources").Add(groupBox2);

        Label turn = document.rootVisualElement.Q<Label>("Turn");
        GameManager.instance.turnLabel = turn;

        document.rootVisualElement.Q<Button>("Exit").clicked += () => SceneManager.LoadScene("MainMenu");
    }
}
