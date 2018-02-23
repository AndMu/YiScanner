using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Wikiled.Console.Arguments;
using Wikiled.YiScanner.Commands;
using Wikiled.YiScanner.Monitoring;
using Wikiled.YiScanner.Monitoring.Config;
using Wikiled.YiScanner.Network;

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
                if (!File.Exists(Path.Combine(directory, "service.json")))
                {
                    log.Error("Configuration file service.json not found");
                    return;
                }

                MonitoringConfig config = JsonConvert.DeserializeObject<MonitoringConfig>(File.ReadAllText(Path.Combine(directory, "service.json")));
                if (!config.Validate())
                {
                    log.Error("Invalid configuration");
                    return;
                }

                NetworkScanner scanner = new NetworkScanner(TaskPoolScheduler.Default);
                log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
                foreach (var address in scanner.GetLocalIPAddress())
                {
                    log.Info("Starting on local IP: [{0}]", address);
                }
                
                List<Command> commandsList = new List<Command>();
                commandsList.Add(new MonitorCommand(config));
                commandsList.Add(new DownloadCommand(config));
                var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

                if (args.Length == 0 ||
                    !commands.TryGetValue(args[0], out var command))
                {
                    log.Info("Starting as service");
                    ServiceStarter serviceStarter = new ServiceStarter();
                    serviceStarter.StartService(config);
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
    }
}
