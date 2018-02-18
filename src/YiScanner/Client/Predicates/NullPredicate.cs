using System;
using NLog;

namespace Wikiled.YiScanner.Client.Predicates
{
    public class NullPredicate : IPredicate
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public NullPredicate()
        {
            log.Debug("Constructing");
        }

        public bool CanDownload(DateTime? lastScan, string file, DateTime modified)
        {
            return true;
        }

        public void Downloaded(string file)
        {
        }
    }
}
