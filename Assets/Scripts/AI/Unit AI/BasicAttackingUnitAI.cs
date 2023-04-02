using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicAttackingUnitAI
{
    private float time = 0.0f;
    private List<Unit> units = new List<Unit>();
    private int unitID;

    public void TakeTurnUnit(Unit unit)
    {
        if (unit == null || unit.gameObject == null) return;
        unit.commands.Clear();
        if (enemiesToAttackExist(unit.player))
        {
            HexCell targetCell = Pathfinding.getClosestAttackPositions(unit, canMoveOutsideBounds: !(HexGrid.instance.CellInsideBounds(HexGrid.instance.GetCell(unit.coordinates))))[0];
            unit.commands.Enqueue(new Move(targetCell, unit));
            List<HexCell> neighbors = targetCell.GetNeighbors();
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].building != null && neighbors[i].building.buildingData.type == BuildingType.Ziggurat)
                {
                    unit.commands.Enqueue(new AttackCommand(neighbors[i].building.city));
                    break;
                }
            }
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i].unit != null && neighbors[i].unit.player != unit.player)
                {
                    unit.commands.Enqueue(new AttackCommand(neighbors[i].unit));
                    break;
                }
            }
        }
    }
    public void TakeTurn(List<Unit> unitsInit)
    {
        time = 0.0f;
        units.Clear();
        foreach (Unit unit in unitsInit)
        {
            units.Add(unit);
        }

        unitID = units.Count - 1;
        if (unitID >= 0)
            TakeTurnUnit(units[unitID]);
    }

    private bool ExecutionFinishedUnit() {
        if (units[unitID] == null || units[unitID].gameObject == null) return true;
        if (units[unitID].commands.Count > 0 && units[unitID].commands.Peek() != null && (!units[unitID].commands.Peek().finishedThisTurn || units[unitID].commands.Peek().finished))
        {
            return false;
        }
        return true;
    }

    private bool enemiesToAttackExist(Player player)
    {
        foreach (Player otherPlayer in GameManager.instance.players)
        {
            if (otherPlayer != player && (otherPlayer.units.Count > 0 || otherPlayer.cities.Count > 0)) return true;
        }
        return false;
    }

    public bool ExecutionFinished()
    {
        if (unitID < 0) return true;
        else
        {
            if (ExecutionFinishedUnit())
            {
                unitID--;
                if (unitID < 0) return true;
                else TakeTurnUnit(units[unitID]);
            }
        }
        return false;
    }

    private struct UnitAI
    {
        Unit unit;
        bool finished;

        public UnitAI(Unit unit, bool finished) : this()
        {
            this.unit = unit;
            this.finished = finished;
        }
    }
}
