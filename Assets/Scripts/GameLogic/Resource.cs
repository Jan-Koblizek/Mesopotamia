using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resource")]
public class Resource : ScriptableObject
{
    public string nameOfTheResource;
    public Sprite icon;
    [Tooltip("Order in which the resources are displayed, lower number means higher priority")]
    public int displayPriority;
}

[System.Serializable]
public class ResourceAmount
{
    public Resource resource;
    public int Amount
    {
        get { return amount; }
        set
        {
            amount = value;
            if (label != null)
                label.text = stringValue();
        }
    }
    public int Production
    {
        get { return production; }
        set {
            production = value;
            if (label != null)
                label.text = stringValue();
        }
    }
    public int Consumption
    {
        get { return consumption; }
        set
        {
            consumption = value;
            if (label != null)
                label.text = stringValue();
        }
    }

    [SerializeField]
    private int amount;
    [SerializeField]
    private int production;
    [SerializeField]
    private int consumption;

    private Label label = null;

    private string stringValue()
    {
        int netIncome = production - consumption;
        return $"{amount}({(netIncome >= 0 ? "+" : "")}{netIncome})";
    }

    public void LinkToLabel(Label label)
    {
        this.label = label;
        label.text = stringValue();
    }
}

[System.Serializable]
public class Cost
{
    [SerializeField]
    public List<ResourceAmount> resources;
}