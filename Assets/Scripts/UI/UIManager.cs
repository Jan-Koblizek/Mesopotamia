using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public CitySidePanelManager citySidePanel;
    public ChooseBuildingPanelManager chooseBuildingPanel;
    public ChooseUnitPanelManager chooseUnitPanel;
    public BuildingPlacesManager buildingPlacesManager;
    public TrainingPlacesManager trainingPlacesManager;
    public CityOverheadManager cityOverheadManager;

    public UnitOverheadManager unitOverheadManager;
    public UnitMenuManager unitMenuManager;

    public TooltipManager tooltipManager;
    // Start is called before the first frame update
    void OnEnable()
    {
        instance = this;
    }

    public void DisplayCitySidePanel()
    {
        citySidePanel.gameObject.SetActive(true);
    }

    public void HideCitySidePanel()
    {
        citySidePanel.gameObject.SetActive(false);
        HideChooseUnitPanel();
        HideChooseBuildingPanel();
}

    public void DisplayChooseBuildingPanel()
    {
        chooseBuildingPanel.gameObject.SetActive(true);
        HideChooseUnitPanel();
        buildingPlacesManager.gameObject.SetActive(false);
        chooseBuildingPanel.UpdateButtonAvailability();
    }

    public void HideChooseBuildingPanel()
    {
        chooseBuildingPanel.gameObject.SetActive(false);
        buildingPlacesManager.gameObject.SetActive(false);
    }

    public void DisplayChooseUnitPanel()
    {
        chooseUnitPanel.gameObject.SetActive(true);
        HideChooseBuildingPanel();
        chooseUnitPanel.UpdateButtonAvailability();
    }

    public void HideChooseUnitPanel()
    {
        chooseUnitPanel.gameObject.SetActive(false);
        trainingPlacesManager.gameObject.SetActive(false);
    }

    public void Refresh()
    {
        if (chooseBuildingPanel.gameObject.activeSelf)
        {
            chooseBuildingPanel.UpdateButtonAvailability();
        }

        if (chooseUnitPanel.gameObject.activeSelf)
        {
            chooseUnitPanel.UpdateButtonAvailability();
        }

        if (GameManager.instance.players[GameManager.instance.playerTurn] != GameManager.instance.thisPlayer)
        {
            buildingPlacesManager.gameObject.SetActive(false);
            trainingPlacesManager.gameObject.SetActive(false);
        }
    }
}
