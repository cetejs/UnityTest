using UnityEngine;

namespace DataStructure
{
    public static class Vector4Util
    {
        public static CustomVector4 ToCustomVector4(this Vector4 v)
        {
            return new CustomVector4(v.x, v.y, v.z, v.w);
        }

        public static Vector4 ToVector4(this CustomVector4 v)
        {
            return new Vector4(v.x, v.y, v.z, v.w);
        }

        public static Vector3 RandomVector3(float v)
        {
            return new Vector3(Random.Range(-v, v), Random.Range(-v, v), Random.Range(-v, v));
        }
    }
}