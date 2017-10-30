using System;

namespace Wikiled.YiScanner.Client.Predicates
{
    public interface IPredicate
    {
        bool CanDownload(DateTime? lastScan, string file, DateTime modified);
    }
}