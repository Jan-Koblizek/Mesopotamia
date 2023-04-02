using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitDefinition))]
public class UnitDefinitionEditor : Editor
{
    private Texture attackIcon;
    private Texture armorIcon;
    private Texture rangeIcon;
    private Texture healthIcon;
    private Texture movementIcon;

    public void OnEnable()
    {
        attackIcon = Resources.Load<Texture>("Textures/swordIcon");
        armorIcon = Resources.Load<Texture>("Textures/shieldIcon");
        rangeIcon = Resources.Load<Texture>("Textures/rangeIcon");
        healthIcon = Resources.Load<Texture>("Textures/healthIcon");
        movementIcon = Resources.Load<Texture>("Textures/movementIcon");
    }

    private Texture2D MakeColorTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public override void OnInspectorGUI()
    {
        UnitDefinition unitDefinition = (UnitDefinition)target;
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        GUIStyleState boxStyleState = boxStyle.normal;
        boxStyleState.scaledBackgrounds = null;
        boxStyleState.background = MakeColorTex(2, 2, new Color(0f, 0f, 0f, 0.0f));
        boxStyle.normal = boxStyleState;
        boxStyle.focused = boxStyleState;
        boxStyle.active = boxStyleState;
        boxStyle.hover = boxStyleState;
        boxStyle.onNormal = boxStyleState;
        boxStyle.onFocused = boxStyleState;
        boxStyle.onActive = boxStyleState;
        boxStyle.onHover = boxStyleState;

        EditorGUILayout.LabelField("Recruitment", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Icon", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"), GUIContent.none,
                                    GUILayout.MaxWidth(100), GUILayout.MinWidth(20));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unit Prefab", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"), GUIContent.none,
                                    GUILayout.MaxWidth(100), GUILayout.MinWidth(20));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unit Name", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unitName"), GUIContent.none,
                                    GUILayout.MaxWidth(100), GUILayout.MinWidth(20));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Description", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "Description displayed on the unit recruitment panel (short description of the unit's strengths and weaknesses"), boxStyle);
        EditorGUILayout.BeginVertical(GUILayout.Height(60));
        EditorStyles.textField.wordWrap = true;
        unitDefinition.description = EditorGUILayout.TextArea(unitDefinition.description, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cost"));

        EditorGUILayout.Space(10);






        EditorGUILayout.LabelField("Combat", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unit Class", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "The class of this unit - units will gain bunuses or receive penalties based on the classes of the units they are fighting."), boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unitClass"), GUIContent.none,
                                    GUILayout.MaxWidth(100), GUILayout.MinWidth(20));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Is Military", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "Military units are capable of attacking and responding to attacks from other units"), boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canAttack"), GUIContent.none,
                                    GUILayout.MaxWidth(100), GUILayout.MinWidth(20));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Health", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "The amount of damage the unit can take before it is killed"), boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"), GUIContent.none,
                                    GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
        Rect positionRect = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(positionRect.max.x + 5, positionRect.min.y, 16, 16), healthIcon);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Attack", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "The amount of HP this unit can take from an enemy in one attack"), boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseAttack"), GUIContent.none,
                                    GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
        positionRect = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(positionRect.max.x + 5, positionRect.min.y, 16, 16), attackIcon);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Armor", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "The amount of attack points the unit can cancel"), boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseArmor"), GUIContent.none,
                                    GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
        positionRect = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(positionRect.max.x + 5, positionRect.min.y, 16, 16), armorIcon);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Range", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "Distance over which the unit can attack (set to 1 for melee units)"), boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"), GUIContent.none,
                                    GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
        positionRect = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(positionRect.max.x + 5, positionRect.min.y, 16, 16), rangeIcon);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("unitClassBonusses"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("terrainBonusses"));

        EditorGUILayout.Space(10);






        EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Movement Per Turn", GUILayout.MinWidth(50), GUILayout.MaxWidth(120));
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "Movement points gained each turn used to travel across the map"), boxStyle);
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementPerTurn"), GUIContent.none, 
                                    GUILayout.MaxWidth(30), GUILayout.MinWidth(20));
        positionRect = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(positionRect.max.x + 5, positionRect.min.y, 16, 16), movementIcon);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementCosts"));

        EditorGUILayout.Space(10);




        EditorGUILayout.LabelField("Unit Actions", EditorStyles.boldLabel);
        GUI.Box(GUILayoutUtility.GetLastRect(), new GUIContent("", "Possible actions the unit can take (represented by prefabs with scripts assigned to them)"), boxStyle);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("availableActions"), GUIContent.none);
    }
}
