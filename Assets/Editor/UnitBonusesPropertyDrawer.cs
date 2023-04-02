using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnitClassBonuses))]
public class UnitBonusesPropertyDrawer : PropertyDrawer
{
    private Material spriteMaterial;
    private List<UnitClass> _classes = new List<UnitClass>();
    private List<float> _bonuses = new List<float>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        List<UnitClassBonus> classBonuses = (property.GetUnderlyingValue() as UnitClassBonuses).bonuses;
        if (_classes.Count == 0)
        {
            GetUnitClasses();
            foreach (UnitClassBonus bonus in classBonuses)
            {
                int i = _classes.FindIndex(r => r == bonus.unitClass);
                _bonuses[i] = bonus.bonus;
            }
        }
        if (spriteMaterial == null)
        {
            spriteMaterial = new Material(Shader.Find("Unlit/Transparent"));
        }
        Draw(position, label, property);
        List<UnitClassBonus> classBonusesResult = new List<UnitClassBonus>();
        for (int i = 0; i < _classes.Count; i++)
        {
            UnitClassBonus unitClassBonus = new UnitClassBonus();
            unitClassBonus.unitClass = _classes[i];
            unitClassBonus.bonus = _bonuses[i];
            classBonusesResult.Add(unitClassBonus);
        }

        UnitClassBonuses result = new UnitClassBonuses();
        result.bonuses = classBonusesResult;
        property.SetUnderlyingValue(result);
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }


    private void Draw(Rect position, GUIContent label, SerializedProperty property)
    {
        int offset = 0;
        //GUIStyle headerStyle = new GUIStyle();
        //headerStyle.fontSize = 14;
        //headerStyle.normal.textColor = Color.white;
        GUIContent costLabel = new GUIContent("Unit Class Bonuses", "Bonuses against different unit classes (0.2 means +20% bonus and -0.3 30% penalty");
        EditorGUI.LabelField(new Rect(position.min.x, position.min.y + 10, position.width, 14), costLabel);
        for (int i = 0; i < _classes.Count; i++)
        {
            UnitClass unitClass = _classes[i];
            Rect rectUnitBonus = new Rect(position.min.x + offset * 40, position.min.y + 30, 40, 40);
            Rect classIconRect = new Rect(rectUnitBonus.min.x + 4, rectUnitBonus.min.y, 24, 24);
            Rect valueRect = new Rect(rectUnitBonus.min.x, rectUnitBonus.min.y + 30, 32, 20);
            EditorGUI.DrawPreviewTexture(classIconRect, unitClass.icon, spriteMaterial, ScaleMode.StretchToFill, 0, -1);
            EditorGUI.LabelField(classIconRect, new GUIContent("", unitClass.typeName));
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
        string[] guids = AssetDatabase.FindAssets(String.Format("t:{0}", typeof(UnitClass)));
        for (int i = 0; i < guids.Length; i++)
        {
            _classes.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(UnitClass)) as UnitClass);
        }
        _bonuses.Clear();
        for (int i = 0; i < _classes.Count; i++)
        {
            _bonuses.Add(0.0f);
        }
        _classes.Sort((x, y) => x.displayPriority.CompareTo(y.displayPriority));
    }
}
