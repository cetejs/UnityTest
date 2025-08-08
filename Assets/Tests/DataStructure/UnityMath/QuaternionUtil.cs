using UnityEngine;

namespace DataStructure
{
    public static class QuaternionUtil
    {
        public static CustomQuaternion ToCustomQuaternion(this Quaternion q)
        {
            return new CustomQuaternion(q.x, q.y, q.z, q.w);
        }

        public static Quaternion ToQuaternion(this CustomQuaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static float WrapPi(float a)
        {
            if (Mathf.Abs(a) > 180)
            {
                return a - 360 * Mathf.Floor((a + 180) / 360f);
            }

            return a;
        }
    }
}