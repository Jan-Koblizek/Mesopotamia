using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManager : MonoBehaviour
{
    public UIDocument document;

    private VisualElement root;
    private GroupBox tooltipBox;
    private Label tooltipTitle;
    private Label tooltipText;

    private bool top;
    private bool right;
    private float height;
    private float width;

    private VisualElement currentElement;
    private EventCallback<MouseEnterEvent> mouseEnterCallback;
    private EventCallback<GeometryChangedEvent> geometryChangedCallback;
    private EventCallback<MouseMoveEvent> mouseMoveCallback;
    private EventCallback<MouseLeaveEvent> mouseLeaveCallback;

    // Start is called before the first frame update
    private void Awake()
    {
        root = document.rootVisualElement;
        tooltipBox = root.Q<GroupBox>("Tooltip");
        tooltipTitle = root.Q<Label>("TooltipTitle");
        tooltipText = root.Q<Label>("TooltipText");
    }

    private void ShowTooltip(string title, string text, VisualElement element)
    {
        tooltipBox.style.visibility = UnityEngine.UIElements.Visibility.Visible;
        Vector2 absolutePosition = utils.UIElementsUtils.GetAbsolutePosition(element);
        float elementX = absolutePosition.x + element.resolvedStyle.width / 2;
        float elementY = absolutePosition.y + element.resolvedStyle.height / 2;
        this.top = elementY > root.resolvedStyle.height / 2;
        this.right = elementX > root.resolvedStyle.width / 2;

        tooltipTitle.text = title;
        tooltipText.text = text;
    }

    private void RecomputeWidthAndHeight()
    {
        width = tooltipBox.resolvedStyle.width;
        height = tooltipBox.resolvedStyle.height;

        if (width == 0 || height == 0) tooltipBox.style.visibility = UnityEngine.UIElements.Visibility.Hidden;
        else MoveToolTip();
    }

    private void MoveToolTip()
    {
        if (right)
        {
            tooltipBox.style.left = (Input.mousePosition.x) * (root.resolvedStyle.width / Screen.width) - (width + 10);
        }
        else
        {
            tooltipBox.style.left = (Input.mousePosition.x) * (root.resolvedStyle.width / Screen.width) + 10;
        }

        if (top)
        {
            tooltipBox.style.top = (Screen.height - Input.mousePosition.y) * (root.resolvedStyle.height / Screen.height) - (height + 10);
        }
        else
        {
            tooltipBox.style.top = (Screen.height - Input.mousePosition.y) * (root.resolvedStyle.height / Screen.height) + 10;
        }
    }

    private void HideTooltip()
    {
        tooltipBox.style.visibility = UnityEngine.UIElements.Visibility.Hidden;
        tooltipBox.style.left = Input.mousePosition.x;

        tooltipBox.style.top = -1000;
    }

    public void AttachTooltip(VisualElement element, string title, string text)
    {
        currentElement = element;
        mouseEnterCallback = e =>
        {
            ShowTooltip(title, text, element);
        };

        mouseLeaveCallback = e =>
        {
            HideTooltip();
        };

        mouseMoveCallback = e =>
        {
            MoveToolTip();
        };

        geometryChangedCallback = e =>
        {
            RecomputeWidthAndHeight();
        };

        element.RegisterCallback<MouseEnterEvent>(mouseEnterCallback);

        tooltipBox.RegisterCallback<GeometryChangedEvent>(geometryChangedCallback);

        element.RegisterCallback<MouseLeaveEvent>(mouseLeaveCallback);

        element.RegisterCallback<MouseMoveEvent>(mouseMoveCallback);
    }
}
