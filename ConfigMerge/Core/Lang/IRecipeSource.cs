using System.Collections.Generic;
using System.IO;

namespace ConfigMerge.Core.Lang
{
    public interface IRecipeSource : IEnumerable<Token>
    {
        string BasePath { get; }
        string FullPath { get; }
        string Name { get; }
        Stream GetStream();
    }
}