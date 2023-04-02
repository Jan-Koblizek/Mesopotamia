using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settle : UnitAction
{
    public override void ActionUpdate() {}

    public override bool IsAvailable(Unit unit)
    {
        if (unit.player != GameManager.instance.players[GameManager.instance.playerTurn]) return false;
        if (HexGrid.instance.GetCell(unit.coordinates).TileType.name != "Plains") return false;
        foreach (var player in GameManager.instance.players)
        {
            foreach (City city in player.cities) {
                if (unit.coordinates.DistanceDirect(city.ziggurat.cell.coordinates) < 5)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnActivate(Unit unit)
    {
        HexCell cell = HexGrid.instance.GetCell(unit.coordinates);
        GameObject zigg = Instantiate(cell.ziggurat, new Vector3(0.0f, 0.0f, 0.0f) + cell.transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f), cell.transform);
        zigg.AddComponent<City>();
        zigg.GetComponent<Ziggurat>().Initialize(cell, zigg.GetComponent<City>());
        zigg.GetComponent<City>().Initialize(zigg.GetComponent<Ziggurat>(), GameManager.instance.thisPlayer);
        cell.building = zigg.GetComponent<Building>();
        SoundsManager.PlaySound(SoundsManager.Sound.Building);
        unit.RemoveUnit();
    }

    public override void OnDeselect(Unit unit)
    {
        unit.action = null;
    }

    public override void OnSelect(Unit unit)
    {
        unit.action = this;
        OnActivate(unit);
    }
}
