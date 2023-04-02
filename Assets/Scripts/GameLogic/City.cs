using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour, Selectable
{
    public Ziggurat ziggurat;
    public List<HexCell> cells = new List<HexCell>();
    public Player player;

    public int maxHealth = 100;
    [HideInInspector]
    public int health;

    private List<Building> buildings = new List<Building>();

    public void Initialize(Ziggurat ziggurat, Player player)
    {
        health = maxHealth;
        this.player = player;
        player.cities.Add(this);
        this.ziggurat = ziggurat;
        cells.Add(ziggurat.cell);
        foreach (HexCell neighbor in ziggurat.cell.GetNeighbors(true))
        {
            if (neighbor != null)
            {
                cells.Add(neighbor);
            }
        }

        UIManager.instance.cityOverheadManager.CreateCityOverhead(this);
    }

    public void Attack(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            UIManager.instance.cityOverheadManager.UpdateCityHealth(this, damage);
            if (health <= 0) CityDestroyed();
        }
    }

    private void CityDestroyed()
    {
        UIManager.instance.cityOverheadManager.RemoveCity(this);

        foreach (Building building in buildings) building.Destroy();

        if (player.resourceByName("citizens").Amount < 0)
        {
            player.population.Amount -= player.resourceByName("citizens").Amount;
            player.resourceByName("citizens").Amount = 0;
        }
        ziggurat?.Destroy();
        Deselect();
        player.cities.Remove(this);
        Destroy(gameObject);
    }

    public List<HexCell> getFreeCells()
    {
        List<HexCell> result = new List<HexCell>();
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] != null && cells[i].building == null)
            {
                result.Add(cells[i]);
            }
        }
        return result;
    }

    public void AddCellsNearBuilding(HexCell cell)
    {
        foreach (HexCell neighbor in cell.GetNeighbors(true))
        {
            if (neighbor && ziggurat.cell.coordinates.DistanceDirect(neighbor.coordinates) < 3 && !cells.Contains(neighbor) && HexGrid.instance.CellInsideBounds(neighbor))
            {
                cells.Add(neighbor);
            }
        }
    }

    public void AddBuilding(Building building)
    {
        buildings.Add(building);
    }

    public void GetHighlights()
    {
        foreach (HexCell cell in cells)
        {
            HexGrid.instance.Highlight(cell);
        }
    }

    public void Select()
    {
        if (GameManager.instance.thisPlayer == player)
        {
            GameManager.instance.SelectedCity = this;
            SoundsManager.PlaySound(SoundsManager.Sound.City);
            GetHighlights();
        }
    }

    public void Deselect()
    {
        if (GameManager.instance.thisPlayer == player)
        {
            HexGrid.instance.ClearHighlights();
            UIManager.instance.HideCitySidePanel();
        }
    }

    public void ShowAttackIcon(bool show, Unit attacker)
    {
        if (show)
        {
            UIManager.instance.cityOverheadManager.ShowAttackIcon(this, attacker);
        }
        else
        {
            UIManager.instance.cityOverheadManager.HideAttackIcon(this);
        }
    }
    //public City(Ziggurat ziggurat) Start is called before the first frame update
}
