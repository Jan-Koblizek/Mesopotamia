using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "ScriptableObjects/TileType")]
public class TileType : ScriptableObject
{
    public string tileTypeName;
    [TextArea(2, 5)]
    public string description;
    public Texture icon;
    [Tooltip("Order in which the tile types are displayed, lower number means higher priority")]
    public int displayPriority;
}
