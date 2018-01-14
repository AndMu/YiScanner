using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.YiScanner.Destinations
{
    public class ExecutePostAction : IPostAction
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly ActionConfig config;

        public ExecutePostAction(ActionConfig config)
        {
            Guard.NotNull(() => config, config);
            this.config = config;
        }

        public Task<bool> AfterTransfer(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                log.Warn("Can't process file: {0}", fileName);
                return Task.FromResult(false);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            var path = config.Cmd.Replace("%1", fileName);
            log.Debug("Executing: {0}", fileName);
            var blocks = path.Split(' ');
            startInfo.FileName = blocks[0];
            startInfo.Arguments = blocks.Skip(1).AccumulateItems(" ");
            startInfo.CreateNoWindow = true;
            startInfo.ErrorDialog = false;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            var process = new Process();
            process.StartInfo = startInfo;
            var result = process.Start();
            return Task.FromResult(result);
        }
    }
}
