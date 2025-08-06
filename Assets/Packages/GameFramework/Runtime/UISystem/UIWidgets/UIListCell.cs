using UnityEngine;

namespace GameFramework
{
    public class UIListCell : PoolObject
    {
        private RectTransform rectTransform;

        public int Index { get; internal set; }

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }

                return rectTransform;
            }
        }

        public T Cast<T>() where T : UIListCell
        {
            return this as T;
        }
    }
}
