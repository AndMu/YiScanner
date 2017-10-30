using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentFTP;
using Wikiled.Core.Utility.Arguments;
using Wikiled.YiScanner.Destinations;

namespace Wikiled.YiScanner.Client
{
    public class FtpDownloader
    {
        private readonly CameraDescription camera;

        private readonly IDestination destination;

        private readonly FtpClient client;

        public FtpDownloader(CameraDescription camera, IDestination destination)
        {
            Guard.NotNull(() => camera, camera);
            Guard.NotNull(() => destination, destination);
            this.camera = camera;
            this.destination = destination;

            // Get the object used to communicate with the server.  
            client = new FtpClient(camera.Address.ToString());
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
                    await destination.Transfer(new VideoHeader(camera.Name, Path.GetFileName(item.FullName)), stream).ConfigureAwait(false);
                }
                else if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    await Retrieve(item.FullName).ConfigureAwait(false);
                }
            }
        }
    }
}
