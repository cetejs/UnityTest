using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        private readonly bool enableInEditor;

        public bool IsReadOnly
        {
            get { return enableInEditor || Application.isPlaying; }
        }

        public ReadOnlyAttribute()
        {
            enableInEditor = true;
        }

        public ReadOnlyAttribute(bool enableInEditor)
        {
            this.enableInEditor = enableInEditor;
        }
    }
}
