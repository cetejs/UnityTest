namespace GameFramework
{
    public static class PathUtility
    {
        public static string Combine(string path1, string path2)
        {
            return string.Concat(path1, "/", path2);
        }

        public static string Combine(params string[] values)
        {
            return string.Join("/", values);
        }

        public static string CombineWithExtension(string path1, string path2, string extension)
        {
            return string.Concat(path1, "/", path2, extension);
        }

        public static string CombineWithExtension(string extension, params string[] values)
        {
            return string.Concat(string.Join("/", values), extension);
        }
    }
}