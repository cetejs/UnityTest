using UnityEngine;

namespace GameFramework
{
    public static class DevGUISamples
    {
#if ENABLE_CONSOLE
        [RuntimeInitializeOnLoadMethod]
        private static void Initialization()
        {
            IDevGUISection commonSection = DevConsole.AddSection("Common");
            commonSection.AddSelector("Debug", () => Debug.unityLogger.logEnabled, v => Debug.unityLogger.logEnabled = v);
            commonSection.AddMethod<string>("Log", text => Debug.Log(text));
            commonSection.AddMethod<string>("LogWarring", text => Debug.LogWarning(text));
            commonSection.AddMethod<string>("LogError", text => Debug.LogError(text));
            commonSection.AddSelector("TimeScale", () => Time.timeScale, v => Time.timeScale = v, 0.1f);
            commonSection.AddSelector("FrameRate", () => Application.targetFrameRate, v => Application.targetFrameRate = v, 10);
        }
#endif
    }
}
