using NUnit.Framework;
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

        private IDestination instance;

        private string outFile;

        [SetUp]
        public void Setup()
        {
            var outPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data");
            destination = new FileDestination(outPath);
            outFile = Path.Combine(outPath, "camera", "test.zip");
            instance = ChainedPriorActionDestination.CreateCompressed(destination);
            if (File.Exists(outFile))
            {
                File.Delete(outFile);
            }
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
    }
}
