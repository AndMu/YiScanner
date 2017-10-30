using Wikiled.Core.Utility.Arguments;

namespace Wikiled.YiScanner.Client
{
    public class VideoHeader
    {
        public VideoHeader(CameraDescription camera, string fileName)
        {
            Guard.NotNull(() => camera, camera);
            Guard.NotNullOrEmpty(() => fileName, fileName);
            Camera = camera;
            FileName = fileName;
        }

        public CameraDescription Camera { get; }

        public string FileName { get; }
    }
}
