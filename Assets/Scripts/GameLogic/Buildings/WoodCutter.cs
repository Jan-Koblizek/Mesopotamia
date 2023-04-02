using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodCutter : Building
{
    [HideInInspector]
    public int production = 3;

    public override Building Initialize(HexCell cell, City city)
    {
        city.AddCellsNearBuilding(cell);
        GameObject building = Instantiate(this.gameObject, cell.transform.position, Quaternion.identity, cell.transform);
        Instantiate(buildingData.prefab, cell.transform.position, Quaternion.identity, building.transform);
        city.GetHighlights();

        WoodCutter instance = building.GetComponent<WoodCutter>();
        instance.cell = cell;
        instance.city = city;
        instance.production = getWoodYield(cell.Water);
        city.player.resourceByName("wood").Production += instance.production;
        return building.GetComponent<Building>();
    }

    public override void Destroy()
    {
        city.player.resourceByName("wood").Production -= production;
        GameManager.instance.thisPlayer.resourceByName("citizens").Amount += 1;
        utils.GameObjectUtils.DestroyAllChildrenTransitive(gameObject);
        Destroy(gameObject);
    }

    public override bool CanPlaceOnCell(HexCell cell)
    {
        return cell.TileType.tileTypeName == "Forest" && cell.Elevation >= -0.2f;
    }

    public override string GetPlaceInfo(HexCell cell)
    {
        return getWoodYield(cell.Water).ToString() + "<wood>";
    }

    private int getWoodYield(float waterAvailability)
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
