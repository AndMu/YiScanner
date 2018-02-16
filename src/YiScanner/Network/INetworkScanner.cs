using System;
using System.Net;
using System.Threading.Tasks;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Network
{
    public interface INetworkScanner
    {
        IObservable<HostInformation> FindAddresses(string network, int port);

        Task<bool> ScanPort(IPAddress address, int port);
    }
}