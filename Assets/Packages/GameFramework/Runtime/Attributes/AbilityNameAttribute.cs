using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AbilityNameAttribute : Attribute
    {
        public readonly string Name;

        public AbilityNameAttribute(string name)
        {
            Name = name;
        }
    }
}
