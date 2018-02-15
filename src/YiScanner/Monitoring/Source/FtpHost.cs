namespace Wikiled.YiScanner.Monitoring.Source
{
    public class FtpHost
    {
        public FtpHost(string name, string address, int port = 21)
        {
            Name = name;
            Address = address;
            Port = port;
        }

        public string Name { get; }

        public string Address { get; }

        public int Port { get; }
    }
}
