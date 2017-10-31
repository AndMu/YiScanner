using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
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

        private readonly FtpConfiguration configuration;

        private DateTime? lastScan;

        private Regex maskRegex;

        public FtpDownloader(FtpConfiguration configuration, CameraDescription camera, IDestination destination, IPredicate predicate)
        {
            Guard.NotNull(() => configuration, configuration);
            Guard.NotNull(() => camera, camera);
            Guard.NotNull(() => destination, destination);
            Guard.NotNull(() => predicate, predicate);
            maskRegex = FileMask.GenerateFitMask(configuration.FileMask);
            this.camera = camera;
            this.destination = destination;
            this.predicate = predicate;
            this.configuration = configuration;
        }

        public async Task Download()
        {
            // Get the object used to communicate with the server.  
            using (var client = new FtpClient(camera.Address))
            {
                log.Debug("Connecting: {0}", camera.Address);
                client.Credentials = new NetworkCredential(configuration.Login, configuration.Password);
                client.Connect();
                log.Debug("Connected: {0}!", camera.Address);
                await Retrieve(client, configuration.Path).ConfigureAwait(false);
            }

            lastScan = DateTime.Now;
        }

        private async Task Retrieve(FtpClient client, string path)
        {
            foreach (FtpListItem item in client.GetListing(path))
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    if (maskRegex.IsMatch(item.FullName) &&
                        predicate.CanDownload(lastScan, item.FullName, item.Modified))
                    {
                        await ProcessFile(client, item).ConfigureAwait(false);
                    }
                    else
                    {
                        log.Debug("Ignoring file: <{0}>", item.FullName);
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
            Stream stream = null;
            try
            {
                var header = new VideoHeader(camera, item.FullName);
                if (!destination.IsDownloaded(header))
                {
                    log.Info("Downloading <{0}>", item.FullName);
                    stream = await client.OpenReadAsync(item.FullName).ConfigureAwait(false);
                    await destination.Transfer(header, stream).ConfigureAwait(false);
                    stream = null;
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
            finally
            {
                stream?.Dispose();
            }
        }
    }
}
