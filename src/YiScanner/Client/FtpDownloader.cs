using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentFTP;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Client.Predicates;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Client
{
    public class FtpDownloader
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly CameraDescription camera;

        private readonly IDestination destination;

        private readonly IPredicate predicate;

        private DateTime? lastScan;

        public FtpDownloader(CameraDescription camera, IDestination destination, IPredicate predicate)
        {
            Guard.NotNull(() => camera, camera);
            Guard.NotNull(() => destination, destination);
            Guard.NotNull(() => predicate, predicate);
            this.camera = camera;
            this.destination = destination;
            this.predicate = predicate;
        }

        public async Task Download()
        {
            // Get the object used to communicate with the server.  
            using (var client = new FtpClient(camera.Address.ToString()))
            {
                log.Debug("Connecting: {0}", camera.Address);
                client.Credentials = new NetworkCredential("root", string.Empty);
                client.Connect();
                log.Debug("Connected: {0}!", camera.Address);
                await Retrieve(client, "/tmp/sd/record/").ConfigureAwait(false);
            }

            lastScan = DateTime.Now;
        }

        private async Task Retrieve(FtpClient client, string path)
        {
            foreach (FtpListItem item in client.GetListing(path))
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    if (predicate.CanDownload(lastScan, item.FullName, item.Modified))
                    {
                        await ProcessFile(client, item).ConfigureAwait(false);
                    }
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await Retrieve(client, item.FullName).ConfigureAwait(false);
                }
            }
        }

        private async Task ProcessFile(FtpClient client, FtpListItem item)
        {
            try
            {
                var stream = await client.OpenReadAsync(item.FullName).ConfigureAwait(false);
                var header = new VideoHeader(camera, Path.GetFileName(item.FullName));
                if (!destination.IsDownloaded(header))
                {
                    log.Info("Downloading <{0}>", item.FullName);
                    await destination.Transfer(header, stream).ConfigureAwait(false);
                }
                else
                {
                    log.Info("File is already downloaded - <{0}>", item.FullName);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
