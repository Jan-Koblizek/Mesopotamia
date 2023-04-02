using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("Color of the player (shown using unit highlight rings)")]
    public Color color;
    [Tooltip("Resources and their production rates the player starts with")]
    public List<ResourceAmount> startingResources;
    [Tooltip("Starting population + population icon")]
    public Population population;
    [Tooltip("AI script use for an AI player - for human player leave empty")]
    public PlayerAI AI;

    [HideInInspector]
    public List<City> cities;
    [HideInInspector]
    public List<Unit> units;

    private List<UnitSpawner> spawners = new List<UnitSpawner>();

    private void Start()
    {
        if (AI != null)
        {
            AI.player = this;
        }
        startingResources.Sort((x, y) => x.resource.displayPriority.CompareTo(y.resource.displayPriority));
    }

    public void AddSpawner(UnitSpawner spawner)
    {
        spawners.Add(spawner);
    }

    public ResourceAmount resourceByName(string name)
    {
        return startingResources.Find(x => x.resource.nameOfTheResource == name);
    }

    public void Pay(Cost cost)
    {
        foreach (ResourceAmount amount in cost.resources)
        {
            startingResources.Find(x => amount.resource == x.resource).Amount -= amount.Amount;
        }
    }

    public bool EnoughResources(Cost cost)
    {
        bool result = true;
        foreach (ResourceAmount amount in cost.resources)
        {
            result = result && startingResources.Find(x => amount.resource == x.resource).Amount >= amount.Amount;
        }
        return result;
    }

    public void TurnEnd()
    {
        foreach (Unit unit in units)
        {
            unit.turnEnd();
        }
    }

    public void TurnStart()
    {
        foreach (Unit unit in units)
        {
            unit.turnStart();
        }
        foreach (UnitSpawner spawner in spawners) spawner.TurnStart();
        foreach (ResourceAmount ra in startingResources)
        {
            ra.Amount = Mathf.Max(ra.Amount + ra.Production - ra.Consumption, 0);
        }
        ResourceAmount citizens = resourceByName("citizens");
        UpdatePopulationAmount(population.Amount + (citizens.Production - citizens.Consumption));
        resourceByName("gold").Production = citizens.Amount + 1;
    }

    public void UpdatePopulationAmount(int value) {
        population.Amount = value;
        ResourceAmount citizens = resourceByName("citizens");
        ResourceAmount food = resourceByName("food");
        food.Consumption = population.Amount;
        citizens.Production = Mathf.Max(Mathf.Min(
                                            cities.Count, 
                                            (population.Limit - population.Amount), 
                                            food.Amount + food.Production - food.Consumption),
                                        -citizens.Amount);
    }
}
