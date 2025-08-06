using System.IO;

namespace GameFramework
{
    public class DataTableSetting : ScriptableObjectSingleton<DataTableSetting>
    {
        [ReadOnly]
        public string RootPath;
        public string ExcelRootPath = "RawTables";
        public string TableBuildPath = "Assets/Configs/Tables";
        public string ScriptBuildPath = "Assets/Scripts/Tables";
        public string CharacterSetBuildPath = "Assets/CharacterSet";
        public string ScriptNamespace = "GameFramework";
        public string LocalizationName = "Localization";
        public bool ExportCommonCharacterSet = true;
        public bool UseCrypto = false;
        [Condition("UseCrypto", true)]
        public string Password = "password";

        private void OnEnable()
        {
            RootPath = Path.GetFullPath(".");
        }

        public string OutputTablePath
        {
            get { return PathUtility.Combine(RootPath, TableBuildPath); }
        }

        public string OutputScriptPath
        {
            get { return PathUtility.Combine(RootPath, ScriptBuildPath); }
        }

        public string OutputCharacterSetPath {
            
            get { return PathUtility.Combine(RootPath, CharacterSetBuildPath); }
        }

        public string LoadTablePath
        {
            get { return TableBuildPath; }
        }

        public string LoadLocalizationPath
        {
            get { return PathUtility.Combine(TableBuildPath, LocalizationName); }
        }
    }
}
