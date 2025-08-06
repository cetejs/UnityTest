using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevCmdInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private InputField input;
        private Vector2 startDragPosition;
        private Vector2 draggingPosition;
        private Vector2 dragDirection;
        private bool dragging;

        private float inputSpeed;
        private float inputTime;
        private const float InputStartSpeed = 2f;
        private const float InputAcceleration = 20f;

        public InputField Ipt
        {
            get { return input; }
        }
        public UnityEvent<MoveDirection> OnMove;

        private void Update()
        {
            MoveDirection moveDirection;
            if (dragging)
            {
                if (Vector2.Dot(dragDirection, Vector2.up) >= 0)
                {
                    moveDirection = MoveDirection.Up;
                }
                else
                {
                    moveDirection = MoveDirection.Down;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    moveDirection = MoveDirection.Up;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    moveDirection = MoveDirection.Down;
                }
                else
                {
                    moveDirection = MoveDirection.None;
                }
            }

            UpdateInputDirection(moveDirection);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startDragPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            draggingPosition = eventData.position;
            dragDirection = (draggingPosition - startDragPosition).normalized;
            dragging = dragDirection != Vector2.zero;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            startDragPosition = Vector2.zero;
            draggingPosition = Vector2.zero;
            dragDirection = Vector2.zero;
            dragging = false;
        }

        private void UpdateInputDirection(MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case MoveDirection.None:
                    inputSpeed = InputStartSpeed;
                    inputTime = 0;
                    break;
                case MoveDirection.Up:
                case MoveDirection.Down:
                    input.MoveTextEnd(false);
                    inputSpeed += Time.deltaTime * InputAcceleration;
                    break;
            }

            if (Time.time > inputTime && inputSpeed > InputStartSpeed)
            {
                inputTime = Time.time + 1 / inputSpeed;
                OnMove?.Invoke(moveDirection);
            }
        }

        public enum MoveDirection
        {
            None,
            Up,
            Down
        }
    }
}
