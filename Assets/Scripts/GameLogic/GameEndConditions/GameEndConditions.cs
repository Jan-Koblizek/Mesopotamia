using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndConditions : MonoBehaviour
{
    [Tooltip("Conditions, that must be all satisfied for the player to win")]
    public List<PlayerConditionPair> victoryConditions = new List<PlayerConditionPair>();
    [Tooltip("If any of these conditions holds, the player lost the game")]
    public List<PlayerConditionPair> defeatConditions = new List<PlayerConditionPair>();

    private void Start()
    {
        foreach (PlayerConditionPair pair in victoryConditions)
        {
            pair.condition.Init(pair.player);
        }

        foreach (PlayerConditionPair pair in defeatConditions)
        {
            pair.condition.Init(pair.player);
        }
    }

    public bool Won()
    {
        foreach (PlayerConditionPair pair in victoryConditions)
        {
            pair.condition.Evaluate();
            if (!pair.condition.satisfied) return false;
        }
        return true;
    }

    public bool Lost()
    {
        foreach (PlayerConditionPair pair in defeatConditions)
        {
            pair.condition.Evaluate();
            if (pair.condition.satisfied) return true;
        }
        return false;
    }
}

[System.Serializable]
public struct PlayerConditionPair
{
    public Player player;
    public GameEndCondition condition;
}
