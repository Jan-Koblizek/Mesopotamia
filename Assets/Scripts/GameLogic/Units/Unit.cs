using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Unit : MonoBehaviour, Selectable
{
    public UnitDefinition unitSpecification;
    [HideInInspector]
    public int movement;
    [HideInInspector]
    public int health;
    [HideInInspector]
    public HexCoordinates coordinates;
    [HideInInspector]
    public Player player;
    [Tooltip("White circular ring placed under the unit, that will change color based on the unit owner.")]
    public GameObject highlightRing;

    [HideInInspector]
    public bool selected { get; private set; }
    [HideInInspector]
    public bool moving = false;

    public Queue<UnitCommand> commands = new Queue<UnitCommand>();

    private HexCell lastGoalPosition;

    private Vector3 mouseDownCoords = new Vector3 { x = -1000, y = -1000, z = 0 };

    private List<Unit> nearbyEnemies = new List<Unit>();
    private List<City> nearbyEnemyCities = new List<City>();
    [HideInInspector]
    public List<HexCell> movementPositions = new List<HexCell>();

    [HideInInspector]
    public UnitStatus status;
    private bool usedAttack = true;
    [HideInInspector]
    public UnitAction action;

    private bool _movementCommandSelected = false;
    public bool movementCommandSelected
    {
        get
        {
            return _movementCommandSelected;
        }
        set
        {
            _movementCommandSelected = value;
        }
    }


    private bool _displayMovementHighlight = false;
    public bool displayMovementHighlight
    {
        get
        {
            return _displayMovementHighlight;
        }
        set
        {
            if (value != _displayMovementHighlight)
            {
                _displayMovementHighlight = value;
                if (value)
                {
                    UpdateHighlights();
                }
                else
                {
                    HexGrid.instance.ClearHighlights();
                }
            }
        }
    }

    private Animator animator;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        animator = GetComponentInChildren<Animator>();
        player.units.Add(this);
        selected = false;
        highlightRing.GetComponent<Renderer>().material.color = player.color;
        UIManager.instance.unitOverheadManager.CreateUnitOverhead(this);
        coordinates = HexCoordinates.FromPosition(transform.position);
        HexGrid.instance.GetCell(coordinates).unit = this;
        //highlightRing.transform.position = HexGrid.instance.GetCell(coordinates).transform.position + new Vector3(0.0f, 2.0f, 0.0f);
    }

    public void Move(HexCell goal)
    {
        if (goal != null && goal.unit == null && 
            (HexGrid.instance.CellInsideBounds(goal) || !HexGrid.instance.CellInsideBounds(HexGrid.instance.GetCell(coordinates))))
        {
            commands.Enqueue(new Move(goal, this));
        }
    }

    private void processMovementCommands()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            mouseDownCoords = Input.mousePosition;
        }
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && Vector3.Distance(Input.mousePosition, mouseDownCoords) < 50)
        {
            HexCell selectedCell = MovementPositionSelection.SelectMovementPosition();
            if (selectedCell != null && selectedCell.unit == null)
            {
                if ((movementCommandSelected || (movementPositions.Contains(selectedCell) && displayMovementHighlight)) &&
                    (HexGrid.instance.CellInsideBounds(selectedCell) || !HexGrid.instance.CellInsideBounds(HexGrid.instance.GetCell(coordinates))))
                {
                    commands.Clear();
                    Move(selectedCell);
                    hideAttackButtons();
                }
            }
        }
    }
    
    private void visualizePath()
    {
        if (lastGoalPosition != MovementPositionSelection.SelectMovementPosition() && MovementPositionSelection.SelectMovementPosition() != null
            && (HexGrid.instance.CellInsideBounds(MovementPositionSelection.SelectMovementPosition()) || !HexGrid.instance.CellInsideBounds(HexGrid.instance.GetCell(coordinates))))
        {
            lastGoalPosition = MovementPositionSelection.SelectMovementPosition();
            int pathLength = 0;
            Stack<HexCell> visualizedPath = new Stack<HexCell>();
            if (lastGoalPosition.unit == null || lastGoalPosition.unit == this)
            {
                visualizedPath = Pathfinding.GetPathTo(this, lastGoalPosition, out pathLength, false);
            }           
            PathVisualizer.instance.VisualizePath(visualizedPath, this, pathLength);
        }

    }

    private void Update()
    {
        if (selected)
        { 
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(1))
            {
                Deselect();
            }

            if (!moving)
            {
                if (movementCommandSelected) visualizePath();
                processMovementCommands();
            }
        }
        if (commands.Count > 0 && commands.Peek() != null)
        {
            if (!commands.Peek().finishedThisTurn)
            {
                commands.Peek().Execute(this);
            }
            if (commands.Peek().finished)
            {
                commands.Dequeue();
            }
        }
        if (action != null)
        {
            action.ActionUpdate();
        }
    }

    public int GetMovementCost(TileType tileType)
    {
        return unitSpecification.GetMovementCost(tileType);
    }

    private void PlayAttackAnimation(Vector3 target)
    {
        if (unitSpecification.unitClass.typeName == "Ranged")
        {
            SoundsManager.PlaySound(SoundsManager.Sound.Shooting);
        }
        else
        {
            SoundsManager.PlaySound(SoundsManager.Sound.Attack);
        }
        animator.SetBool("Attacking", true);
        Vector3 lookPos = target - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
     
        StartCoroutine(finishAttack());
    }

    public void Attack(Unit target)
    {
        if (!usedAttack && unitSpecification.canAttack)
        {
            movement = 0;
            usedAttack = true;
            UpdateStatus();
            target.Attacked(GetAttack(target), this);

            PlayAttackAnimation(target.transform.position);
        }
    }

    private IEnumerator finishAttack()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("Attacking", false);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            //Wait every frame until animation has finished
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void AttackCity(City target)
    {
        if (!usedAttack && unitSpecification.canAttack)
        {
            movement = 0;
            usedAttack = true;
            UpdateStatus();
            int attack = unitSpecification.baseAttack;
            target.Attack(attack);

            PlayAttackAnimation(target.transform.position);
        }
    }

    public int GetAttack(Unit unit)
    {
        float bonus = unitSpecification.getAttackBonus(unit.unitSpecification.unitClass);
        return (int)(unitSpecification.baseAttack * (1.0f + bonus) - unit.unitSpecification.baseArmor);
    }

    public void Attacked(int attack, Unit attacker, bool isResponse = false) {
        TileType tileType = HexGrid.instance.GetCell(coordinates).TileType;
        if (HexGrid.instance.GetCell(coordinates).building != null) tileType = Terrain.getTileType("Urban");
        float bonus = unitSpecification.getDefenseBonus(tileType);
        health -= (int)(attack * (1.0f - bonus));
        UIManager.instance.unitOverheadManager.UpdateUnitHealth(this, (int)(attack * (1.0f - bonus)));
        if (selected)
            UIManager.instance.unitMenuManager.UpdateUnitMenu(this);

        if (unitSpecification.canAttack && !isResponse && unitSpecification.unitClass.typeName != "Ranged" && attacker.unitSpecification.unitClass.typeName != "Ranged")
        {
            attacker.Attacked(GetAttack(attacker), this, true);
            PlayAttackAnimation(attacker.transform.position);
        }
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        Deselect();
        UIManager.instance.unitOverheadManager.RemoveUnit(this);
        player.UpdatePopulationAmount(player.population.Amount - 1);
        player.units.Remove(this);
        HexGrid.instance.GetCell(coordinates).unit = null;
        StartCoroutine(delayedDestruction());
    }

    /// <summary>
    /// Remove unit - like death, but no sound immediate (can be used fo example for the settler removal when founding a city)
    /// </summary>
    public void RemoveUnit()
    {
        Deselect();
        UIManager.instance.unitOverheadManager.RemoveUnit(this);
        player.UpdatePopulationAmount(player.population.Amount - 1);
        player.units.Remove(this);
        HexGrid.instance.GetCell(coordinates).unit = null;
        Destroy(this.gameObject);
    }

    private IEnumerator delayedDestruction()
    {
        yield return new WaitForSeconds(0.5f);
        while (unitSpecification.canAttack && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            //Wait every frame until animation has finished
            yield return null;
        }
        SoundsManager.PlaySound(SoundsManager.Sound.UnitDeath);
        Destroy(this.gameObject);
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            SelectionManager.Instance.Select(this);
    }

    private void hideAttackButtons()
    {
        foreach (Unit enemyUnit in nearbyEnemies)
        {
            enemyUnit.ShowAttackIcon(false, this);
        }
        foreach (City enemyCity in nearbyEnemyCities)
        {
            enemyCity.ShowAttackIcon(false, this);
        }
    }

    private void showAttackButtons()
    {
        if (unitSpecification.canAttack)
        {
            nearbyEnemies = GetNearbyEnemyUnits();
            if (!usedAttack)
            {
                foreach (Unit enemyUnit in nearbyEnemies)
                {
                    enemyUnit.ShowAttackIcon(true, this);
                }
            }
            nearbyEnemyCities = GetNearbyEnemyCities();
            if (!usedAttack)
            {
                foreach (City enemyCity in nearbyEnemyCities)
                {
                    enemyCity.ShowAttackIcon(true, this);
                }
            }
        }
    }

    public void UpdateHighlights()
    {
        hideAttackButtons();
        showAttackButtons();
        HexGrid.instance.ClearHighlights();
        GetHighlights();
        if (selected)
            UIManager.instance.unitMenuManager.UpdateUnitMenu(this);
    }

    public void GetHighlights()
    {
        movementPositions = Pathfinding.GetMovementPositions(this);
        if (displayMovementHighlight)
        {
            for (int i = 0; i < movementPositions.Count; i++)
            {
                HexGrid.instance.Highlight(movementPositions[i]);
            }
        }
    }

    public void Select()
    {
        if (GameManager.instance.thisPlayer == player)
        {
            displayMovementHighlight = true;
            selected = true;
            UpdateStatus();
            UIManager.instance.unitMenuManager.ShowUnitMenu(this);
        }
    }

    public void Deselect()
    {
        if (action != null) action.OnDeselect(this);
        displayMovementHighlight = false;
        PathVisualizer.instance.RemovePathVisualization();
        selected = false;
        UIManager.instance.unitMenuManager.HideUnitMenu();
    }

    public void turnEnd()
    {
        moving = false;
        while (commands.Count > 0 && commands.Peek() != null)
        {
            commands.Peek().TurnEnd(this);
            if (commands.Peek().finished) commands.Dequeue();
            if (commands.Peek().finishedThisTurn) break;
        }
        movement = 0;
        status = UnitStatus.Exhausted;
        usedAttack = true;
        UpdateStatus();
        if (selected)
        {
            UpdateHighlights();
            UIManager.instance.unitMenuManager.UpdateUnitMenu(this);
        }
    }

    public void turnStart()
    {
        movement = unitSpecification.movementPerTurn;
        usedAttack = false;
        UpdateStatus();
        if (commands.Count > 0 && commands.Peek() != null)
        {
            commands.Peek().TurnStart(this);
        }
        if (selected)
        {
            UpdateHighlights();
            UIManager.instance.unitMenuManager.UpdateUnitMenu(this);
        }
    }

    public void UpdateStatus()
    {
        if (movement > 0)
        {
            status = UnitStatus.CanMove;
        }
        else if (!usedAttack)
        {
            status = UnitStatus.CanAttack;
        }
        else
        {
            status = UnitStatus.Exhausted;
            hideAttackButtons();
        }
        UIManager.instance.unitOverheadManager.UpdateUnitStatus(this);
        if (selected)
        {
            UIManager.instance.unitMenuManager.UpdateUnitMenu(this);
            UpdateHighlights();
        }
    }

    public List<Unit> GetNearbyEnemyUnits()
    {
        List<Unit> result = new List<Unit>();
        foreach (Player player in GameManager.instance.players)
        {
            if (player != this.player)
            {
                foreach (Unit enemyUnit in player.units)
                {
                    if (enemyUnit.coordinates.DistanceDirect(coordinates) <= unitSpecification.attackRange)
                    {
                        result.Add(enemyUnit);
                    }
                }
            }
        }
        return result;
    }

    public List<City> GetNearbyEnemyCities()
    {
        List<City> result = new List<City>();
        foreach (Player player in GameManager.instance.players)
        {
            if (player != this.player)
            {
                foreach (City enemyCity in player.cities)
                {
                    if (enemyCity.ziggurat.cell.coordinates.DistanceDirect(coordinates) < 2)
                    {
                        result.Add(enemyCity);
                    }
                }
            }
        }
        return result;
    }

    public void ShowAttackIcon(bool show, Unit attacker)
    {
        if (show)
        {
            UIManager.instance.unitOverheadManager.ShowAttackIcon(this, attacker);
        }
        else
        {
            UIManager.instance.unitOverheadManager.HideAttackIcon(this);
        }
    }
}

public enum UnitStatus
{
    CanMove,
    CanAttack,
    Exhausted
}
