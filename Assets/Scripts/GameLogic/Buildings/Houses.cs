using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Houses : Building
{
    [HideInInspector]
    public int capacity = 3;
    public override Building Initialize(HexCell cell, City city)
    {
        city.AddCellsNearBuilding(cell);
        GameObject building = Instantiate(this.gameObject, cell.transform.position, Quaternion.identity, cell.transform);
        GameObject buildingObject = Instantiate(buildingData.prefab, cell.transform.position, Quaternion.identity, building.transform);
        cell.TileType = Terrain.getTileType("Plains");
        city.GetHighlights();
        Houses instance = building.GetComponent<Houses>();
        instance.cell = cell;
        instance.city = city;
        instance.capacity = getCitizensCount(cell.Water);
        GameManager.instance.thisPlayer.population.Limit += instance.capacity;
        ResourceAmount food = city.player.resourceByName("food");
        city.player.resourceByName("citizens").Production = Mathf.Min(GameManager.instance.thisPlayer.cities.Count, (GameManager.instance.thisPlayer.population.Limit - GameManager.instance.thisPlayer.population.Amount), food.Amount + food.Production - food.Consumption);

        return building.GetComponent<Building>();
    }

    public override void Destroy()
    {
        city.player.population.Limit -= capacity;
        if (city.player.population.Limit < city.player.population.Amount)
        {
            city.player.resourceByName("citizens").Amount -= city.player.population.Amount - city.player.population.Limit;
            city.player.population.Amount = city.player.population.Limit;
        }
        utils.GameObjectUtils.DestroyAllChildrenTransitive(gameObject);
        Destroy(gameObject);
    }

    public override bool CanPlaceOnCell(HexCell cell)
    {
        return (cell.TileType.tileTypeName == "Plains" || cell.TileType.tileTypeName == "Forest") && cell.Elevation >= -0.2f;
    }

    public override string GetPlaceInfo(HexCell cell)
    {
        return getCitizensCount(cell.Water).ToString() + "<population>";
    }

    private int getCitizensCount(float waterAvailability)
    {
        if (waterAvailability > 0.9f)
        {
            return 4;
        }
        else if (waterAvailability > 0.55f)
        {
            return 3;
        }
        else
        {
            return 2;
        }
    }
}
