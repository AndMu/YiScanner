using System;

namespace Wikiled.YiScanner.Client
{
    public interface IAnalyzer
    {
        bool CanDownload(DateTime? lastScan, string file, DateTime modified);
    }
}