using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Network
{
    public class NetworkScanner : INetworkScanner
    {
        public IObservable<IPAddress> FindAddresses(int port)
        {
            return GetAllAdresses()
                   .ToObservable()
                   .SelectMany(item => Observable.Start(async () => (await ScanPort(item, port), item))
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
                    await client.ConnectAsync(address, port);
                    return client.Connected;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public IEnumerable<IPAddress> GetAllAdresses()
        {
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (!ip.IsDnsEligible)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ip.Address))
                        {
                            yield return ip.Address;
                        }
                    }
                }
            }
        }
    }
}
