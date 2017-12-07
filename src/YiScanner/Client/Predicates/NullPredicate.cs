using System;

namespace Wikiled.YiScanner.Client.Predicates
{
    public class NullPredicate : IPredicate
    {
        public bool CanDownload(DateTime? lastScan, string file, DateTime modified)
        {
            return true;
        }

        public void Downloaded(string file)
        {
        }
    }
}
