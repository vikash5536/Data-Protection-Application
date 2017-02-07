//*******************************************************//
//                                                       //
//CSharp.Net Data Potection Application Custom Action Lib//
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DataProtectionApplication.SpectraCustomAction
{
    public class PromptCloseApplication : IDisposable
    {
        #region Properties and Variables
        private readonly string _productName;
        private readonly string _processName;
        private readonly string _displayName;
        private System.Threading.Timer _timer;
        private Form _form;
        private IntPtr _mainWindowHanle;
        #endregion
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Parameterised Constructor
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="processName"></param>
        /// <param name="displayName"></param>
        public PromptCloseApplication(string productName, string processName, string displayName)
        {
            _productName = productName;
            _processName = processName;
            _displayName = displayName;
        }

        /// <summary>
        /// Method used to check propt
        /// </summary>
        /// <returns></returns>
        public bool Prompt()
        {
            if (IsRunning(_processName))
            {
                _form = new ClosePromptForm(String.Format("Please close running instances of {0} before running {1} before uninstalling or upgrading.", _displayName, _productName));
                _mainWindowHanle = FindWindow(null, _productName + " Setup");
                if (_mainWindowHanle == IntPtr.Zero)
                    _mainWindowHanle = FindWindow("#32770", _productName);

                _timer = new System.Threading.Timer(TimerElapsed, _form, 200, 200);

                return ShowDialog();
            }
            return true;
        }

        /// <summary>
        /// Method return whether dialog should be shown or not
        /// </summary>
        /// <returns></returns>
        bool ShowDialog()
        {
            if (_form.ShowDialog(new WindowWrapper(_mainWindowHanle)) == DialogResult.OK)
                return !IsRunning(_processName) || ShowDialog();
            return false;
        }
        
        /// <summary>
        /// Method used for timer
        /// </summary>
        /// <param name="sender"></param>
        private void TimerElapsed(object sender)
        {
            if (_form == null || IsRunning(_processName) || !_form.Visible)
                return;
            _form.DialogResult = DialogResult.OK;
            _form.Close();
        }

        /// <summary>
        /// Method used to check whether process is running or not
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        static bool IsRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        /// <summary>
        /// Method is used to dispose form
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
            if (_form != null && _form.Visible)
                _form.Close();
        }
    }
}
