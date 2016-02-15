using System;

namespace ConfigMerge.Common
{
    public static class StringExtensions
    {
        public static string[] SplitLines(this string value)
        {
            return value?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }
    }
}