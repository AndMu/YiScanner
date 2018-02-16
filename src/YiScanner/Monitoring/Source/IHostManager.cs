using System;
using System.Collections.Generic;

namespace Wikiled.YiScanner.Monitoring.Source
{
    public interface IHostManager : IDisposable
    {
        IEnumerable<HostInformation> GetHosts();
    }
}