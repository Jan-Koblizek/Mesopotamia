using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexEditor : MonoBehaviour
{
	public Color[] colors;

	public HexGrid hexGrid;
	public TileType noneTileType;

	private Color activeColor;
	private float activeElevation;
	private float activeWater;
	int brushSize;

    public Camera myCam;
    private float startXPos;
    private float startYPos;

	private TileType tileType;

    private bool isDragging = false;

    public void SetBrushSize(int size)
	{
		Debug.Log(size);
		brushSize = (int)size;
	}

	void Awake()
	{
		SelectColor(0);
		myCam = Camera.main;
	}

	private void Start()
	{
		tileType = noneTileType;
    }

	void Update()
	{
		if (Input.GetMouseButton(0) &&
			!EventSystem.current.IsPointerOverGameObject())
		{
			HandleInput();
		}
    }


    void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			EditCells(hexGrid.GetCell(hit.point));
		}
	}
	void EditCells(HexCell center)
	{
		if (center)
		{
			int centerX = center.coordinates.X;
			int centerZ = center.coordinates.Z;
			for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
			{
				for (int x = centerX - r; x <= centerX + brushSize; x++)
				{
					EditCell(hexGrid.GetCell(new HexCoordinates(x, z)), 1.0f);
				}
			}
			for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
			{
				for (int x = centerX - brushSize; x <= centerX + r; x++)
				{
					EditCell(hexGrid.GetCell(new HexCoordinates(x, z)), 1.0f);
				}
			}
		}
	}

	void EditCell(HexCell cell, float strength)
	{
		if (cell)
		{
			cell.Color = activeColor;
			if (activeElevation != 0)
			{
				cell.Elevation += activeElevation * Time.deltaTime * strength;
			}
			cell.Water += activeWater * Time.deltaTime * strength * 0.2f;
            if (tileType.tileTypeName != "None" && cell.TileType != tileType)
            {
                cell.TileType = tileType;
            }
        }
	}

	public void SelectTileType(int index)
	{
		index -= 1;
		if (index == -1)
		{
			this.tileType = noneTileType;
		}
		else
		{
			this.tileType = Terrain.instance.tileTypes[index];
		}
	}

	public void SelectColor(int index)
	{
		activeColor = colors[index];
	}

	public void ChangeElevation(int value)
    {
		value -= 2;
		activeElevation = value;
    }

	public void ChangeWater(int value)
	{
		value -= 2;
		activeWater = value;
	}
}