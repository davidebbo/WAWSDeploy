using Args;
using System.Diagnostics;

namespace WAWSDeploy
{
    /// <summary>
    /// Arguments used for publishing
    /// </summary>
    public class DeploymentArgs
    {
        /// <summary>
        /// Gets or sets the folder.
        /// </summary>
        /// <value>The folder.</value>
        [ArgsMemberSwitch(0)]
        public string Folder { get; set; }

        /// <summary>
        /// Gets or sets the publish settings file.
        /// </summary>
        /// <value>The publish settings file.</value>
        [ArgsMemberSwitch(1)]
        public string PublishSettingsFile { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [ArgsMemberSwitch("p", "password", "pw")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the allowUntrusted flag
        /// </summary>
        /// <value>true or false</value>
        [ArgsMemberSwitch("u", "allowuntrusted", "au")]
        public bool AllowUntrusted { get; set; }

        [ArgsMemberSwitch("d", "deleteexistingfiles", "def")]
        public bool DeleteExistingFiles { get; set; }

        [ArgsMemberSwitch("v", "verbose", "vb")]
        public bool Verbose { get; set; }

        [ArgsMemberSwitch("w", "whatif", "wi")]
        public bool WhatIf { get; set; }

        [ArgsMemberSwitch("s", "sitename", "sn")]
        public string SiteName { get; set; }

        public TraceLevel TraceLevel
        {
            get
            {
                if (Verbose)
                    return System.Diagnostics.TraceLevel.Verbose;

                return System.Diagnostics.TraceLevel.Off;
            }
        }
    }
}
