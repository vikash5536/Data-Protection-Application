using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.TaskSchedulingApp.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TaskSchedulingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Logger logger = new Logger(typeof(App));
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                String thisprocessname = Process.GetCurrentProcess().ProcessName;
                if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
                {
                    new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Warning, CustomPopup.ePopupTitle.Warning, "Another Instance is already running", CustomPopup.ePopupButton.OK);
                    Application.Current.Shutdown();
                }
                MainWindow mainwin = new MainWindow();
                mainwin.Show();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in Application_Startup. Message : {0}", ex.Message));
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
