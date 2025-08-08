using UnityEngine;

public static class Vector4Util {
    public static Vector4 ToCustomVector4(this UnityEngine.Vector4 v) {
        return new Vector4(v.x, v.y, v.z, v.w);
    }

    public static UnityEngine.Vector4 ToVector4(this Vector4 v) {
        return new UnityEngine.Vector4(v.x, v.y, v.z, v.w);
    }

    public static Vector3 RandomVector3(float v) {
        return new Vector3(Random.Range(-v, v), Random.Range(-v, v), Random.Range(-v, v));
    }
}