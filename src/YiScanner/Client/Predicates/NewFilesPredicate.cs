using System;
using System.Collections.Concurrent;
using NLog;

namespace Wikiled.YiScanner.Client.Predicates
{
    public class NewFilesPredicate : IPredicate
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, bool> table = new ConcurrentDictionary<string, bool>();

        public NewFilesPredicate()
        {
            log.Debug("Constructing");
        }

        public bool CanDownload(DateTime? lastScan, string file, DateTime modified)
        {
            if (table.ContainsKey(file))
            {
                return false;
            }

            if (lastScan == null)
            {
                table[file] = true;
            }

            var result = lastScan != null;
            log.Debug("Can download ({1}): {0}", file, result);
            return result;
        }

        public void Downloaded(string file)
        {
            table[file] = true;
        }
    }
}
