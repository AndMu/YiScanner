using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Client.Predicates
{
    public class NullPredicate : IPredicate
    {
        public bool CanDownload(DateTime? lastScan, string file, DateTime modified)
        {
            return true;
        }
    }
}
