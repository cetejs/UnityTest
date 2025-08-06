using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GameFramework
{
    internal static class ExcelToCharacterSetEditor
    {
        private static string CommonCharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789`,.;:\"'?/\\!@#$%^&*()_+=-[]{}|<>";

        public static void Build(DataTable dataTable)
        {
            if (dataTable.Rows.Count < 3 || dataTable.Columns.Count < 2)
            {
                return;
            }

            DataTableSetting setting = DataTableSetting.Instance;
            string fullPath = string.Concat(setting.OutputCharacterSetPath, ".txt");
            HashSet<char> characterSet = new HashSet<char>();
            if (setting.ExportCommonCharacterSet)
            {
                for (int i = 0; i < CommonCharacterSet.Length; i++)
                {
                    characterSet.Add(CommonCharacterSet[i]);
                }
            }

            for (int i = 3; i < dataTable.Rows.Count; i++)
            {
                for (int j = 1; j < dataTable.Columns.Count; j++)
                {
                    string text = dataTable.Rows[i][j].ToString();
                    for (int k = 0; k < text.Length; k++)
                    {
                        characterSet.Add(text[k]);
                    }
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var character in characterSet)
            {
                stringBuilder.Append(character);
            }

            FileUtility.WriteAllText(fullPath, stringBuilder.ToString());
        }
    }
}
