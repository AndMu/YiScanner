using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Topshelf;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Commands;
using Wikiled.YiScanner.Monitoring;

namespace Wikiled.YiScanner
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!File.Exists(Path.Combine(directory, "appsettings.json")))
                {
                    log.Error("Configuration file appsettings.json not found");
                    return;
                }

                JObject ftpBlock = JObject.Parse(File.ReadAllText(Path.Combine(directory, "appsettings.json")));
                FtpConfiguration ftpConfiguration = ftpBlock["ftp"].ToObject<FtpConfiguration>();

                log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
                List<Command> commandsList = new List<Command>();
                commandsList.Add(new MonitorCommand(ftpConfiguration));
                commandsList.Add(new DownloadCommand(ftpConfiguration));
                var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

                if (args.Length == 0 ||
                    !commands.TryGetValue(args[0], out var command))
                {
                    log.Info("Starting as service");
                    StartService(directory, ftpConfiguration);
                    return;
                }
                
                command.ParseArguments(args.Skip(1));
                command.Execute();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void StartService(string directory, FtpConfiguration ftpConfiguration)
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
