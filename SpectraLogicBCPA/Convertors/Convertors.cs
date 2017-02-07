//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using DataProtectionApplication.CommonLibrary.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DataProtectionApplication.TaskSchedulingApp.Convertors
{
    #region TreeView Convertor
    /// <summary>
    /// This class is used to show Tree view structure 
    /// </summary>
    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        public static int bucketFlag = 0;

        /// <summary>
        /// Event used to convert values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bucketFlag == 0)
            {
                bucketFlag = 1;
                Uri uri = new Uri("pack://application:,,,/../Images/bucket_1.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else if ((value as string).Contains(@"."))
            {
                Uri uri = new Uri("pack://application:,,,/../Images/file.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = new Uri("pack://application:,,,/../Images/folder.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        /// <summary>
        /// Event used to convert back values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region Task Result Converter
    /// <summary>
    /// Task Result convertor class
    /// </summary>

    public class TaskResultConverter : IValueConverter
    {
        /// <summary>
        /// Event used to convert result codes to respective messages.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string Result = "";
            int lastRunResult = (int)value;
            switch ((HRESULT)lastRunResult)
            {
                case HRESULT.SCHED_S_TASK_SUCCESS:
                    Result = "Success";
                    break;
                case HRESULT.SCHED_S_TASK_READY:
                    Result = "The task is ready to run at its next scheduled time.";
                    break;
                case HRESULT.SCHED_S_TASK_RUNNING:
                    Result = "The task is currently running.";
                    break;
                case HRESULT.SCHED_S_TASK_DISABLED:
                    Result = "The task will not run at the scheduled times because it has been disabled.";
                    break;
                case HRESULT.SCHED_S_TASK_HAS_NOT_RUN_WIN7:
                case HRESULT.SCHED_S_TASK_HAS_NOT_RUN:
                    Result = "The task has not yet run.";
                    break;
                case HRESULT.SCHED_S_TASK_NO_MORE_RUNS:
                    Result = "There are no more runs scheduled for this task.";
                    break;
                case HRESULT.SCHED_S_TASK_NOT_SCHEDULED:
                    Result = "One or more of the properties that are needed to run this task on a schedule have not been set.";
                    break;
                case HRESULT.SCHED_S_TASK_TERMINATED:
                    Result = "The last run of the task was terminated by the user.";
                    break;
                case HRESULT.SCHED_S_TASK_NO_VALID_TRIGGERS:
                    Result = "Either the task has no triggers or the existing triggers are disabled or not set.";
                    break;
                case HRESULT.SCHED_S_EVENT_TRIGGER:
                    Result = "Event triggers do not have set run times.";
                    break;
                case HRESULT.SCHED_E_TRIGGER_NOT_FOUND:
                    Result = "A task's trigger is not found.";
                    break;
                case HRESULT.SCHED_E_TASK_NOT_READY:
                    Result = "One or more of the properties required to run this task have not been set.";
                    break;
                case HRESULT.SCHED_E_TASK_NOT_RUNNING:
                    Result = "There is no running instance of the task.";
                    break;
                case HRESULT.SCHED_E_SERVICE_NOT_INSTALLED:
                    Result = "The Task Scheduler service is not installed on this computer.";
                    break;
                case HRESULT.SCHED_E_CANNOT_OPEN_TASK:
                    Result = "TThe task object could not be opened.";
                    break;
                case HRESULT.SCHED_E_INVALID_TASK:
                    Result = "The object is either an invalid task object or is not a task object.";
                    break;
                case HRESULT.SCHED_E_ACCOUNT_INFORMATION_NOT_SET:
                    Result = "No account information could be found in the Task Scheduler security database for the task indicated.";
                    break;
                case HRESULT.SCHED_E_ACCOUNT_NAME_NOT_FOUND:
                    Result = "Unable to establish existence of the account specified.";
                    break;
                case HRESULT.SCHED_E_ACCOUNT_DBASE_CORRUPT:
                    Result = "Corruption was detected in the Task Scheduler security database; the database has been reset.";
                    break;
                case HRESULT.SCHED_E_NO_SECURITY_SERVICES:
                    Result = "Task Scheduler security services are available only on Windows NT.";
                    break;
                case HRESULT.SCHED_E_UNKNOWN_OBJECT_VERSION:
                    Result = "The task object version is either unsupported or invalid.";
                    break;
                case HRESULT.SCHED_E_UNSUPPORTED_ACCOUNT_OPTION:
                    Result = "The task has been configured with an unsupported combination of account settings and run time options.";
                    break;
                case HRESULT.SCHED_E_SERVICE_NOT_RUNNING:
                    Result = "The Task Scheduler Service is not running.";
                    break;
                case HRESULT.SCHED_E_UNEXPECTEDNODE:
                    Result = "The task XML contains an unexpected node.";
                    break;
                case HRESULT.SCHED_E_NAMESPACE:
                    Result = "The task XML contains an element or attribute from an unexpected namespace.";
                    break;
                case HRESULT.SCHED_E_INVALIDVALUE:
                    Result = "The task XML contains a value which is incorrectly formatted or out of range.";
                    break;
                case HRESULT.SCHED_E_MISSINGNODE:
                    Result = "The task XML is missing a required element or attribute.";
                    break;
                case HRESULT.SCHED_E_MALFORMEDXML:
                    Result = "The task XML is malformed.";
                    break;
                case HRESULT.SCHED_S_SOME_TRIGGERS_FAILED:
                    Result = "The task is registered, but not all specified triggers will start the task.";
                    break;
                case HRESULT.SCHED_S_BATCH_LOGON_PROBLEM:
                    Result = "The task is registered, but may fail to start. Batch logon privilege needs to be enabled for the task principal.";
                    break;
                case HRESULT.SCHED_E_TOO_MANY_NODES:
                    Result = "The task XML contains too many nodes of the same type.";
                    break;
                case HRESULT.SCHED_E_PAST_END_BOUNDARY:
                    Result = "The task cannot be started after the trigger end boundary.";
                    break;
                case HRESULT.SCHED_E_ALREADY_RUNNING:
                    Result = "An instance of this task is already running.";
                    break;
                case HRESULT.SCHED_E_USER_NOT_LOGGED_ON:
                    Result = "The task will not run because the user is not logged on.";
                    break;
                case HRESULT.SCHED_E_INVALID_TASK_HASH:
                    Result = "The task image is corrupt or has been tampered with.";
                    break;
                case HRESULT.SCHED_E_SERVICE_NOT_AVAILABLE:
                    Result = "The Task Scheduler service is not available.";
                    break;
                case HRESULT.SCHED_E_SERVICE_TOO_BUSY:
                    Result = "The Task Scheduler service is too busy to handle your request. Please try again later.";
                    break;
                case HRESULT.SCHED_E_TASK_ATTEMPTED:
                    Result = "The Task Scheduler service attempted to run the task, but the task did not run due to one of the constraints in the task definition.";
                    break;
                case HRESULT.SCHED_S_TASK_QUEUED:
                    Result = "The Task Scheduler service has asked the task to run.";
                    break;
                case HRESULT.SCHED_E_TASK_DISABLED:
                    Result = "The task is disabled.";
                    break;
                case HRESULT.SCHED_E_TASK_NOT_V1_COMPAT:
                    Result = "The task has properties that are not compatible with earlier versions of Windows.";
                    break;
                case HRESULT.SCHED_E_START_ON_DEMAND:
                    Result = "The task settings do not allow the task to start on demand.";
                    break;
                case HRESULT.SCHED_E_PATH_NOT_FOUND_Win7:
                case HRESULT.SCHED_E_PATH_NOT_FOUND_Win10:
                    Result = "The system cannot find the path specified.";
                    break;
                case HRESULT.TASK_NAME_NOT_SUPPLIED:
                    Result = "Task name was not supplied in action parameter.";
                    break;
                case HRESULT.SOURCE_LOCATION_DOES_NOT_EXISTS:
                    Result = "Source location does not exists for backup.";
                    break;
                case HRESULT.UNABLE_TO_PARSE_ACTION_PARAMETERS:
                    Result = "Unable to parse action parameters of task";
                    break;               
                case HRESULT.SCHED_F_TASK_FAILED:
                    Result = "Task Failed";
                    break;
                case HRESULT.UNABLE_TO_PARSE_DEST_BP_CONFIG:
                    Result = "Unable to parse destination black pearl configuration.";
                    break;
                case HRESULT.UNABLE_TO_PARSE_SOURCE_BP_CONFIG:
                    Result = "Unable to parse source black pearl configuration.";
                    break;
                case HRESULT.FULL_SERVER_BACKUP_FAILED:
                    Result = "Task for full server backup failed.";
                    break;
                case HRESULT.FULL_LOCAL_BACKUP_FAILED:
                    Result = "Task for full local backup failed.";
                    break;
                case HRESULT.PARTIAL_LOCAL_BACKUP_FAILED:
                    Result = "Task for partial local backup failed.";
                    break;
                case HRESULT.PARTIAL_SERVER_BACKUP_FAILED:
                    Result = "Task for partial server backup failed.";
                    break;
                case HRESULT.INVALID_ACTION_PARAMETER_COUNT:
                    Result = "Suppiled Action Parameters count is not valid in a task.";
                    break;
                default:
                    Result = string.Format("(0x{0})", ((int)value).ToString("X"));
                    break;
            }
            return Result;
        }

        /// <summary>
        /// Event used to convert back values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value.ToString() == "")
                return null;
            else
                return value;
        }
    }
    #endregion
    #region Context Menu Convertor
    /// <summary>
    /// Context menu enable/disable convertor
    /// </summary>
    public class EnableDisableConverter : IValueConverter
    {
        /// <summary>
        /// Event used to convert values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "Disabled")
            {
                return "Enabled";
            }
            else
                return "Disabled";
        }

        /// <summary>
        /// Event used to convert back values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value.ToString() == "")
                return null;
            else
                return value;
        }
    }

    /// <summary>
    /// Context menu run/end enable/disable convertor
    /// </summary>
    public class RunEndEnableDisableConverter : IValueConverter
    {
        /// <summary>
        /// Event used to convert values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "Disabled")
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Event used to convert back values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)value.ToString() == "")
                return null;
            else
                return value;
        }
    }
    #endregion
}
