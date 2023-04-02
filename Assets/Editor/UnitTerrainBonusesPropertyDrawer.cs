using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnitTerrainBonuses))]
public class UnitTerrainBonusesPropertyDrawer : PropertyDrawer
{
    private Material spriteMaterial;
    private List<TileType> _terrains = new List<TileType>();
    private List<float> _bonuses = new List<float>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        List<TerrainBonus> terrainBonuses = (property.GetUnderlyingValue() as UnitTerrainBonuses).bonuses;
        if (_terrains.Count == 0)
        {
            GetUnitClasses();
            foreach (TerrainBonus bonus in terrainBonuses)
            {
                int i = _terrains.FindIndex(r => r == bonus.terrain);
                _bonuses[i] = bonus.bonus;
            }
        }
        if (spriteMaterial == null)
        {
            spriteMaterial = new Material(Shader.Find("Unlit/Transparent"));
        }
        Draw(position, label, property);
        List<TerrainBonus> terrainBonusesResult = new List<TerrainBonus>();
        for (int i = 0; i < _terrains.Count; i++)
        {
            TerrainBonus terrainBonus = new TerrainBonus();
            terrainBonus.terrain = _terrains[i];
            terrainBonus.bonus = _bonuses[i];
            terrainBonusesResult.Add(terrainBonus);
        }

        UnitTerrainBonuses result = new UnitTerrainBonuses();
        result.bonuses = terrainBonusesResult;
        property.SetUnderlyingValue(result);
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }


    private void Draw(Rect position, GUIContent label, SerializedProperty property)
    {
        int offset = 0;
        //GUIStyle headerStyle = new GUIStyle();
        //headerStyle.fontSize = 14;
        //headerStyle.normal.textColor = Color.white;
        GUIContent costLabel = new GUIContent("Terrain Bonuses", "Bonuses for different terrain types (0.2 means +20% bonus and -0.3 30% penalty");
        EditorGUI.LabelField(new Rect(position.min.x, position.min.y + 10, position.width, 14), costLabel);
        for (int i = 0; i < _terrains.Count; i++)
        {
            TileType terrain = _terrains[i];
            Rect rectTerrainBonus = new Rect(position.min.x + offset * 40, position.min.y + 30, 40, 40);
            Rect terrainIconRect = new Rect(rectTerrainBonus.min.x + 4, rectTerrainBonus.min.y, 24, 24);
            Rect valueRect = new Rect(rectTerrainBonus.min.x, rectTerrainBonus.min.y + 30, 32, 20);
            EditorGUI.DrawPreviewTexture(terrainIconRect, terrain.icon, spriteMaterial, ScaleMode.StretchToFill, 0, -1);
            EditorGUI.LabelField(terrainIconRect, new GUIContent("", terrain.tileTypeName));
            _bonuses[i] = EditorGUI.FloatField(valueRect, _bonuses[i]);
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
        _bonuses.Clear();
        for (int i = 0; i < _terrains.Count; i++)
        {
            _bonuses.Add(0.0f);
        }
        _terrains.Sort((x, y) => x.displayPriority.CompareTo(y.displayPriority));
    }
}