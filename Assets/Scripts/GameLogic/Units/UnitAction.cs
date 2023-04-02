using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class UnitAction : MonoBehaviour
{
    [Tooltip("Distinct icon representing the action")]
    public Texture Icon;
    public string ActionName;
    [Tooltip("The description will be shown as a tooltip, when the player hovers over the action icon")]
    [TextArea(2, 5)]
    public string ActionDescription;
    [HideInInspector]
    public VisualElement element;


    public abstract void OnSelect(Unit unit);

    public abstract void OnDeselect(Unit unit);

    public abstract bool IsAvailable(Unit unit);

    public abstract void ActionUpdate();

    public void Click(Unit unit, VisualElement element)
    {
        this.element = element;
        if (IsAvailable(unit)) {
            if (!selected(unit))
            {
                if (unit.action != null) unit.action.OnDeselect(unit);
                unit.action = this;
                OnSelect(unit);                
            }
            else if (selected(unit))
            {
                OnDeselect(unit);
            }
        }
    }

    public bool selected(Unit unit)
    {
        return unit.action == this;
    }
}
