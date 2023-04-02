using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Attack : UnitAction
{
    private List<Unit> targetUnits = new List<Unit>();
    private List<HexCell> attackPositions = new List<HexCell>();
    private Unit commandedUnit;
    private Unit target;
    private bool movementStarted = false;
    private Vector3 mouseDownCoords;

    public override bool IsAvailable(Unit unit)
    {
        if (unit.player != GameManager.instance.players[GameManager.instance.playerTurn]) return false;
        if (unit.status != UnitStatus.Exhausted)
        {
            foreach (Player player in GameManager.instance.players)
            {
                if (player != unit.player)
                {
                    foreach (Unit u in player.units)
                    {
                        if (Pathfinding.CanReachEnemyUnitAttack(unit, u))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public void AttackClicked(Unit attacked)
    {
        target = attacked;
        DisplayAttackPositions();
        foreach (Unit u in targetUnits)
        {
            UIManager.instance.unitOverheadManager.HideAttackIcon(u);
        }
    }

    private void DisplayAttackPositions()
    {
        attackPositions.Clear();
        foreach (HexCell hexCell in commandedUnit.movementPositions)
        {
            if (hexCell.coordinates.DistanceDirect(target.coordinates) <= commandedUnit.unitSpecification.attackRange && 
                (
                commandedUnit.unitSpecification.attackRange > 1 || 
                HexGrid.instance.GetCell(target.coordinates).neighbors.Contains(hexCell)
                ))
            {
                attackPositions.Add(hexCell);
            }
        }

        HexGrid.instance.ClearHighlights();
        foreach (HexCell highlight in attackPositions)
            HexGrid.instance.Highlight(highlight);
    }

    public override void OnDeselect(Unit unit)
    {
        unit.action = null;
        unit.displayMovementHighlight = true;

        foreach (Unit u in targetUnits)
        {
            UIManager.instance.unitOverheadManager.HideAttackIcon(u);
        }

        element.style.borderBottomWidth = 0;
        element.style.borderTopWidth = 0;
        element.style.borderLeftWidth = 0;
        element.style.borderRightWidth = 0;
    }

    public override void OnSelect(Unit unit)
    {
        target = null;
        movementStarted = false;
        unit.action = this;
        unit.displayMovementHighlight = false;
        commandedUnit = unit;
        attackPositions.Clear();

        foreach (Player player in GameManager.instance.players)
        {
            if (player != unit.player)
            {
                foreach (Unit u in player.units)
                {
                    if (Pathfinding.CanReachEnemyUnit(unit, u))
                    {
                        targetUnits.Add(u);
                    }
                }
            }
        }

        foreach (Unit u in targetUnits)
        {
            UIManager.instance.unitOverheadManager.ShowAttackIconCommand(u, unit, this);
        }

        element.style.borderBottomWidth = 5;
        element.style.borderTopWidth = 5;
        element.style.borderLeftWidth = 5;
        element.style.borderRightWidth = 5;
    }

    public override void ActionUpdate()
    {
        if (!commandedUnit.moving)
        {
            if (target != null &&
                movementStarted &&
                HexGrid.instance.GetCell(target.coordinates).neighbors.Contains(HexGrid.instance.GetCell(commandedUnit.coordinates)))
            {
                commandedUnit.Attack(target);
                OnDeselect(commandedUnit);
            }
            else
            {
                if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
                {
                    mouseDownCoords = Input.mousePosition;
                }
                if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && Vector3.Distance(Input.mousePosition, mouseDownCoords) < 50)
                {
                    HexCell selectedCell = MovementPositionSelection.SelectMovementPosition();
                    if (selectedCell != null)
                    {
                        commandedUnit.Move(selectedCell);
                        movementStarted = true;
                    }
                }
            }
        }
    }
}
