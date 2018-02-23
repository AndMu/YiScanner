using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Network
{
    public interface INetworkScanner
    {
        IObservable<Host> FindAddresses(string network, int port);

        Task<bool> ScanPort(IPAddress address, int port);

        IEnumerable<IPAddress> GetLocalIPAddress();
    }
}