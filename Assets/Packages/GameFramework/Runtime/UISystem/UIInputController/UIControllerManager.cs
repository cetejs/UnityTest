using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;

namespace GameFramework
{
    internal class UIControllerManager
    {
        private string currentControllerName;
        private UIControllerPresetManager presetManager;
        private UIControllerPreset currentPreset;
        private Vector2 lastMousePosition;
        private GameObject lastSelectedGameObject;
        private float changeControllerTime;
        public bool sendNavigation = true;
        public bool mouseForceNavigation = false;
        public Action onControllerChanged;

        private const string KeyboardMouse = "Keyboard Mouse";
        private const string XboxController = "Xbox Controller";
        private const string PlayStationController = "PlayStation Controller";

        private static readonly List<GamepadButton> GamepadButtons = new List<GamepadButton>()
        {
            GamepadButton.DpadUp,
            GamepadButton.DpadDown,
            GamepadButton.DpadLeft,
            GamepadButton.DpadRight,
            GamepadButton.North,
            GamepadButton.East,
            GamepadButton.South,
            GamepadButton.West,
            GamepadButton.LeftStick,
            GamepadButton.RightStick,
            GamepadButton.LeftShoulder,
            GamepadButton.RightShoulder,
            GamepadButton.Start,
            GamepadButton.Select,
            GamepadButton.LeftTrigger,
            GamepadButton.RightTrigger,
        };

        public UIControllerItem GetControllerItem(string itemName)
        {
            return currentPreset?.GetControllerItem(itemName);
        }

        public void Awake()
        {
            presetManager = UISetting.Instance.ControllerPresetManager;
        }

        public void Start()
        {
            if (Gamepad.current != null)
            {
                SwitchToGamepad();
            }
            else
            {
                SwitchToKeyboard();
            }
        }

        public void Update()
        {
            if (presetManager == null)
            {
                return;
            }

            if (currentControllerName != KeyboardMouse)
            {
                if (Keyboard.current != null)
                {
                    if (Mouse.current.leftButton.wasPressedThisFrame ||
                        Mouse.current.rightButton.wasPressedThisFrame ||
                        Mouse.current.middleButton.wasPressedThisFrame ||
                        Mouse.current.scroll.value != Vector2.zero ||
                        Keyboard.current.anyKey.wasPressedThisFrame)
                    {
                        SwitchToKeyboard();
                    }
                }
            }

            if (currentControllerName == KeyboardMouse || Gamepad.all.Count > 1)
            {
                if (Gamepad.current != null)
                {
                    foreach (GamepadButton button in GamepadButtons)
                    {
                        if (Gamepad.current[button].wasPressedThisFrame ||
                            Gamepad.current.leftStick.value != Vector2.zero ||
                            Gamepad.current.rightStick.value != Vector2.zero)
                        {
                            SwitchToGamepad();
                        }
                    }
                }
            }

            if (sendNavigation)
            {
                if (mouseForceNavigation)
                {
                    if (EventSystem.current != null && EventSystem.current.currentInputModule is InputSystemUIInputModule inputModule)
                    {
                        if (inputModule.move.action.ReadValue<Vector2>() != Vector2.zero)
                        {
                            mouseForceNavigation = false;
                        }
                    }
                }

                if (currentControllerName != KeyboardMouse || !mouseForceNavigation)
                {
                    Vector2 mousePosition = Mouse.current.position.value;
                    if (lastMousePosition == Vector2.zero)
                    {
                        lastMousePosition = mousePosition;
                    }

                    if (mousePosition != lastMousePosition)
                    {
                        lastMousePosition = mousePosition;
                        SwitchToKeyboard();
                        mouseForceNavigation = true;
                    }
                }
            }
        }

        public void LateUpdate()
        {
            if (sendNavigation)
            {
                if (EventSystem.current != null)
                {
                    if (EventSystem.current.currentSelectedGameObject != null)
                    {
                        lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
                    }
                    else if (lastSelectedGameObject != null)
                    {
                        EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
                    }
                }
            }
        }

        private void SwitchToKeyboard()
        {
            SwitchController(KeyboardMouse);
            Cursor.visible = true;
        }

        private void SwitchToGamepad()
        {
            if (Gamepad.current is XInputController)
            {
                SwitchController(XboxController);
            }
            else if (Gamepad.current is DualShockGamepad)
            {
                SwitchController(PlayStationController);
            }
            else
            {
                SwitchController(Gamepad.current.displayName);
            }

            Cursor.visible = false;
        }

        private void SwitchController(string controllerName)
        {
            if (Time.unscaledTime < changeControllerTime)
            {
                return;
            }

            if (currentControllerName == controllerName || presetManager == null)
            {
                return;
            }

            currentControllerName = controllerName;

            if (currentPreset != null)
            {
                currentPreset.Release();
            }

            switch (controllerName)
            {
                case KeyboardMouse:
                    currentPreset = presetManager.KeyboardMousePreset;
                    break;
                case XboxController:
                    currentPreset = presetManager.XboxControllerPreset;
                    break;
                case PlayStationController:
                    currentPreset = presetManager.PlayStationControllerPreset;
                    break;
                default:
                    foreach (UIControllerPreset preset in presetManager.CustomControllerPresets)
                    {
                        if (preset.ControllerName == controllerName)
                        {
                            currentPreset = preset;
                            break;
                        }
                    }

                    break;
            }

            if (currentPreset != null)
            {
                currentPreset.Init();
            }

            onControllerChanged?.Invoke();
            changeControllerTime = Time.unscaledTime + 0.5f;
        }
    }
}
