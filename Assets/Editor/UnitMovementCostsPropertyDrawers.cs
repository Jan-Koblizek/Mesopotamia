using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnitMovementCosts))]
public class UnitMovementCostsPropertyDrawer : PropertyDrawer
{
    private Material spriteMaterial;
    private List<TileType> _terrains = new List<TileType>();
    private List<int> _movementCosts = new List<int>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        List<MovementCost> terrainBonuses = (property.GetUnderlyingValue() as UnitMovementCosts).costs;
        if (_terrains.Count == 0)
        {
            GetUnitClasses();
            foreach (MovementCost movementCost in terrainBonuses)
            {
                int i = _terrains.FindIndex(r => r == movementCost.terrain);
                _movementCosts[i] = movementCost.movementCost;
            }
        }
        if (spriteMaterial == null)
        {
            spriteMaterial = new Material(Shader.Find("Unlit/Transparent"));
        }
        Draw(position, label, property);
        List<MovementCost> movementCostsResult = new List<MovementCost>();
        for (int i = 0; i < _terrains.Count; i++)
        {
            if (_movementCosts[i] > 0)
            {
                MovementCost movementCost = new MovementCost();
                movementCost.terrain = _terrains[i];
                movementCost.movementCost = _movementCosts[i];
                movementCostsResult.Add(movementCost);
            }
        }

        UnitMovementCosts result = new UnitMovementCosts();
        result.costs = movementCostsResult;
        property.SetUnderlyingValue(result);
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }


    private void Draw(Rect position, GUIContent label, SerializedProperty property)
    {
        int offset = 0;
        //GUIStyle headerStyle = new GUIStyle();
        //headerStyle.fontSize = 14;
        //headerStyle.normal.textColor = Color.white;
        GUIContent costLabel = new GUIContent("Terrain Movement Costs", "Cost of movement on different tile types. movement cost of 0 means that the movement is impossible.");
        EditorGUI.LabelField(new Rect(position.min.x, position.min.y + 10, position.width, 14), costLabel);
        for (int i = 0; i < _terrains.Count; i++)
        {
            TileType terrain = _terrains[i];
            Rect rectMovementCost = new Rect(position.min.x + offset * 40, position.min.y + 30, 40, 40);
            Rect terrainIconRect = new Rect(rectMovementCost.min.x + 4, rectMovementCost.min.y, 24, 24);
            Rect valueRect = new Rect(rectMovementCost.min.x, rectMovementCost.min.y + 30, 32, 20);
            EditorGUI.DrawPreviewTexture(terrainIconRect, terrain.icon, spriteMaterial, ScaleMode.StretchToFill, 0, -1);
            EditorGUI.LabelField(terrainIconRect, new GUIContent("", terrain.tileTypeName));
            _movementCosts[i] = EditorGUI.IntField(valueRect, _movementCosts[i]);
            offset++;
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 80;
    }

    private void GetUnitClasses()
    {
        string[] guids = AssetDatabase.FindAssets(String.Format("t:{0}", typeof(TileType)));
        for (int i = 0; i < guids.Length; i++)
        {
            TileType tileType = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(TileType)) as TileType;
            if (tileType == null || tileType.tileTypeName == "None") continue;
            _terrains.Add(tileType);
        }
        _movementCosts.Clear();
        for (int i = 0; i < _terrains.Count; i++)
        {
            _movementCosts.Add(0);
        }
        _terrains.Sort((x, y) => x.displayPriority.CompareTo(y.displayPriority));
    }
}