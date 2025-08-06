using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameFramework
{
    internal static class DomainReset
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitializeOnLoad()
        {
            foreach (string assembly in GameSetting.Instance.AssemblyNames)
            {
                if (!AssemblyUtility.ExistAssembly(assembly))
                {
                    continue;
                }

                foreach (Type type in AssemblyUtility.GetTypes(Assembly.Load(assembly)))
                {
                    if (type.IsGenericType)
                    {
                        continue;
                    }

                    FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (FieldInfo fieldInfo in fieldInfos)
                    {
                        if (!fieldInfo.IsDefined(typeof(DomainResetAttribute)))
                        {
                            continue;
                        }

                        DomainResetAttribute attribute = fieldInfo.GetCustomAttribute<DomainResetAttribute>(true);
                        fieldInfo.SetValue(null, attribute.Value);
                    }

                    PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (!propertyInfo.IsDefined(typeof(DomainResetAttribute)))
                        {
                            continue;
                        }

                        DomainResetAttribute attribute = propertyInfo.GetCustomAttribute<DomainResetAttribute>(true);
                        propertyInfo.SetValue(null, attribute.Value);
                    }

                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (MethodInfo methodInfo in methodInfos)
                    {
                        if (!methodInfo.IsDefined(typeof(DomainResetAttribute)))
                        {
                            continue;
                        }

                        methodInfo.Invoke(null, null);
                    }
                }
            }
        }
    }
}
