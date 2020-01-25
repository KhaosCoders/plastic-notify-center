using System.Diagnostics;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.SCM
{
    /// <summary>
    /// A helper with starting console processes
    /// </summary>
    public class ProcessHelper
    {

        /// <summary>
        /// Runs a console command and returns its output on standard-out
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="arguments">Arguments</param>
        public static async Task<string> GetCmdResultAsync(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(command)
            {
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (Process proc = Process.Start(startInfo))
            {
                return await proc.StandardOutput.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Runs a console command
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="arguments">Arguments</param>
        public static async Task RunCmdAsync(string command, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(command)
            {
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            using (Process proc = Process.Start(startInfo))
            {
                await Task.Run(proc.WaitForExit);
            }
        }
    }
}