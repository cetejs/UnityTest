using System.Text;

namespace GameFramework
{
    public static class StringBuilderExtension
    {
        public static void RemoveFirstCount(this StringBuilder builder, int count = 1)
        {
            if (builder.Length < count)
            {
                return;
            }

            builder.Remove(0, count);
        }

        public static void RemoveLastCount(this StringBuilder builder, int count = 1)
        {
            if (builder.Length < count)
            {
                return;
            }

            builder.Remove(builder.Length - count, count);
        }

        public static void AppendPadLeft(this StringBuilder builder, string text, int width)
        {
            builder.Append(' ', width);
            builder.Append(text);
        }
    }
}
