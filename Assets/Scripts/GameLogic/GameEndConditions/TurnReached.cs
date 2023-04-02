using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurnReached", menuName = "ScriptableObjects/EndConditions/TurnReached")]
public class TurnReached : GameEndCondition
{
    public int turn;
    public override bool satisfied => (GameManager.instance.Turn > turn);

    public override string text => $"Reach Turn {turn} ({turn - GameManager.instance.Turn} Turns Left)";

    public override void Evaluate() {}

    public override void Init(Player player)
    {
        this.player = player;
    }
}
