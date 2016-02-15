namespace ConfigMerge.Core.Lang
{
    public struct SourcePosition
    {
        public IRecipeSource Source { get; }
        public int LineNumber { get; }
        public int ColumnNumber { get; }

        public SourcePosition(IRecipeSource source, int lineNumber, int columnNumber)
        {
            Source = source;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        public override string ToString()
        {
            return string.Format($"{Source.FullPath}({LineNumber},{ColumnNumber})");
        }
    }
}