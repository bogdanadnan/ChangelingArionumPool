using System;
namespace ChangelingBizUtils
{
    public static class Utils
    {
        public static string Sanitize(string source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return source;
            }

            char[] arr = source.ToCharArray();
            arr = Array.FindAll<char>(arr, (c => char.IsLetterOrDigit(c)));
            return new string(arr);
        }
    }
}
