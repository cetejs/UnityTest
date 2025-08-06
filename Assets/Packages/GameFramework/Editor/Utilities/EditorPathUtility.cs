using System.IO;

namespace GameFramework
{
    public static class EditorPathUtility
    {
        public static string GetPackageFullPath()
        {
            string package = Path.GetFullPath("Packages/com.cetejs.gameframework");
            if (Directory.Exists(package))
            {
                return package;
            }

            return Path.GetFullPath("Assets/Packages/GameFramework");
        }
    }
}
