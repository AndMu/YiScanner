using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;

namespace Wikiled.YiScanner.Monitoring
{
    public class MonitoringInstance
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IMonitoringConfig configuration;

        private readonly FtpDownloader[] downloaders;

        private readonly List<IDisposable> connections = new List<IDisposable>();

        public MonitoringInstance(IMonitoringConfig configuration, IDestinationFactory downloaderFactory)
        {
            Guard.NotNull(() => configuration, configuration);
            Guard.NotNull(() => downloaderFactory, downloaderFactory);
            this.configuration = configuration;
            downloaders = downloaderFactory.GetDestinations();
        }

        public bool Start()
        {
            if (downloaders.Length == 0)
            {
                return false;
            }
            
            if (configuration.Archive.HasValue)
            {
                var archiving = Observable.Interval(TimeSpan.FromDays(1), TaskPoolScheduler.Default)
                                          .Select(item => Archiving())
                                          .Replay();
                connections.Add(archiving.Connect());
            }

            var observable = Observable.Interval(TimeSpan.FromSeconds(configuration.Scan), TaskPoolScheduler.Default)
                                       .Select(item => Download())
                                       .Replay();
            connections.Add(observable.Connect());

            return true;
        }

        public void Stop()
        {
            foreach (var connection in connections)
            {
                connection.Dispose();
            }

            connections.Clear();
        }

        private bool Archiving()
        {
            var archiving = new DeleteArchiving();
            log.Info("Archiving...");
            archiving.Archive(configuration.Out, TimeSpan.FromDays(configuration.Archive.Value));
            log.Info("Archiving. Done!");
            return true;
        }

        private async Task Download()
        {
            log.Info("Checking Ftp....");
            try
            {
                var tasks = downloaders.Select(ftpDownloader => ftpDownloader.Download());
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            log.Info("Done!");
        }
    }
}
