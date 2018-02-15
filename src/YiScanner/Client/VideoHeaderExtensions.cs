using System.IO;
using Wikiled.Common.Arguments;
using Wikiled.Common.Extensions;

namespace Wikiled.YiScanner.Client
{
    public static class VideoHeaderExtensions
    {
        public static string GetPath(this VideoHeader header, string destination)
        {
            Guard.NotNull(() => header, header);
            Guard.NotNullOrEmpty(() => destination, destination);
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
