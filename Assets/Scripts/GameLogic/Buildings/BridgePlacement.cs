using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgePlacement : ObjectPlacement
{
    private float rotation;
    private HexCell cell;
    private bool placing;
    // Start is called before the first frame update
    void Start()
    {
        placing = true;
    }

    public override void Place(HexCell inputCell)
    {
        placing = true;
        cell = inputCell;
    }

    private void place(HexCell cell)
    {
        bool northEast = cell.neighbors[0] != null && cell.neighbors[3] != null &&
                cell.neighbors[0].TileType.tileTypeName != "River" && cell.neighbors[3].TileType.tileTypeName != "River" &&
                cell.neighbors[0].Elevation >= -0.2f && cell.neighbors[3].Elevation >= -0.2f;
        bool east = cell.neighbors[1] != null && cell.neighbors[4] != null &&
                cell.neighbors[1].TileType.tileTypeName != "River" && cell.neighbors[4].TileType.tileTypeName != "River" &&
                cell.neighbors[1].Elevation >= -0.2f && cell.neighbors[4].Elevation >= -0.2f;
        bool southEast = cell.neighbors[2] != null && cell.neighbors[5] != null &&
                cell.neighbors[2].TileType.tileTypeName != "River" && cell.neighbors[5].TileType.tileTypeName != "River" &&
                cell.neighbors[2].Elevation >= -0.2f && cell.neighbors[5].Elevation >= -0.2f;

        if (east && southEast) rotation = 120;
        else if (northEast && east) rotation = 60;
        else if (northEast && southEast) rotation = 0;
        else if (northEast) rotation = 30;
        else if (east) rotation = 90;
        else if (southEast) rotation = 150;

        transform.RotateAround(transform.position, Vector3.up, rotation);
        placing = false;
    }

    private void Update()
    {
        if (placing)
        {
            place(cell);
        }
    }
}
