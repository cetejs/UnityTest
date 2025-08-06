using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class DomainResetAttribute : Attribute
    {
        public readonly object Value;

        public DomainResetAttribute()
        {
        }

        public DomainResetAttribute(object value)
        {
            Value = value;
        }
    }
}
