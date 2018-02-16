using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Tests.Destinations
{
    [TestFixture]
    public class FileDestinationTests
    {
        private FileDestination instance;

        private string outPath;

        private VideoHeader header;

        private MemoryStream stream;

        [SetUp]
        public void SetUp()
        {
            header = new VideoHeader(new HostInformation("Camera", IPAddress.Any), "test.mov");
            outPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Out");
            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }

            instance = CreateFileDestination();
            stream = new MemoryStream(new byte[] { 1 });
        }

        [TearDown]
        public void TestCleanup()
        {
            stream.Dispose();
        }

        [Test]
        public void CheckArguments()
        {
            Assert.Throws<ArgumentNullException>(() => instance.IsDownloaded(null));
            Assert.Throws<ArgumentNullException>(() => instance.ResolveName(null));
            Assert.ThrowsAsync<ArgumentNullException>(() => instance.Transfer(header, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => instance.Transfer(null, stream));
        }

        [Test]
        public async Task TestFunctionality()
        {
            var result = instance.IsDownloaded(header);
            Assert.IsFalse(result);
            await instance.Transfer(header, stream).ConfigureAwait(false);
            result = instance.IsDownloaded(header);
            Assert.IsTrue(result);
            var name = instance.ResolveName(header);
            result = File.Exists(name);
            Assert.IsTrue(result);
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new FileDestination(null));
        }

        private FileDestination CreateFileDestination()
        {
            return new FileDestination(outPath);
        }
    }
}