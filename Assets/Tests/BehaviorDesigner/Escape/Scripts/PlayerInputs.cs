using System;
using GameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Escape
{
    public class PlayerInputs : MonoBehaviour
    {
        [SerializeField]
        private Vector2 look;
        [SerializeField]
        private Vector2 move;
        [SerializeField]
        private float zoom;
        [SerializeField]
        private bool slow;
        [SerializeField]
        private bool sprint;

        public Vector2 Look
        {
            get { return look; }
        }

        public float Zoom
        {
            get { return zoom; }
        }

        public Vector2 Move
        {
            get { return move; }
        }

        public bool IsSlow
        {
            get { return slow; }
            set { slow = value; }
        }

        public bool IsSprint
        {
            get { return sprint; }
            set { sprint = value; }
        }

        private void OnDisable()
        {
            look = Vector2.zero;
            move = Vector2.zero;
            zoom = 0f;
            slow = false;
            sprint = false;
        }

        public void OnMove(InputValue input)
        {
            move = input.Get<Vector2>();
        }
        
        public void OnLook(InputValue input)
        {
            look = input.Get<Vector2>();
        }
        
        public void OnZoom(InputValue input)
        {
            zoom = input.Get<float>();
        }
        
        public void OnSlow(InputValue input)
        {
            slow = input.isPressed;
        }
        
        public void OnSprint(InputValue input)
        {
            sprint = input.isPressed;
        }
    }
}