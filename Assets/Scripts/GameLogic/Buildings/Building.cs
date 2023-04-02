using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [HideInInspector]
    public HexCell cell;
    [HideInInspector]
    public City city;
    public BuildingData buildingData;

    public abstract Building Initialize(HexCell cell, City city);
    public abstract void Destroy();

    public abstract bool CanPlaceOnCell(HexCell cell);

    public abstract string GetPlaceInfo(HexCell cell);
}

public enum BuildingType
{
    Ziggurat,
    Bridge,
    Houses,
    Farms,
    Brickyard,
    WoodCutter
}