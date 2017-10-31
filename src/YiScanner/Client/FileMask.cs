using System.Text.RegularExpressions;

namespace Wikiled.YiScanner.Client
{
    public static class FileMask
    {
        public static Regex GenerateFitMask(string fileMask)
        {
            string pattern =
                '^' +
                Regex.Escape(fileMask.Replace(".", "__DOT__")
                                     .Replace("*", "__STAR__")
                                     .Replace("?", "__QM__"))
                     .Replace("__DOT__", "[.]")
                     .Replace("__STAR__", ".*")
                     .Replace("__QM__", ".")
                + '$';
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}
