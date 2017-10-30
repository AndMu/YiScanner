using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentFTP;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Client
{
    public class FtpDownloader
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly CameraDescription camera;

        private readonly IDestination destination;

        private readonly IAnalyzer analyzer;

        private DateTime? lastScan;

        public FtpDownloader(CameraDescription camera, IDestination destination, IAnalyzer analyzer)
        {
            Guard.NotNull(() => camera, camera);
            Guard.NotNull(() => destination, destination);
            Guard.NotNull(() => analyzer, analyzer);
            this.camera = camera;
            this.destination = destination;
            this.analyzer = analyzer;
        }

        public async Task Download()
        {
            // Get the object used to communicate with the server.  
            using (var client = new FtpClient(camera.Address.ToString()))
            {
                client.Credentials = new NetworkCredential("root", string.Empty);
                client.Connect();
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
                    if (analyzer.CanDownload(lastScan, item.FullName, item.Modified))
                    {
                        log.Info("Downloading {0}", item.FullName);
                        var stream = await client.OpenReadAsync(item.FullName).ConfigureAwait(false);
                        await destination.Transfer(new VideoHeader(camera.Name, Path.GetFileName(item.FullName)), stream).ConfigureAwait(false);
                    }
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await Retrieve(client, item.FullName).ConfigureAwait(false);
                }
            }
        }
    }
}
