using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace GameFramework
{
    public class UIInputField : TMP_InputField
    {
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (!isFocused)
            {
                return;
            }

            if (eventData.currentInputModule is InputSystemUIInputModule inputModule)
            {
                if (inputModule.submit.action.WasPressedThisFrame())
                {
                    KeyPressed(Event.KeyboardEvent("[enter]"));
                    SendOnSubmit();
                    DeactivateInputField();
                    eventData.Use();
                    return;
                }

                if (inputModule.cancel.action.WasPressedThisFrame())
                {
                    KeyPressed(Event.KeyboardEvent("[esc]"));
                    DeactivateInputField();
                    eventData.Use();
                    return;
                }
            }

            base.OnUpdateSelected(eventData);
        }
    }
}
