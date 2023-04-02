using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public GameObject pathPartPrefab;
    public GameObject pathPartMinorPrefab;
    private List<GameObject> pathSegmentList = new List<GameObject>();
    public static PathVisualizer instance;

    public void VisualizePath(Stack<HexCell> path, Unit unit, int rounds)
    {
        int movement = unit.movement;
        HexCell startPos = HexGrid.instance.GetCell(unit.coordinates);
        RemovePathVisualization();
        if (path != null)
        {
            HexCell currentCell = startPos;
            HexCell previousCell = null;
            foreach (HexCell cell in path)
            {
                previousCell = currentCell;
                currentCell = cell;
                if (previousCell != null)
                {
                    pathSegmentList.Add(Instantiate(pathPartMinorPrefab, (3 * cell.transform.position + previousCell.transform.position) / 4 + new Vector3(0.0f, 2.3f, 0.0f), Quaternion.identity, this.transform));
                    pathSegmentList.Add(Instantiate(pathPartMinorPrefab, (2 * cell.transform.position + 2 * previousCell.transform.position) / 4 + new Vector3(0.0f, 2.3f, 0.0f), Quaternion.identity, this.transform));
                    pathSegmentList.Add(Instantiate(pathPartMinorPrefab, (cell.transform.position + 3 * previousCell.transform.position) / 4 + new Vector3(0.0f, 2.3f, 0.0f), Quaternion.identity, this.transform));
                }
                if ((previousCell != null) && movement - unit.GetMovementCost(cell.TileType) < 0)
                {
                    if (previousCell.unit != null)
                    {
                        GameObject removed = pathSegmentList[pathSegmentList.Count - 1];
                        pathSegmentList.RemoveAt(pathSegmentList.Count - 1);

                        pathSegmentList.Add(Instantiate(pathPartPrefab, removed.transform.position, Quaternion.identity, this.transform));
                        Destroy(removed);

                        pathSegmentList.Add(Instantiate(pathPartMinorPrefab, previousCell.transform.position + new Vector3(0.0f, 2.3f, 0.0f), Quaternion.identity, this.transform));
                        movement = unit.unitSpecification.movementPerTurn - (unit.GetMovementCost(cell.TileType) + unit.GetMovementCost(previousCell.TileType));
                    }
                    else
                    {
                        pathSegmentList.Add(Instantiate(pathPartPrefab, previousCell.transform.position + new Vector3(0.0f, 2.3f, 0.0f), Quaternion.identity, this.transform));
                        movement = unit.unitSpecification.movementPerTurn - unit.GetMovementCost(cell.TileType);
                    }
                }
                else if (previousCell != null)
                {
                    pathSegmentList.Add(Instantiate(pathPartMinorPrefab, previousCell.transform.position + new Vector3(0.0f, 2.3f, 0.0f), Quaternion.identity, this.transform));
                    movement -= unit.GetMovementCost(cell.TileType);
                }
            }
            pathSegmentList.Add(Instantiate(pathPartPrefab, currentCell.transform.position + new Vector3(0.0f, 2.3f, 0.0f), Quaternion.identity, this.transform));
        }
    }

    public void RemovePathVisualization()
    {
        for (int i = pathSegmentList.Count - 1; i >= 0; i--)
        {
            Destroy(pathSegmentList[i]);
        }
        pathSegmentList.Clear();
    }

    public void Start()
    {
        instance = this;
    }
}
