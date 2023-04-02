using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brickyard : Building
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

        Brickyard instance = building.GetComponent<Brickyard>();
        instance.cell = cell;
        instance.city = city;
        instance.production = getBrickYield(cell.Water);
        city.player.resourceByName("bricks").Production += instance.production;
        return building.GetComponent<Building>();
    }

    public override void Destroy()
    {
        city.player.resourceByName("bricks").Production -= production;
        GameManager.instance.thisPlayer.resourceByName("citizens").Amount += 1;
        utils.GameObjectUtils.DestroyAllChildrenTransitive(gameObject);
        Destroy(gameObject);
    }

    public override bool CanPlaceOnCell(HexCell cell)
    {
        return (cell.TileType.tileTypeName == "Plains" || cell.TileType.tileTypeName == "Forest") && cell.Elevation >= -0.2f;
    }

    public override string GetPlaceInfo(HexCell cell)
    {
        return getBrickYield(cell.Water).ToString() + "<bricks>";
    }

    private int getBrickYield(float waterAvailability)
    {
        if (waterAvailability > 0.55f)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}
