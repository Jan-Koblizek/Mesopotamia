using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [Tooltip("Texture specifying the elevation and terrain type at different positions")]
    public Texture2D terrainTexture;
    [Tooltip("Always set as true. In edit mode, it can be set to false to start with a blank texture")]
    public bool loadFromTexture;
    [Tooltip("Mode allowing you to edit the map - don't use")]
    public bool editMode;
    [Tooltip("Chunks are groups of cells - 2 x 2")]
    public int chunkCountX = 1, chunkCountZ = 1;
    int cellCountX, cellCountZ;

    public static HexGrid instance;

    public HexCell cellPrefab;

    HexCell[] cells;

    [Tooltip("Default color for painting in edit mode - don't change")]
    public Color defaultColor;
    [Tooltip("Texture, that adds variety to the terrain elevation, color and transitions")]
    public Texture2D noiseSource;
    public HexGridChunk chunkPrefab;
    [Tooltip("Material used for the terrain rendering")]
    public Material groundMaterial;
    HexGridChunk[] chunks;

    private int chunkUpdated = -1;
    private bool textureChanged;

    private List<HexCell> highlightedCells = new List<HexCell>();

    void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }
    void Awake()
    {
        if (!editMode)
        {
            Texture2D copy = new Texture2D(terrainTexture.width, terrainTexture.height, terrainTexture.format, false);
            copy.filterMode = FilterMode.Point;
            Graphics.CopyTexture(terrainTexture, copy);
            terrainTexture = copy;
        }
        groundMaterial.SetTexture("_Terrain", terrainTexture);
        HexMetrics.noiseSource = noiseSource;
        instance = this;
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;
        CreateChunks();
        CreateCells();
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0.0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.initialization = true;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;
        cell.Elevation = 0.2f;
        if (!loadFromTexture)
        {
            cell.Water = 0.0f;
        }

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }

        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        //cell.SetText(cell.coordinates.ToStringOnSeparateLines());
        AddCellToChunk(x, z, cell);
        if (loadFromTexture)
        {
            Color textureColor = terrainTexture.GetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z);
            cell.Elevation = textureColor.r * (HexMetrics.maxElevation - HexMetrics.minElevation) + HexMetrics.minElevation;
            cell.Water = textureColor.g;
            cell.TileType = Terrain.instance.tileTypes[Mathf.RoundToInt(textureColor.b * 16)];
            terrainTexture.SetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z, new Color(textureColor.r, textureColor.g, textureColor.b, 0.0f));
        }
        if (editMode)
        {
            cell.Initialize();
        }
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        if (index < 0 || index >= cells.Length)
        {
            return null;
        }
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    private void Update()
    {
        if (editMode)
        {
            if (chunkUpdated >= 0)
            {
                int updates = chunks[chunkUpdated].updatesNeeded + chunks[chunkUpdated + 1].updatesNeeded +
                    chunks[chunkUpdated + chunkCountX].updatesNeeded + chunks[chunkUpdated + chunkCountX + 1].updatesNeeded;
                if (updates > 0)
                {
                    HexMesh.FixSeams(
                        chunks[chunkUpdated].hexMesh,
                        chunks[chunkUpdated + 1].hexMesh,
                        chunks[chunkUpdated + chunkCountX].hexMesh,
                        chunks[chunkUpdated + chunkCountX + 1].hexMesh
                        );
                    if (chunks[chunkUpdated].updatesNeeded > 0) chunks[chunkUpdated].updatesNeeded--;
                    if (chunks[chunkUpdated + chunkCountX + 1].updatesNeeded > 0) chunks[chunkUpdated].updatesNeeded--;
                }
            }
            chunkUpdated++;
            if (chunkUpdated == chunks.Length - (chunkCountX + 1)) chunkUpdated = 0;
        }
        if (textureChanged)
        {
            terrainTexture.Apply();
            groundMaterial.SetTexture("_Terrain", terrainTexture);
            textureChanged = false;
        }
    }

    
    public void UpdateWater(HexCell cell)
    {
        Color colorPixel = terrainTexture.GetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z);
        terrainTexture.SetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z,
            new Color(colorPixel.r, cell.Water, colorPixel.b, colorPixel.a));

        textureChanged = true;
    }

    public void UpdateTileType(HexCell cell) {
        Color colorPixel = terrainTexture.GetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z);
        terrainTexture.SetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z,
            new Color(colorPixel.r, colorPixel.g, ((float)Terrain.instance.tileTypes.FindIndex(type => type == cell.TileType)) / 16, colorPixel.a));
    }

    public void Highlight(HexCell cell)
    {
        highlightedCells.Add(cell);
        Color colorPixel = terrainTexture.GetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z);
        terrainTexture.SetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z,
            new Color(colorPixel.r, colorPixel.g, colorPixel.b, 1.0f));

        textureChanged = true;
    }

    public void ClearHighlights()
    {
        foreach (HexCell cell in highlightedCells)
        {
            Color colorPixel = terrainTexture.GetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z);
            terrainTexture.SetPixel(cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z,
                new Color(colorPixel.r, colorPixel.g, colorPixel.b, 0.0f));
        }
        highlightedCells.Clear();
        textureChanged = true;
    }

    public bool CellInsideBounds(HexCell cell)
    {
        if (Mathf.Sqrt((cell.transform.position.x - 250) * (cell.transform.position.x - 250) + (cell.transform.position.z - 232) * (cell.transform.position.z - 232)) < 212)
        {
            return true;
        }
        return false;
    }
    
    /*
    public void SaveIntoTexture()
    {
        for (int z = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                HexCell cell = GetCell(HexCoordinates.FromOffsetCoordinates(x, z));
                if (cell == null) { Debug.Log(x); Debug.Log(z); }
                terrainTexture.SetPixel(
                    cell.coordinates.X + cell.coordinates.Z / 2, cell.coordinates.Z,
                    new Color(
                        (cell.Elevation - HexMetrics.minElevation) / (HexMetrics.maxElevation - HexMetrics.minElevation),
                        cell.Water, ((float)Terrain.instance.tileTypes.FindIndex(type => type == cell.TileType)) / 16, 0));
            }
        }
        terrainTexture.Apply();

        string path = Application.dataPath + "/../" + AssetDatabase.GetAssetPath(terrainTexture);
        System.IO.File.WriteAllBytes(path, terrainTexture.EncodeToPNG());
    }
    */
}
