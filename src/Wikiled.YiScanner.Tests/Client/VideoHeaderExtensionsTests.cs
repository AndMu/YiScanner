using NUnit.Framework;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Tests.Client
{
    [TestFixture]
    public class VideoHeaderExtensionsTests
    {
        [TestCase("file", @"c:\out\Camera\file")]
        [TestCase(@"c:\file", @"c:\out\Camera\file")]
        [TestCase(@"c:\Dir\file", @"c:\out\Camera\Dir\file")]
        [TestCase(@"c:\Dir2\Dir\file", @"c:\out\Camera\Dir\file")]
        public void GetPath(string fileName, string expected)
        {
            VideoHeader header = new VideoHeader(new CameraDescription("Camera", "Host"), fileName);
            var result = header.GetPath(@"c:\out");
            Assert.AreEqual(expected, result);
        }
    }
}
