using System.IO;
using System.Threading.Tasks;
using Wikiled.Common.Arguments;
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
            var fileDestination = header.GetPath(destination);
            return File.Exists(fileDestination);
        }

        public string ResolveName(VideoHeader header)
        {
            Guard.NotNull(() => header, header);
            return header.GetPath(destination);
        }

        public async Task Transfer(VideoHeader header, Stream source)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNull(() => source, source);
            var fileDestination = ResolveName(header);
            using (StreamWriter write = new StreamWriter(fileDestination))
            {
                await source.CopyToAsync(write.BaseStream).ConfigureAwait(false);
            }
        }
    }
}
