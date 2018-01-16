using System;
using System.Net;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Network
{
    public interface INetworkScanner
    {
        IObservable<IPAddress> FindAddresses(string network, int port);

        Task<bool> ScanPort(IPAddress address, int port);
    }
}