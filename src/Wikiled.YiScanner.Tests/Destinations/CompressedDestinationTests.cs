using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Tests.Destinations
{
    [TestFixture]
    public class CompressedDestinationTests
    {
        private IDestination destination;

        private CompressedDestination instance;

        private string outFile;

        [SetUp]
        public void Setup()
        {
            var outPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data");
            destination = new FileDestination(outPath);
            outFile = Path.Combine(outPath, "camera", "test.zip");
            instance = CreateCompressedDestination();
            
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(outFile))
            {
                File.Delete(outFile);
            }
        }

        [Test]
        public void Construct()
        {
            Assert.Throws<ArgumentNullException>(() => new CompressedDestination(null));
        }

        [Test]
        public async Task Transfer()
        {
            Assert.IsFalse(File.Exists(outFile));
            VideoHeader header = new VideoHeader(new CameraDescription("camera", "localhost"), "test.txt");
            using (StreamReader reader = new StreamReader(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Test.txt")))
            {
                await instance.Transfer(header, reader.BaseStream).ConfigureAwait(false);
            }

            Assert.IsTrue(File.Exists(outFile));
        }

        private CompressedDestination CreateCompressedDestination()
        {
            return new CompressedDestination(destination);
        }
    }
}
