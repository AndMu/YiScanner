using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Tests.Destinations
{
    [TestFixture]
    public class ChainedActionDestinationTests
    {
        private Mock<IDestination> mockDestination;

        private Mock<IPriorAction> mockAction;

        private ChainedPriorActionDestination instance;

        private VideoHeader header;

        private Mock<Stream> stream;

        [SetUp]
        public void SetUp()
        {
            header = new VideoHeader(new HostInformation("Camera", IPAddress.Any), "test.mov");
            mockDestination = new Mock<IDestination>();
            mockAction = new Mock<IPriorAction>();
            stream = new Mock<Stream>();
            instance = CreateChainedActionDestination();
        }

        [Test]
        public void CheckArguments()
        {
            Assert.Throws<ArgumentNullException>(() => instance.IsDownloaded(null));
            Assert.Throws<ArgumentNullException>(() => instance.ResolveName(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => instance.Transfer(header, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => instance.Transfer(null, stream.Object));
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
            mockAction.Setup(item => item.BeforeTransfer(header, stream.Object))
                      .Returns(Task.FromResult((header, stream.Object)));
            mockDestination.Setup(item => item.ResolveName(header)).Returns("Test");
            mockDestination.Setup(item => item.Transfer(It.IsAny<VideoHeader>(), stream.Object)).Returns(Task.CompletedTask);
            await instance.Transfer(header, stream.Object).ConfigureAwait(false);
            mockAction.Verify(item => item.BeforeTransfer(header, stream.Object), Times.Exactly(1));
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new ChainedPriorActionDestination(null, mockAction.Object));
            Assert.Throws<ArgumentNullException>(() => new ChainedPriorActionDestination(mockDestination.Object, null));
        }

        private ChainedPriorActionDestination CreateChainedActionDestination()
        {
            return new ChainedPriorActionDestination(mockDestination.Object, mockAction.Object);
        }
    }
}