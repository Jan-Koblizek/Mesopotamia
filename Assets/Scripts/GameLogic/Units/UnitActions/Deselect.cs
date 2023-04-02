using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Deselect : UnitAction
{
    public override void ActionUpdate() { }

    public override bool IsAvailable(Unit unit)
    {
        if (unit.player != GameManager.instance.players[GameManager.instance.playerTurn]) return false;
        return true;
    }

    public void OnActivate(Unit unit)
    {
        unit.Deselect();
        OnDeselect(unit);
    }

    public override void OnDeselect(Unit unit)
    {
        unit.action = null;
    }

    public override void OnSelect(Unit unit)
    {
        unit.action = this;
        OnActivate(unit);
    }
}
