using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visibility : MonoBehaviour
{
    [HideInInspector]
    public Texture2D visibilityMap;
    public static Visibility instance;
    [HideInInspector]
    public VisibilityStatus[,] visibility = new VisibilityStatus[128, 128];

    private bool changed = false;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        visibilityMap = new Texture2D(128, 128);
        visibilityMap.filterMode = FilterMode.Point;
        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 128; j++)
            {
                visibilityMap.SetPixel(i, j, new Color((float)visibility[i, j] / 3, 0.0f, 0.0f, 0.0f));
            }
        }

        visibilityMap.Apply();
    }

    private void Update()
    {
        if (changed)
        {
            visibilityMap.Apply();
        }
    }

    public void UpdateVisibility(HexCell cell)
    {
        visibilityMap.SetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z,
            new Color((float)cell.VisibilityStatus / 3, 0.0f, 0.0f, 0.0f));

        visibility[cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z] = cell.VisibilityStatus;
        changed = true;
    }
}


public enum VisibilityStatus{
    Unknown,
    Discovered,
    Visible
}