using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Tests.Destinations
{
    [TestFixture]
    public class ChainedPostActionDestinationTests
    {
        private Mock<IDestination> mockDestination;

        private Mock<IPostAction> mockPostAction;

        private VideoHeader header;

        private Mock<Stream> stream;

        private ChainedPostActionDestination instance;

        [SetUp]
        public void SetUp()
        {
            header = new VideoHeader(new CameraDescription("Test", "local"), "test.mov");
            stream = new Mock<Stream>();
            mockDestination = new Mock<IDestination>();
            mockPostAction = new Mock<IPostAction>();
            instance = CreateChainedPostActionDestination();
        }
        [Test]
        public void ResolveName()
        {
            mockDestination.Setup(item => item.ResolveName(It.IsAny<VideoHeader>())).Returns("test");
            var name = instance.ResolveName(header);
            Assert.AreEqual("test", name);
        }

        [Test]
        public void IsDownloaded()
        {
            mockDestination.Setup(item => item.IsDownloaded(It.IsAny<VideoHeader>())).Returns(true);
            var result = instance.IsDownloaded(header);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task Transfer()
        {
            mockPostAction.Setup(item => item.AfterTransfer("Test")).Returns(Task.FromResult(true));
            mockDestination.Setup(item => item.ResolveName(header)).Returns("Test");
            mockDestination.Setup(item => item.Transfer(It.IsAny<VideoHeader>(), stream.Object)).Returns(Task.CompletedTask);
            await instance.Transfer(header, stream.Object);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ChainedPostActionDestination(null, mockPostAction.Object));
            Assert.Throws<ArgumentNullException>(() => new ChainedPostActionDestination(mockDestination.Object, null));
        }

        private ChainedPostActionDestination CreateChainedPostActionDestination()
        {
            return new ChainedPostActionDestination(
                mockDestination.Object,
                mockPostAction.Object);
        }
    }
}