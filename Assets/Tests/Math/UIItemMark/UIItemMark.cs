using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tests
{
    public class UIItemMark : UIBehaviour
    {
        [SerializeField]
        private RectTransform mark;
        [SerializeField]
        private Transform target;
        [SerializeField]
        private Vector2 offset;
        [SerializeField]
        private bool isShowInScreen = true;

        private Camera cam;
        private CanvasScaler scaler;
        private Vector2 boundOffset;

        protected override void Start()
        {
            cam = Camera.main;
        }

        // 当自身尺寸发生变化时回调，重新计算边界偏移值
        protected override void OnRectTransformDimensionsChange()
        {
            if (scaler == null)
            {
                scaler = GetComponentInParent<CanvasScaler>();
            }

            if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                var resolution = scaler.referenceResolution;
                var widthRatio = Screen.width / resolution.x;
                var heightRatio = Screen.height / resolution.y;
                var halfSize = mark.sizeDelta / 2;
                boundOffset.x = (halfSize.x + offset.x) * widthRatio;
                boundOffset.y = (halfSize.y + offset.y) * heightRatio;
            }
            else
            {
                boundOffset = (offset + mark.sizeDelta) / 2;
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void Update()
        {
            if (target == null)
            {
                return;
            }

            Vector2 screenPoint;
            var viewPoint = MatrixUtil.WorldToViewPoint(cam, target.position);
            if (viewPoint.z > 0)
            {
                // 检测物体在相机背面，使用方向向量计算标记位置
                var direction = new Vector2(viewPoint.x, viewPoint.y).normalized;
                var screenCenterPoint = new Vector2(Screen.width, Screen.height) / 2;
                screenPoint = screenCenterPoint + direction * 10000;
                if (IsInScreen(screenPoint))
                {
                    // 避免方向向量太小显示在屏幕内
                    screenPoint.y = screenPoint.y > screenCenterPoint.y ? Screen.height : 0;
                }
            }
            else
            {
                screenPoint = MatrixUtil.WorldToScreenPoint(cam, target.position);
                if (!isShowInScreen && IsInScreen(screenPoint))
                {
                    mark.gameObject.SetActive(false);
                    return;
                }
            }

            if (!isShowInScreen)
            {
                mark.gameObject.SetActive(true);
            }

            screenPoint.x = Mathf.Clamp(screenPoint.x, boundOffset.x, Screen.width - boundOffset.x);
            screenPoint.y = Mathf.Clamp(screenPoint.y, boundOffset.y, Screen.height - boundOffset.y);
            mark.position = screenPoint;
        }

        private bool IsInScreen(Vector2 point)
        {
            if (point.x < 0 || point.x > Screen.width || point.y < 0 || point.y > Screen.height)
            {
                return false;
            }

            return true;
        }
    }
}