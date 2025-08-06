using UnityEngine;

namespace GameFramework
{
    public class UIWindowControl : MonoBehaviour
    {
        public void ShowWindow(string path)
        {
            UIManager.ShowWindowAsync(path);
        }

        public void HideWindow(string path)
        {
            UIManager.HideWindow(path);
        }

        public void CloseWindow(string path)
        {
            UIManager.CloseWindow(path);
        }
    }
}
