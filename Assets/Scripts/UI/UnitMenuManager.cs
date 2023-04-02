using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Image = UnityEngine.UIElements.Image;

public class UnitMenuManager : MonoBehaviour
{
    public UIDocument document;
    public Texture swordIcon;
    public Texture movementIcon;
    private Unit unit;
    private BonusType bonusTypeSelected = BonusType.None;

    VisualElement root;
    VisualElement unitInfoUI;

    VisualElement unitTypeBonuses;
    VisualElement terrainDefenseBonuses;
    VisualElement terrainMovement;

    Label unitName;
    VisualElement unitClassIcon;
    VisualElement unitIcon;
    Label health;
    Label attack;
    Label defense;
    Label movement;
    VisualElement healthBar;

    VisualElement unitBonusesInfo;
    Label unitBonusesInfoHeader;
    GroupBox unitBonusesInfoContent;

    GroupBox unitActionsPanel;

    List<(UnitAction, Button)> actions = new List<(UnitAction, Button)> ();

    private void Start()
    {
        unitTypeBonuses = document.rootVisualElement.Q<VisualElement>("UnitBonuses");
        terrainDefenseBonuses = document.rootVisualElement.Q<VisualElement>("TerrainBonuses");
        terrainMovement = document.rootVisualElement.Q<VisualElement>("TerrainMovement");
        root = document.rootVisualElement;
        unitInfoUI = document.rootVisualElement.Q<GroupBox>("UnitInfoUI");

        unitName = document.rootVisualElement.Q<Label>("UnitName");
        unitClassIcon = document.rootVisualElement.Q<VisualElement>("UnitClassIcon");
        unitIcon = document.rootVisualElement.Q<VisualElement>("UnitIcon");
        health = document.rootVisualElement.Q<Label>("Health");
        UIManager.instance.tooltipManager.AttachTooltip(
            document.rootVisualElement.Q<VisualElement>("HealthIcon"), 
            "Health", 
            "Health represents the amount of damage the unit can take before it dies. When these points reach 0, the unit is killed."
        );

        attack = document.rootVisualElement.Q<Label>("Attack");
        UIManager.instance.tooltipManager.AttachTooltip(
            document.rootVisualElement.Q<VisualElement>("AttackIcon"),
            "Attack",
            "Attack represents the damage the unit does. Each point in attack means the enemy losing 1 HP in the fight."
         );

        defense = document.rootVisualElement.Q<Label>("Defense");
        UIManager.instance.tooltipManager.AttachTooltip(
            document.rootVisualElement.Q<VisualElement>("ArmorIcon"),
            "Armor",
            "Armor reduces the amount of damage the unit receives. Each armor point reduces damage by 1"
        );

        movement = document.rootVisualElement.Q<Label>("Movement");
        UIManager.instance.tooltipManager.AttachTooltip(
            document.rootVisualElement.Q<VisualElement>("MovementIcon"),
            "Movement",
            "Movement points help you move across the map. Movement through each tile takes a certain number of these points and they are regenerated every turn."
        );

        healthBar = document.rootVisualElement.Q<VisualElement>("HealthBar");

        unitBonusesInfo = document.rootVisualElement.Q<GroupBox>("InfoPanel");
        unitBonusesInfoHeader = unitBonusesInfo.Q<Label>("Header");
        unitBonusesInfoContent = unitBonusesInfo.Q<GroupBox>("Content");

        unitActionsPanel = root.Q<GroupBox>("Actions");
    }

    public void ShowUnitMenu(Unit unit)
    {
        this.unit = unit;
        
        unitInfoUI.style.display = DisplayStyle.Flex;
        //root.style.visibility = UnityEngine.UIElements.Visibility.Visible;
        if (unit != null)
        {
            unitName.text = unit.unitSpecification.unitName;
            unitClassIcon.style.backgroundImage = (StyleBackground)unit.unitSpecification.unitClass.icon;
            UIManager.instance.tooltipManager.AttachTooltip(
                unitClassIcon,
                unit.unitSpecification.unitClass.typeName,
                unit.unitSpecification.unitClass.description
            );

            unitIcon.style.backgroundImage = unit.unitSpecification.icon.texture;
            health.text = unit.health + "/" + unit.unitSpecification.maxHealth;
            attack.text = unit.unitSpecification.baseAttack.ToString();
            defense.text = unit.unitSpecification.baseArmor.ToString();
            movement.text = unit.movement + "/" + unit.unitSpecification.movementPerTurn;
            healthBar.style.width = new StyleLength(new Length(((float)unit.health / unit.unitSpecification.maxHealth) * 100, LengthUnit.Percent));

            terrainDefenseBonuses.RegisterCallback<MouseEnterEvent>(e =>
            {
                ShowUnitBonusesInfo(BonusType.TerrainDefense, false);
            });
            terrainDefenseBonuses.RegisterCallback<MouseLeaveEvent>(e =>
            {
                HideUnitBonusesInfo();
            });
            terrainDefenseBonuses.RegisterCallback<ClickEvent>(e =>
            {
                ShowUnitBonusesInfo(BonusType.TerrainDefense, true);
            });

            unitTypeBonuses.RegisterCallback<MouseEnterEvent>(e =>
            {
                ShowUnitBonusesInfo(BonusType.UnitType, false);
            });
            unitTypeBonuses.RegisterCallback<MouseLeaveEvent>(e =>
            {
                HideUnitBonusesInfo();
            });
            unitTypeBonuses.RegisterCallback<ClickEvent>(e =>
            {
                ShowUnitBonusesInfo(BonusType.UnitType, true);
            });

            terrainMovement.RegisterCallback<MouseEnterEvent>(e =>
            {
                ShowUnitBonusesInfo(BonusType.TerrainMovement, false);
            });
            terrainMovement.RegisterCallback<MouseLeaveEvent>(e =>
            {
                HideUnitBonusesInfo();
            });
            terrainMovement.RegisterCallback<ClickEvent>(e =>
            {
                ShowUnitBonusesInfo(BonusType.TerrainMovement, true);
            });

            unitActionsPanel.Clear();
            actions.Clear();
            for (int i = 0; i < unit.unitSpecification.availableActions.Count; i++)
            {
                UnitAction action = unit.unitSpecification.availableActions[i];
                Button actionVisual = new Button();
                actionVisual.AddToClassList("UnitAction");
                UIManager.instance.tooltipManager.AttachTooltip(actionVisual, action.ActionName, action.ActionDescription);
                actionVisual.clicked += () => {
                    action.Click(unit, actionVisual);
                };
                actionVisual.style.backgroundImage = (StyleBackground)action.Icon;
                actionVisual.SetEnabled(action.IsAvailable(unit));
                unitActionsPanel.Add(actionVisual);
                actions.Add((action, actionVisual));
            }
        }
    }

    public void UpdateUnitMenu(Unit unit)
    {
        health.text = unit.health + "/" + unit.unitSpecification.maxHealth;
        attack.text = unit.unitSpecification.baseAttack.ToString();
        defense.text = unit.unitSpecification.baseArmor.ToString();
        movement.text = unit.movement + "/" + unit.unitSpecification.movementPerTurn;
        healthBar.style.width = new StyleLength(new Length(((float)unit.health / unit.unitSpecification.maxHealth) * 100, LengthUnit.Percent));

        foreach ((UnitAction, Button) pair in actions)
        {
            pair.Item2.SetEnabled(pair.Item1.IsAvailable(unit));
        }
    }

    public void HideUnitMenu()
    {
        unitInfoUI.style.display = DisplayStyle.None;
    }


    private void HideUnitBonusesInfo()
    {
        if (bonusTypeSelected == BonusType.None)
        {
            unitBonusesInfo.style.display = DisplayStyle.None;
            elementBorderColor(terrainDefenseBonuses, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
            elementBorderColor(terrainMovement, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
            elementBorderColor(unitTypeBonuses, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
        }
    }

    private void ShowUnitBonusesInfo(BonusType type, bool clicked)
    {
        
        if (clicked && type == bonusTypeSelected)
        {
            unitBonusesInfo.style.display = DisplayStyle.None;
            elementBorderColor(terrainDefenseBonuses, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
            elementBorderColor(terrainMovement, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
            elementBorderColor(unitTypeBonuses, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
            bonusTypeSelected = BonusType.None;
            return;
        }

        if (bonusTypeSelected == BonusType.None || clicked)
        {
            unitBonusesInfo.style.display = DisplayStyle.Flex;
            if (clicked)
            {
                bonusTypeSelected = type;
                elementBorderColor(terrainDefenseBonuses, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
                elementBorderColor(terrainMovement, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
                elementBorderColor(unitTypeBonuses, new Color((float)241 / 255, (float)181 / 255, (float)0 / 255));
            }
            switch (type)
            {
                case BonusType.TerrainDefense:
                    unitBonusesInfo.Clear();
                    unitBonusesInfoHeader = new Label();
                    unitBonusesInfoHeader.AddToClassList("InfoPanelHeader");
                    unitBonusesInfoHeader.text = "Terrain Damage Reduction";
                    unitBonusesInfo.Add(unitBonusesInfoHeader);
                    unitBonusesInfo.Add(unitBonusesInfoContent);
                    UIManager.instance.tooltipManager.AttachTooltip(
                        unitBonusesInfoHeader,
                        "Terrain Damage Reduction",
                        "Some tile types provide a defensive bonus (or penalty) to your unit which can reduce the damage received. The reduction is calculated after the armor is applied.");
                    unitBonusesInfoContent.Clear();
                    elementBorderColor(terrainDefenseBonuses, new Color((float)255 / 255, (float)236 / 255, (float)177 / 255));
                    foreach (TerrainBonus bonus in unit.unitSpecification.terrainBonusses.bonuses)
                    {
                        unitBonusesInfoContent.Add(createTerrainBonusDisplay(bonus));
                    }
                    break;
                case BonusType.UnitType:
                    unitBonusesInfo.Clear();
                    unitBonusesInfoHeader = new Label();
                    unitBonusesInfoHeader.AddToClassList("InfoPanelHeader");
                    unitBonusesInfoHeader.text = "Unit Type Bonus";
                    unitBonusesInfo.Add(unitBonusesInfoHeader);
                    unitBonusesInfo.Add(unitBonusesInfoContent);
                    UIManager.instance.tooltipManager.AttachTooltip(
                        unitBonusesInfoHeader,
                        "Unit Type Bonus",
                        "Units often perform better or worse against certain classes of units. They receive a percentage bonus or a penalty to their attack bases on the unit they are fighting.");
                    unitBonusesInfoContent.Clear();
                    elementBorderColor(unitTypeBonuses, new Color((float)255 / 255, (float)236 / 255, (float)177 / 255));
                    foreach (UnitClassBonus bonus in unit.unitSpecification.unitClassBonusses.bonuses)
                    {
                        unitBonusesInfoContent.Add(createUnitClassBonusDisplay(bonus));
                    }
                    break;
                case BonusType.TerrainMovement:
                    unitBonusesInfo.Clear();
                    unitBonusesInfoHeader = new Label();
                    unitBonusesInfoHeader.AddToClassList("InfoPanelHeader");
                    unitBonusesInfoHeader.text = "Terrain Movement Speed";
                    unitBonusesInfo.Add(unitBonusesInfoHeader);
                    unitBonusesInfo.Add(unitBonusesInfoContent);
                    UIManager.instance.tooltipManager.AttachTooltip(
                        unitBonusesInfoHeader,
                        "Terrain Movement",
                        "Your unit does not move at the same speed in all terrains. This is represented by different costs of movement (movement points).");
                    unitBonusesInfoContent.Clear();
                    elementBorderColor(terrainMovement, new Color((float)255 / 255, (float)236 / 255, (float)177 / 255));
                    foreach (MovementCost movementCost in unit.unitSpecification.movementCosts.costs)
                    {
                        unitBonusesInfoContent.Add(createTerrainMovementDisplay(movementCost));
                    }
                    break;
            }
        }
    }

    private GroupBox createUnitClassBonusDisplay(UnitClassBonus bonus)
    {
        GroupBox result = new GroupBox();
        result.AddToClassList("InfoPanelContentItem");

        Image icon = new Image();
        icon.AddToClassList("InfoPanelContentIcon");
        icon.image = bonus.unitClass.icon;
        icon.style.backgroundImage = null;
        UIManager.instance.tooltipManager.AttachTooltip(icon, bonus.unitClass.typeName, bonus.unitClass.description);

        Label value = new Label();
        value.AddToClassList("InfoPanelContentValue");
        value.text = (int)(bonus.bonus * 100) + "%";

        Image unitIcon = new Image();
        unitIcon.AddToClassList("InfoPanelContentUnitIcon");
        unitIcon.image = swordIcon;
        UIManager.instance.tooltipManager.AttachTooltip(unitIcon, "Attack", "Attack represents the damage the unit does.Each point in attack means the enemy losing 1 HP in the fight.");

        result.Add(icon);
        result.Add(value);
        result.Add(unitIcon);
        return result;
    }

    private GroupBox createTerrainBonusDisplay(TerrainBonus bonus)
    {
        GroupBox result = new GroupBox();
        result.AddToClassList("InfoPanelContentItem");

        Image icon = new Image();
        icon.AddToClassList("InfoPanelContentIcon");
        icon.style.backgroundImage = (StyleBackground)bonus.terrain.icon;
        UIManager.instance.tooltipManager.AttachTooltip(icon, bonus.terrain.tileTypeName, bonus.terrain.description);

        Label value = new Label();
        value.AddToClassList("InfoPanelContentValue");
        value.text = (int)(bonus.bonus * 100) + "%";

        Image unitIcon = new Image();
        unitIcon.AddToClassList("InfoPanelContentUnitIcon");
        unitIcon.image = swordIcon;
        UIManager.instance.tooltipManager.AttachTooltip(unitIcon, "Attack", "Attack represents the damage the unit does.Each point in attack means the enemy losing 1 HP in the fight.");

        result.Add(icon);
        result.Add(value);
        result.Add(unitIcon);
        return result;
    }

    private GroupBox createTerrainMovementDisplay(MovementCost movementCost)
    {
        GroupBox result = new GroupBox();
        result.AddToClassList("InfoPanelContentItem");

        Image icon = new Image();
        icon.AddToClassList("InfoPanelContentIcon");
        icon.style.backgroundImage = (StyleBackground)movementCost.terrain.icon;
        UIManager.instance.tooltipManager.AttachTooltip(icon, movementCost.terrain.tileTypeName, movementCost.terrain.description);

        Label value = new Label();
        value.AddToClassList("InfoPanelContentValue");
        value.text = movementCost.movementCost.ToString();
        value.style.width = 30;

        Image unitIcon = new Image();
        unitIcon.AddToClassList("InfoPanelContentUnitIcon");
        unitIcon.image = movementIcon;
        UIManager.instance.tooltipManager.AttachTooltip(unitIcon, "Movement", "Movement points help you move across the map. Movement through each tile takes a certain number of these points and they are regenerated every turn.");


        result.Add(icon);
        result.Add(value);
        result.Add(unitIcon);
        return result;
    }

    private void elementBorderColor(VisualElement element, Color color)
    {
        element.style.borderBottomColor = color;
        element.style.borderLeftColor = color;
        element.style.borderTopColor = color;
        element.style.borderRightColor = color;
    }


    private enum BonusType
    {
        TerrainDefense,
        TerrainMovement,
        UnitType,
        None
    }
}
