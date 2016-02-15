using ConfigMerge.Common;

namespace ConfigMerge.Core.Lang
{
    public static class TokenExtensions
    {
        public static bool LooksLikeFilePath(this Token token)
        {
            return token.Type == TokenType.String || token.Value.In(".", "/", "\\");
        }
    }
}