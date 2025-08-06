using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    [ExecuteAlways]
    public class UIFreeLayoutGroup : LayoutGroup
    {
        [SerializeField]
        private Vector2 spacing = Vector2.zero;
        [SerializeField]
        private float cellHeight = 100f;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            SetLayoutInputForAxis(0, rectTransform.rect.width, 0, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            float width = rectTransform.rect.width;
            int rectChildrenCount = rectChildren.Count;
            float totalWeight = padding.left;
            float totalHeight = 0;
            for (int i = 0; i < rectChildrenCount; i++)
            {
                RectTransform child = rectChildren[i];
                float childWeight = child.sizeDelta.x * child.localScale.x;
                if (i == 0)
                {
                    totalHeight = cellHeight;
                    totalWeight += childWeight + spacing.x;
                    continue;
                }

                if (totalWeight + childWeight + padding.right >= width)
                {
                    totalWeight = padding.left;
                    totalHeight += cellHeight + spacing.y;
                }

                totalWeight += childWeight + spacing.x;
            }

            SetLayoutInputForAxis(0, padding.vertical + totalHeight, 0, 1);
        }

        public override void SetLayoutHorizontal()
        {
            int rectChildrenCount = rectChildren.Count;
            for (int i = 0; i < rectChildrenCount; i++)
            {
                RectTransform rect = rectChildren[i];

                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);

                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.up;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, cellHeight);
            }
        }

        public override void SetLayoutVertical()
        {
            float width = rectTransform.rect.width;
            int rectChildrenCount = rectChildren.Count;
            float startPosX = padding.left;
            float startPosY = padding.top;
            for (int i = 0; i < rectChildrenCount; i++)
            {
                RectTransform child = rectChildren[i];
                float childWeight = child.sizeDelta.x * child.localScale.x;
                if (i == 0)
                {
                    SetChildAlongAxisWithScale(child, 0, startPosX, 1);
                    SetChildAlongAxisWithScale(child, 1, startPosY, 1);
                    startPosX += childWeight + spacing.x;
                    continue;
                }

                if (startPosX + childWeight + padding.right >= width)
                {
                    startPosX = padding.left;
                    startPosY += cellHeight + spacing.y;
                }

                SetChildAlongAxisWithScale(child, 0, startPosX, 1);
                SetChildAlongAxisWithScale(child, 1, startPosY, 1);
                startPosX += childWeight + spacing.x;
            }
        }
    }
}
