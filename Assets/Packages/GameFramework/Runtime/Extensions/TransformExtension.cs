using UnityEngine;

namespace GameFramework
{
    public static class TransformExtension
    {
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void AdjustAnchor(this RectTransform rectTransform, AnchorLeftType leftType, AnchorTopType topType)
        {
            if (leftType == AnchorLeftType.Stretch || topType == AnchorTopType.Stretch)
            {
                if (leftType != AnchorLeftType.Stretch)
                {
                    float leftValue = GetAnchorLeftValue(leftType);
                    rectTransform.anchorMin = new Vector2(0f, leftValue);
                    rectTransform.anchorMax = new Vector2(1f, leftValue);
                    rectTransform.pivot = new Vector2(0.5f, leftValue);
                }
                else if (topType != AnchorTopType.Stretch)
                {
                    float topValue = GetAnchorTopValue(topType);
                    rectTransform.anchorMin = new Vector2(topValue, 0f);
                    rectTransform.anchorMax = new Vector2(topValue, 1f);
                    rectTransform.pivot = new Vector2(topValue, 0.5f);
                }
                else
                {
                    rectTransform.anchorMin = new Vector2(0f, 0f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                }

                rectTransform.offsetMin = new Vector2(0f, 0f);
                rectTransform.offsetMax = new Vector2(0f, 0f);
            }
            else
            {
                float leftValue = GetAnchorLeftValue(leftType);
                float topValue = GetAnchorTopValue(topType);
                rectTransform.anchorMin = new Vector2(topValue, leftValue);
                rectTransform.anchorMax = new Vector2(topValue, leftValue);
                rectTransform.pivot = new Vector2(topValue, leftValue);
            }
        }

        private static float GetAnchorLeftValue(AnchorLeftType type)
        {
            switch (type)
            {
                case AnchorLeftType.Top:
                    return 1f;
                case AnchorLeftType.Middle:
                    return 0.5f;
                case AnchorLeftType.Bottom:
                    return 0f;
            }

            return -1f;
        }

        private static float GetAnchorTopValue(AnchorTopType type)
        {
            switch (type)
            {
                case AnchorTopType.Left:
                    return 0f;
                case AnchorTopType.Center:
                    return 0.5f;
                case AnchorTopType.Right:
                    return 1f;
            }

            return -1f;
        }


        public static string GetScenePath(this Transform transform)
        {
            string path = transform.name;
            Transform parent = transform.parent;
            while (parent != null)
            {
                path = string.Concat(parent.name, "/", path);
                parent = parent.parent;
            }

            return path;
        }
    }

    public enum AnchorLeftType
    {
        Top,
        Middle,
        Bottom,
        Stretch
    }

    public enum AnchorTopType
    {
        Left,
        Center,
        Right,
        Stretch
    }
}
