namespace ConfigMerge.Core.Lang
{
    public static class CharacterExtensions
    {
        public static bool IsSymbol(this char c)
        {
            return char.IsSymbol(c);
        }

        public static bool IsLetter(this char c)
        {
            return char.IsLetter(c);
        }

        public static bool IsPunctuation(this char c)
        {
            return char.IsPunctuation(c);
        }

        public static bool IsLetterOrDigit(this char c)
        {
            return char.IsLetterOrDigit(c);
        }

        public static bool IsNumber(this char c)
        {
            return char.IsNumber(c);
        }

        public static bool IsWhitespace(this char c)
        {
            return char.IsWhiteSpace(c);
        }
    }
}