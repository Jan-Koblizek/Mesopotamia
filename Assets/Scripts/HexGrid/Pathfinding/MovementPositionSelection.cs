using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MovementPositionSelection
{
    public static HexCell SelectMovementPosition()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                return HexGrid.instance.GetCell(hit.point);
            }
        }
        return null;
    }
}
