using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Deployment;

namespace WAWSDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Syntax: WAWSDeploy.exe FolderToDeploy MySite.PublishSettings");
                return;
            }

            try
            {
                var webDeployHelper = new WebDeployHelper();
                Console.WriteLine("Starting deployment...");
                DeploymentChangeSummary changeSummary = webDeployHelper.DeployContentToOneSite(args[0], args[1]);

                Console.WriteLine("BytesCopied: {0}", changeSummary.BytesCopied);
                Console.WriteLine("Added: {0}", changeSummary.ObjectsAdded);
                Console.WriteLine("Updated: {0}", changeSummary.ObjectsUpdated);
                Console.WriteLine("Deleted: {0}", changeSummary.ObjectsDeleted);
                Console.WriteLine("Errors: {0}", changeSummary.Errors);
                Console.WriteLine("Warnings: {0}", changeSummary.Warnings);
                Console.WriteLine("Total changes: {0}", changeSummary.TotalChanges);
            }
            catch (Exception e)
            {
                Console.WriteLine("Deployment failed: {0}", e.Message);
            }
        }
    }
}
