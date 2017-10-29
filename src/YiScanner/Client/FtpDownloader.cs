using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentFTP;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Client
{
    public class FtpDownloader
    {
        private readonly string cameraName;

        private readonly IDestination destination;

        private readonly FtpClient client;

        public FtpDownloader(string cameraName, string location, IDestination destination)
        {
            this.cameraName = cameraName;
            this.destination = destination;

            // Get the object used to communicate with the server.  
            client = new FtpClient(location);
            client.Credentials = new NetworkCredential("root", string.Empty);
            client.Connect();
        }

        public Task Download()
        {
            return Retrieve("/tmp/sd/record/");
        }

        private async Task Retrieve(string path)
        {
            foreach (FtpListItem item in client.GetListing(path))
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    var stream = await client.OpenReadAsync(item.FullName).ConfigureAwait(false);
                    await destination.Transfer(new VideoHeader(cameraName, Path.GetFileName(item.FullName)), stream).ConfigureAwait(false);
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await Retrieve(item.FullName).ConfigureAwait(false);
                }
            }
        }
    }
}
