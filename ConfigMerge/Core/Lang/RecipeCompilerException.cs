using System;

namespace ConfigMerge.Core.Lang
{
    public class RecipeCompilerException : Exception
    {
        public RecipeCompilerException(SourcePosition position, string message) : base(Format(position, message))
        {
        }

        private static string Format(SourcePosition position, string message)
        {
            return $"{position}: error: {message}";
        }

        public static RecipeCompilerException UnexpectedEndAt(Token token)
        {
            return new RecipeCompilerException(token.Position, "Unexpected end of recipe");
        }
    }
}