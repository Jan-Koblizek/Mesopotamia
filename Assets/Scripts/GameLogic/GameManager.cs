using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameEndConditions gameEndConditions;
    //public UIManager uiManager;

    [Tooltip("Buildings the player can construct")]
    public List<Building> availableBuildings;
    [Tooltip("Units the player can train")]
    public List<UnitDefinition> availableUnits;

    public List<Player> players;
    [Tooltip("Human controled plaeyr")]
    public Player thisPlayer;

    public int playerTurn = 0;

    [HideInInspector]
    public Label turnLabel;
    [HideInInspector]
    private int turn = 0;
    public int Turn
    {
        get { return turn; }
        set {
            turn = value;
            turnLabel.text = $"Turn: {turn}";
        }
    }

    [HideInInspector]
    private City selectedCity = null;
    public City SelectedCity
    {
        get { return selectedCity; }
        set { 
            selectedCity = value;
            if (selectedCity != null)
            {
                SoundsManager.PlaySound(SoundsManager.Sound.City);
                HexGrid.instance.ClearHighlights();
                selectedCity.GetHighlights();
                UIManager.instance.DisplayCitySidePanel();
            }
            else
            {
                HexGrid.instance.ClearHighlights();
                UIManager.instance.HideCitySidePanel();
            }
            //uiManager.DisplayCityOptions(value);
        }
    }

    public void LeaveCity()
    {
        SelectedCity = null;
    }

    public void BuildingSelect(Building building)
    {
        List<HexCell> freeCells = selectedCity.getFreeCells();
        List<BuildingPlace> buildingPlaces = new List<BuildingPlace>();
        for (int i=0; i < freeCells.Count; i++)
        {
            if (building.CanPlaceOnCell(freeCells[i])) buildingPlaces.Add(new BuildingPlace(building, freeCells[i]));
        }
        UIManager.instance.buildingPlacesManager.gameObject.SetActive(true);
        UIManager.instance.buildingPlacesManager.DisplayPlaces(buildingPlaces);
    }

    public bool HasAvailableCell(Building building)
    {
        List<HexCell> freeCells = selectedCity.getFreeCells();
        for (int i = 0; i < freeCells.Count; i++)
        {
            if (building.CanPlaceOnCell(freeCells[i])) return true;
        }
        return false;
    }

    public void Build(Building building, HexCell cell)
    {
        SoundsManager.PlaySound(SoundsManager.Sound.Building);
        cell.building = building;
        Building buildingInstance = building.Initialize(cell, SelectedCity);
        selectedCity.AddBuilding(buildingInstance);
        thisPlayer.Pay(building.buildingData.cost);
        UIManager.instance.buildingPlacesManager.HidePlaces();
        UIManager.instance.buildingPlacesManager.gameObject.SetActive(false);
        UIManager.instance.Refresh();
        if (buildingInstance.GetComponent<ObjectPlacement>() != null)
        {
            buildingInstance.GetComponent<ObjectPlacement>().Place(cell);
        }
    }

    public void Train(UnitDefinition unit, HexCell cell)
    {
        SoundsManager.PlaySound(SoundsManager.Sound.UnitTrained);
        GameObject unitInstance = Instantiate(unit.prefab, cell.transform.position, Quaternion.identity, null);
        unitInstance.GetComponent<Unit>().player = thisPlayer;
        unitInstance.GetComponent<Unit>().health = unit.maxHealth;
        unitInstance.GetComponent<Unit>().movement = 0;
        unitInstance.GetComponent<Unit>().status = UnitStatus.Exhausted;
        thisPlayer.Pay(unit.cost);
        UIManager.instance.trainingPlacesManager.HidePlaces();
        UIManager.instance.trainingPlacesManager.gameObject.SetActive(false);
        UIManager.instance.Refresh();
    }

    /*
    public void NextTurn()
    {
        thisPlayer.TurnEnd();
        Turn += 1;
        UIManager.instance.Refresh();
    }
    */

    public void NextTurnCurrentPlayer()
    {
        int playerTurnBefore = playerTurn;
        playerTurn++;
        if (playerTurn % players.Count == 0)
        {
            playerTurn = 0;
            Turn += 1;
        }
        players[playerTurnBefore].TurnEnd();
        players[playerTurn].TurnStart();
        UIManager.instance.Refresh();

        if (players[playerTurn] != thisPlayer)
        {
            if (players[playerTurn].AI != null)
            {
                players[playerTurn].AI.StartTurn();
            }
            else
            {
                NextTurnCurrentPlayer();
            }
        }
    }

    private void Update()
    {
        if (gameEndConditions.Lost())
        {
            PlayerPrefs.SetInt("Won", 0);
            SceneManager.LoadScene("EndScene");
        }
        else if (gameEndConditions.Won())
        {
            PlayerPrefs.SetInt("Won", 1);
            SceneManager.LoadScene("EndScene");
        }


        if (players[playerTurn].AI != null)
        {
            if (players[playerTurn].AI.finished == true) NextTurnCurrentPlayer();
            else players[playerTurn].AI.ExecuteTurn();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    internal void UnitSelect(UnitDefinition unit)
    {
        List<TrainingPlace> trainingPlaces = new List<TrainingPlace>();
        foreach (HexCell cell in selectedCity.ziggurat.cell.GetNeighbors(true))
        {
            if (cell.unit == null)
            {
                trainingPlaces.Add(new TrainingPlace(unit, cell));
            }
        }
        UIManager.instance.trainingPlacesManager.gameObject.SetActive(true);
        UIManager.instance.trainingPlacesManager.DisplayPlaces(trainingPlaces);
    }
}
