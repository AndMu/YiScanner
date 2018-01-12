using System.Threading.Tasks;

namespace Wikiled.YiScanner.Destinations
{
    public interface IPostAction
    {
        Task<bool> AfterTransfer(string fileName);
    }
}
