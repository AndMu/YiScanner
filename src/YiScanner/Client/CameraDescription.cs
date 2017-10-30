using System.Net;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.YiScanner.Client
{
    public class CameraDescription
    {
        public CameraDescription(string name, IPAddress address)
        {
            Guard.NotNull(() => name, name);
            Name = name;
            Address = address;
        }

        public string Name { get; }

        public IPAddress Address { get; }
    }
}
