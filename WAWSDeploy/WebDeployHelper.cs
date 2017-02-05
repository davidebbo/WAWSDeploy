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
            string targetPath = null,
            bool useChecksum = false,
            bool appOfflineEnabled = false)
        {
            sourcePath = Path.GetFullPath(sourcePath);

            var sourceBaseOptions = new DeploymentBaseOptions();

            DeploymentBaseOptions destBaseOptions;
            string destinationPath = SetBaseOptions(publishSettingsFile, out destBaseOptions, allowUntrusted);

            destBaseOptions.TraceLevel = traceLevel;
            destBaseOptions.Trace += destBaseOptions_Trace;

            // use the password from the command line args if provided
            if (!string.IsNullOrEmpty(password))
                destBaseOptions.Password = password;

            var sourceProvider = DeploymentWellKnownProvider.ContentPath;
            var targetProvider = DeploymentWellKnownProvider.ContentPath;

            // If a target path was specified, it could be virtual or physical
            if (!string.IsNullOrEmpty(targetPath))
            {
                if (Path.IsPathRooted(targetPath))
                {
                    // If it's rooted (e.g. d:\home\site\foo), use DirPath
                    sourceProvider = targetProvider = DeploymentWellKnownProvider.DirPath;

                    destinationPath = targetPath;
                }
                else
                {
                    // It's virtual, so append it to what we got from the publish profile
                    destinationPath += "/" + targetPath;
                }
            }

            // If the content path is a zip file, use the Package provider
            if (Path.GetExtension(sourcePath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                // For some reason, we can't combine a zip with a physical target path
                // Maybe there is some way to make it work?
                if (targetProvider == DeploymentWellKnownProvider.DirPath)
                {
                    throw new Exception("A source zip file can't be used with a physical target path");
                }

                sourceProvider = DeploymentWellKnownProvider.Package;
            }

            var syncOptions = new DeploymentSyncOptions
            {
                DoNotDelete = doNotDelete, 
                WhatIf = whatIf,
                UseChecksum = useChecksum
            };
            if (appOfflineEnabled) AddDeploymentRule(syncOptions, "AppOffline");
            
            // Publish the content to the remote site
            using (var deploymentObject = DeploymentManager.CreateObject(sourceProvider, sourcePath, sourceBaseOptions))
            {
                // Note: would be nice to have an async flavor of this API...

                return deploymentObject.SyncTo(targetProvider, destinationPath, destBaseOptions, syncOptions);
            }
        }

        private void AddDeploymentRule(DeploymentSyncOptions syncOptions, string name)
        {
            DeploymentRule newRule;
            var rules = DeploymentSyncOptions.GetAvailableRules();
            if (rules.TryGetValue(name, out newRule))
            {
                syncOptions.Rules.Add(newRule);
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
