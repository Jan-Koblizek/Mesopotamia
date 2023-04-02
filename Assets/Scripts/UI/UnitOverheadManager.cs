using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class UnitOverheadManager : MonoBehaviour
{
    public UIDocument document;
    [Tooltip("Icon indicating, that the unit has movement points and can attack")]
    [SerializeField]
    private Sprite unitCanMove;
    [Tooltip("Icon indication, that the unit has run out out of movement points, but can still attack")]
    [SerializeField]
    private Sprite unitCanAttack;
    [Tooltip("Icon indicating, that the unit can neither move nor attack")]
    [SerializeField]
    private Sprite unitExhausted;


    private Dictionary<Unit, SingleUnitOverheadUI> unitToUI = new Dictionary<Unit, SingleUnitOverheadUI>();
    private Camera cam;

    private VisualElement root;

    private Vector3 unitHeightOffset = new Vector3(0.0f, 15.0f, 0.0f);

    private void OnEnable()
    {
        cam = Camera.main;
        document.rootVisualElement.Clear();
    }

    public void CreateUnitOverhead(Unit unit)
    {
        root = document.rootVisualElement;
        if (unit != null)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.AddToClassList("UnitOverheadBox");

            float health = (float)unit.health / unit.unitSpecification.maxHealth;
            int level = 1;

            Image status = new Image();
            status.AddToClassList("UnitStatus");
            switch (unit.status) {
                case UnitStatus.CanMove:
                    status.sprite = unitCanMove;
                    break;
                case UnitStatus.CanAttack:
                    status.sprite = unitCanAttack;
                    break;
                case UnitStatus.Exhausted:
                    status.sprite = unitExhausted;
                    break;
            }
            status.name = "Status";
            groupBox.Add(status);

            Label levelLabel = new Label();
            levelLabel.AddToClassList("UnitLevel");
            levelLabel.text = level.ToString();
            levelLabel.name = "Level";
            groupBox.Add(levelLabel);

            GroupBox abilities = new GroupBox();
            abilities.AddToClassList("UnitAbilities");
            abilities.name = "Abilities";
            groupBox.Add(abilities);

            VisualElement healthBarBackground = new VisualElement();
            healthBarBackground.AddToClassList("UnitHealthBarBackground");
            VisualElement healthBar = new VisualElement();
            healthBar.AddToClassList("UnitHealthBar");
            healthBar.name = "HealthBar";
            healthBar.style.width = new StyleLength(new Length(health * 100, LengthUnit.Percent));
            healthBarBackground.Add(healthBar);
            groupBox.Add(healthBarBackground);

            Vector2 viewPortPoint = getViewportPoint(unit);

            groupBox.style.left = viewPortPoint.x - 50;
            groupBox.style.top = viewPortPoint.y - 30;

            Button attackButton = new Button();
            attackButton.AddToClassList("AttackButton");
            groupBox.Add(attackButton);

            GroupBox damageDisplayBox = new GroupBox();
            damageDisplayBox.AddToClassList("DamageBox");

            Label damageValue = new Label();
            damageValue.AddToClassList("DamageBoxLabel");

            VisualElement damageIcon = new VisualElement();
            damageIcon.AddToClassList("DamageBoxIcon");

            damageDisplayBox.Add(damageValue);
            damageDisplayBox.Add(damageIcon);
            root.Add(damageDisplayBox);

            root.Add(groupBox);
            SingleUnitOverheadUI overheadUI = new SingleUnitOverheadUI();
            overheadUI.groupBox = groupBox;
            overheadUI.status = status;
            overheadUI.level = levelLabel;
            overheadUI.healthBar = healthBar;
            overheadUI.abilities = abilities;

            overheadUI.attackButton = attackButton;
            overheadUI.attackAction = (e) => { };

            overheadUI.damageDisplayBox = damageDisplayBox;
            overheadUI.damageValue = damageValue;
            unitToUI[unit] = overheadUI;
        }
    }

    public void RemoveUnit(Unit unit)
    {
        if (unitToUI[unit] != null)
        {
            GroupBox unitUI = unitToUI[unit].groupBox;
            unitToUI.Remove(unit);
            document.rootVisualElement.Remove(unitUI);
        }
    }

    public void UpdateUnitStatus(Unit unit)
    {
        if (unitToUI.ContainsKey(unit))
        {
            SingleUnitOverheadUI overheadUI = unitToUI[unit];
            if (overheadUI != null)
            {
                switch (unit.status)
                {
                    case UnitStatus.CanMove:
                        overheadUI.status.sprite = unitCanMove;
                        break;
                    case UnitStatus.CanAttack:
                        overheadUI.status.sprite = unitCanAttack;
                        break;
                    case UnitStatus.Exhausted:
                        overheadUI.status.sprite = unitExhausted;
                        break;
                }
            }
        }
    }

    public void UpdateUnitHealth(Unit unit, int damage)
    {
        float health = (float)unit.health / unit.unitSpecification.maxHealth;
        if (unitToUI.ContainsKey(unit))
        {
            SingleUnitOverheadUI overheadUI = unitToUI[unit];
            overheadUI.healthBar.style.width = new StyleLength(new Length(health * 100, LengthUnit.Percent));

            Vector2 viewPortPoint = getViewportPoint(unit);

            overheadUI.damageDisplayBox.style.left = viewPortPoint.x - 50;

            overheadUI.damageDisplayBox.style.display = DisplayStyle.Flex;
            overheadUI.damageDisplayBox.style.opacity = 0.0f;
            overheadUI.damageDisplayBox.style.top = viewPortPoint.y - 55;
            overheadUI.damageValue.text = (-damage).ToString();

            DOTween.To(
                () => overheadUI.damageDisplayBox.style.opacity.value,
                x => overheadUI.damageDisplayBox.style.opacity = x,
                1.0f,
                1.0f).SetEase(Ease.OutQuad).onComplete = () => DOTween.To(
                () => overheadUI.damageDisplayBox.style.opacity.value,
                x => overheadUI.damageDisplayBox.style.opacity = x,
                0.0f,
                1.0f).SetEase(Ease.InQuad).onComplete = () => overheadUI.damageDisplayBox.style.display = DisplayStyle.None;

            DOTween.To(
                () => overheadUI.damageDisplayBox.style.top.value.value,
                x => overheadUI.damageDisplayBox.style.top = x,
                viewPortPoint.y - 80,
                2.1f).onComplete = () =>
                {
                    overheadUI.damageDisplayBox.style.top = viewPortPoint.y - 55;
                    if (unit == null) root.Remove(overheadUI.damageDisplayBox);
                };
        }
    }

    public void ShowAttackIcon(Unit unit, Unit attacker)
    {
        if (unit != null)
        {
            SingleUnitOverheadUI overheadUI = unitToUI[unit];
            overheadUI.attackButton.style.display = DisplayStyle.Flex;
            overheadUI.attackButton.UnregisterCallback<ClickEvent>(overheadUI.attackAction);
            overheadUI.attackAction = (e) => { attacker.commands.Clear(); attacker.Attack(unit); };
            overheadUI.attackButton.RegisterCallback<ClickEvent>(overheadUI.attackAction);
        }
    }

    public void ShowAttackIconCommand(Unit attacked, Unit attacker, Attack action)
    {
        if (attacked != null)
        {
            SingleUnitOverheadUI overheadUI = unitToUI[attacked];
            overheadUI.attackButton.style.display = DisplayStyle.Flex;
            overheadUI.attackButton.UnregisterCallback<ClickEvent>(overheadUI.attackAction);
            overheadUI.attackAction = (e) => action.AttackClicked(attacked);
            overheadUI.attackButton.RegisterCallback<ClickEvent>(overheadUI.attackAction);
        }
    }

    public void HideAttackIcon(Unit unit)
    {
        if (unit != null && unitToUI.ContainsKey(unit))
        {
            SingleUnitOverheadUI overheadUI = unitToUI[unit];
            overheadUI.attackButton.style.display = DisplayStyle.None;
            overheadUI.attackButton.UnregisterCallback<ClickEvent>(overheadUI.attackAction);
        }
    }

    private void Update()
    {
        foreach (KeyValuePair<Unit, SingleUnitOverheadUI> pair in unitToUI)
        {
            if (pair.Key != null)
            {
                Unit unit = pair.Key;
                Vector2 viewPortPoint = getViewportPoint(unit);

                pair.Value.groupBox.style.left = viewPortPoint.x - 50;
                pair.Value.groupBox.style.top = viewPortPoint.y - 30;
            }
        }
    }

    private Vector2 getViewportPoint(Unit unit)
    {
        Vector3 viewPortPoint = cam.WorldToViewportPoint(unit.transform.position + unitHeightOffset);
        return new Vector2(root.resolvedStyle.width * viewPortPoint.x, root.resolvedStyle.height - root.resolvedStyle.height * viewPortPoint.y);
    }

    private class SingleUnitOverheadUI
    {
        public GroupBox groupBox;
        public Label level;
        public VisualElement healthBar;
        public Image status;
        public GroupBox abilities;

        public Button attackButton;
        public EventCallback<ClickEvent> attackAction;

        public GroupBox damageDisplayBox;
        public Label damageValue;
    }
}