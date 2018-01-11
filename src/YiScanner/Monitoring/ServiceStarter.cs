using System.IO;
using System.Reactive.Concurrency;
using Newtonsoft.Json;
using NLog;
using Topshelf;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
using Wikiled.YiScanner.Client.Predicates;

namespace Wikiled.YiScanner.Monitoring
{
    public class ServiceStarter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void StartService(string directory, FtpConfig ftpConfig)
        {
            var serviceName = Path.Combine(directory, "service.json");
            if (!File.Exists(serviceName))
            {
                log.Error($"Configuration file {serviceName} not found");
                return;
            }

            MonitoringConfig config = JsonConvert.DeserializeObject<MonitoringConfig>(File.ReadAllText(serviceName));
            var predicate = config.All ? new NullPredicate() : (IPredicate)new NewFilesPredicate();
            DestinationFactory factory = new DestinationFactory(ftpConfig, config, predicate);
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
