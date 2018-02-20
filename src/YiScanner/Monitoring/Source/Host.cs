using System.Net;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public class Host
    {
        public Host(string name, IPAddress address, int port = 21)
        {
            Name = name;
            Address = address;
            Port = port;
        }

        public string Name { get; }

        public IPAddress Address { get; }

        public int Port { get; }
    }
}
