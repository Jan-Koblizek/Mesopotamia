using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.UIElements;

public class HexCell : MonoBehaviour
{
    [HideInInspector]
    public TextMeshProUGUI debugText;
    [HideInInspector]
    public HexCoordinates coordinates;
    private Color color;
    //public RectTransform uiRect;
    [HideInInspector]
    public HexGridChunk chunk;
    public List<GameObject> rockPrefabs;
    public List<GameObject> bushPrefabs;
    public List<GameObject> treePrefabs;
    [HideInInspector]
    public GameObject ziggurat;
    [Tooltip("The number of rocks forming a cliff on a side of the tile")]
    public int cliffRockCount = 6;

    [HideInInspector]
    public Building building = null;
    [HideInInspector]
    public Unit unit = null;

    private VisibilityStatus visibilityStatus = VisibilityStatus.Unknown;
    public VisibilityStatus VisibilityStatus
    {
        get
        {
            return visibilityStatus;
        }
        set
        {

            if (visibilityStatus == VisibilityStatus.Unknown && value != VisibilityStatus.Unknown)
            {
                Initialize();
            }

            visibilityStatus = value;
            Visibility.instance.UpdateVisibility(this);        }
    }
    
    private List<GameObject>[] cliffs = new List<GameObject>[6];
    [HideInInspector]
    public List<GameObject> bushes = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> trees = new List<GameObject>();
    [HideInInspector]
    public bool initialization = true;
    [HideInInspector]
    private TileType tileType;

    private List<GameObject> others = new List<GameObject>();

    public TileType TileType
    {
        get
        {
            return tileType;
        }
        set
        {
            tileType = value;
            for (int i = trees.Count - 1; i >= 0; i--)
            {
                Destroy(trees[i]);
                trees.RemoveAt(i);
            }
            if (tileType.tileTypeName == "Forest"/* && visibilityStatus != VisibilityStatus.Unknown*/)
            {
                foreach (HexDirection dir in Enum.GetValues(typeof(HexDirection)))
                {
                    Vector2 realDir = dir.VectorDirection() * 5f;
                    GameObject tree = Instantiate(
                        treePrefabs[Random.Range(0, treePrefabs.Count)],
                        new Vector3(transform.position.x + Random.Range(-2.5f, 2.5f) + realDir.x, transform.position.y, transform.position.z + Random.Range(-2.5f, 2.5f) + realDir.y),
                        Quaternion.Euler(0.0f, Random.Range(-60, 60), 0.0f),
                        transform);
                    trees.Add(tree);
                }
            }
            HexGrid.instance.UpdateTileType(this);
        }
    }

    static int rockCount = 0;

    public Color Color
    {
        get
        {
            return color;
        }
        set
        {
            if (color == value)
            {
                return;
            }
            color = value;
            Refresh();
        }
    }
    public float Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            Vector3 position = transform.localPosition;
            float newHeight = Mathf.Clamp(value, HexMetrics.minElevation, HexMetrics.maxElevation);
            float maxHeightDifference = 0;
            float maxHeightDifferenceBefore = 0;
            foreach (HexCell neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    float difference = Mathf.Abs(neighbor.transform.position.y - newHeight);
                    if (difference > maxHeightDifference)
                    {
                        maxHeightDifference = difference;
                    }

                    float differenceBefore = Mathf.Abs(neighbor.transform.position.y - position.y);
                    if (differenceBefore > maxHeightDifferenceBefore)
                    {
                        maxHeightDifferenceBefore = differenceBefore;
                    }
                }
            }
            if ((maxHeightDifference < maxHeightDifferenceBefore ||
                (maxHeightDifference <= HexMetrics.maxNeighborHeightDifference &&
                (newHeight > -0.5f || maxHeightDifference <= HexMetrics.maxNeighborHeightDifferenceWater)) || initialization))
            {
                position.y = newHeight;
                elevation = newHeight;
            }
            transform.localPosition = position;
            if (elevation < 0.0)
            {
                foreach (GameObject bush in bushes)
                {
                    bush.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject bush in bushes)
                {
                    bush.SetActive(true);
                }
            }
            Refresh();
        }
    }

    float elevation;

    public float Water
    {
        get
        {
            return water;
        }
        set
        {
            Vector3 position = transform.localPosition;
            float newWater = Mathf.Clamp(value, 0.0f, 1.0f);
            water = newWater;
            HexGrid.instance.UpdateWater(this);
        }
    }

    float water;
    [SerializeField]
    [HideInInspector]
    public HexCell[] neighbors;
    private bool[] neighborCliffs = new bool[6];

    public void Initialize()
    {
        if (tileType.tileTypeName == "Forest")
        {
            foreach (HexDirection dir in Enum.GetValues(typeof(HexDirection)))
            {
                Vector2 realDir = dir.VectorDirection() * 5f;
                GameObject tree = Instantiate(
                    treePrefabs[Random.Range(0, treePrefabs.Count)],
                    new Vector3(transform.position.x + Random.Range(-2.5f, 2.5f) + realDir.x, transform.position.y, transform.position.z + Random.Range(-2.5f, 2.5f) + realDir.y),
                    Quaternion.Euler(0.0f, Random.Range(-180, 180), 0.0f),
                    transform);
                trees.Add(tree);
            }
        }

        initialization = false;
    }

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            cliffs[i] = new List<GameObject>();
        }
    }

    private void Start()
    {
        if (tileType == null) Terrain.getTileType("Plains");
        if (transform.position.y < -0.3f) tileType = Terrain.getTileType("Sea");
        if (tileType.tileTypeName == "Plains" && coordinates.Z == 22 && coordinates.X == -3)
        {
            GameObject zigg = Instantiate(ziggurat, new Vector3(0.0f, 0.0f, 0.0f) + transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f), transform);
            zigg.AddComponent<City>();
            zigg.GetComponent<Ziggurat>().Initialize(this, zigg.GetComponent<City>());
            zigg.GetComponent<City>().Initialize(zigg.GetComponent<Ziggurat>(), GameManager.instance.thisPlayer);
            building = zigg.GetComponent<Building>();
        }


        if (tileType.tileTypeName == "Plains" && coordinates.Z == 26 && coordinates.X == 2)
        {
            GameObject zigg = Instantiate(ziggurat, new Vector3(0.0f, 0.0f, 0.0f) + transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f), transform);
            zigg.AddComponent<City>();
            zigg.GetComponent<Ziggurat>().Initialize(this, zigg.GetComponent<City>());
            zigg.GetComponent<City>().Initialize(zigg.GetComponent<Ziggurat>(), GameManager.instance.thisPlayer);
            building = zigg.GetComponent<Building>();
        }
    }

    public void SetText(string text)
    {
        debugText.text = text;
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public List<HexCell> GetNeighbors(bool excludeCliffs = false)
    {
        if (excludeCliffs)
        {
            List<HexCell> result = new List<HexCell> ();
            for (int i = 0; i < 6; i++)
            {
                if (!neighborCliffs[i]) result.Add(neighbors[i]);
            }
            return result;
        }
        return neighbors.ToList();
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }

    public void AddCliffs(HexDirection dir, Vector3 v1, Vector3 v2)
    {
        neighborCliffs[(int)dir] = true;
        if (cliffs[(int)dir].Count == 0)
        {
            for (int i = 0; i < cliffRockCount; i++)
            {
                float relativePos = (float)i / (cliffRockCount);
                GameObject rock = Instantiate(
                    rockPrefabs[Random.Range(0, rockPrefabs.Count)], 
                    relativePos * v1 + (1.0f - relativePos) * v2,
                    Quaternion.Euler(0.0f, Random.Range(0, 360), 0.0f),
                    transform);
                cliffs[(int)dir].Add(rock);
                rockCount++;
            }
        }
        else
        {
            for (int i = 0; i < cliffs[(int)dir].Count; i++)
            {
                float relativePos = (float)i / (cliffs[(int)dir].Count);
                if ((relativePos * v1 + (1.0f - relativePos) * v2).x == float.NaN)
                {
                    Debug.Log("NaN");
                    Debug.Log(v1);
                    Debug.Log(v2);
                }
                cliffs[(int)dir][i].transform.position = relativePos * v1 + (1.0f - relativePos) * v2;
                cliffs[(int)dir][i].SetActive(true);
            }
        }
    }

    public void RemoveCliffs(HexDirection dir, bool cliffsOpposite)
    {
        if (cliffsOpposite) neighborCliffs[(int)dir] = true;
        else neighborCliffs[(int)dir] = false;
        for (int i = cliffs[(int)dir].Count - 1; i >= 0; i--)
        {
            cliffs[(int)dir][i].SetActive(false);
        }
    }
}
