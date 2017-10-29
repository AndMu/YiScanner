using Wikiled.Core.Utility.Arguments;

namespace Wikiled.YiScanner.Client
{
    public class VideoHeader
    {
        public VideoHeader(string camera, string fileName)
        {
            Guard.NotNullOrEmpty(() => camera, camera);
            Guard.NotNullOrEmpty(() => fileName, fileName);
            Camera = camera;
            FileName = fileName;
        }

        public string Camera { get; }

        public string FileName { get; }
    }
}
