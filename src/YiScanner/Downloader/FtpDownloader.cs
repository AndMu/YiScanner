using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using NLog;
using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Client;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Downloader
{
    public class FtpDownloader : IDownloader
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly HostTracking tracking;

        private readonly IDestination destination;

        private readonly Regex maskRegex;

        private readonly IPredicate predicate;

        public FtpDownloader(
            HostTracking tracking,
            IDestination destination,
            IPredicate predicate)
        {
            Guard.NotNull(() => tracking, tracking);
            Guard.NotNull(() => destination, destination);
            Guard.NotNull(() => predicate, predicate);
            maskRegex = FileMask.GenerateFitMask(tracking.Config.FileMask);
            log.Debug("Generated mask: {0}", maskRegex);
            this.tracking = tracking;
            this.destination = destination;
            this.predicate = predicate;
        }

        public async Task<DateTime> Download(CancellationToken cancellation)
        {
            // Get the object used to communicate with the server.  
            using (var client = new FtpClient(tracking.Host.Address.ToString()))
            {
                log.Info("Connecting: {0}", tracking.Host.Address);
                client.Credentials = new NetworkCredential(
                    tracking.Config.Login,
                    tracking.Config.Password);
                client.Connect();
                log.Info("Connected: {0}!", tracking.Host.Address);
                await Retrieve(client, tracking.Config.Path, cancellation).ConfigureAwait(false);
            }

            var now = tracking.Scanned();
            return now;
        }

        private async Task ProcessFile(FtpClient client, FtpListItem item)
        {
            Stream stream = null;
            try
            {
                var header = new VideoHeader(tracking.Host, item.FullName);
                if (!destination.IsDownloaded(header))
                {                    
                    log.Info("Downloading <{0}> from [{1}]", item.FullName, tracking.Host.Name);
                    stream = await client.OpenReadAsync(item.FullName).ConfigureAwait(false);
                    await destination.Transfer(header, stream).ConfigureAwait(false);
                    var reply = await client.GetReplyAsync().ConfigureAwait(false);
                    if (reply.Success)
                    {
                        log.Info(
                            "Download Success:{0} Message:{1}: Type:{2} Code:{3} From: [{4}]",
                            reply.Success,
                            reply.Message,
                            reply.Type,
                            reply.Code,
                            tracking.Host.Name);
                    }
                    else
                    {
                        log.Error(
                            "Download Error:{0} Type:{1}: Code:{2} From: [{3}]",
                            reply.ErrorMessage,
                            reply.Type,
                            reply.Code,
                            tracking.Host.Name);
                    }
                }
                else
                {
                    log.Info("File is already downloaded - <{0}> {1}", item.FullName, tracking.Host.Name);
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

        private async Task Retrieve(FtpClient client, string path, CancellationToken cancellation)
        {
            foreach (FtpListItem item in client.GetListing(path))
            {
                cancellation.ThrowIfCancellationRequested();
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    if (maskRegex.IsMatch(item.FullName) &&
                        predicate.CanDownload(tracking.LastScanned, item.FullName, item.Modified))
                    {
                        if (item.Modified < DateTime.Now.AddMinutes(1))
                        {
                            await ProcessFile(client, item).ConfigureAwait(false);
                            predicate.Downloaded(item.FullName);
                        }
                        else
                        {
                            log.Debug("Ignoring file - too recent: <{0}>", item.FullName);
                        }
                    }
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await Retrieve(client, item.FullName, cancellation).ConfigureAwait(false);
                }
            }
        }
    }
}
