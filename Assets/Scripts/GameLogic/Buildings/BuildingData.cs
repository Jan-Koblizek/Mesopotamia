using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData")]
public class BuildingData : ScriptableObject
{
    public Cost cost;
    [Tooltip("Icon representing the building")]
    public Sprite sprite;
    [Tooltip("Displayed name of the building")]
    public string buildingName;
    [TextArea(2, 5)]
    [Tooltip("Description of the building (its purpose / function)")]
    public string buildingDescription;
    [Tooltip("Prefab to be instantiated, when the building is built")]
    public GameObject prefab;
    [Tooltip("Building type associated with this specification")]
    public BuildingType type;
}