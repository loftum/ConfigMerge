namespace ConfigMerge.Core.Lang
{
    public struct Token
    {
        public SourcePosition Position { get; }
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, SourcePosition position, string value)
        {
            Type = type;
            Position = position;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Position} {Type} '{Value}'";
        }
    }

    public enum TokenType
    {
        String,
        LineComment,
        Symbol,
        Punctuation,
        Whitespace,
        Number
    }
}