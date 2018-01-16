using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
            var result = await instance.FindAddresses("192.168.0.0/255.255.255.0", 21).ToArray();
        }

        private NetworkScanner CreateNetworkScanner()
        {
            return new NetworkScanner();
        }
    }
}