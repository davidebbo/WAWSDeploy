using System;
using System.Diagnostics;
using Microsoft.Web.Deployment;

namespace WAWSDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Trace.WriteLine(@"Syntax 1: WAWSDeploy.exe c:\SomeFolder MySite.PublishSettings [/p password]");
                Trace.WriteLine(@"Syntax 2: WAWSDeploy.exe c:\SomeFile.zip MySite.PublishSettings [/p password]");
                return;
            }

            // parse the command line args
            var command = Args.Configuration.Configure<DeploymentArgs>().CreateAndBind(args);

            try
            {
                var webDeployHelper = new WebDeployHelper();
                Trace.WriteLine("Starting deployment...");
                DeploymentChangeSummary changeSummary = webDeployHelper.DeployContentToOneSite(command.Folder, command.PublishSettingsFile, command.Password);

                Trace.WriteLine(string.Format("BytesCopied: {0}", changeSummary.BytesCopied));
                Trace.WriteLine(string.Format("Added: {0}", changeSummary.ObjectsAdded));
                Trace.WriteLine(string.Format("Updated: {0}", changeSummary.ObjectsUpdated));
                Trace.WriteLine(string.Format("Deleted: {0}", changeSummary.ObjectsDeleted));
                Trace.WriteLine(string.Format("Errors: {0}", changeSummary.Errors));
                Trace.WriteLine(string.Format("Warnings: {0}", changeSummary.Warnings));
                Trace.WriteLine(string.Format("Total changes: {0}", changeSummary.TotalChanges));
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Deployment failed: {0}", e.Message));
            }
        }
    }
}
