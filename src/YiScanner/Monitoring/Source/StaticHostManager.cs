using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using NLog;
using Wikiled.Common.Arguments;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class StaticHostManager : IHostManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IScanConfig config;

        public StaticHostManager(IScanConfig config)
        {
            Guard.NotNull(() => config, config);
            this.config = config;
        }

        public IEnumerable<Host> GetHosts()
        {
            return GetHostsInternal();
        }

        public void Dispose()
        {
        }

        private IEnumerable<Host> GetHostsInternal()
        {
            if (string.IsNullOrEmpty(config.Cameras))
            {
                log.Error("Invalid camera(s) names");
                yield break;
            }

            if (string.IsNullOrEmpty(config.Hosts))
            {
                log.Error("Invalid camera(s) hosts");
                yield break;
            }

            var listOfCameras = config.Cameras.Split(',');
            var listOfHosts = config.Hosts.Split(',');
            if (listOfHosts.Length != listOfCameras.Length)
            {
                log.Error("List of camera names and hosts does not match");
                yield break;
            }

            for (int i = 0; i < listOfCameras.Length; i++)
            {
                yield return new Host(listOfCameras[i], IPAddress.Parse(listOfHosts[i]));
            }
        }
    }
}
