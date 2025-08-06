using System;

namespace GameFramework
{
    public static class CovertUtility
    {
        public static object ChangeType(object value, Type type)
        {
            double minValue;
            double maxValue;

            if (type == typeof(bool))
            {
                minValue = 0;
                maxValue = 1;
            }
            else if (type == typeof(byte))
            {
                minValue = byte.MinValue;
                maxValue = byte.MaxValue;
            }
            else if (type == typeof(sbyte))
            {
                minValue = byte.MinValue;
                maxValue = byte.MaxValue;
            }
            else if (type == typeof(char))
            {
                minValue = char.MinValue;
                maxValue = char.MaxValue;
            }
            else if (type == typeof(double))
            {
                minValue = double.MinValue;
                maxValue = double.MaxValue;
            }
            else if (type == typeof(decimal))
            {
                minValue = double.MinValue;
                maxValue = double.MaxValue;
            }
            else if (type == typeof(short))
            {
                minValue = short.MinValue;
                maxValue = short.MaxValue;
            }
            else if (type == typeof(ushort))
            {
                minValue = ushort.MinValue;
                maxValue = ushort.MaxValue;
            }
            else if (type == typeof(int))
            {
                minValue = int.MinValue;
                maxValue = int.MaxValue;
            }
            else if (type == typeof(uint))
            {
                minValue = uint.MinValue;
                maxValue = uint.MaxValue;
            }
            else if (type == typeof(long))
            {
                minValue = long.MinValue;
                maxValue = long.MaxValue;
            }
            else if (type == typeof(ulong))
            {
                minValue = ulong.MinValue;
                maxValue = ulong.MaxValue;
            }
            else if (type == typeof(float))
            {
                minValue = float.MinValue;
                maxValue = float.MaxValue;
            }
            else
            {
                minValue = 0;
                maxValue = 0;
            }

            double doubleValue = Convert.ToDouble(value);
            doubleValue = Math.Clamp(doubleValue, minValue, maxValue);
            return Convert.ChangeType(doubleValue, type);
        }
    }
}
