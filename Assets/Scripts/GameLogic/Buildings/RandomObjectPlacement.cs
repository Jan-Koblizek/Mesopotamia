using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectPlacement : ObjectPlacement
{
    [Tooltip("Random rotation in degrees - 15 means rotation in range (-15, 15)")]
    public float RotationRange;
    [Tooltip("Random displacement along X and Z axis in all directions. Value of 1 means displacement in range (-1, 1) in both directions.")]
    public float DisplacementRange;
    [Tooltip("Trees within this box will be removed (so they do not grow from the building)")]
    public BoxCollider removeVegetationBox;

    private Vector3 basePosition;
    private Quaternion baseRotation;
    private float rotation;
    private HexCell cell;
    private bool placing;
    // Start is called before the first frame update
    void Start()
    {
        basePosition = transform.localPosition;
        baseRotation = transform.rotation;
        placing = true;
    }

    public override void Place(HexCell inputCell)
    {
        //shouldPlace = true;
        placing = true;
        cell = inputCell;
    }

    private void place(HexCell cell)
    {
        transform.localPosition = basePosition + new Vector3(Random.Range(-DisplacementRange, DisplacementRange), 0.0f, Random.Range(-DisplacementRange, DisplacementRange));
        rotation = Random.Range(-RotationRange, RotationRange);
        transform.rotation = baseRotation;
        transform.RotateAround(transform.position, Vector3.up, rotation);

        if (removeVegetationBox != null)
        {
            foreach (GameObject tree in cell.trees)
            {
                Vector3 treeRelativePos = tree.transform.position - transform.position;
                if (removeVegetationBox.bounds.Contains(Quaternion.Euler(0, rotation, 0) * treeRelativePos + transform.position))
                {
                    tree.SetActive(false);
                }
            }

            foreach (GameObject bush in cell.bushes)
            {
                Vector3 bushRelativePos = bush.transform.position - transform.position;
                if (removeVegetationBox.bounds.Contains(Quaternion.Euler(0, rotation, 0) * bushRelativePos + transform.position))
                {
                    bush.SetActive(false);
                }
            }
        }

        ObjectPlacement[] childrenPlacements = gameObject.GetComponentsInChildren<ObjectPlacement>();

        foreach (ObjectPlacement childrenPlacement in childrenPlacements)
        {
            childrenPlacement.Place(cell);
        }
        placing = false;
    }

    private void Update()
    {
        if (placing) {
            place(cell);
        }
    }
}
