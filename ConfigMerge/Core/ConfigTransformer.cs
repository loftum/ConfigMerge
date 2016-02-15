using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigMerge.Core.IO;
using ConfigMerge.Core.Merging;
using ConfigMerge.Logging;

namespace ConfigMerge.Core
{
    public class ConfigTransformer : IConfigTransformer
    {
        private static readonly ILogger Log = Logger.For<ConfigTransformer>();

        private readonly FileHandler _fileHandler = new FileHandler();
        private readonly XmlMerger _merger;

        private readonly string _basePath;

        public ConfigTransformer(string basePath, TransformOptions options)
        {
            _basePath = basePath;
            _merger = new XmlMerger(options);
        }

        public void Transform(string outputFile, IEnumerable<string> inputFiles)
        {
            Log.Trace($"Given output: {outputFile}");
            Log.Trace($"Given inputs: {string.Join(", ", inputFiles)}");
            var outputPath = ToFullOrRelative(outputFile);
            var inputPaths = inputFiles.Select(ToFullOrRelative);
            DoTransform(outputPath, inputPaths);
        }

        private string ToFullOrRelative(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.Combine(_basePath, path);
        }

        private void DoTransform(string outputPath, IEnumerable<string> inputPaths)
        {
            var inputs = inputPaths.Select(_fileHandler.ReadXmlIfExists).ToList();
            foreach (var nonexisting in inputs.Where(i => !i.Exists))
            {
                Log.Important($"Input not found: {nonexisting.FilePath}");
            }
            var sources = inputs.Where(i => i.Exists).ToList();
            if (!sources.Any())
            {
                throw new Exception($"Cannot create {outputPath}. None of the input config files exist: [{string.Join(", ", inputs.Select(i => i.FilePath))}]");
            }
            Log.Log($"<== [{string.Join(", ", sources.Select(s => s.FilePath))}]");
            var result = _merger.Merge(sources);
            Log.Log($"==> {outputPath}");
            _fileHandler.WriteXml(outputPath, result);
        }
    }
}