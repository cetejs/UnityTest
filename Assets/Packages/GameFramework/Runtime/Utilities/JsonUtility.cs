using System;
using LitJson;
using UnityEngine;

namespace GameFramework
{
    public static class JsonUtility
    {
        public static string ToJson(object obj)
        {
            try
            {
                return JsonMapper.ToJson(obj);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

        public static object ToObject(string json, Type type)
        {
            try
            {
                return JsonMapper.ToObject(json, type);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return default;
        }

        public static T ToObject<T>(string json)
        {
            try
            {
                return JsonMapper.ToObject<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return default;
        }

        public static string ConvertToJson<T>(T obj)
        {
            if (obj == null)
            {
                Debug.LogError("Object to json is fail, because obj is invalid");
                return null;
            }

            return ConvertToJson(obj, typeof(T));
        }

        public static string ConvertToJson(object obj, Type type)
        {
            if (obj == null)
            {
                Debug.LogError("Object to json is fail, because obj is invalid");
                return null;
            }

            if (IsComplexObject(type))
            {
                return ToJson(obj);
            }

            if (type == typeof(bool))
            {
                return Convert.ToByte(obj).ToString();
            }

            return obj.ToString();
        }

        public static T ConvertToObject<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError("Json to object is fail, because text is invalid");
                return default;
            }

            return (T)ConvertToObject(text, typeof(T));
        }

        public static object ConvertToObject(string text, Type type)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError("Json to object is fail, because text is invalid");
                return null;
            }

            if (IsComplexObject(type))
            {
                return ToObject(text, type);
            }

            object value;
            if (type == typeof(bool))
            {
                value = byte.Parse(text) > 0;
            }
            else if (type == typeof(byte))
            {
                value = byte.Parse(text);
            }
            else if (type == typeof(sbyte))
            {
                value = sbyte.Parse(text);
            }
            else if (type == typeof(char))
            {
                value = char.Parse(text);
            }
            else if (type == typeof(double))
            {
                value = double.Parse(text);
            }
            else if (type == typeof(decimal))
            {
                value = decimal.Parse(text);
            }
            else if (type == typeof(short))
            {
                value = short.Parse(text);
            }
            else if (type == typeof(ushort))
            {
                value = ushort.Parse(text);
            }
            else if (type == typeof(int))
            {
                value = int.Parse(text);
            }
            else if (type == typeof(uint))
            {
                value = uint.Parse(text);
            }
            else if (type == typeof(long))
            {
                value = long.Parse(text);
            }
            else if (type == typeof(ulong))
            {
                value = ulong.Parse(text);
            }
            else if (type == typeof(float))
            {
                value = float.Parse(text);
            }
            else
            {
                value = text;
            }

            return value;
        }

        public static bool TryToObject(string json, Type type, out object value)
        {
            try
            {
                value = JsonMapper.ToObject(json, type);
            }
            catch (Exception e)
            {
                value = null;
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        public static bool TryCovertToObject<T>(string text, out T value)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError("Json to object is fail, because text is invalid");
                value = default;
            }

            bool success = TryCovertToObject(text, typeof(T), out object result);
            value = (T)result;
            return success;
        }

        public static bool TryCovertToObject(string text, Type type, out object value)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogError("Json to object is fail, because text is invalid");
                value = null;
                return false;
            }

            if (IsComplexObject(type))
            {
                return TryToObject(text, type, out value);
            }

            bool success;
            if (type == typeof(bool))
            {
                success = byte.TryParse(text, out byte result);
                value = result > 0;
            }

            if (type == typeof(byte))
            {
                success = byte.TryParse(text, out byte result);
                value = result;
            }
            else if (type == typeof(sbyte))
            {
                success = sbyte.TryParse(text, out sbyte result);
                value = result;
            }
            else if (type == typeof(char))
            {
                success = char.TryParse(text, out char result);
                value = result;
            }
            else if (type == typeof(double))
            {
                success = double.TryParse(text, out double result);
                value = result;
            }
            else if (type == typeof(decimal))
            {
                success = decimal.TryParse(text, out decimal result);
                value = result;
            }
            else if (type == typeof(short))
            {
                success = short.TryParse(text, out short result);
                value = result;
            }
            else if (type == typeof(ushort))
            {
                success = ushort.TryParse(text, out ushort result);
                value = result;
            }
            else if (type == typeof(int))
            {
                success = int.TryParse(text, out int result);
                value = result;
            }
            else if (type == typeof(uint))
            {
                success = uint.TryParse(text, out uint result);
                value = result;
            }
            else if (type == typeof(long))
            {
                success = long.TryParse(text, out long result);
                value = result;
            }
            else if (type == typeof(ulong))
            {
                success = ulong.TryParse(text, out ulong result);
                value = result;
            }
            else if (type == typeof(float))
            {
                success = float.TryParse(text, out float result);
                value = result;
            }
            else
            {
                success = true;
                value = text;
            }

            return success;
        }

        private static bool IsComplexObject<T>(T value)
        {
            return !(typeof(T).IsPrimitive || value is string || value is decimal);
        }

        private static bool IsComplexObject(Type type)
        {
            return !(type.IsPrimitive || type == typeof(string) || type == typeof(decimal));
        }
    }
}
