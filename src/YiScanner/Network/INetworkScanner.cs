using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Network
{
    public interface INetworkScanner
    {
        IObservable<IPAddress> FindAddresses(int port);

        Task<bool> ScanPort(IPAddress address, int port);

        IEnumerable<IPAddress> GetAllAdresses();
    }
}