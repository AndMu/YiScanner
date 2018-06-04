using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Monitoring.Config;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Downloader
{
    public class FileDownloader : IDownloader
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IDestination destination;

        private readonly ServerConfig config;

        private readonly IPredicate predicate;

        private DateTime? lastScanned;

        public FileDownloader(ServerConfig config, IDestination destination, IPredicate predicate)
        {
            Guard.NotNull(() => config, config);
            Guard.NotNull(() => destination, destination);
            Guard.NotNull(() => predicate, predicate);
            this.config = config;
            this.predicate = predicate;
            this.destination = destination;
        }

        public async Task<DateTime> Download(CancellationToken cancellation)
        {
            var path = Path.Combine(Environment.CurrentDirectory, config.Path);
            log.Info("Checking files: {0}", path);
            if (!Directory.Exists(path))
            {
                log.Error("Directory not found: {0}", path);
                return DateTime.Now;
            }

            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    cancellation.ThrowIfCancellationRequested();
                    if (predicate.CanDownload(lastScanned, file, File.GetLastWriteTime(file)))
                    {
                        await ProcessFile(file).ConfigureAwait(false);
                    }

                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            try
            {
                var directories = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories);
                foreach (var directory in directories)
                {
                    cancellation.ThrowIfCancellationRequested();
                    if (!Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        log.Info("Removing empty: {0}", directory);
                        Directory.Delete(directory);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            log.Info("Completed: {0}", path);
            var now = DateTime.Now;
            lastScanned = now;
            return now;
        }

        private async Task ProcessFile(string file)
        {
            StreamReader stream = null;
            try
            {
                var tracking = new Host("FTP", IPAddress.Loopback);
                var header = new VideoHeader(tracking, file);
                if (!destination.IsDownloaded(header))
                {
                    log.Info("Copy <{0}>", file);
                    stream = new StreamReader(file);
                    await destination.Transfer(header, stream.BaseStream).ConfigureAwait(false);
                }
                else
                {
                    log.Info("File is already copied <{0}", file);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                stream?.Dispose();
            }
        }
    }
}
