using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Commands;

namespace Wikiled.YiScanner
{
    public class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            try
            {
                if (!File.Exists("appsettings.json"))
                {
                    log.Error("Configuration file appsettings.json not found");
                    return;
                }

                JObject ftpBlock = JObject.Parse(File.ReadAllText("appsettings.json"));
                FtpConfiguration ftpConfiguration = ftpBlock["ftp"].ToObject<FtpConfiguration>();

                log.Info("Starting {0} version utility...", Assembly.GetExecutingAssembly().GetName().Version);
                List<Command> commandsList = new List<Command>();
                commandsList.Add(new MonitorCommand(ftpConfiguration));
                commandsList.Add(new DownloadCommand(ftpConfiguration));
                var commands = commandsList.ToDictionary(item => item.Name, item => item, StringComparer.OrdinalIgnoreCase);

                if (args.Length == 0)
                {
                    log.Warn("Please specify arguments");
                    CommandLineParser.PrintCommands(commands.Values);
                    return;
                }

                if (!commands.TryGetValue(args[0], out var command))
                {
                    log.Error("Unknown Command");
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
