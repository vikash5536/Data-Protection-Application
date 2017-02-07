//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using Microsoft.Win32.TaskScheduler;

namespace DataProtectionApplication.TaskSchedulingApp.Model
{
    /// <summary>
    /// This class is used to maintain status of trigger created in a task.
    /// </summary>
    public class CustomTrigger
    {
        /// <summary>
        /// Trigger :  Microsoft.Win32.TaskScheduler
        /// </summary>
        public Trigger _Trigger { get; set; }
        /// <summary>
        /// Maintain Trigger Status : Microsoft.Win32.TaskScheduler.Trigger status showing unbound value.
        /// </summary>
        public bool _Enabled { get; set; }

        public string _Details { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CustomTrigger()
        {

        }

        /// <summary>
        /// Parameterized Constructor 
        /// </summary>
        /// <param name="trigger"> Microsoft.Win32.TaskScheduler Trigger</param>
        /// <param name="enabled">Trigger Status</param>
        public CustomTrigger(Trigger trigger,bool enabled,string details)
        {
            this._Trigger = trigger;
            this._Enabled = enabled;
            this._Details = details;
        }
    }
}
