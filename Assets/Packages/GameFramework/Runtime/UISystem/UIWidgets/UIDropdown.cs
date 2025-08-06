using TMPro;
using UnityEngine.EventSystems;

namespace GameFramework
{
    public class UIDropdown : TMP_Dropdown
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            GetComponentInChildren<UIPanel>()?.OnFocus();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            GetComponentInChildren<UIPanel>()?.OnFocus();
        }
    }
}
