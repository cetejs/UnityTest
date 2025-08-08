using DataStructure;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class DictionaryTest
    {
        [Test]
        public void Test()
        {
            CustomDictionary<string, string> dict = new CustomDictionary<string, string>();
            for (int i = 0; i < 4; i++)
            {
                string key = i.ToString();
                string value = (i + 10).ToString();
                dict.Add(key, value);
                Debug.Log($"Add Key = {key} Value = {value}");
            }

            for (int i = 0; i < 4; i++)
            {
                string key = i.ToString();
                Debug.Log($"Get Key = {key} Value = {dict[key]}");
            }

            for (int i = 0; i < 2; i++)
            {
                string key = i.ToString();
                Debug.Log($"Remove Key = {key} Success = {dict.RemoveKey(key)}");
            }

            for (int i = -4; i < 0; i++)
            {
                string key = i.ToString();
                string value = (i - 10).ToString();
                dict.Add(key, value);
                Debug.Log($"Add Key = {key} Value = {value}");
            }

            for (int i = -4; i < 0; i++)
            {
                string key = i.ToString();
                Debug.Log($"Get Key = {key} Value = {dict[key]}");
            }

            for (int i = 2; i < 4; i++)
            {
                string key = i.ToString();
                Debug.Log($"Get Key = {key} Value = {dict[key]}");
            }
        }
    }
}