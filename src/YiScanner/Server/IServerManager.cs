using System;

namespace Wikiled.YiScanner.Server
{
    public interface IServerManager : IDisposable
    {
        void Start();
    }
}