using System.Xml.XPath;
using ConfigMerge.Core.Merging;
using NUnit.Framework;

namespace ConfigMerge.IntegrationTesting.Core.Merging
{
    [TestFixture]
    public class XmlMergerTest
    {
        private XmlMerger _merger;
        private static readonly XmlSource RootConfig;
        private static readonly XmlSource OverrideConfig;

        static XmlMergerTest()
        {
            RootConfig = TestData.ReadXml("App.root.config");
            OverrideConfig = TestData.ReadXml("App.override.config");
        }

        [SetUp]
        public void Setup()
        {
            _merger = new XmlMerger(new TransformOptions());
        }

        [Test]
        public void OverrideAppSetting()
        {
            var result = _merger.Merge(new [] { RootConfig, OverrideConfig } );
            var overriddenSetting = result.XPathSelectElement("configuration//appSettings//add[@key='toBeOverridden']");

            var value = overriddenSetting.Attribute("value").Value;
            Assert.That(value, Is.EqualTo("overridden"));
        }

        [Test]
        public void AddAppSetting()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var addedSetting = result.XPathSelectElement("configuration//appSettings//add[@key='added']");

            Assert.That(addedSetting, Is.Not.Null);
        }

        [Test]
        public void DeleteAppSetting()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var deletedSetting = result.XPathSelectElement("configuration//appSettings//add[@key='toBeDeleted']");
            Assert.That(deletedSetting, Is.Null);
        }

        [Test]
        public void UnchangedAppSetting()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var leftAloneSetting = result.XPathSelectElement("configuration//appSettings//add[@key='toBeleftAlone']");
            var value = leftAloneSetting.Attribute("value").Value;
            Assert.That(value, Is.EqualTo("unchanged"));
        }

        [Test]
        public void OverrideConnectionString()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var overriddenConnectionString = result.XPathSelectElement("configuration//connectionStrings//add[@name='toBeOverridden']");
            var value = overriddenConnectionString.Attribute("connectionString").Value;
            Assert.That(value, Is.EqualTo("overridden"));
        }

        [Test]
        public void DeleteConnectionString()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var deletedConnectionString = result.XPathSelectElement("configuration//connectionStrings//add[@name='toBeDeleted']");
            Assert.That(deletedConnectionString, Is.Null);
        }

        [Test]
        public void AddConnectionString()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var addedConnectionString = result.XPathSelectElement("configuration//connectionStrings//add[@name='added']");

            Assert.That(addedConnectionString, Is.Not.Null);
        }

        [Test]
        public void AddSection()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var addedSection = result.XPathSelectElement("configuration/assemblyBinding");
            Assert.That(addedSection, Is.Not.Null);
        }

        [Test]
        public void DeleteSection()
        {
            var result = _merger.Merge(new[] { RootConfig, OverrideConfig });
            var deletedSecion = result.XPathSelectElement("configuration/system.runtime.caching");
            Assert.That(deletedSecion, Is.Null);
        }
    }
}