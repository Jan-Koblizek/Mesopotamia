using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackCommand : UnitCommand
{
    private City targetCity;
    private Unit targetUnit;
    private HexCell targetCell;

    public AttackCommand(City targetCity)
    {
        this.targetCity = targetCity;
        this.targetUnit = null;
    }

    public AttackCommand(Unit targetUnit)
    {
        this.targetUnit = targetUnit;
        this.targetCity = null;
    }

    private bool _finished = false;
    private bool _finishedThisTurn = false;
    public override bool finished => _finished;

    public override bool finishedThisTurn => _finishedThisTurn;

    public override void Execute(Unit unit)
    {
        if (targetCity != null && targetCity.gameObject != null) targetCell = targetCity.ziggurat.cell;
        if (targetUnit != null && targetUnit.gameObject != null) targetCell = HexGrid.instance.GetCell(targetUnit.coordinates);
        if ((targetCity == null || targetCity.gameObject == null) && (targetUnit == null || targetUnit.gameObject == null))
        {
            _finished = true;
            _finishedThisTurn = true;
        }
        else if (!HexGrid.instance.GetCell(unit.coordinates).neighbors.Contains(targetCell))
        {
            _finished = true;
            _finishedThisTurn = true;
        }
        else if (unit.status == UnitStatus.Exhausted) _finishedThisTurn = true;
        else
        {
            if (targetUnit != null)
            {
                unit.Attack(targetUnit);
            }
            else if (targetCity != null)
            {
                unit.AttackCity(targetCity);
            }
            else
            {
                Debug.Log("impossible to end here");
            }
        }
    }

    public override void TurnEnd(Unit unit)
    {
        Execute(unit);
    }

    public override void TurnStart(Unit unit)
    {
        _finishedThisTurn = false;
    }
}
