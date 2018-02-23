using System;
using System.IO;
using System.Reflection;
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

        public void Dispose()
        {
            ftpServer?.Dispose();
        }

        public void Start()
        {
            var outPath = Path.Combine(Environment.CurrentDirectory, config.Path);
            outPath.EnsureDirectoryExistence();
            log.Debug("Start FTP server: [{0}]", outPath);
            var membershipProvider = new AnonymousMembershipProvider(new NoValidation());
            var provider = new DotNetFileSystemProvider(outPath, false);

            // Initialize the FTP server
            ftpServer = new FtpServer(provider, membershipProvider, "127.0.0.1", config.Port, new AssemblyFtpCommandHandlerFactory(typeof(FtpServer).GetTypeInfo().Assembly));
            ftpServer.LogManager = new FtpLogManager();

            // Start the FTP server
            ftpServer.Start();
        }
    }
}
