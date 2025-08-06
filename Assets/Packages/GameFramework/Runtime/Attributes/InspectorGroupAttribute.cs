using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InspectorGroupAttribute : PropertyAttribute
    {
        public readonly string GroupName;
        public readonly bool GroupAllFieldsUntilNextGroupAttribute;

        public InspectorGroupAttribute(string groupName)
        {
            GroupName = groupName;
            GroupAllFieldsUntilNextGroupAttribute = true;
        }

        public InspectorGroupAttribute(string groupName, bool groupAllFieldsUntilNextGroupAttribute = true)
        {
            GroupName = groupName;
            GroupAllFieldsUntilNextGroupAttribute = groupAllFieldsUntilNextGroupAttribute;
        }
    }
}
