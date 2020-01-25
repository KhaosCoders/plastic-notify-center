using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.SCM
{
    /// <summary>
    /// A very simple wrapper arround the Plastic SCM `cm` command
    /// </summary>
    public class PlasticCLI
    {
        #region cm command names

        private const string CMFileName = "cm";
        private const string CMVersionCommand = "version";
        private const string CMLicenceInfoCommand = "li";
        private const string CMclConfigureClient = "clconfigureclient";

        #endregion

        #region cm path

        private string CMPath { get; set; }

        /// <summary>
        /// Asserts that cm command is found in supplied path
        /// </summary>
        private void AssertCMPath()
        {
            // supplied path can be file or dir
            if (File.Exists(CMPath))
            {
                // Only accept cm.* for security reasons
                if (!Path.GetFileNameWithoutExtension(CMPath).Equals(CMFileName, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new CLIException("Only cm command accepted");
                }
                return;
            }
            else if (Directory.Exists(CMPath))
            {
                // look for a cm.* file in the dir
                string file = Directory.GetFiles(CMPath)
                                       .FirstOrDefault(
                                            f => Path.GetFileNameWithoutExtension(f)
                                                     .Equals(CMFileName, System.StringComparison.CurrentCultureIgnoreCase));
                if (!string.IsNullOrWhiteSpace(file))
                {
                    CMPath = file;
                    return;
                }
            }
            throw new CLIException("CM not found!", new FileNotFoundException("File not found", CMPath));
        }

        #endregion

        public PlasticCLI(string cmPath)
        {
            CMPath = cmPath;
            AssertCMPath();
        }

        /// <summary>
        /// Sets up the cm command for the current user session
        /// </summary>
        /// <param name="Host">Plastic server host name</param>
        /// <param name="Port">Plastic server host port</param>
        /// <param name="WorkingMode">User authentication mode</param>
        /// <param name="Username">Username used to login</param>
        /// <param name="Password">Password used to login</param>
        public async Task<bool> SetupClient(string Host, int Port, string WorkingMode, string Username, string Password)
        {
            StringBuilder arguments = new StringBuilder();
            arguments.AppendFormat(" --language=en")
                     .AppendFormat(" --workingmode={0}", WorkingMode)
                     .AppendFormat(" --user={0}", Username)
                     .AppendFormat(" --password={0}", Password)
                     .AppendFormat(" --server={0}", Host)
                     .AppendFormat(" --port={0}", Port);

            // Write client configuration into current user config
            await ProcessHelper.RunCmdAsync(CMclConfigureClient, arguments.ToString());

            // Test configuration
            string liResult = await ProcessHelper.GetCmdResultAsync(CMPath, CMLicenceInfoCommand);
            // If any error occured the li command does not return anything
            return !string.IsNullOrWhiteSpace(liResult);
        }

        /// <summary>
        /// Get the cm version
        /// </summary>
        public async Task<string> GetVersionAsync()
        {
            string version = await ProcessHelper.GetCmdResultAsync(CMPath, CMVersionCommand);
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new CLIException("cm returned no content!");
            }
            return version.Trim();
        }
    }
}