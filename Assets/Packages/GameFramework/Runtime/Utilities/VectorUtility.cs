using UnityEngine;

namespace GameFramework
{
    public static class VectorUtility
    {
        public static bool Approximately(Vector2 v1, Vector2 v2, float eps = 1e-5f)
        {
            return Mathf.Abs(v1.x - v2.x) <= eps &&
                   Mathf.Abs(v1.y - v2.y) <= eps;
        }

        public static bool Approximately(Vector3 v1, Vector3 v2, float eps = 1e-5f)
        {
            return Mathf.Abs(v1.x - v2.x) <= eps &&
                   Mathf.Abs(v1.y - v2.y) <= eps &&
                   Mathf.Abs(v1.z - v2.z) <= eps;
        }

        public static bool Approximately(Vector4 v1, Vector4 v2, float eps = 1e-5f)
        {
            return Mathf.Abs(v1.x - v2.x) <= eps &&
                   Mathf.Abs(v1.y - v2.y) <= eps &&
                   Mathf.Abs(v1.z - v2.z) <= eps &&
                   Mathf.Abs(v1.w - v2.w) <= eps;
        }

        public static bool IsInScreen(Camera cam, Vector3 worldPoint)
        {
            Vector3 viewPoint = cam.worldToCameraMatrix.MultiplyPoint3x4(worldPoint);
            if (viewPoint.z > 0f)
            {
                return false;
            }

            Vector3 screenPoint = cam.WorldToScreenPoint(worldPoint);
            return IsInScreen(screenPoint);
        }

        public static bool IsInScreen(Vector2 screenPoint)
        {
            if (screenPoint.x < 0f || screenPoint.x > Screen.width ||
                screenPoint.y < 0f || screenPoint.y > Screen.height)
            {
                return false;
            }

            return true;
        }
    }
}
