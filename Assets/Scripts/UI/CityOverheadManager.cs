using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class CityOverheadManager : MonoBehaviour
{
    public UIDocument document;

    private Dictionary<City, SingleCityOverheadUI> cityToUI = new Dictionary<City, SingleCityOverheadUI>();
    private Camera cam;

    private VisualElement root;

    private Vector3 cityHeightOffset = new Vector3(0.0f, 4.0f, 0.0f);

    private void OnEnable()
    {
        cam = Camera.main;
        document.rootVisualElement.Clear();
    }

    public void CreateCityOverhead(City city)
    {
        root = document.rootVisualElement;
        if (city != null)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.AddToClassList("CityOverheadBox");

            float health = (float)city.health / city.maxHealth;

            VisualElement healthBarBackground = new VisualElement();
            healthBarBackground.AddToClassList("CityHealthBarBackground");
            VisualElement healthBar = new VisualElement();
            healthBar.AddToClassList("CityHealthBar");
            healthBar.name = "HealthBar";
            healthBar.style.width = new StyleLength(new Length(health * 100, LengthUnit.Percent));
            healthBarBackground.Add(healthBar);
            groupBox.Add(healthBarBackground);

            Vector2 viewPortPoint = getViewportPoint(city);

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

            groupBox.style.display = DisplayStyle.None;
            root.Add(groupBox);
            SingleCityOverheadUI overheadUI = new SingleCityOverheadUI();
            overheadUI.groupBox = groupBox;
            overheadUI.healthBar = healthBar;

            overheadUI.attackButton = attackButton;
            overheadUI.attackAction = (e) => { };

            overheadUI.damageDisplayBox = damageDisplayBox;
            overheadUI.damageValue = damageValue;
            cityToUI[city] = overheadUI;
        }
    }

    public void RemoveCity(City city)
    {
        if (cityToUI[city] != null)
        {
            GroupBox cityUI = cityToUI[city].groupBox;
            cityToUI.Remove(city);
            document.rootVisualElement.Remove(cityUI);
        }
    }

    public void UpdateCityHealth(City city, int damage)
    {
        float health = (float)city.health / city.maxHealth;
        SingleCityOverheadUI overheadUI = cityToUI[city];
        overheadUI.groupBox.style.display = DisplayStyle.Flex;
        overheadUI.healthBar.style.width = new StyleLength(new Length(health * 100, LengthUnit.Percent));

        Vector2 viewPortPoint = getViewportPoint(city);

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
                if (city == null) root.Remove(overheadUI.damageDisplayBox);
            };
        /*
        Tween decreaseOpacity = DOTween.To(
            () => unitToUI[unit].damageDisplayBox.style.opacity.value,
            x => unitToUI[unit].damageDisplayBox.style.opacity = x,
            0.0f,
            10.0f);*/

        //decreaseOpacity.onComplete = () => unitToUI[unit].damageDisplayBox.style.display = DisplayStyle.None;
        //increaseOpacity.onComplete = () => decreaseOpacity.Play();
        //increaseOpacity.Play();
    }

    public void ShowAttackIcon(City city, Unit attacker)
    {
        if (city != null)
        {
            SingleCityOverheadUI overheadUI = cityToUI[city];
            overheadUI.attackButton.style.display = DisplayStyle.Flex;
            overheadUI.attackButton.UnregisterCallback<ClickEvent>(overheadUI.attackAction);
            overheadUI.attackAction = (e) => attacker.AttackCity(city);
            overheadUI.attackButton.RegisterCallback<ClickEvent>(overheadUI.attackAction);
        }
    }

    public void HideAttackIcon(City city)
    {
        if (city != null)
        {
            SingleCityOverheadUI overheadUI = cityToUI[city];
            overheadUI.attackButton.style.display = DisplayStyle.None;
            overheadUI.attackButton.UnregisterCallback<ClickEvent>(overheadUI.attackAction);
        }
    }

    private void Update()
    {
        foreach (KeyValuePair<City, SingleCityOverheadUI> pair in cityToUI)
        {
            if (pair.Key != null)
            {
                City city = pair.Key;
                Vector2 viewPortPoint = getViewportPoint(city);

                pair.Value.groupBox.style.left = viewPortPoint.x - 50;
                pair.Value.groupBox.style.top = viewPortPoint.y - 30;
            }
        }
    }

    private Vector2 getViewportPoint(City city)
    {
        Vector3 viewPortPoint = cam.WorldToViewportPoint(city.ziggurat.transform.position + cityHeightOffset);
        return new Vector2(root.resolvedStyle.width * viewPortPoint.x, root.resolvedStyle.height - root.resolvedStyle.height * viewPortPoint.y);
    }

    private class SingleCityOverheadUI
    {
        public GroupBox groupBox;
        public VisualElement healthBar;

        public Button attackButton;
        public EventCallback<ClickEvent> attackAction;

        public GroupBox damageDisplayBox;
        public Label damageValue;
    }
}