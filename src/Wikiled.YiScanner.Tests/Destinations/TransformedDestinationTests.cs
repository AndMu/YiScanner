using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Tests.Destinations
{
    [TestFixture]
    public class TransformedDestinationTests
    {
        private Mock<IDestination> mockDestination;

        private Func<string, string> renameFunc;

        private TransformedDestination instance;

        private VideoHeader header;

        private Mock<Stream> stream;

        [SetUp]
        public void SetUp()
        {
            header = new VideoHeader(new HostInformation("Test", IPAddress.Any), "test.mov");
            mockDestination = new Mock<IDestination>();
            renameFunc = name => "test";
            stream = new Mock<Stream>();
            instance = CreateTransformedDestination(); 
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
            VideoHeader inpuParameter = null;
            mockDestination.Setup(item => item.ResolveName(It.IsAny<VideoHeader>())).Returns("test")
                           .Callback<VideoHeader>(
                               parameter => { inpuParameter = parameter; });
            var name = instance.ResolveName(header);
            Assert.AreEqual("test", name);
            Assert.AreEqual("test", inpuParameter.FileName);
        }

        [Test]
        public void IsDownloaded()
        {
            VideoHeader inpuParameter = null;
            mockDestination.Setup(item => item.IsDownloaded(It.IsAny<VideoHeader>())).Returns(true)
                           .Callback<VideoHeader>(
                parameter => { inpuParameter = parameter; });
            var result = instance.IsDownloaded(header);
            Assert.IsTrue(result);
            Assert.AreEqual("test", inpuParameter.FileName);
        }

        [Test]
        public async Task Transfer()
        {
            VideoHeader inpuParameter = null;
            mockDestination.Setup(item => item.Transfer(It.IsAny<VideoHeader>(), stream.Object)).Returns(Task.CompletedTask)
                           .Callback<VideoHeader, Stream>(
                               (parameter, inputStream) => { inpuParameter = parameter; });
            await instance.Transfer(header, stream.Object);
            Assert.AreEqual("test", inpuParameter.FileName);
        }

        [Test]
        public void ChangeExtension()
        {
            VideoHeader inpuParameter = null;
            mockDestination.Setup(item => item.ResolveName(It.IsAny<VideoHeader>())).Returns("test")
                           .Callback<VideoHeader>(
                               parameter => { inpuParameter = parameter; });
            var result = TransformedDestination.ChangeExtension(mockDestination.Object, "png");
            result.ResolveName(header);
            Assert.AreEqual("test.png", inpuParameter.FileName);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new TransformedDestination(null, renameFunc));
            Assert.Throws<ArgumentNullException>(() => new TransformedDestination(mockDestination.Object, null));
        }

        private TransformedDestination CreateTransformedDestination()
        {
            return new TransformedDestination(mockDestination.Object, renameFunc);
        }
    }
}