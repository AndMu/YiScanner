using System;
using System.IO;
using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using FubarDev.FtpServer.FileSystem.DotNet;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Monitoring.Config;

namespace Wikiled.YiScanner.Server
{
    public class ServerManager : IServerManager
    {
        private readonly ServerConfig config;

        private FtpServer ftpServer;

        public ServerManager(ServerConfig config)
        {
            Guard.NotNull(() => config, config);
            this.config = config;
        }

        public void Start()
        {
            var membershipProvider = new AnonymousMembershipProvider();
            var provider = new DotNetFileSystemProvider(Path.Combine(Environment.CurrentDirectory, config.Path), false);
            
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
