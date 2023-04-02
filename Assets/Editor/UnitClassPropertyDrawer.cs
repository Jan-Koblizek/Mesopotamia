using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnitClass))]
public class UnitClassPropertyDrawer : PropertyDrawer
{
    private readonly GenericMenu.MenuFunction2 _onSelected;
    private GUIContent _popupContent = new GUIContent();

    private readonly int _controlHint = typeof(UnitClass).GetHashCode();
    private UnitClass _selectedClass;
    private List<UnitClass> _unitClasses = new List<UnitClass>();
    private int _selectedControlID;
    private bool _isChanged;

    public UnitClassPropertyDrawer()
    {
        _onSelected = OnSelected;

        EditorApplication.projectChanged += _unitClasses.Clear;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 20;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_unitClasses.Count == 0)
        {
            GetObjects();
        }

        Draw(position, label, property);
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }


    private void Draw(Rect position, GUIContent label,
    SerializedProperty property)
    {
        if (label != null && label != GUIContent.none)
            position = EditorGUI.PrefixLabel(position, label);

        UpdateSelectionControl(position, label, property);
    }

    private void UpdateSelectionControl(Rect position, GUIContent label,
    SerializedProperty property)
    {
        UnitClass output = DrawSelectionControl(new Rect(position.x, position.y, position.width, 20), label, property.objectReferenceValue as UnitClass);
        if (_isChanged)
        {
            _isChanged = false;
            property.objectReferenceValue = output;
        }
    }

    private UnitClass DrawSelectionControl(Rect position, GUIContent label,
    UnitClass unitClass)
    {
        bool triggerDropDown = false;
        int controlID = GUIUtility.GetControlID(_controlHint, FocusType.Keyboard, position);

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.ExecuteCommand:
                if (Event.current.commandName == "ReferenceUpdated")
                {
                    if (_selectedControlID == controlID)
                    {
                        if (unitClass != _selectedClass)
                        {
                            unitClass = _selectedClass;
                            _isChanged = true;
                        }

                        _selectedControlID = 0;
                        _selectedClass = null;
                    }
                }
                break;

            case EventType.MouseDown:
                if (GUI.enabled && position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.keyboardControl = controlID;
                    triggerDropDown = true;
                    Event.current.Use();
                }
                break;

            case EventType.KeyDown:
                if (GUI.enabled && GUIUtility.keyboardControl == controlID)
                {
                    if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space)
                    {
                        triggerDropDown = true;
                        Event.current.Use();
                    }
                }
                break;

            case EventType.Repaint:
                if (unitClass == null)
                {
                    _popupContent.text = "Nothing";
                }
                else
                {
                    _popupContent.text = unitClass.name;
                }
                EditorStyles.popup.Draw(position, _popupContent, controlID);
                break;
        }

        if (_unitClasses.Count != 0 && triggerDropDown)
        {
            _selectedControlID = controlID;
            _selectedClass = unitClass;

            DisplayDropDown(position, unitClass);
        }
        return unitClass;
    }

    private void DisplayDropDown(Rect position, UnitClass selectedClass)
    {
        var menu = new GenericMenu();

        for (int i = 0; i < _unitClasses.Count; ++i)
        {
            UnitClass unitClass = _unitClasses[i];

            string menuLabel = _unitClasses[i].name;
            if (string.IsNullOrEmpty(menuLabel))
                continue;

            var content = new GUIContent(menuLabel);
            menu.AddItem(content, unitClass == selectedClass, _onSelected, unitClass);
        }

        menu.DropDown(position);
    }

    private void OnSelected(object userData)
    {
        _selectedClass = userData as UnitClass;
        var ReferenceUpdatedEvent = EditorGUIUtility.CommandEvent("ReferenceUpdated");
        EditorWindow.focusedWindow.SendEvent(ReferenceUpdatedEvent);
    }

    private void GetObjects()
    {
        string[] guids = AssetDatabase.FindAssets(String.Format("t:{0}", typeof(UnitClass)));
        for (int i = 0; i < guids.Length; i++)
        {
            _unitClasses.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(UnitClass)) as UnitClass);
        }
    }
}
