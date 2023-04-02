using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : UnitCommand
{
    private Vector3 movementPosition;
    private Stack<HexCell> path;

    private HexCell lastGoalPosition;
    private HexCell movementEndCell;
    private HexCell goal;

    private int thisTurn;

    private bool _finished;
    public override bool finished => _finished;

    private bool _finishedThisTurn;
    public override bool finishedThisTurn => _finishedThisTurn;

    public Move(HexCell goal, Unit unit)
    {
        this.goal = goal;
        movementEndCell = HexGrid.instance.GetCell(unit.coordinates);
        RecomputePath(goal, unit);
        if (path != null && path.Count > 0)
        {
            StartMovement(unit);
            PathVisualizer.instance.RemovePathVisualization();
            if (unit.movementCommandSelected)
            {
                unit.action.OnDeselect(unit);
            }
        }
        else
        {
            _finished = true;
            _finishedThisTurn = true;
        }
    }

    private void RecomputePath(HexCell goal, Unit unit)
    {
        if (goal.unit != null && goal.unit != unit) path = null;
        else
        {
            path = Pathfinding.GetPathTo(unit, goal, out _, !(HexGrid.instance.CellInsideBounds(HexGrid.instance.GetCell(unit.coordinates))), false);
            if (path == null) _finished = true;
            if (path != null && path.Count > 0)
            {
                if (unit.selected)
                {
                    unit.UpdateHighlights();
                }
                lastGoalPosition = null;
                movementPosition = unit.transform.position;
            }
        }
    }

    public override void Execute(Unit unit)
    {
        if (Vector3.Distance(movementPosition, unit.transform.position) > 0.5f)
        {
            if (unit.player != GameManager.instance.thisPlayer)
                unit.transform.position += (movementPosition - unit.transform.position).normalized * 50 * Time.deltaTime;
            else
                unit.transform.position += (movementPosition - unit.transform.position).normalized * 10 * Time.deltaTime;
        }
        else
        {
            unit.UpdateStatus();
            if (path != null && path.Count > 0)
            {
                if (HexGrid.instance.GetCell(unit.coordinates) == movementEndCell)
                {
                    _finishedThisTurn = true;
                    unit.moving = false;
                    if (unit.selected)
                    {
                        unit.UpdateHighlights();
                    }
                    return;
                }
                HexCell nextCell = path.Peek();
                if (unit.movement - unit.GetMovementCost(nextCell.TileType) >= 0)
                {
                    unit.coordinates = nextCell.coordinates;
                    unit.movement = unit.movement - unit.GetMovementCost(nextCell.TileType);
                    movementPosition = path.Pop().transform.position;
                }
                else
                {
                    _finishedThisTurn = true;
                    unit.moving = false;
                    if (unit.selected)
                    {
                        unit.UpdateHighlights();
                    }
                }
            }
            else
            {
                unit.moving = false;
                _finished = true;
                if (unit.selected)
                {
                    unit.UpdateHighlights();
                }
            }
        }
    }

    public override void TurnStart(Unit unit)
    {
        _finishedThisTurn = false;
        unit.moving = true;
        RecomputePath(goal, unit);
        if (path != null && path.Count > 0)
        {
            StartMovement(unit);
        }
        else
        {
            _finishedThisTurn = true;
            _finished = true;
        }
    }

    public override void TurnEnd(Unit unit)
    {
        if (HexGrid.instance.GetCell(unit.coordinates) == movementEndCell)
        {
            _finishedThisTurn = true;
            unit.moving = false;
            unit.transform.position = movementPosition;
            return;
        }
        while (!_finishedThisTurn && path != null && path.Count > 0)
        {
            HexCell nextCell = path.Peek();
            if (nextCell == movementEndCell)
            {
                _finishedThisTurn = true;
                unit.moving = false;
            }
            if (unit.movement - unit.GetMovementCost(nextCell.TileType) >= 0)
            {
                unit.coordinates = nextCell.coordinates;
                unit.movement = unit.movement - unit.GetMovementCost(nextCell.TileType);
                unit.transform.position = path.Pop().transform.position;
                movementPosition = unit.transform.position;
            }
        }
    }

    private void StartMovement(Unit unit)
    {
        Stack<HexCell>.Enumerator enumerator = path.GetEnumerator();
        int enumeratorMovement = unit.movement;
        movementEndCell = HexGrid.instance.GetCell(unit.coordinates);
        enumerator.MoveNext();
        while (true)
        {
            enumeratorMovement -= unit.unitSpecification.GetMovementCost(enumerator.Current.TileType);
            if (enumeratorMovement >= 0)
            {
                if (enumerator.Current.unit == null || enumerator.Current.unit == unit)
                    movementEndCell = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        if (movementEndCell.unit != null && movementEndCell.unit != unit)
        {
            path = null;
            unit.moving = false;
            //Move(goal);
        }
        else
        {
            HexGrid.instance.GetCell(unit.coordinates).unit = null;
            movementEndCell.unit = unit;
            unit.moving = true;
        }
    }
}
