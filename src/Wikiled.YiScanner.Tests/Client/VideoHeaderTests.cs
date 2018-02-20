using System;
using System.Net;
using NUnit.Framework;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Tests.Client
{
    [TestFixture]
    public class VideoHeaderTests
    {
        private Host cameraDescription;

        private VideoHeader instance;

        [SetUp]
        public void Setup()
        {
            cameraDescription = new Host("Test", IPAddress.None);
            instance = CreateVideoHeader();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new VideoHeader(null, "Test"));
            Assert.Throws<ArgumentNullException>(() => new VideoHeader(cameraDescription, null));
            Assert.AreEqual(cameraDescription, instance.Camera);
            Assert.AreEqual("Test", instance.FileName);
        }

        private VideoHeader CreateVideoHeader()
        {
            return new VideoHeader(cameraDescription, "Test");
        }
    }
}
