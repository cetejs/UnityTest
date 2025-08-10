using GameFramework;
using UnityEngine;

namespace RougeSouls
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            UIManager.ShowWindowAsync("MenuWindow");
            UIManager.ShowWindowAsync("HotkeyWindow");
            InitGameSettings();
        }

        private void InitGameSettings()
        {
            IPersistentStorage storage = PersistentManager.Load("GameSettings");

            int width = storage.GetData("Resolution.x", Screen.currentResolution.width);
            int height = storage.GetData("Resolution.y", Screen.currentResolution.height);
            int fullScreen = storage.GetData("FullScreen", (int) Screen.fullScreenMode);
            Screen.SetResolution(width, height, (FullScreenMode) fullScreen);

            string languageType = storage.GetData("Localization", "EN");
            Localization.ChangeLanguage(languageType);
        }
    }
}
