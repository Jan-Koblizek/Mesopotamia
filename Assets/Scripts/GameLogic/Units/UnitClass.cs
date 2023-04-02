using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitClass", menuName = "ScriptableObjects/UnitClass")]
public class UnitClass : ScriptableObject
{
    [Tooltip("Name of the unit class")]
    public string typeName;
    [TextArea(2, 5)]
    public string description;
    [Tooltip("Small image representing the unit class")]
    public Texture icon;
    [Tooltip("Order in which the unit classes are displayed, lower number means higher priority")]
    public int displayPriority;
}
