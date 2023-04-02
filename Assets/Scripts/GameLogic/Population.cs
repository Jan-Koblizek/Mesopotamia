using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Population
{
    public Sprite icon;
    private Label label = null;
    [SerializeField]
    private int limit;
    [SerializeField]
    private int amount;

    public int Amount
    {
        get { return amount; }
        set
        {
            amount = value;
            if (label != null) label.text = stringValue();
        }
    }
    public int Limit
    {
        get { return limit; }
        set
        {
            limit = value;
            if (label != null) label.text = stringValue();
        }
    }

    private string stringValue()
    {
        return $"{amount}/{limit}";
    }

    public void LinkToLabel(Label label)
    {
        this.label = label;
        label.text = stringValue();
    }
}
