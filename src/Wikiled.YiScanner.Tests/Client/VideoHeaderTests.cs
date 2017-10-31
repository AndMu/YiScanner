using Moq;
using NUnit.Framework;
using System;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Tests.Client
{
    [TestFixture]
    public class VideoHeaderTests
    {
        private CameraDescription cameradescription;

        private VideoHeader instance;

        [SetUp]
        public void Setup()
        {
            cameradescription = new CameraDescription("Test", "test");
            instance = CreateVideoHeader();
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new VideoHeader(null, "Test"));
            Assert.Throws<ArgumentNullException>(() => new VideoHeader(cameradescription, null));
            Assert.AreEqual(cameradescription, instance.Camera);
            Assert.AreEqual("Test", instance.FileName);
        }

        private VideoHeader CreateVideoHeader()
        {
            return new VideoHeader(cameradescription, "Test");
        }
    }
}
