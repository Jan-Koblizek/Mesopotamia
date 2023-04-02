using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAI : MonoBehaviour
{
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public bool finished;
    public abstract void StartTurn();
    public abstract void ExecuteTurn();
}
