using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitDefinition", menuName = "ScriptableObjects/UnitDefinition")]
public class UnitDefinition : ScriptableObject
{
    public Sprite icon;
    public GameObject prefab;
    public string unitName;
    public string description;

    public Cost cost;

    public UnitClass unitClass;
    public bool canAttack = true;
    public int maxHealth;
    public int baseAttack;
    public int baseArmor;
    public int attackRange = 1;

    public UnitClassBonuses unitClassBonusses;
    public UnitTerrainBonuses terrainBonusses;

    public int movementPerTurn;
    public UnitMovementCosts movementCosts;

    public List<UnitAction> availableActions;

    public int GetMovementCost(TileType tileType)
    {
        MovementCost movementCost = movementCosts.costs.FirstOrDefault(x => x.terrain == tileType);
        if (movementCost != null)
        {
            return movementCost.movementCost;
        }
        else
        {
            return int.MaxValue;
        }
    }

    public float getAttackBonus(UnitClass unitType)
    {
        UnitClassBonus bonus = unitClassBonusses.bonuses.FirstOrDefault(x => x.unitClass == unitType);
        if (bonus != null)
        {
            return bonus.bonus;
        }
        else
        {
            return 0.0f;
        }
    }

    public float getDefenseBonus(TileType tileType)
    {
        TerrainBonus bonus = terrainBonusses.bonuses.FirstOrDefault(x => x.terrain == tileType);
        if (bonus != null)
        {
            return bonus.bonus;
        }
        else
        {
            return 0.0f;
        }
    }
}

[System.Serializable]
public class TerrainBonus
{
    public float bonus;
    public TileType terrain;
}

[System.Serializable]
public class MovementCost
{
    public int movementCost;
    public TileType terrain;
}

[System.Serializable]
public class UnitClassBonus
{
    public float bonus;
    public UnitClass unitClass;
}

[System.Serializable] 
public class UnitClassBonuses
{
    public List<UnitClassBonus> bonuses;
}

[System.Serializable]
public class UnitTerrainBonuses
{
    public List<TerrainBonus> bonuses;
}

[System.Serializable]
public class UnitMovementCosts
{
    public List<MovementCost> costs;
}
