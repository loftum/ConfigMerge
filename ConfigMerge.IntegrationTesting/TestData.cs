using System;
using System.IO;
using ConfigMerge.Core.IO;
using ConfigMerge.Core.Merging;

namespace ConfigMerge.IntegrationTesting
{
    public class TestData
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly FileHandler FileHandler = new FileHandler();

        public static XmlSource ReadXml(string filename)
        {
            var path = PathTo(filename);
            return FileHandler.ReadXml(path);
        }

        public static string PathTo(string filename)
        {
            return Path.Combine(BaseDirectory, "Inputs", filename);
        }
    }
}