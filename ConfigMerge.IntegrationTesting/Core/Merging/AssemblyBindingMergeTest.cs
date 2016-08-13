using System.Xml.XPath;
using ConfigMerge.Core.Merging;
using NUnit.Framework;

namespace ConfigMerge.IntegrationTesting.Core.Merging
{
    [TestFixture]
    public class AssemblyBindingMergeTest
    {
        private XmlMerger _merger;
        private static readonly XmlSource RootConfig;
        private static readonly XmlSource OverrideConfig;

        static AssemblyBindingMergeTest()
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
        public void OverrideDependentAssembly()
        {
            var result = _merger.Merge(new [] { RootConfig, OverrideConfig});

            var overridden = result.XPathSelectElementOrThrow("configuration//runtime//assemblyBinding//dependentAssembly//assemblyIdentity[@name='To.Be.Overridden']")?.Parent;
            var bindingRedirect = overridden.Element("bindingRedirect");
            Assert.That(bindingRedirect.Attribute("oldVersion").Value, Is.EqualTo("overriddenOldVersion"));
            Assert.That(bindingRedirect.Attribute("newVersion").Value, Is.EqualTo("overriddenNewVersion"));
        }

        [Test]
        public void DeleteDependentAssembly()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });

            var identifier = result.XPathSelectElement("configuration//runtime//assemblyBinding//dependentAssembly//assemblyIdentity[@name='To.Be.Deleted']");
            Assert.That(identifier, Is.Null);
        }

        [Test]
        public void UnchangedDependentAssembly()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var leftAlone = result.XPathSelectElementOrThrow("configuration//runtime//assemblyBinding//dependentAssembly//assemblyIdentity[@name='To.Be.Left.Alone']")?.Parent;
            var bindingRedirect = leftAlone.Element("bindingRedirect");
            Assert.That(bindingRedirect.Attribute("oldVersion").Value, Is.EqualTo("originalOldVersion"));
            Assert.That(bindingRedirect.Attribute("newVersion").Value, Is.EqualTo("originalNewVersion"));
        }

        [Test]
        public void AddDependentAssembly()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var added = result.XPathSelectElementOrThrow("configuration//runtime//assemblyBinding//dependentAssembly//assemblyIdentity[@name='To.Be.Added']")?.Parent;
            Assert.That(added, Is.Not.Null);
        }
    }
}