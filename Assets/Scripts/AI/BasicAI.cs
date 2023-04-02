using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : PlayerAI
{
    BasicAttackingUnitAI unitAI;
    private void Start()
    {
        unitAI = new BasicAttackingUnitAI();
    }
    public override void ExecuteTurn()
    {
        if (unitAI.ExecutionFinished()) finished = true;
    }

    public override void StartTurn()
    {
        finished = false;
        unitAI.TakeTurn(player.units);
    }
}
