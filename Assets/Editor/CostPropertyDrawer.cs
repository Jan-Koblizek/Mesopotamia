using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Cost))]
public class CostPropertyDrawer : PropertyDrawer
{
    private Material spriteMaterial;
    private List<Resource> _resources = new List<Resource>();
    private List<int> _resourceAmounts = new List<int>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Cost cost = (property.GetUnderlyingValue() as Cost);
        if (_resources.Count == 0)
        {
            GetResources();
            foreach (ResourceAmount resourceAmount in cost.resources)
            {
                int i = _resources.FindIndex(r => r == resourceAmount.resource);
                _resourceAmounts[i] = resourceAmount.Amount;
            }
        }
        if (spriteMaterial == null)
        {
            spriteMaterial = new Material(Shader.Find("UI/Default"));
        }
        Draw(position, label, property);
        List<ResourceAmount> resourcesResult = new List<ResourceAmount>();
        for (int i = 0; i < _resources.Count; i++)
        {
            if (_resourceAmounts[i] > 0)
            {
                ResourceAmount ra = new ResourceAmount();
                ra.resource = _resources[i];
                ra.Amount = _resourceAmounts[i];
                ra.Production = 0;
                ra.Consumption = 0;
                resourcesResult.Add(ra);
            }
        }
        Cost result = new Cost();
        result.resources = resourcesResult;
        property.SetUnderlyingValue(result);
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }


    private void Draw(Rect position, GUIContent label, SerializedProperty property)
    {
        int offset = 0;
        //GUIStyle headerStyle = new GUIStyle();
        //headerStyle.fontSize = 14;
        //headerStyle.normal.textColor = Color.white;
        GUIContent costLabel = new GUIContent("Cost");
        EditorGUI.LabelField(new Rect(position.min.x, position.min.y + 10, position.width, 14), costLabel);
        for (int i = 0; i < _resources.Count; i++)
        {
            Resource resource = _resources[i];
            Rect rectResource = new Rect(position.min.x + offset * 40, position.min.y + 30, 40, 40);
            Rect resourceIconRect = new Rect(rectResource.min.x + 4, rectResource.min.y, 24, 24);
            Rect valueRect = new Rect(rectResource.min.x, rectResource.min.y + 30, 32, 20);
            EditorGUI.DrawPreviewTexture(resourceIconRect, resource.icon.texture, spriteMaterial, ScaleMode.StretchToFill, 0, -1);
            EditorGUI.LabelField(resourceIconRect, new GUIContent("", resource.nameOfTheResource));
            _resourceAmounts[i] = EditorGUI.IntField(valueRect, _resourceAmounts[i]);
            offset++;
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 80;
    }

    private void GetResources()
    {
        string[] guids = AssetDatabase.FindAssets(String.Format("t:{0}", typeof(Resource)));
        for (int i = 0; i < guids.Length; i++)
        {
            _resources.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), typeof(Resource)) as Resource);
        }
        _resourceAmounts.Clear();
        for (int i = 0; i < _resources.Count; i++)
        {
            _resourceAmounts.Add(0);
        }
        _resources.Sort((x, y) => x.displayPriority.CompareTo(y.displayPriority));
    }
}