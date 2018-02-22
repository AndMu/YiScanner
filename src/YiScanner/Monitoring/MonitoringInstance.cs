using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Monitoring.Config;
using Wikiled.YiScanner.Monitoring.Source;
using Wikiled.YiScanner.Network;
using Wikiled.YiScanner.Server;

namespace Wikiled.YiScanner.Monitoring
{
    public class MonitoringInstance
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly MonitoringConfig configuration;

        private readonly IDeleteArchiving archiving;

        private readonly List<IDisposable> connections = new List<IDisposable>();

        private readonly IScheduler scheduler;

        private readonly ISourceFactory downloaderFactory;

        private IHostManager hostManager;

        private ServerManager server;

        public MonitoringInstance(IScheduler scheduler, MonitoringConfig configuration, ISourceFactory downloaderFactory, IDeleteArchiving archiving)
        {
            Guard.NotNull(() => configuration, configuration);
            Guard.NotNull(() => scheduler, scheduler);
            Guard.NotNull(() => downloaderFactory, downloaderFactory);
            Guard.NotNull(() => archiving, archiving);
            this.configuration = configuration;
            this.archiving = archiving;
            this.scheduler = scheduler;
            this.downloaderFactory = downloaderFactory;
        }

        public bool Start()
        {
            hostManager = configuration.AutoDiscovery?.On == true ? 
                              new DynamicHostManager(configuration, new NetworkScanner(TaskPoolScheduler.Default), scheduler) : 
                              (IHostManager)new StaticHostManager(configuration.Known);
            if (configuration.Archive.HasValue)
            {
                var archivingObservable = Observable.Empty<bool>()
                                                    .Delay(TimeSpan.FromDays(1), scheduler)
                                                    .Concat(Observable.FromAsync(Archiving, scheduler))
                                                    .Repeat()
                                                    .SubscribeOn(scheduler)
                                                    .Subscribe();
                connections.Add(archivingObservable);
            }

            if (configuration.Server != null)
            {
                log.Info("Setting FTP server");
                server = new ServerManager(configuration.Server);
                server.Start();
            }

            var observableMonitor = Observable.Empty<bool>()
                                              .Delay(TimeSpan.FromSeconds(configuration.Scan), scheduler)
                                              .Concat(Observable.FromAsync(Download, scheduler))
                                              .Repeat()
                                              .SubscribeOn(scheduler)
                                              .Subscribe();

            connections.Add(observableMonitor);
            return true;
        }

        public void Stop()
        {
            hostManager.Dispose();
            foreach (var connection in connections)
            {
                connection.Dispose();
            }

            server?.Dispose();
            connections.Clear();
        }

        private async Task<bool> Archiving()
        {
            log.Info("Archiving...");
            await archiving.Archive(configuration.Output.Out, TimeSpan.FromDays(configuration.Archive.Value)).ConfigureAwait(false);
            log.Info("Archiving. Done!");
            return true;
        }

        private async Task<bool> Download()
        {
            log.Info("Checking Ftp....");
            try
            {
                var sources  = downloaderFactory.GetSources(hostManager).ToArray();
                List<Task> tasks = new List<Task>();
                log.Info("Downloading from {0} cameras", sources.Length);
                foreach (var item in sources)
                {
                    tasks.Add(item.Download());
                }
                
                await Task.WhenAll(tasks).ConfigureAwait(false);
                log.Info("Done!");
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return false;
        }
    }
}
