using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCommand
{
    public abstract void Execute(Unit unit);

    public abstract bool finished { get; }

    public abstract bool finishedThisTurn { get; }

    public abstract void TurnStart(Unit unit);

    public abstract void TurnEnd(Unit unit);
}
