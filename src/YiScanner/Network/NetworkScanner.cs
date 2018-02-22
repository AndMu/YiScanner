using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Network
{
    public class NetworkScanner : INetworkScanner
    {
        private readonly ILookup<IPAddress, IPAddress> localAddresses;

        private readonly IScheduler scheduler;

        public NetworkScanner(IScheduler scheduler)
        {
            Guard.NotNull(() => scheduler, scheduler);
            this.scheduler = scheduler;
            localAddresses = GetIPAddress().ToLookup(item => item);
        }

        public IObservable<Host> FindAddresses(string network, int port)
        {
            IPNetwork ipNetwork = IPNetwork.Parse(network);
            return ipNetwork.ListIPAddress()
                            .Where(item => !localAddresses.Contains(item))
                            .ToObservable()
                            .SelectMany(
                                item => Observable.Start(async () => (await ScanPort(item, port).ConfigureAwait(false), item), scheduler)
                                                  .Merge()
                                                  .Where(pair => pair.Item1)
                                                  .Select(pair => new Host(GetHostName(pair.Item2), pair.Item2)));
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
                catch(Exception)
                {
                    return false;
                }
            }
        }

        private static IEnumerable<IPAddress> GetIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    yield return ip;
                }
            }
        }

        private string GetHostName(IPAddress ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                return entry.HostName.Split('.')[0];
            }
            catch(SocketException)
            {
            }

            return ipAddress.ToString();
        }
    }
}
