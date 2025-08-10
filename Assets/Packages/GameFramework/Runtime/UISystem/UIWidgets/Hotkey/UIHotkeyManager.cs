using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    public class UIHotkeyManager : MonoBehaviour
    {
        [SerializeField]
        private UIHotkeyDisplay prefab;
        private List<UIHotkeyDisplay> hotkeys = new List<UIHotkeyDisplay>();
        private ObjectPool<UIHotkeyDisplay> pool;

        private void Awake()
        {
            pool = new ObjectPool<UIHotkeyDisplay>(prefab, transform);
        }

        private void OnEnable()
        {
            UIManager.EventPool?.Register<GenericData<string, string>>(UIEventKey.RegisterHotkeyEvent, RegisterHotkey);
            UIManager.EventPool?.Register<GenericData<string, string>>(UIEventKey.UnregisterHotkeyEvent, UnregisterHotkey);
        }

        private void OnDisable()
        {
            UIManager.EventPool?.Unregister<GenericData<string, string>>(UIEventKey.RegisterHotkeyEvent, RegisterHotkey);
            UIManager.EventPool?.Unregister<GenericData<string, string>>(UIEventKey.UnregisterHotkeyEvent, UnregisterHotkey);
        }

        private void Update()
        {
            for (int i = hotkeys.Count - 1; i >= 0; i--)
            {
                if (hotkeys[i].ReadyToClear)
                {
                    pool.Release(hotkeys[i]);
                    hotkeys.RemoveAt(i);
                }
            }
        }

        private void RegisterHotkey(GenericData<string, string> data)
        {
            foreach (UIHotkeyDisplay hotkey in hotkeys)
            {
                if (hotkey.HotkeyId == data.Item1 && hotkey.HotkeyLabel == data.Item2)
                {
                    hotkey.RegisterCount++;
                    return;
                }
            }

            UIHotkeyDisplay newHotkey = pool.Get(transform);
            newHotkey.SetData(data.Item1, data.Item2);
            newHotkey.RegisterCount++;
            hotkeys.Add(newHotkey);
        }

        private void UnregisterHotkey(GenericData<string, string> data)
        {
            foreach (UIHotkeyDisplay hotkey in hotkeys)
            {
                if (hotkey.HotkeyId == data.Item1 && hotkey.HotkeyLabel == data.Item2)
                {
                    hotkey.RegisterCount--;
                    return;
                }
            }
        }
    }
}
