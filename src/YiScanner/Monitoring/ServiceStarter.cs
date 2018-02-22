using System.Reactive.Concurrency;
using NLog;
using Topshelf;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Monitoring.Config;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Monitoring
{
    public class ServiceStarter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void StartService(MonitoringConfig config)
        {
            Guard.NotNull(() => config, config);
            var predicate = config.Output.All ? new NullPredicate() : (IPredicate)new NewFilesPredicate();
            SourceFactory factory = new SourceFactory(config, predicate);
            HostFactory.Run(
                x =>
                {
                    x.Service<MonitoringInstance>(
                        s =>
                        {
                            s.ConstructUsing(name => new MonitoringInstance(TaskPoolScheduler.Default, config, factory, new DeleteArchiving()));
                            s.WhenStarted(tc => tc.Start());
                            s.WhenStopped(tc => tc.Stop());
                        });

                    x.RunAsLocalSystem();
                    x.SetDescription("Camera Monitoring Service");
                    x.SetDisplayName("YiScanner Service");
                    x.SetServiceName("YiScanner");
                });
        }
    }
}
