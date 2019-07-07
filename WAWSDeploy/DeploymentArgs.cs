using Args;
using System.ComponentModel;
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
        [ArgsMemberSwitch("p", "password", "pw"),
            Description("provide the password if it's not in the profile (default: use from the publish settings).")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the allowUntrusted flag
        /// </summary>
        /// <value>true or false</value>
        [ArgsMemberSwitch("u", "allowuntrusted", "au"),
            Description("skip cert verification (default: false).")]
        public bool AllowUntrusted { get; set; }

        [ArgsMemberSwitch("d", "deleteexistingfiles", "def"),
            Description("delete target files that don't exist at the source (default: false).")]
        public bool DeleteExistingFiles { get; set; }

        [ArgsMemberSwitch("v", "verbose", "vb"),
            Description("Verbose mode (default: false)")]
        public bool Verbose { get; set; }

        [ArgsMemberSwitch("w", "whatif", "wi"),
            Description("don't actually perform the publishing (default: false).")]
        public bool WhatIf { get; set; }

        [ArgsMemberSwitch("t", "targetpath", "tp"),
            Description("the virtual or physical directory to deploy to.")]
        public string TargetPath { get; set; }

        [ArgsMemberSwitch("c", "usechecksum", "cs"),
            Description("use checksum (default: false).")]
        public bool UseChecksum { get; set; }

        [ArgsMemberSwitch("o", "appoffline", "off"),
            Description("attempt to automatically take an ASP.Net application offline before publishing to it (default: false).")]
        public bool AppOffline { get; set; }

        [ArgsMemberSwitch("r", "retryattempts", "ra"), 
            Description("number of deployment retry attempts in case of failure (default: 5).")]
        public int? RetryAttempts { get; set; }

        [ArgsMemberSwitch("i", "retryinterval", "ri"),
            Description("retry interval in ms between attempts in case of failure (default: 1000).")]
        public int? RetryInterval { get; set; }

        public TraceLevel TraceLevel
        {
            get
            {
                if (Verbose)
                    return TraceLevel.Verbose;

                return TraceLevel.Off;
            }
        }
    }
}
