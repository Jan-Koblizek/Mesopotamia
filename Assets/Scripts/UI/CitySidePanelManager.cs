using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CitySidePanelManager : MonoBehaviour
{
    public UIDocument document;
    // Start is called before the first frame update
    void OnEnable()
    {
        Button leave = document.rootVisualElement.Q<Button>("Leave");
        leave.clicked -= GameManager.instance.LeaveCity;
        leave.clicked += GameManager.instance.LeaveCity;
        leave.clicked += () => ButtonEffects.ButtonClick(leave);
        leave.RegisterCallback<MouseEnterEvent>(e =>
        {
            ButtonEffects.ButtonMouseEnter(leave);
        });
        leave.RegisterCallback<MouseLeaveEvent>(e =>
        {
            ButtonEffects.ButtonMouseExit(leave);
        });

        Button build = document.rootVisualElement.Q<Button>("Build");
        build.clicked -= UIManager.instance.DisplayChooseBuildingPanel;
        build.clicked += UIManager.instance.DisplayChooseBuildingPanel;
        build.clicked += () => ButtonEffects.ButtonClick(build);
        build.RegisterCallback<MouseEnterEvent>(e =>
        {
            ButtonEffects.ButtonMouseEnter(build);
        });
        build.RegisterCallback<MouseLeaveEvent>(e =>
        {
            ButtonEffects.ButtonMouseExit(build);
        });

        Button train = document.rootVisualElement.Q<Button>("Train");
        train.clicked -= UIManager.instance.DisplayChooseUnitPanel;
        train.clicked += UIManager.instance.DisplayChooseUnitPanel;
        train.clicked += () => ButtonEffects.ButtonClick(train);
        train.RegisterCallback<MouseEnterEvent>(e =>
        {
            ButtonEffects.ButtonMouseEnter(train);
        });
        train.RegisterCallback<MouseLeaveEvent>(e =>
        {
            ButtonEffects.ButtonMouseExit(train);
        });
    }
}
