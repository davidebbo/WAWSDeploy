﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Web.Deployment;

namespace WAWSDeploy
{
    public class WebDeployHelper
    {
        /// <summary>
        /// Deploys the content to one site.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <param name="publishSettingsFile">The publish settings file.</param>
        /// <param name="password">The password.</param>
        /// <returns>DeploymentChangeSummary.</returns>
        public DeploymentChangeSummary DeployContentToOneSite(string contentPath, string publishSettingsFile, string password = null)
        {
            contentPath = Path.GetFullPath(contentPath);

            var sourceBaseOptions = new DeploymentBaseOptions();
            DeploymentBaseOptions destBaseOptions;
            string siteName = ParsePublishSettings(publishSettingsFile, out destBaseOptions);

            // use the password from the command line args if provided
            if (!string.IsNullOrEmpty(password))
                destBaseOptions.Password = password;

            // If the content path is a zip file, use the Package provider
            DeploymentWellKnownProvider provider;
            if (Path.GetExtension(contentPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                provider = DeploymentWellKnownProvider.Package;
            }
            else
            {
                provider = DeploymentWellKnownProvider.ContentPath;
            }

            // Publish the content to the remote site
            using (var deploymentObject = DeploymentManager.CreateObject(provider, contentPath, sourceBaseOptions))
            {
                // Note: would be nice to have an async flavor of this API...
                return deploymentObject.SyncTo(DeploymentWellKnownProvider.ContentPath, siteName, destBaseOptions, new DeploymentSyncOptions());
            }
        }

        private string ParsePublishSettings(string path, out DeploymentBaseOptions deploymentBaseOptions)
        {
            var document = XDocument.Load(path);
            var profile = document.Descendants("publishProfile").First();

            string siteName = profile.Attribute("msdeploySite").Value;

            deploymentBaseOptions = new DeploymentBaseOptions
            {
                ComputerName = String.Format("https://{0}/msdeploy.axd?site={1}", profile.Attribute("publishUrl").Value, siteName),
                UserName = profile.Attribute("userName").Value,
                Password = profile.Attribute("userPWD").Value,
                AuthenticationType = "Basic"
            };

            return siteName;
        }
    }
}
