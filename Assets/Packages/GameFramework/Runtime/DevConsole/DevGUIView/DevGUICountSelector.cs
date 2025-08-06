using UnityEngine;

namespace GameFramework
{
    public abstract class DevGUICountSelector : DevGUISelector
    {
        [SerializeField]
        private float startSpeed = 2f;
        [SerializeField]
        private float acceleration = 20f;

        private float inputSpeed;
        private float inputTime;
        private bool pressedLeftBtn;
        private bool pressedRightBtn;

        public void OnCountButtonDown(bool leftBtn)
        {
            if (leftBtn)
            {
                pressedLeftBtn = true;
            }
            else
            {
                pressedRightBtn = true;
            }
        }

        public void OnCountButtonUp(bool leftBtn)
        {
            if (leftBtn)
            {
                pressedLeftBtn = false;
            }
            else
            {
                pressedRightBtn = false;
            }
        }

        protected virtual void Update()
        {
            if (!pressedLeftBtn && !pressedRightBtn || pressedLeftBtn && pressedRightBtn)
            {
                inputSpeed = startSpeed;
                inputTime = 0;
                return;
            }

            inputSpeed += Time.deltaTime * acceleration;
            if (Time.time > inputTime && inputSpeed > startSpeed)
            {
                inputTime = Time.time + 1 / inputSpeed;
                OnCountValue(pressedRightBtn);
            }
        }

        protected abstract void OnCountValue(bool plus);
    }
}
