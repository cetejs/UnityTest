using UnityEngine;

public static class QuaternionUtil {
    public static Quaternion ToCustomQuaternion(this UnityEngine.Quaternion q) {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }
    
    public static UnityEngine.Quaternion ToQuaternion(this Quaternion q) {
        return new UnityEngine.Quaternion(q.x, q.y, q.z, q.w);
    }

    public static float WrapPi(float a) {
        if (Mathf.Abs(a) > 180) {
            return a - 360 * Mathf.Floor((a + 180) / 360f);
        }

        return a;
    }
}