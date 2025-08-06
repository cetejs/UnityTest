using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DevCmdAttribute : Attribute
    {
        public readonly string Cmd;
        public readonly string Info;

        public DevCmdAttribute(string name) : this(name, string.Empty)
        {
        }

        public DevCmdAttribute(string name, string info)
        {
            Cmd = name;
            Info = info;
        }
    }
}
