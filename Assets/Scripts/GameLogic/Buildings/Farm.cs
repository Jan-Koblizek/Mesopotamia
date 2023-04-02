using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Building
{
    [HideInInspector]
    public int production = 3;

    public override Building Initialize(HexCell cell, City city)
    {
        city.AddCellsNearBuilding(cell);

        GameObject building = Instantiate(this.gameObject, cell.transform.position, Quaternion.identity, cell.transform);
        Instantiate(buildingData.prefab, cell.transform.position, Quaternion.identity, building.transform);
        cell.TileType = Terrain.getTileType("Plains");
        city.GetHighlights();

        Farm instance = building.GetComponent<Farm>();
        instance.cell = cell;
        instance.city = city;
        instance.production = getCropYield(cell.Water);
        city.player.resourceByName("food").Production += instance.production;
        ResourceAmount food = city.player.resourceByName("food");
        city.player.resourceByName("citizens").Production = Mathf.Min(GameManager.instance.thisPlayer.cities.Count, (GameManager.instance.thisPlayer.population.Limit - GameManager.instance.thisPlayer.population.Amount), food.Amount + food.Production - food.Consumption);
        return building.GetComponent<Building>();
    }

    public override void Destroy()
    {
        GameManager.instance.thisPlayer.resourceByName("food").Production -= production;
        GameManager.instance.thisPlayer.resourceByName("citizens").Amount += 1;
        utils.GameObjectUtils.DestroyAllChildrenTransitive(gameObject);
        Destroy(gameObject);
    }

    public override bool CanPlaceOnCell(HexCell cell)
    {
        return (cell.TileType.tileTypeName == "Plains" || cell.TileType.tileTypeName == "Forest") && cell.Water > 0.3 && cell.Elevation >= -0.2f;
    }

    public override string GetPlaceInfo(HexCell cell)
    {
        return getCropYield(cell.Water).ToString() + "<food>";
    }

    private int getCropYield(float waterAvailability)
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
