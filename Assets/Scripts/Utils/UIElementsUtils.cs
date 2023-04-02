using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace utils
{
    public static class UIElementsUtils
    {
        public static Vector2 GetAbsolutePosition(VisualElement element)
        {
            if (element == null) return Vector2.zero;

            return new Vector2(element.resolvedStyle.left, element.resolvedStyle.top) + GetAbsolutePosition(element.parent);
        }
    }
}
