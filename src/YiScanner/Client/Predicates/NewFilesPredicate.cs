using System;
using System.Collections.Concurrent;

namespace Wikiled.YiScanner.Client.Predicates
{
    public class NewFilesPredicate : IPredicate
    {
        private readonly ConcurrentDictionary<string, bool> table = new ConcurrentDictionary<string, bool>();

        public bool CanDownload(DateTime? lastScan, string file, DateTime modified)
        {
            if (table.ContainsKey(file))
            {
                return false;
            }

            return lastScan != null;
        }

        public void Downloaded(string file)
        {
            table[file] = true;
        }
    }
}
