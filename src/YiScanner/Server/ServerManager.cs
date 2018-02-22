using System;
using System.IO;
using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using FubarDev.FtpServer.AccountManagement.Anonymous;
using FubarDev.FtpServer.FileSystem.DotNet;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.Common.Extensions;
using Wikiled.YiScanner.Monitoring.Config;

namespace Wikiled.YiScanner.Server
{
    public class ServerManager : IServerManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ServerConfig config;

        private FtpServer ftpServer;

        public ServerManager(ServerConfig config)
        {
            Guard.NotNull(() => config, config);
            this.config = config;
        }

        public void Start()
        {
            log.Debug("Start");
            var outPath = Path.Combine(Environment.CurrentDirectory, config.Path);
            outPath.EnsureDirectoryExistence();
            var membershipProvider = new AnonymousMembershipProvider(new NoValidation());
            var provider = new DotNetFileSystemProvider(outPath, false);

            // Initialize the FTP server
            ftpServer = new FtpServer(provider, membershipProvider, "127.0.0.1");

            // Start the FTP server
            ftpServer.Start();
        }

        public void Dispose()
        {
            ftpServer?.Dispose();
        }
    }
}
