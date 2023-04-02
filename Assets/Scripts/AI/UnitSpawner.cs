using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Tooltip("Cooldown period - period between the unit spawns")]
    public int period;
    [Tooltip("Turn on which the units will start to spawn")]
    public int startTurn;
    [Tooltip("Unit to be instantiated")]
    public Unit prefab;
    [Tooltip("The player who will gain the unit")]
    public Player player;

    private HexCell cell;
    private int cooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
        cell = HexGrid.instance.GetCell(transform.position);
        player.AddSpawner(this);
    }

    public void TurnStart()
    {
        if (GameManager.instance.Turn >= startTurn)
        {
            if (cooldown <= 0)
            {
                GameObject unitInstance = Instantiate(prefab.unitSpecification.prefab, cell.transform.position, Quaternion.identity, null);
                unitInstance.GetComponent<Unit>().player = player;
                unitInstance.GetComponent<Unit>().health = prefab.unitSpecification.maxHealth;
                unitInstance.GetComponent<Unit>().movement = 0;
                unitInstance.GetComponent<Unit>().status = UnitStatus.Exhausted;
                cooldown = period;
            }
            else
            {
                cooldown--;
            }
        }
    }
}
