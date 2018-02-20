using Wikiled.Common.Arguments;
using Wikiled.YiScanner.Monitoring.Source;

namespace Wikiled.YiScanner.Client
{
    public class VideoHeader
    {
        public VideoHeader(Host camera, string fileName)
        {
            Guard.NotNull(() => camera, camera);
            Guard.NotNullOrEmpty(() => fileName, fileName);
            Camera = camera;
            FileName = fileName;
        }

        public Host Camera { get; }

        public string FileName { get; }
    }
}
