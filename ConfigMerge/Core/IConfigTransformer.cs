using System.Collections.Generic;

namespace ConfigMerge.Core
{
    public interface IConfigTransformer
    {
        void Transform(string outputFile, IEnumerable<string> inputFiles);
    }
}