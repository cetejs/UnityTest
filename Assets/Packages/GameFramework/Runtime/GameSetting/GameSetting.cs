using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameFramework
{
    public class GameSetting : ScriptableObjectSingleton<GameSetting>
    {
        [InspectorGroup("Common")]
        public List<string> AssemblyNames = new List<string>()
        {
            "Assembly-CSharp",
            "Assembly-CSharp-Editor",
            "GameFramework",
            "GameFramework.Editor"
        };
        
        [InspectorGroup("DevConsole")]
        public KeyCode OpenConsole = KeyCode.Tab;
        public int ConsoleMaxLogCount = 1000;
    }
}
