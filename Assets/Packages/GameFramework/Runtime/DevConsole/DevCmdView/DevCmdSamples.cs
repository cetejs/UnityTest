using System;
using UnityEngine;

namespace GameFramework
{
    internal static class DevCmdSamples
    {
        [DevCmd("log", "output log.")]
        public static void Log(string value)
        {
            Debug.Log(value);
        }

        [DevCmd("log -w", "output log warning.")]
        public static void LogWarning(string value)
        {
            Debug.LogWarning(value);
        }

        [DevCmd("log -e", "output log error.")]
        public static void LogError(string value)
        {
            Debug.LogError(value);
        }
        
        [DevCmd("time", "output now time.")]
        public static void NowTime()
        {
            Debug.Log(DateTime.Now);
        }

        [DevCmd("time -s", "output log error.")]
        public static void TimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
            Debug.Log(Time.timeScale);
        }

        [DevCmd("sys", "out put system info.")]
        public static void LogSystemInfo()
        {
            Debug.Log(DebugUtility.GetSystemInfo());
        }
    }
}
