using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : UnitAction
{
    public override void ActionUpdate() { }

    public override bool IsAvailable(Unit unit)
    {
        if (unit.player != GameManager.instance.players[GameManager.instance.playerTurn]) return false;
        return Pathfinding.GetMovementPositions(unit).Count > 0;
    }

    public override void OnDeselect(Unit unit)
    {
        unit.action = null;
        unit.movementCommandSelected = false;

        element.style.borderBottomWidth = 0;
        element.style.borderTopWidth = 0;
        element.style.borderLeftWidth = 0;
        element.style.borderRightWidth = 0;
    }

    public override void OnSelect(Unit unit)
    {
        unit.action = this;
        unit.movementCommandSelected = true;

        element.style.borderBottomWidth = 5;
        element.style.borderTopWidth = 5;
        element.style.borderLeftWidth = 5;
        element.style.borderRightWidth = 5;
    }
}
