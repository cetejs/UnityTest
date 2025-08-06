using UnityEngine;

namespace GameFramework
{
    public abstract class PoolObject : MonoBehaviour
    {
        private ObjectPool pool;

        public bool IsReleased { get; private set; }

        internal void Init(ObjectPool pool)
        {
            this.pool = pool;
        }

        internal void WakeUp()
        {
            IsReleased = false;
            OnWakeUp();
        }

        internal void Sleep()
        {
            OnSleep();
            IsReleased = true;
        }

        public void Release()
        {
            if (!IsReleased && pool != null)
            {
                pool.Release(this);
            }
        }

        protected virtual void OnWakeUp()
        {
        }

        protected virtual void OnSleep()
        {
        }
    }
}
