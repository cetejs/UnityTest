using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework
{
    public class UIToggleGroup : ToggleGroup
    {
        [SerializeField]
        private bool allowLoop;

        public void OnPrevious()
        {
            ChangeToggle(false);
        }

        public void OnNext()
        {
            ChangeToggle(true);
        }

        private void ChangeToggle(bool isNext)
        {
            if (m_Toggles.Count <= 1)
            {
                return;
            }

            int index = 0;
            for (int i = 0; i < m_Toggles.Count; i++)
            {
                if (m_Toggles[i].isOn)
                {
                    index = i;
                    m_Toggles[i].isOn = false;
                    break;
                }
            }

            index = isNext ? index + 1 : index - 1;
            if (allowLoop)
            {
                if (index < 0)
                {
                    index = m_Toggles.Count - 1;
                }
                else if (index == m_Toggles.Count)
                {
                    index = 0;
                }
            }
            else
            {
                index = Mathf.Clamp(index, 0, m_Toggles.Count - 1);
            }

            if (!m_Toggles[index].isOn)
            {
                m_Toggles[index].isOn = true;
                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(m_Toggles[index].gameObject);
                }
            }
        }
    }
}
