using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class Pathfinding
{
    public static List<HexCell> GetMovementPositions(Unit unit, bool canMoveOutsideBounds = false, bool canMoveOverCliffs = false)
    {
        int movementPoints = unit.movement;
        Dictionary<HexCell, PathfindingCellInfo> cellMap = new Dictionary<HexCell, PathfindingCellInfo>();
        PriorityQueue<int, PathfindingCellInfo> queue = new PriorityQueue<int, PathfindingCellInfo>();
        List<HexCell> movementPositions = new List<HexCell>();

        HexCell startCell = HexGrid.instance.GetCell(unit.coordinates);
        PathfindingCellInfo startCellInfo = new PathfindingCellInfo(0, PathfindingCellStatus.open, startCell);
        queue.Enqueue(0, startCellInfo);
        cellMap[HexGrid.instance.GetCell(unit.coordinates)] = startCellInfo;

        while (queue.Count() != 0)
        {
            PathfindingCellInfo processedCell = queue.Dequeue().item;
            if ((processedCell.cell.building != null && processedCell.cell.building.buildingData.type == BuildingType.Ziggurat) ||
                (processedCell.cell.unit != null && processedCell.cell.unit.player != unit.player)) continue;

            if (processedCell.distance < unit.movement)
            {
                List<HexCell> neighbors = processedCell.cell.GetNeighbors(!canMoveOverCliffs);

                foreach (HexCell neighbor in neighbors)
                {
                    if (neighbor == null ||
                        (!HexGrid.instance.CellInsideBounds(neighbor) && !canMoveOutsideBounds)) continue;
                    int movementCost = unit.GetMovementCost(neighbor.TileType);
                    if (movementCost > 1000) continue;
                    bool shouldAdd = false;
                    if (!cellMap.ContainsKey(neighbor))
                    {
                        cellMap[neighbor] = new PathfindingCellInfo(movementCost + processedCell.distance, PathfindingCellStatus.open, neighbor);
                        shouldAdd = true;
                    }
                    else if (cellMap[neighbor].status == PathfindingCellStatus.open && cellMap[neighbor].distance > movementCost + processedCell.distance)
                    {
                        cellMap[neighbor].distance = movementCost + processedCell.distance;
                        shouldAdd = true;
                    }

                    if (shouldAdd)
                    {
                        cellMap[neighbor].predecessor = processedCell;
                        queue.Enqueue(cellMap[neighbor].distance, cellMap[neighbor]);
                    }
                }
            }
            
            if (processedCell.distance <= unit.movement && 
                (processedCell.cell.unit == null || processedCell.cell.unit == unit))
            {
                movementPositions.Add(processedCell.cell);
            }

            processedCell.status = PathfindingCellStatus.closed;
        }

        return movementPositions;
    }

    public static List<HexCell> getClosestAttackPositions(Unit unit, bool includeCities = true, bool canMoveOutsideBounds = false, bool canMoveOverCliffs = false)
    {
        int movementPoints = unit.movement;
        Dictionary<HexCell, PathfindingCellInfo> cellMap = new Dictionary<HexCell, PathfindingCellInfo>();
        PriorityQueue<int, PathfindingCellInfo> queue = new PriorityQueue<int, PathfindingCellInfo>();
        PriorityQueue<int, PathfindingCellInfo> queue2 = new PriorityQueue<int, PathfindingCellInfo>();
        List<HexCell> attackPositions = new List<HexCell>();

        HexCell startCell = HexGrid.instance.GetCell(unit.coordinates);
        PathfindingCellInfo startCellInfo = new PathfindingCellInfo(0, PathfindingCellStatus.open, startCell);
        queue.Enqueue(0, startCellInfo);
        cellMap[HexGrid.instance.GetCell(unit.coordinates)] = startCellInfo;

        while (attackPositions.Count == 0)
        {
            while (queue.Count() != 0)
            {
                PathfindingCellInfo processedCell = queue.Dequeue().item;
                bool processedCellFree = (processedCell.cell.unit == null || processedCell.cell.unit == unit) &&
                            (processedCell.cell.building == null || processedCell.cell.building.buildingData.type != BuildingType.Ziggurat);

                if ((processedCell.cell.building != null && processedCell.cell.building.buildingData.type == BuildingType.Ziggurat) ||
                    (processedCell.cell.unit != null && processedCell.cell.unit.player != unit.player)) continue;
                List<HexCell> neighbors = processedCell.cell.GetNeighbors(!canMoveOverCliffs);

                foreach (HexCell neighbor in neighbors)
                {
                    if (neighbor == null ||
                        (!HexGrid.instance.CellInsideBounds(neighbor) && !canMoveOutsideBounds)) continue;

                    if (neighbor.unit != null && neighbor.unit.player != unit.player && 
                        !attackPositions.Contains(processedCell.cell) && processedCellFree)
                    {
                        attackPositions.Add(processedCell.cell);
                    }
                    if (neighbor.building != null && neighbor.building.buildingData.type == BuildingType.Ziggurat && 
                        !attackPositions.Contains(processedCell.cell) && processedCellFree)
                    {
                        attackPositions.Add(processedCell.cell);
                    }

                    int movementCost = unit.GetMovementCost(neighbor.TileType);
                    if (movementCost > 1000) continue;
                    int distance = movementCost + processedCell.distance;
                    if (distance > movementPoints) distance = movementPoints + movementCost;


                    bool shouldAdd = false;
                    if (!cellMap.ContainsKey(neighbor))
                    {
                        if (distance <= movementPoints)
                        {
                            cellMap[neighbor] = new PathfindingCellInfo(distance, PathfindingCellStatus.open, neighbor);
                            shouldAdd = true;
                        }
                        else if (processedCellFree)
                        {
                            cellMap[neighbor] = new PathfindingCellInfo(distance, PathfindingCellStatus.open, neighbor);
                            shouldAdd = true;
                        }
                    }
                    else if (cellMap[neighbor].status == PathfindingCellStatus.open && cellMap[neighbor].distance > movementCost + processedCell.distance)
                    {
                        if (distance <= movementPoints)
                        {
                            cellMap[neighbor].distance = distance;
                            shouldAdd = true;
                        }
                        else if (processedCellFree) {
                            cellMap[neighbor].distance = distance;
                            shouldAdd = true;
                        }
                    }

                    if (shouldAdd)
                    {
                        cellMap[neighbor].predecessor = processedCell;
                        if (cellMap[neighbor].distance <= movementPoints) queue.Enqueue(cellMap[neighbor].distance, cellMap[neighbor]);
                        else queue2.Enqueue(cellMap[neighbor].distance, cellMap[neighbor]);
                    }
                }
                processedCell.status = PathfindingCellStatus.closed;
            }
            movementPoints += unit.unitSpecification.movementPerTurn;
            if (queue2.Count() == 0) break;
            queue = queue2;
            queue2 = new PriorityQueue<int, PathfindingCellInfo>();
        }
        return attackPositions;
    }



    public static bool CanMoveToPositionOneTurn(Unit unit, HexCell position)
    {
        List<HexCell> movementPositions = GetMovementPositions(unit);
        if (movementPositions.Contains(position)) return true;
        return false;
    }

    public static Stack<HexCell> GetPathTo(Unit unit, HexCell goal, out int length, bool canMoveOutsideBounds = false, bool canMoveOverCliffs = false)
    {
        int movementPerTurn = unit.unitSpecification.movementPerTurn;
        length = 0;
        int movementPoints = unit.movement;
        Dictionary<HexCell, PathfindingCellInfo> cellMap = new Dictionary<HexCell, PathfindingCellInfo>();
        PriorityQueue<int, PathfindingCellInfo> queue = new PriorityQueue<int, PathfindingCellInfo>();

        HexCell startCell = HexGrid.instance.GetCell(unit.coordinates);
        PathfindingCellInfo startCellInfo = new PathfindingCellInfo(0, PathfindingCellStatus.open, startCell);
        queue.Enqueue(movementPerTurn - movementPoints, startCellInfo);
        cellMap[HexGrid.instance.GetCell(unit.coordinates)] = startCellInfo;

        while (queue.Count() != 0)
        {
            PathfindingCellInfo processedCell = queue.Dequeue().item;
            if (processedCell.cell == goal)
            {
                Stack<HexCell> result = new Stack<HexCell>();
                HexCell currentCell = processedCell.cell;
                while (currentCell != startCell)
                {
                    result.Push(currentCell);
                    currentCell = cellMap[currentCell].predecessor.cell;
                }
                length = processedCell.distance;
                return result;
            }

            if (processedCell.distance % movementPerTurn == 0 &&
                processedCell.cell.unit != null &&
                processedCell.cell.unit != unit) continue;

            List<HexCell> neighbors = processedCell.cell.GetNeighbors(!canMoveOverCliffs);

            foreach (HexCell neighbor in neighbors)
            {
                if (neighbor == null) continue;
                int movementCost = unit.GetMovementCost(neighbor.TileType);
                if (movementCost > 1000 ||
                    (neighbor.building != null && neighbor.building.buildingData.type == BuildingType.Ziggurat) ||
                    (neighbor.unit != null && unit.player != neighbor.unit.player) ||
                    (!HexGrid.instance.CellInsideBounds(neighbor) && !canMoveOutsideBounds)) continue;
                if (movementCost + processedCell.distance % movementPerTurn > movementPerTurn)
                {
                    if (processedCell.cell.unit != null && processedCell.cell.unit != unit) continue;
                    movementCost = movementCost + movementPerTurn - processedCell.distance % movementPerTurn;
                }
                bool shouldAdd = false;
                if (!cellMap.ContainsKey(neighbor))
                {
                    cellMap[neighbor] = new PathfindingCellInfo(movementCost + processedCell.distance, PathfindingCellStatus.open, neighbor);
                    shouldAdd = true;
                }
                else if (cellMap[neighbor].status == PathfindingCellStatus.open && cellMap[neighbor].distance > movementCost + processedCell.distance)
                {
                    cellMap[neighbor].distance = movementCost + processedCell.distance;
                    shouldAdd = true;
                }

                if (shouldAdd)
                {
                    cellMap[neighbor].predecessor = processedCell;
                    queue.Enqueue(cellMap[neighbor].distance, cellMap[neighbor]);
                }
            }
            processedCell.status = PathfindingCellStatus.closed;
        }
        return null;
    }

    public static bool CanReachEnemyUnit(Unit unit, Unit enemy)
    {
        if (enemy == null) return false;
        if (unit.coordinates.DistanceDirect(enemy.coordinates) > unit.movement + 1) return false;
        
        HexCell unitCell = HexGrid.instance.GetCell(unit.coordinates);
        HexCell enemyCell = HexGrid.instance.GetCell(enemy.coordinates);

        foreach (HexCell cell in unit.movementPositions)
        {
            if (cell.coordinates.DistanceDirect(enemy.coordinates) <= 1)
            {
                if (enemyCell.neighbors.Contains(cell)) return true;
            }
        }

        return false;
    }

    public static bool CanReachEnemyUnitAttack(Unit unit, Unit enemy)
    {
        if (enemy == null) return false;
        if (unit.coordinates.DistanceDirect(enemy.coordinates) > unit.movement + unit.unitSpecification.attackRange) return false;

        HexCell unitCell = HexGrid.instance.GetCell(unit.coordinates);
        HexCell enemyCell = HexGrid.instance.GetCell(enemy.coordinates);

        foreach (HexCell cell in unit.movementPositions)
        {
            if (cell.coordinates.DistanceDirect(enemy.coordinates) <= unit.unitSpecification.attackRange)
            {
                if (enemyCell.neighbors.Contains(cell)) return true;
                else if (unit.unitSpecification.attackRange > 1) return true;
            }
        }

        return false;
    }
}


public class PathfindingCellInfo
{
    public int distance;
    public PathfindingCellStatus status;
    public HexCell cell;
    public PathfindingCellInfo predecessor;

    public PathfindingCellInfo(int distance, PathfindingCellStatus status, HexCell cell)
    {
        this.distance = distance;
        this.status = status;
        this.cell = cell;
        predecessor = null;
    }
}

public enum PathfindingCellStatus
{
    open,
    closed
}