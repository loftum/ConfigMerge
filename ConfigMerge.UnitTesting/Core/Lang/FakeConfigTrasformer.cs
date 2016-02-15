using System;
using System.Collections.Generic;
using ConfigMerge.Core;

namespace ConfigMerge.UnitTesting.Core.Lang
{
    public class FakeConfigTrasformer : IConfigTransformer
    {
        public void Transform(string outputFile, IEnumerable<string> inputFiles)
        {
            Console.WriteLine($"Transform({outputFile}, [{string.Join(", ", inputFiles)}])");
        }
    }
}