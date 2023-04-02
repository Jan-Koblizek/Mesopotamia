using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ziggurat : Building
{
    private float clickTime;

    private void OnMouseDown()
    {
        clickTime = Time.time;
    }

    private void OnMouseUp()
    {
        if (Time.time - clickTime < 1)
        {
            SelectionManager.Instance.Select(city);
        }
    }
    /*
    public void UpdateVisibility()
    {
        if (cell)
        {
            int centerZ = cell.coordinates.Z;
            int centerX = cell.coordinates.X;


            for (int r = 0, z = centerZ - visibilityRadius; z <= centerZ; z++, r++)
            {
                for (int x = centerX - r; x <= centerX + visibilityRadius; x++)
                {
                    UpdateCellVisibility(HexGrid.instance.GetCell(new HexCoordinates(x, z)), VisibilityStatus.Visible);
                }
            }
            for (int r = 0, z = centerZ + visibilityRadius; z > centerZ; z--, r++)
            {
                for (int x = centerX - visibilityRadius; x <= centerX + r; x++)
                {
                    UpdateCellVisibility(HexGrid.instance.GetCell(new HexCoordinates(x, z)), VisibilityStatus.Visible);
                }
            }
        }
    }
    */

    /*
    void UpdateCellVisibility(HexCell cell, VisibilityStatus status)
    {
        if (cell)
        {
            cell.VisibilityStatus = status;
        }
    }
    */

    public override Building Initialize(HexCell cell, City city)
    {
        this.cell = cell;
        this.city = city;
        //GameManager.instance.thisPlayer.cities.Add(city);
        //UpdateVisibility();
        return null;
    }

    public override void Destroy()
    {
        utils.GameObjectUtils.DestroyAllChildrenTransitive(gameObject);
        Destroy(gameObject);
    }

    public override bool CanPlaceOnCell(HexCell cell)
    {
        return (cell.TileType.tileTypeName == "Plains" || cell.TileType.tileTypeName == "Forest") && cell.transform.position.y > 0.1 && cell.Water > 0.55f;
    }

    public override string GetPlaceInfo(HexCell cell)
    {
        return "";
    }
}
