using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class GameEndCondition : ScriptableObject
{
    [HideInInspector]
    internal Player player;
    public abstract bool satisfied { get; }
    public abstract string text { get; }
    public abstract void Evaluate();

    public abstract void Init(Player player);
}
