using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Network
{
    public class NetworkScanner : INetworkScanner
    {
        public IObservable<IPAddress> FindAddresses(string network, int port)
        {
            IPNetwork ipnetwork = IPNetwork.Parse(network);
            return IPNetwork.ListIPAddress(ipnetwork)
                   .ToObservable()
                   .SelectMany(item => Observable.Start(async () => (await ScanPort(item, port).ConfigureAwait(false), item))
                                                 .Merge()
                                                 .Where(pair => pair.Item1)
                                                 .Select(pair => pair.Item2));
        }

        public async Task<bool> ScanPort(IPAddress address, int port)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(address, port).ConfigureAwait(false);
                    return client.Connected;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
