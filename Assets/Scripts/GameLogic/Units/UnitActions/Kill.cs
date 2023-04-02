using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : UnitAction
{
    public override void ActionUpdate() { }

    public override bool IsAvailable(Unit unit)
    {
        if (unit.player != GameManager.instance.players[GameManager.instance.playerTurn]) return false;
        return true;
    }

    public void OnActivate(Unit unit)
    {
        unit.Death();
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
