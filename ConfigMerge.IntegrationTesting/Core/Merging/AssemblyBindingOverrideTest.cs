using System;
using ConfigMerge.Core.Merging;
using NUnit.Framework;

namespace ConfigMerge.IntegrationTesting.Core.Merging
{
    [TestFixture]
    public class AssemblyBindingOverrideTest
    {
        private XmlMerger _merger;
        private static readonly XmlSource RootConfig;
        private static readonly XmlSource OverrideConfig;

        static AssemblyBindingOverrideTest()
        {
            RootConfig = TestData.ReadXml("App.assemblybinding.root.config");
            OverrideConfig = TestData.ReadXml("App.assemblybinding.override.config");
        }

        [SetUp]
        public void Setup()
        {
            _merger = new XmlMerger(new TransformOptions());
        }

        [Test]
        public void MergeAssemblyRedirects()
        {
            var result = _merger.Merge(new [] { RootConfig, OverrideConfig});
            Console.WriteLine(result);
        }
    }
}