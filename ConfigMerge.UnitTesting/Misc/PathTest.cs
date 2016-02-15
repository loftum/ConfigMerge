using System.IO;
using NUnit.Framework;

namespace ConfigMerge.UnitTesting.Misc
{
    [TestFixture]
    public class PathTest
    {
        [Test]
        public void RootedPathIsRooted()
        {
            Assert.That(Path.IsPathRooted(@"\something"));
        }

        [Test]
        public void RelativePathIsNotRooted()
        {
            Assert.That(Path.IsPathRooted(@".\something"), Is.False);
        }

        [Test]
        public void RootedUnixStylePathIsRooted()
        {
            Assert.That(Path.IsPathRooted(@"/something"));
        }
    }
}