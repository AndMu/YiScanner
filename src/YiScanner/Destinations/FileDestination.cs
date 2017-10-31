using System.IO;
using System.Threading.Tasks;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;
using Wikiled.YiScanner.Client;

namespace Wikiled.YiScanner.Destinations
{
    public class FileDestination : IDestination
    {
        private readonly string destination;

        public FileDestination(string destination)
        {
            Guard.NotNullOrEmpty(() => destination, destination);
            this.destination = destination;
        }

        public bool IsDownloaded(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            var fileDestination = GetFilePath(header);
            return File.Exists(fileDestination);
        }

        public async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            var fileDestination = GetFilePath(header);
            using (StreamWriter write = new StreamWriter(fileDestination))
            {
                await source.CopyToAsync(write.BaseStream).ConfigureAwait(false);
            }
        }

        private string GetFilePath(VideoHeader header)
        {
            var fileName = Path.GetFileName(header.FileName);
            var dirName = Path.GetDirectoryName(header.FileName);
            dirName = Path.GetFileName(dirName);
            var dirDestination = Path.Combine(destination, header.Camera.Name, dirName);
            dirDestination.EnsureDirectoryExistence();
            var fileDestination = Path.Combine(dirDestination, fileName);
            return fileDestination;
        }
    }
}
