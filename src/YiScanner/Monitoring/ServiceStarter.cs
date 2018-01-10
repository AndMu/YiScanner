using System.IO;
using Newtonsoft.Json;
using NLog;
using Topshelf;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;

namespace Wikiled.YiScanner.Monitoring
{
    public class ServiceStarter
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public void StartService(string directory, FtpConfiguration ftpConfiguration)
        {
            if (!File.Exists(Path.Combine(directory, "service.json")))
            {
                log.Error("Configuration file appsettings.json not found");
                return;
            }

            MonitoringConfig config = JsonConvert.DeserializeObject<MonitoringConfig>(File.ReadAllText(Path.Combine(directory, "service.json")));
            var predicate = config.All ? new NullPredicate() : (IPredicate)new NewFilesPredicate();
            DestinationFactory factory = new DestinationFactory(ftpConfiguration, config, predicate);
            HostFactory.Run(x =>
            {
                x.Service<MonitoringInstance>(s =>
                {
                    s.ConstructUsing(name => new MonitoringInstance(config, factory));
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
