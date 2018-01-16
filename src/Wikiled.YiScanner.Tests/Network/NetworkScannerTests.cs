using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Wikiled.YiScanner.Network;

namespace Wikiled.YiScanner.Tests.Network
{
    [TestFixture]
    public class NetworkScannerTests
    {

        private NetworkScanner instance;

        [SetUp]
        public void SetUp()
        {
            instance = CreateNetworkScanner();
        }
        
        [Test]
        public async Task FindAddress()
        {
            var result = await instance.FindAddresses(80).ToArray();
        }

        [Test]
        public void GetAllAdresses()
        {
            var result = instance.GetAllAdresses().ToArray();
            Assert.Greater(result.Length, 1);
        }

        private NetworkScanner CreateNetworkScanner()
        {
            return new NetworkScanner();
        }
    }
}