using System;
using System.Net;
using System.Reactive.Linq;
using Wikiled.YiScanner.Network;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class DynamicHostManager : IHostManager
    {
        private IObservable<FtpHost> result;

        public DynamicHostManager(IScanConfig config, INetworkScanner scanner)
        {
            result = scanner.FindAddresses(config.NetworkMask, 21)
                                     .Select(GetHost);
                  
            var data = Observable.Interval(TimeSpan.FromMinutes(10))
                                 .Select(item => scanner.FindAddresses(config.NetworkMask, 21))
                                 .Merge();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IObservable<FtpHost> GetHosts()
        {
            throw new NotImplementedException();
        }

        private FtpHost GetHost(IPAddress address)
        {
            return new FtpHost(null, null);
        }
    }
}
