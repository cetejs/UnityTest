using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollPanel : UIPanel
    {
        [SerializeField]
        private Padding padding;
        private ScrollRect scrollRect;
        private RectTransform viewRect;
        private RectTransform contentRect;
        private RectTransform selectRect;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            viewRect = (RectTransform)scrollRect.transform;
            contentRect = scrollRect.content;
        }

        private bool MoveScrollView(Vector2 move)
        {
            if (move == Vector2.zero || EventSystem.current == null)
            {
                return false;
            }

            GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (selectedGameObject != null && selectedGameObject.transform.parent == scrollRect.content)
            {
                selectRect = (RectTransform)selectedGameObject.transform;
                Vector2 contentPos = scrollRect.content.anchoredPosition;
                Vector2 viewSize = viewRect.rect.size;
                Vector2 contentSize = contentRect.rect.size;
                Vector2 pos = selectRect.anchoredPosition;
                Vector2 size = selectRect.rect.size;

                if (scrollRect.horizontal && Mathf.Abs(move.x) > 0)
                {
                    if (move.x < 0 && contentPos.x >= padding.left)
                    {
                        scrollRect.horizontalNormalizedPosition = 0;
                        return true;
                    }

                    float x = pos.x + contentPos.x;
                    float left = x - size.x * selectRect.pivot.x;
                    if (left < 0)
                    {
                        if (scrollRect.horizontalNormalizedPosition > 0)
                        {
                            scrollRect.content.anchoredPosition -= new Vector2(left, 0);
                            return true;
                        }
                    }

                    if (move.x > 0 && contentPos.x + viewSize.x >= contentSize.x - padding.right)
                    {
                        scrollRect.horizontalNormalizedPosition = 1;
                        return true;
                    }

                    float right = x + size.x * (1 - selectRect.pivot.x) - viewSize.x;
                    if (right > 0)
                    {
                        if (scrollRect.horizontalNormalizedPosition < 1)
                        {
                            scrollRect.content.anchoredPosition -= new Vector2(right, 0);
                            return true;
                        }
                    }
                }

                if (scrollRect.vertical && Mathf.Abs(move.y) > 0)
                {
                    if (move.y > 0 && contentPos.y <= padding.top)
                    {
                        scrollRect.verticalNormalizedPosition = 1;
                        return true;
                    }

                    float y = pos.y + contentPos.y;
                    float up = y + size.y * (1 - selectRect.pivot.y);
                    if (up > 0)
                    {
                        if (scrollRect.verticalNormalizedPosition < 1)
                        {
                            scrollRect.content.anchoredPosition -= new Vector2(0, up);
                            return true;
                        }
                    }

                    if (move.y < 0 && contentPos.y + viewSize.y >= contentSize.y - padding.bottom)
                    {
                        scrollRect.verticalNormalizedPosition = 0;
                        return true;
                    }

                    float down = y - size.y * selectRect.pivot.y + viewSize.y;
                    if (down < 0)
                    {
                        if (scrollRect.verticalNormalizedPosition > 0)
                        {
                            scrollRect.content.anchoredPosition -= new Vector2(0, down);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected override void Update()
        {
            base.Update();
            if (EventSystem.current != null && EventSystem.current.currentInputModule is InputSystemUIInputModule inputModule)
            {
                MoveScrollView(inputModule.move.action.ReadValue<Vector2>());
            }
        }

        public override void OnFocus()
        {
            if (!MoveScrollView(new Vector2(1, 1)))
            {
                base.OnFocus();
                if (scrollRect.horizontal)
                {
                    scrollRect.horizontalNormalizedPosition = 0;
                }

                if (scrollRect.vertical)
                {
                    scrollRect.verticalNormalizedPosition = 1;
                }
            }
        }

        [Serializable]
        public struct Padding
        {
            public int left;
            public int right;
            public int top;
            public int bottom;
        }
    }
}
