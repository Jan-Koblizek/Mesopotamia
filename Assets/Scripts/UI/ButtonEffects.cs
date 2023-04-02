using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class ButtonEffects
{
    public static void ButtonMouseEnter(Button button)
    {
        if (button.enabledInHierarchy)
        {
            button.style.borderBottomColor = button.style.borderRightColor = button.style.borderLeftColor = button.style.borderTopColor = new Color(0.8372549f, 0.7784314f, 0.2843137f);
            SoundsManager.PlaySound(SoundsManager.Sound.ButtonHover);
        }
    }

    public static void ButtonMouseExit(Button button)
    {
        if (button.enabledInHierarchy)
        {
            button.style.borderBottomColor = button.style.borderRightColor = button.style.borderLeftColor = button.style.borderTopColor = new Color(0.7372549f, 0.6784314f, 0.1843137f);
        }
    }

    public static void MainMenuButtonMouseEnter(Button button)
    {
        if (button.enabledInHierarchy)
        {
            button.style.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.3f);
            SoundsManager.PlaySound(SoundsManager.Sound.ButtonHover);
        }
    }

    public static void MainMenuButtonMouseExit(Button button)
    {
        if (button.enabledInHierarchy)
        {
            button.style.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }

    public static void NextTurnMouseEnter(Button button)
    {
        if (button.enabledInHierarchy)
        {
            button.style.width = button.style.height = 180;
            button.style.bottom = button.style.right = -15;
            //button.style.borderBottomColor = button.style.borderRightColor = button.style.borderLeftColor = button.style.borderTopColor = new Color(0.8372549f, 0.7784314f, 0.2843137f);
            SoundsManager.PlaySound(SoundsManager.Sound.ButtonHover);
        }
    }

    public static void NextTurnMouseExit(Button button)
    {
        if (button.enabledInHierarchy)
        {
            button.style.width = button.style.height = 150;
            button.style.bottom = button.style.right = 0;
            //button.style.borderBottomColor = button.style.borderRightColor = button.style.borderLeftColor = button.style.borderTopColor = new Color(0.7372549f, 0.6784314f, 0.1843137f);
        }
    }

    public static void ButtonClick(Button button)
    {
        if (button.enabledInHierarchy)
        {
            SoundsManager.PlaySound(SoundsManager.Sound.ButtonClick);
        }
    }
}
