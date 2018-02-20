using System;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class HostTracking
    {
        public HostTracking(FtpConfig config, Host host)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => host, host);
            Config = config;
            Host = host;
            LastScanned = null;
        }

        public FtpConfig Config { get; }

        public Host Host { get; }

        public DateTime? LastScanned { get; private set; }

        public DateTime Scanned()
        {
            var now = DateTime.Now;
            LastScanned = now;
            return now;
        }
    }
}
