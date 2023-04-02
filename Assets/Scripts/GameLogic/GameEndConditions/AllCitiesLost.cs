using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllCitiesLost", menuName = "ScriptableObjects/EndConditions/AllCitiesLost")]
public class AllCitiesLost : GameEndCondition
{
    private bool _satisfied = false;
    private int citiesLeft = 0;
    public override bool satisfied => _satisfied;

    public override string text => $"All Cities Lost: {citiesLeft} Left";

    public override void Evaluate()
    {
        citiesLeft = player.cities.Count;
        _satisfied = citiesLeft == 0;
    }

    public override void Init(Player player)
    {
        this.player = player;
        citiesLeft = player.cities.Count;
    }
}
