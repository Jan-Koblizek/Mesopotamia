using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Building
{
    public override bool CanPlaceOnCell(HexCell cell)
    {
        bool result = cell.TileType.tileTypeName == "River";
        bool oppositeLand = false;
        foreach (HexDirection dir in Enum.GetValues(typeof(HexDirection)))
        {
            HexCell neighbor = cell.neighbors[(int)dir];
            HexCell neighborOpposite = cell.neighbors[(int)dir.Opposite()];
            if (neighbor != null && neighborOpposite != null && 
                neighbor.TileType.tileTypeName != "River" && neighborOpposite.TileType.tileTypeName != "River" &&
                neighbor.Elevation >= -0.2f && neighborOpposite.Elevation >= -0.2f)
            {
                oppositeLand = true;
            }
        }
        result = result && oppositeLand;
        return result;
    }

    public override void Destroy()
    {
        utils.GameObjectUtils.DestroyAllChildrenTransitive(gameObject);
        Destroy(gameObject);
    }

    public override string GetPlaceInfo(HexCell cell)
    {
        return "";
    }

    public override Building Initialize(HexCell cell, City city)
    {
        city.AddCellsNearBuilding(cell);
        GameObject building = Instantiate(this.gameObject, cell.transform.position, gameObject.transform.rotation, cell.transform);
        Instantiate(buildingData.prefab, cell.transform.position + new Vector3(0.0f, 1.0f, 0.0f), buildingData.prefab.transform.rotation, building.transform);
        city.GetHighlights();

        Bridge instance = building.GetComponent<Bridge>();
        instance.cell = cell;
        instance.city = city;
        return building.GetComponent<Building>();
    }
}
