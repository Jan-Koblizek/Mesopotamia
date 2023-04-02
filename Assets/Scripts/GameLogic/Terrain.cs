using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [Tooltip("Tile types available in the game")]
    public List<TileType> tileTypes;
    public static Terrain instance;

    private void Awake()
    {
        instance = this;
    }

    public static TileType getTileType(string name)
    {
        for (int i = 0; i < instance.tileTypes.Count; i++)
        {
            if (instance.tileTypes[i].tileTypeName == name) return instance.tileTypes[i];
        }
        throw new InvalidTileTypeException();
    }
}

public class InvalidTileTypeException : System.Exception { }
