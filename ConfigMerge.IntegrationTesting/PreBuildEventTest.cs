using System.Configuration;
using NUnit.Framework;

namespace ConfigMerge.IntegrationTesting
{
    [TestFixture]
    public class PreBuildEventTest
    {
        // App.config is merged as a pre build event. See App.config.recipe

        [Test]
        public void OverrideAppSetting()
        {
            Assert.That(ConfigurationManager.AppSettings["toBeOverridden"], Is.EqualTo("overridden"));
        }

        [Test]
        public void AddAppSetting()
        {
            Assert.That(ConfigurationManager.AppSettings["added"], Is.Not.Null);
        }

        [Test]
        public void DeleteAppSetting()
        {
            Assert.That(ConfigurationManager.AppSettings["toBeDeleted"], Is.Null);
        }

        [Test]
        public void UnchangedAppSetting()
        {
            Assert.That(ConfigurationManager.AppSettings["toBeLeftAlone"], Is.EqualTo("unchanged"));
        }

        [Test]
        public void OverrideConnectionString()
        {
            Assert.That(ConfigurationManager.ConnectionStrings["toBeOverridden"].ConnectionString, Is.EqualTo("overridden"));
        }

        [Test]
        public void DeleteConnectionString()
        {
            Assert.That(ConfigurationManager.ConnectionStrings["toBeDeleted"], Is.Null);
        }

        [Test]
        public void AddConnectionString()
        {
            Assert.That(ConfigurationManager.ConnectionStrings["added"], Is.Not.Null);
        }

        [Test]
        public void AddSection()
        {
            Assert.That(ConfigurationManager.GetSection("assemblyBinding"), Is.Not.Null);
        }

        [Test]
        public void DeleteSection()
        {
            Assert.That(ConfigurationManager.GetSection("system.runtime.caching"), Is.Null);
        }
    }
}