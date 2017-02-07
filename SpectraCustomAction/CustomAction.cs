//*******************************************************//
//                                                       //
//CSharp.Net Data Potection Application Custom Action Lib//
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
using System;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;
using Microsoft.Win32;

namespace DataProtectionApplication.SpectraCustomAction
{
    public class CustomActions
    {
        /// <summary>
        /// This Custom action is used to delete log files on uninstallation
        /// </summary>
        /// <param name="session">object of Session</param>
        /// <returns>ActionResult</returns>

        [CustomAction]
        public static ActionResult DeleteLogFile(Session session)
        {
            try
            {
                var fileName = Environment.GetFolderPath(
             Environment.SpecialFolder.ApplicationData);

                string path = System.IO.Path.Combine(fileName + "\\Spectra_Logic");

                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                session.Log("Inside DeleteLogFile" + ex);
            }
            return ActionResult.Success;
        }

        /// <summary>
        /// This Custom action is used for close prompt
        /// </summary>
        /// <param name="session">object of Session</param>
        /// <returns>ActionResult</returns>

        [CustomAction]
        public static ActionResult ClosePrompt(Session session)
        {
            session.Log("Begin PromptToCloseApplications");
            try
            {
                var productName = session["ProductName"];
                var processes = session["PromptToCloseProcesses"].Split(',');
                var displayNames = session["PromptToCloseDisplayNames"].Split(',');

                if (processes.Length != displayNames.Length)
                {
                    session.Log(@"Please check that 'PromptToCloseProcesses' and 'PromptToCloseDisplayNames' exist and have same number of items.");
                    return ActionResult.Failure;
                }

                for (var i = 0; i < processes.Length; i++)
                {
                    session.Log("Prompting process {0} with name {1} to close.", processes[i], displayNames[i]);
                    using (var prompt = new PromptCloseApplication(productName, processes[i], displayNames[i]))
                        if (!prompt.Prompt())
                            return ActionResult.Failure;
                }
            }
            catch (Exception ex)
            {
                session.Log("Missing properties or wrong values. Please check that 'PromptToCloseProcesses' and 'PromptToCloseDisplayNames' exist and have same number of items. \nException:" + ex.Message);
                return ActionResult.Failure;
            }

            session.Log("End PromptToCloseApplications");
            return ActionResult.Success;
        }

        /// <summary>
        /// This Custom action is used to Modify Registry on uninstallation
        /// </summary>
        /// <param name="session">object of Session</param>
        /// <returns>ActionResult</returns>

        [CustomAction]
        public static ActionResult ModifyRegistry(Session session)
        {
            try
            {
                RegistryKey registry = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
                if (registry != null)
                    registry.SetValue("EnableLinkedConnections", 0);
            }
            catch (Exception ex)
            {
                session.Log("Inside ModifyRegistry" + ex);
            }
            return ActionResult.Success;
        }

    }
}
