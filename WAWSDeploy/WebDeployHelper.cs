using System;
using System.IO;
using Microsoft.Web.Deployment;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;

namespace WAWSDeploy
{
    public class WebDeployHelper
    {

        public event EventHandler<DeploymentTraceEventArgs> DeploymentTraceEventHandler;

        /// <summary>
        /// Deploys the content to one site.
        /// </summary>
        /// <param name="sourcePath">The content path.</param>
        /// <param name="publishSettingsFile">The publish settings file.</param>
        /// <param name="password">The password.</param>
        /// <param name="allowUntrusted">Deploy even if destination certificate is untrusted</param>
        /// <returns>DeploymentChangeSummary.</returns>
        public DeploymentChangeSummary DeployContentToOneSite(string sourcePath,
            string publishSettingsFile,
            string password = null,
            bool allowUntrusted = false,
            bool doNotDelete = true,
            TraceLevel traceLevel = TraceLevel.Off,
            bool whatIf = false,
            string siteName = null)
        {
            sourcePath = Path.GetFullPath(sourcePath);

            var sourceBaseOptions = new DeploymentBaseOptions();

            DeploymentBaseOptions destBaseOptions;
            string destinationPath = SetBaseOptions(publishSettingsFile, out destBaseOptions, allowUntrusted);

            if (!string.IsNullOrEmpty(siteName))
                destinationPath += "/" + siteName;

            destBaseOptions.TraceLevel = traceLevel;
            destBaseOptions.Trace += destBaseOptions_Trace;

            // use the password from the command line args if provided
            if (!string.IsNullOrEmpty(password))
                destBaseOptions.Password = password;

            // If the content path is a zip file, use the Package provider
            DeploymentWellKnownProvider provider;
            if (Path.GetExtension(sourcePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                provider = DeploymentWellKnownProvider.Package;
            }
            else
            {
                provider = DeploymentWellKnownProvider.ContentPath;
            }

            var syncOptions = new DeploymentSyncOptions
            {
                DoNotDelete = doNotDelete, 
                WhatIf = whatIf
            };

            // Publish the content to the remote site
            using (var deploymentObject = DeploymentManager.CreateObject(provider, sourcePath, sourceBaseOptions))
            {
                // Note: would be nice to have an async flavor of this API...

                return deploymentObject.SyncTo(DeploymentWellKnownProvider.ContentPath, destinationPath, destBaseOptions, syncOptions);
            }
        }

        void destBaseOptions_Trace(object sender, DeploymentTraceEventArgs e)
        {
            DeploymentTraceEventHandler.Invoke(sender, e);
        }

        private string SetBaseOptions(string publishSettingsPath, out DeploymentBaseOptions deploymentBaseOptions, bool allowUntrusted)
        {
            PublishSettings publishSettings = new PublishSettings(publishSettingsPath);
            
            deploymentBaseOptions = new DeploymentBaseOptions
            {
                ComputerName = publishSettings.ComputerName,
                UserName = publishSettings.Username,
                Password = publishSettings.Password,
                AuthenticationType = publishSettings.UseNTLM ? "ntlm" : "basic"
            };

            if (allowUntrusted || publishSettings.AllowUntrusted)
            {
                ServicePointManager.ServerCertificateValidationCallback = AllowCertificateCallback;
            }

            return publishSettings.SiteName;
        }

        private static bool AllowCertificateCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
