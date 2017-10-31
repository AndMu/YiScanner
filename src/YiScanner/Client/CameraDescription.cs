using Wikiled.Core.Utility.Arguments;

namespace Wikiled.YiScanner.Client
{
    public class CameraDescription
    {
        public CameraDescription(string name, string address)
        {
            Guard.NotNull(() => name, name);
            Guard.NotNull(() => address, address);
            Name = name;
            Address = address;
        }

        public string Name { get; }

        public string Address { get; }
    }
}
