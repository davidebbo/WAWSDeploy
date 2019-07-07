using System;
using System.Collections.Generic;
using Args.Help;
using Args.Help.Formatters;
using Microsoft.Web.Deployment;

namespace WAWSDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = Args.Configuration.Configure<DeploymentArgs>();
            if (args.Length < 2)
            {
                WriteLine(@"WAWSDeploy version {0}", typeof(Program).Assembly.GetName().Version);
                var help = new HelpProvider();
                new ConsoleHelpFormatter().WriteHelp(help.GenerateModelHelp(model), Console.Out);
                return;
            }

            // parse the command line args
            var command = model.CreateAndBind(args);

            try
            {
                var webDeployHelper = new WebDeployHelper();

                webDeployHelper.DeploymentTraceEventHandler += Trace;

                WriteLine("Starting deployment...");
                DeploymentChangeSummary changeSummary = webDeployHelper.DeployContentToOneSite(
                    command.Folder, 
                    command.PublishSettingsFile, 
                    command.Password, 
                    command.AllowUntrusted,
                    !command.DeleteExistingFiles,
                    command.TraceLevel,
                    command.WhatIf,
                    command.TargetPath,
                    command.UseChecksum,
                    command.AppOffline,
                    command.RetryAttempts,
                    command.RetryInterval,
                    command.SkipAppData,
                    command.SkipFoldersRegexps
                    );

                WriteLine("BytesCopied: {0}", changeSummary.BytesCopied);
                WriteLine("Added: {0}", changeSummary.ObjectsAdded);
                WriteLine("Updated: {0}", changeSummary.ObjectsUpdated);
                WriteLine("Deleted: {0}", changeSummary.ObjectsDeleted);
                WriteLine("Errors: {0}", changeSummary.Errors);
                WriteLine("Warnings: {0}", changeSummary.Warnings);
                WriteLine("Total changes: {0}", changeSummary.TotalChanges);
            }
            catch (Exception e)
            {
                WriteLine("Deployment failed: {0}", e.Message);
                Environment.ExitCode = 1;
            }
        }

        static void Trace(object sender, DeploymentTraceEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static void WriteLine(string message, params object[] args)
        {
            Console.WriteLine(String.Format(message, args));
        }
    }
}
