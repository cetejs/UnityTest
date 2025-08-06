using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(Selectable), typeof(Animator))]
    public partial class UISelectable : MonoBehaviour,
        ISelectHandler, IDeselectHandler, IUpdateSelectedHandler,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        ICancelHandler
    {
        [Flag]
        [InspectorGroup("Navigation", false)]
        public NavigationDirection navigationDirection = NavigationDirection.Up |
                                                         NavigationDirection.Down |
                                                         NavigationDirection.Left |
                                                         NavigationDirection.Right;
        [Range(0, 90)]
        [InspectorGroup("Navigation", false)]
        public float navigationLimitAngle;
        [InspectorGroup("Navigation", false)]
        public bool navigationLimitSameParent;
        [InspectorGroup("Navigation", false)]
        public bool cacheNavigation = true;

        [SerializeField]
        [InspectorGroup("Transition", false)]
        protected Animator animator;
        [SerializeField]
        [InspectorGroup("Transition", false)]
        protected string normalTrigger = "Normal";
        [SerializeField]
        [InspectorGroup("Transition", false)]
        protected string highlightedTrigger = "Highlighted";
        [SerializeField]
        [InspectorGroup("Transition", false)]
        protected string pressedTrigger = "Pressed";
        [SerializeField]
        [InspectorGroup("Transition", false)]
        protected string disabledTrigger = "Disabled";
        protected bool isSelected;
        protected bool isPointEnter;
        protected bool isPointDown;
        protected Selectable selectable;
        protected UIPanel panel;

        private static Selectable[] selectables;
        private static MoveDirection navigationMoveDir;
        private static float navigationTime;
        private static float navigationMoveCount;

        protected virtual void Awake()
        {
            selectable = GetComponent<Selectable>();
        }

        protected virtual void OnEnable()
        {
            panel = GetComponentInParent<UIPanel>();
            DoStateTransition();
        }

        protected virtual void OnDisable()
        {
            isSelected = false;
            isPointEnter = false;
            isPointDown = false;
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            isSelected = true;
            DoStateTransition();
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            isSelected = false;
            DoStateTransition();
        }

        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            if (EventSystem.current == null || !EventSystem.current.sendNavigationEvents)
            {
                return;
            }

            if (eventData.currentInputModule is InputSystemUIInputModule inputModule)
            {
                Vector2 moveVector = inputModule.move.action.ReadValue<Vector2>();
                if (!eventData.used && moveVector.sqrMagnitude > 0)
                {
                    MoveDirection moveDirection;
                    if (Mathf.Abs(moveVector.x) > Mathf.Abs(moveVector.y))
                    {
                        moveDirection = moveVector.x > 0 ? MoveDirection.Right : MoveDirection.Left;
                    }
                    else
                    {
                        moveDirection = moveVector.y > 0 ? MoveDirection.Up : MoveDirection.Down;
                    }

                    if (navigationMoveDir != moveDirection)
                    {
                        navigationMoveCount = 0;
                    }

                    bool allow = true;
                    float time = Time.unscaledTime;
                    if (navigationMoveCount != 0)
                    {
                        if (navigationMoveCount > 1)
                        {
                            allow = time > navigationTime + inputModule.moveRepeatRate;
                        }
                        else
                        {
                            allow = time > navigationTime + inputModule.moveRepeatDelay;
                        }
                    }

                    if (allow)
                    {
                        navigationMoveDir = moveDirection;
                        navigationMoveCount++;
                        navigationTime = time;
                        UpdateNavigation(moveDirection);
                    }
                }
                else
                {
                    navigationMoveCount = 0;
                }
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!Cursor.visible || !UIManager.SendMouseNavigation)
            {
                return;
            }

            if (EventSystem.current != null && selectable.navigation.mode != Navigation.Mode.None)
            {
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);
            }
            else
            {
                isPointEnter = true;
                DoStateTransition();
            }
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!Cursor.visible)
            {
                return;
            }

            isPointEnter = false;
            DoStateTransition();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            isPointDown = true;
            DoStateTransition();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            isPointDown = false;
            DoStateTransition();
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            panel?.OnCancel();
        }

        protected void DoStateTransition()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            string triggerName = GetTriggerName();
            if (string.IsNullOrEmpty(triggerName))
            {
                return;
            }

            ResetTrigger(normalTrigger);
            ResetTrigger(highlightedTrigger);
            ResetTrigger(pressedTrigger);
            ResetTrigger(disabledTrigger);
            animator.SetTrigger(triggerName);
        }

        private string GetTriggerName()
        {
            if (!selectable.interactable)
            {
                return disabledTrigger;
            }

            if (isPointDown)
            {
                if (!string.IsNullOrEmpty(pressedTrigger))
                {
                    return pressedTrigger;
                }

                return highlightedTrigger;
            }

            if (isSelected || isPointEnter)
            {
                return highlightedTrigger;
            }

            return normalTrigger;
        }

        private void ResetTrigger(string triggerName)
        {
            if (!string.IsNullOrEmpty(triggerName))
            {
                animator.ResetTrigger(triggerName);
            }
        }

        private void UpdateNavigation(MoveDirection moveDirection)
        {
            Navigation.Mode targetNavigationMode = Navigation.Mode.None;
            if (navigationDirection > 0)
            {
                targetNavigationMode = Navigation.Mode.Explicit;
            }

            Navigation navigation = selectable.navigation;
            if (navigation.mode != targetNavigationMode)
            {
                navigation.mode = targetNavigationMode;
            }

            switch (moveDirection)
            {
                case MoveDirection.Up:
                    if (cacheNavigation && navigation.selectOnUp != null)
                    {
                        break;
                    }

                    if ((navigationDirection & NavigationDirection.Up) == NavigationDirection.Up)
                    {
                        navigation.selectOnUp = FindSelectable(transform.rotation * Vector3.up);
                    }
                    else
                    {
                        navigation.selectOnUp = null;
                    }

                    break;
                case MoveDirection.Down:
                    if (cacheNavigation && navigation.selectOnDown != null)
                    {
                        break;
                    }

                    if ((navigationDirection & NavigationDirection.Down) == NavigationDirection.Down)
                    {
                        navigation.selectOnDown = FindSelectable(transform.rotation * Vector3.down);
                    }
                    else
                    {
                        navigation.selectOnDown = null;
                    }

                    break;
                case MoveDirection.Left:
                    if (cacheNavigation && navigation.selectOnLeft != null)
                    {
                        break;
                    }

                    if ((navigationDirection & NavigationDirection.Left) == NavigationDirection.Left)
                    {
                        navigation.selectOnLeft = FindSelectable(transform.rotation * Vector3.left);
                    }
                    else
                    {
                        navigation.selectOnLeft = null;
                    }

                    break;
                case MoveDirection.Right:
                    if (cacheNavigation && navigation.selectOnRight != null)
                    {
                        break;
                    }

                    if ((navigationDirection & NavigationDirection.Right) == NavigationDirection.Right)
                    {
                        navigation.selectOnRight = FindSelectable(transform.rotation * Vector3.right);
                    }
                    else
                    {
                        navigation.selectOnRight = null;
                    }

                    break;
            }

            selectable.navigation = navigation;
        }

        private Selectable FindSelectable(Vector3 dir)
        {
            dir = dir.normalized;
            Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
            Vector3 pos = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, localDir));
            float maxScore = Mathf.NegativeInfinity;
            float score = 0;
            Selectable bestPick = null;

            if (selectables == null || selectables.Length < Selectable.allSelectableCount)
            {
                selectables = new Selectable[Selectable.allSelectableCount + 10];
            }

            Selectable.AllSelectablesNoAlloc(selectables);
            for (int i = 0; i < Selectable.allSelectableCount; ++i)
            {
                Selectable sel = selectables[i];
                selectables[i] = null;
                if (sel == selectable)
                {
                    continue;
                }

                if (!sel.IsInteractable() || sel.navigation.mode == Navigation.Mode.None)
                {
                    continue;
                }
#if UNITY_EDITOR
                if (Camera.current != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, Camera.current))
                {
                    continue;
                }
#endif
                if (navigationLimitSameParent && sel.transform.parent != selectable.transform.parent)
                {
                    continue;
                }

                var selRect = sel.transform as RectTransform;
                Vector3 selCenter = selRect != null ? selRect.rect.center : Vector3.zero;
                Vector3 myVector = sel.transform.TransformPoint(selCenter) - pos;

                if (navigationLimitAngle > 0 && Vector3.Angle(dir, myVector) > navigationLimitAngle)
                {
                    continue;
                }

                float dot = Vector3.Dot(dir, myVector);
                if (dot <= 0)
                {
                    continue;
                }

                score = dot / myVector.sqrMagnitude;
                if (score > maxScore)
                {
                    maxScore = score;
                    bestPick = sel;
                }
            }

            return bestPick;
        }

        private Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
            {
                return Vector3.zero;
            }

            if (dir != Vector2.zero)
            {
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            }

            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }

        [Flags]
        public enum NavigationDirection
        {
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8
        }
    }
}
