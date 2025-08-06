using UnityEngine;

namespace GameFramework
{
    public static class QuaternionUtility
    {
        public static Quaternion Multiply(Quaternion a, Quaternion b)
        {
            if (a == Quaternion.identity && b == Quaternion.identity)
            {
                return Quaternion.identity;
            }

            if (a == Quaternion.identity)
            {
                return b;
            }

            if (b == Quaternion.identity)
            {
                return a;
            }

            return a * b;
        }
    }
}
