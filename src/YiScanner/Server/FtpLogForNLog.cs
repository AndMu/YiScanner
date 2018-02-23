using System;
using FubarDev.FtpServer;
using NLog;

namespace Wikiled.YiScanner.Server
{
    public class FtpLogForNLog : IFtpLog
    {
        private readonly ILogger logger;

        private readonly string remoteAddress;

        private readonly string remoteIp;

        private readonly int? remotePort;

        public FtpLogForNLog(FtpConnection connection)
        {
            logger = LogManager.GetLogger("FubarDev.FtpServer.FtpConnection");
            remoteAddress = connection.RemoteAddress.ToString(true);
            remoteIp = connection.RemoteAddress.IpAddress;
            remotePort = connection.RemoteAddress.IpPort;
        }

        public FtpLogForNLog(Type type)
        {
            logger = LogManager.GetLogger(type.FullName);
        }

        public FtpLogForNLog(string name)
        {
            logger = LogManager.GetLogger(name);
        }

        public void Trace(string format, params object[] args)
        {
            Log(LogLevel.Trace, null, format, args);
        }

        public void Trace(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Trace, ex, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, null, format, args);
        }

        public void Debug(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Debug, ex, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, null, format, args);
        }

        public void Info(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Info, ex, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Log(LogLevel.Warn, null, format, args);
        }

        public void Warn(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Warn, ex, format, args);
        }

        public void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, null, format, args);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Error, ex, format, args);
        }

        public void Fatal(string format, params object[] args)
        {
            Log(LogLevel.Fatal, null, format, args);
        }

        public void Fatal(Exception ex, string format, params object[] args)
        {
            Log(LogLevel.Fatal, ex, format, args);
        }

        private void Log(LogLevel logLevel, Exception ex, string format, params object[] args)
        {
            var message = args.Length == 0 ? format : string.Format(format, args);
            logger.Log(new LogEventInfo(logLevel, logger.Name, message)
            {
                Properties =
                {
                    ["RemoteAddress"] = remoteAddress,
                    ["RemoteIp"] = remoteIp,
                    ["RemotePort"] = remotePort,
                },
                Exception = ex,
            });
        }
    }
}
