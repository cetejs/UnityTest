using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIList : MonoBehaviour
    {
        [SerializeField]
        private Direction direction = Direction.Horizontal;
        [SerializeField]
        private Padding padding;
        [SerializeField]
        private float spacing;
        [SerializeField]
        private int row = 1;
        [SerializeField]
        private Vector2 cellSize;
        [SerializeField]
        private AssetReferenceGameObject prefabKey;

        private ScrollRect scrollRect;
        private RectTransform viewRect;
        private RectTransform content;
        private ObjectPool<UIListCell> pool;
        private Action<UIListCell> onCellUpdate;
        private List<CellInfo> list = new List<CellInfo>();
        private bool adjustAnchored;
        private bool initialized;

        public int Count
        {
            get { return list.Count; }
        }

        public int Row
        {
            get
            {
                return row;
            }
            set
            {
                row = Mathf.Max(value, 1);
                SetCount(list.Count);
            }
        }

        public RectTransform Content
        {
            get { return content; }
        }

        public event Action<UIListCell> OnCellUpdate
        {
            add { onCellUpdate += value; }
            remove { onCellUpdate -= value; }
        }

        private void Start()
        {
            Initialization();
        }

        private void OnDestroy()
        {
            pool?.Clear();
            list.Clear();
        }

        private void Initialization()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            scrollRect = GetComponent<ScrollRect>();
            viewRect = GetComponent<RectTransform>();
            content = scrollRect.content;
            scrollRect.onValueChanged.AddListener(OnScrollViewChanged);
            scrollRect.horizontal = direction == Direction.Horizontal;
            scrollRect.vertical = direction == Direction.Vertical;
            if (prefabKey.RuntimeKeyIsValid())
            {
                ObjectPool objPool = ObjectPoolManager.GetObjectPool(prefabKey);
                pool = new ObjectPool<UIListCell>(objPool);
            }
        }

        public void Init(UIListCell listCell)
        {
            if (listCell == null)
            {
                Debug.LogError("Init failed, because cell is invalid");
                return;
            }

            pool = new ObjectPool<UIListCell>(listCell, transform);
        }

        public void SetCount(int count)
        {
            if (count < 0)
            {
                Debug.LogError("Set cell count is invalid");
                return;
            }

            Initialization();
            UpdateCallSize();
            SetContentSize(count);
            SetListSize(count);
            UpdateView();
        }

        public void UpdateList()
        {
            for (int i = 0; i < list.Count; i++)
            {
                CellUpdate(i);
            }
        }

        private void UpdateCallSize()
        {
            if (viewRect.rect.size == Vector2.zero)
            {
                Canvas.ForceUpdateCanvases();
            }

            if (direction == Direction.Horizontal)
            {
                cellSize.y = (viewRect.rect.height - padding.top - padding.bottom) / row;
            }
            else
            {
                cellSize.x = (viewRect.rect.width - padding.left - padding.right) / row;
            }
        }

        private void CellUpdate(int index)
        {
            CellInfo cellInfo = list[index];
            if (cellInfo.cell == null)
            {
                return;
            }

            if (IsOutRange(cellInfo.pos))
            {
                return;
            }

            onCellUpdate?.Invoke(cellInfo.cell);
        }

        private void SetContentSize(int count)
        {
            if (direction == Direction.Horizontal)
            {
                if (!adjustAnchored)
                {
                    adjustAnchored = true;
                    content.AdjustAnchor(AnchorLeftType.Stretch, AnchorTopType.Left);
                }

                float contentWight = (spacing + cellSize.x) * Mathf.CeilToInt(count / (float)row);
                contentWight -= spacing;
                contentWight += padding.left + padding.right;
                Vector2 contentPos = content.anchoredPosition;
                Vector2 contentSize = content.sizeDelta;
                contentSize.x = contentWight;
                contentPos.x = Mathf.Min(contentWight - viewRect.rect.width, contentPos.x);
                contentPos.x = Mathf.Max(contentPos.x, 0);
                content.anchoredPosition = contentPos;
                content.sizeDelta = contentSize;
                scrollRect.horizontal = contentSize.x > viewRect.rect.width;
            }
            else
            {
                if (!adjustAnchored)
                {
                    adjustAnchored = true;
                    content.AdjustAnchor(AnchorLeftType.Top, AnchorTopType.Stretch);
                }

                float contentHeight = (spacing + cellSize.y) * Mathf.CeilToInt(count / (float)row);
                contentHeight -= spacing;
                contentHeight += padding.top + padding.bottom;
                Vector2 contentPos = content.anchoredPosition;
                Vector2 contentSize = content.sizeDelta;
                contentSize.y = contentHeight;
                contentPos.y = Mathf.Min(contentHeight - viewRect.rect.height, contentPos.y);
                contentPos.y = Mathf.Max(contentPos.y, 0);
                content.anchoredPosition = contentPos;
                content.sizeDelta = contentSize;
                scrollRect.vertical = contentSize.y > viewRect.rect.height;
            }
        }

        private void SetListSize(int count)
        {
            if (list.Count > count)
            {
                int diffCount = list.Count - count;
                while (diffCount > 0)
                {
                    int lastIndex = list.Count - 1;
                    UIListCell listCell = list[lastIndex].cell;
                    if (listCell != null)
                    {
                        listCell.Release();
                    }

                    list.RemoveAt(lastIndex);
                    diffCount--;
                }
            }
            else
            {
                int diffCount = count - list.Count;
                while (diffCount > 0)
                {
                    int index = list.Count;
                    CellInfo cellInfo = new CellInfo
                    {
                        pos = GetCellPosition(index)
                    };
                    list.Add(cellInfo);
                    diffCount--;
                }
            }
        }

        private void UpdateView()
        {
            for (int i = 0; i < list.Count; i++)
            {
                CellInfo cellInfo = list[i];
                if (IsOutRange(cellInfo.pos))
                {
                    if (cellInfo.cell != null)
                    {
                        cellInfo.cell.Release();
                        cellInfo.cell = null;
                    }
                }
                else
                {
                    if (cellInfo.cell == null)
                    {
                        UIListCell listCell = pool.Get(content);
                        listCell.Index = i;
                        listCell.RectTransform.AdjustAnchor(AnchorLeftType.Top, AnchorTopType.Left);
                        listCell.RectTransform.sizeDelta = cellSize;
                        listCell.RectTransform.anchoredPosition = cellInfo.pos;
                        listCell.RectTransform.localScale = Vector3.one;
                        cellInfo.cell = listCell;
                        onCellUpdate?.Invoke(listCell);
                    }
                }

                list[i] = cellInfo;
            }
        }

        private Vector2 GetCellPosition(int index)
        {
            Vector2 pos = Vector2.zero;
            if (direction == Direction.Horizontal)
            {
                pos.x = (cellSize.x + spacing) * (index / row);
                pos.y = -(cellSize.y + spacing) * (index % row);
            }
            else
            {
                pos.x = (cellSize.x + spacing) * (index % row);
                pos.y = -(cellSize.y + spacing) * (index / row);
            }

            pos.x += padding.left;
            pos.y -= padding.top;
            return pos;
        }

        private bool IsOutRange(Vector2 pos)
        {
            Vector2 contentPos = content.anchoredPosition;
            Vector2 viewSize = viewRect.rect.size;
            if (direction == Direction.Horizontal)
            {
                float x = pos.x + contentPos.x;
                if (x < 2 * -cellSize.x || x > viewSize.x + cellSize.y)
                {
                    return true;
                }
            }
            else
            {
                float y = pos.y + contentPos.y;
                if (y > 2 * cellSize.y || y < -viewSize.y - cellSize.y)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnScrollViewChanged(Vector2 value)
        {
            UpdateView();
        }

        private enum Direction
        {
            Horizontal,
            Vertical
        }

        [Serializable]
        public struct Padding
        {
            public int left;
            public int right;
            public int top;
            public int bottom;
        }

        private struct CellInfo
        {
            public UIListCell cell;
            public Vector2 pos;
        }
    }
}
