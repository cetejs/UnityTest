using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(Toggle))]
    public class UIToggleHeader : UISelectable, ISubmitHandler
    {
        [SerializeField]
        private UIPanel content;
        private Toggle toggle;
        private bool forceSelectContent;

        public UIPanel Content
        {
            get { return content; }
            set { content = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);
            forceSelectContent = toggle.navigation.mode == Navigation.Mode.None;
        }

        protected override void OnEnable()
        {
            isSelected = toggle.isOn;
            base.OnEnable();
        }

        private void Start()
        {
            OnValueChanged(toggle.isOn);
        }

        private void Update()
        {
            if (forceSelectContent && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                content.OnFocus();
            }
        }

        private void OnValueChanged(bool isOn)
        {
            isSelected = isOn;
            DoStateTransition();
            content.gameObject.SetActiveSafe(isOn);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            toggle.isOn = true;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            toggle.isOn = true;
            if (forceSelectContent)
            {
                content.OnFocus();
            }
            else
            {
                base.OnPointerEnter(eventData);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            content.OnFocus();
        }
    }
}
