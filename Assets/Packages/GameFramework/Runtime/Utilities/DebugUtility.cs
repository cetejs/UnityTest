using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace GameFramework
{
    public static class DebugUtility
    {
        private static StringBuilder stringBuilder = new StringBuilder();

        public static string GetSystemInfo()
        {
            string result = "SYSTEM INFO";

#if UNITY_IOS
                 result += "\n[iPhone generation]iPhone.generation.ToString()";
#endif

#if UNITY_ANDROID
                result += "\n[system info]" + SystemInfo.deviceModel;
#endif

            result += "\nDevice Type : " + SystemInfo.deviceType;
            result += "\nOS Version : " + SystemInfo.operatingSystem;
            result += "\nSystem Memory Size : " + SystemInfo.systemMemorySize;
            result += "\nGraphic Device Name : " + SystemInfo.graphicsDeviceName + " (version " + SystemInfo.graphicsDeviceVersion + ")";
            result += "\nGraphic Memory Size : " + SystemInfo.graphicsMemorySize;
            result += "\nGraphic Max Texture Size : " + SystemInfo.maxTextureSize;
            result += "\nGraphic Shader Level : " + SystemInfo.graphicsShaderLevel;
            result += "\nCompute Shader Support : " + SystemInfo.supportsComputeShaders;

            result += "\nProcessor Count : " + SystemInfo.processorCount;
            result += "\nProcessor Type : " + SystemInfo.processorType;
            result += "\n3D Texture Support : " + SystemInfo.supports3DTextures;
            result += "\nShadow Support : " + SystemInfo.supportsShadows;

            result += "\nPlatform : " + Application.platform;
            result += "\nScreen Size : " + Screen.width + " x " + Screen.height;
            result += "\nDPI : " + Screen.dpi;

            return result;
        }
        
        public static string GetStackTrace(int skipCount)
        {
            stringBuilder.Clear();
            StackFrame skipFrame = null;
            StackFrame[] frames = new StackTrace(skipCount + 1).GetFrames();
            if (frames == null)
            {
                return null;
            }

            foreach (StackFrame frame in frames)
            {
                MethodBase method = frame.GetMethod();
                if (method.DeclaringType == null)
                {
                    continue;
                }

                string nameSpace = method.DeclaringType.Namespace;
                if (nameSpace != null && (nameSpace.StartsWith("UnityEngine") || nameSpace.StartsWith("System")))
                {
                    skipFrame = frame;
                    continue;
                }

                if (skipFrame != null)
                {
                    AddFrame(skipFrame);
                    skipFrame = null;
                }

                AddFrame(frame);
            }

            if (skipFrame != null)
            {
                AddFrame(skipFrame);
            }

            return stringBuilder.ToString();
        }

        private static void AddFrame(StackFrame frame)
        {
            MethodBase method = frame.GetMethod();
            stringBuilder.Append(method.DeclaringType);
            stringBuilder.Append(".");
            stringBuilder.Append(method);
            stringBuilder.Append("\n");
        }
    }
}
