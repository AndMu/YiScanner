using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using Newtonsoft.Json.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Archive;
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
                FtpConfig ftpConfig = ftpBlock["ftp"].ToObject<FtpConfig>();

                log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
                List<Command> commandsList = new List<Command>();
                commandsList.Add(new MonitorCommand(ftpConfig));
                commandsList.Add(new DownloadCommand(ftpConfig));
                var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

                if (args.Length == 0 ||
                    !commands.TryGetValue(args[0], out var command))
                {
                    log.Info("Starting as service");
                    ServiceStarter serviceStarter = new ServiceStarter();
                    serviceStarter.StartService(directory, ftpConfig);
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
