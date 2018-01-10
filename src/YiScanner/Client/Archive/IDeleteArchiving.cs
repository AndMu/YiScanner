using System;
using System.Threading.Tasks;

namespace Wikiled.YiScanner.Client.Archive
{
    public interface IDeleteArchiving
    {
        Task Archive(string destination, TimeSpan time);
    }
}