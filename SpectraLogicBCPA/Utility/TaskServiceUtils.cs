//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Win32.TaskScheduler;
using Microsoft.Win32;
using System.IO;
using DataProtectionApplication.TaskSchedulingApp.Views;
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.CommonLibrary.Constants;

namespace DataProtectionApplication.TaskSchedulingApp.Utility
{
    /// <summary>
    /// Utility class for task scheduler application
    /// </summary>
    public static class TaskServiceUtils
    {

        public static Logger logger = new Logger(typeof(TaskServiceUtils));

        /// <summary>
        /// This method is used to register new task.
        /// </summary>
        /// <param name="taskName">Task Name</param>
        /// <param name="taskDescription">Task Description</param>
        /// <param name="triggers">Triggers to be added in task</param>            
        /// <param name="actionList">Actions to be added in task</param>            

        public static void CreateNewTask(string taskName, string taskDescription, List<Model.CustomTrigger> triggers, List<Microsoft.Win32.TaskScheduler.Action> actionList)
        {
            // Get the service on the local machine
            try
            {
                using (var taskService = new TaskService())
                {
                    // Create a new task definition and assign properties
                    var taskDefinition = taskService.NewTask();
                    taskDefinition.RegistrationInfo.Description = taskDescription;
                    taskDefinition.RegistrationInfo.Author = WindowsIdentity.GetCurrent().Name;

                    // TaskLogonType.S4U = run wether user is logged on or not 
                    taskDefinition.Principal.LogonType = TaskLogonType.S4U;
                    taskDefinition.Principal.UserId = WindowsIdentity.GetCurrent().Name;
                    taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                    taskDefinition.Settings.DisallowStartIfOnBatteries = false;
                    foreach (var trigger in triggers)
                    {
                        taskDefinition.Triggers.Add(trigger._Trigger);
                    }
                    foreach (var action in actionList)
                    {
                        taskDefinition.Actions.Add(action);
                    }


                    var spectraFolder = taskService.RootFolder.SubFolders.Where(sf => sf.Name.Equals(Constant.taskFolderPath));
                    var spectraTaskFolder = spectraFolder.FirstOrDefault() ??
                                            taskService.RootFolder.CreateFolder(Constant.taskFolderPath);

                    // Register the task in the Spectra Logic folder
                    spectraTaskFolder.RegisterTaskDefinition(taskName, taskDefinition);
                    logger.LogInfo(string.Format("Task {0} created successfully.", taskName));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in CreateNewTask method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// This method will be used for get the task from task name.
        /// </summary>
        /// <param name="taskName">TaskName</param>
        /// <returns>Microsoft.Win32.TaskScheduler.Task</returns>

        public static Task GetTaskByName(string taskName)
        {
            return GetAllTasks().FirstOrDefault(t => t.Name.Equals(taskName));
        }

        /// <summary>
        /// This method is used for get the all task in root folder.
        /// </summary>
        /// <returns></returns>

        public static IEnumerable<Task> GetAllTasks()
        {
            using (var ts = new TaskService())
            {
                return EnumFolderTasks(ts.RootFolder.SubFolders);
            }
        }

        /// <summary>
        /// This method is used for get the task list from specified folder collection.
        /// </summary>
        /// <param name="taskFolderCollection"></param>
        /// <returns></returns>

        private static IEnumerable<Task> EnumFolderTasks(TaskFolderCollection taskFolderCollection)
        {
            var tasks = new HashSet<Task>();
            foreach (var taskFolder in taskFolderCollection)
            {
                if (!taskFolder.Name.Equals(Constant.taskFolderPath)) continue;

                foreach (var task in taskFolder.Tasks)
                {
                    tasks.Add(task);
                }
                break;
            }
            return tasks;
        }

        /// <summary>
        /// Method is used to delete the specified task.
        /// </summary>
        /// <param name="taskName">TaskName</param>

        public static void DeleteTask(string taskName)
        {
            try
            {
                using (var taskService = new TaskService())
                {
                    taskService.RootFolder.SubFolders.First(sb => sb.Name.Equals(Constant.taskFolderPath)).DeleteTask(taskName);
                }
                logger.LogError(string.Format("Task {0} deleted successfully.", taskName));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in DeleteTask method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// This method is used to enable task.
        /// </summary>
        /// <param name="task">Microsoft.Win32.TaskScheduler.Task</param>

        public static void EnableTask(Task task)
        {
            task.Enabled = true;
            logger.LogInfo(string.Format("Task {0} is enabled now.", task.Name));
        }

        /// <summary>
        /// This method is used to disable the specified task.
        /// </summary>
        /// <param name="task">Microsoft.Win32.TaskScheduler.Task</param>

        public static void DisableTask(Task task)
        {
            task.Enabled = false;
            logger.LogInfo(string.Format("Task {0} is disabled now.", task.Name));
        }

        /// <summary>
        /// This method is used to run the specified task.
        /// </summary>
        /// <param name="task">Microsoft.Win32.TaskScheduler.Task</param>

        public static void RunTask(Task task)
        {
            if (task.Enabled)
                task.Run();
            logger.LogInfo(string.Format("Task {0} is Running now.", task.Name));
        }

        /// <summary>
        /// This method is used to edit the specified task.
        /// </summary>
        /// <param name="_task">Microsoft.Win32.TaskScheduler.Task</param>
        /// <param name="taskDesc">New task description</param>
        /// <param name="triggers">Updated Trigger list</param>
        /// <param name="actionList">Update Action list</param>

        public static void EditTask(Task _task, string taskDesc, List<Model.CustomTrigger> triggers, List<Microsoft.Win32.TaskScheduler.Action> actionList)
        {
            try
            {
                logger.LogInfo(string.Format("Start EditTask"));
                using (TaskService ts = new TaskService())
                {
                    Task task = _task;
                    TaskDefinition td = task.Definition;

                    td.RegistrationInfo.Description = taskDesc;
                    List<Trigger> CopyRegisterdTriggers = new List<Trigger>();                      //Creating a copy of registerd trigger

                    foreach (var item in triggers)
                    {
                        CopyRegisterdTriggers.Add(item._Trigger);
                    }
                    var RemovableTriggers = td.Triggers.Except(CopyRegisterdTriggers).ToList();     //Removable Trigger List                   

                    foreach (var removable in RemovableTriggers)
                    {
                        td.Triggers.RemoveAt(td.Triggers.IndexOf(removable.Id));
                    }
                    foreach (var item in triggers)                                                  // Adding new triggers
                    {
                        if (!td.Triggers.Contains(item._Trigger))
                            td.Triggers.Add(item._Trigger);
                    }

                    List<Microsoft.Win32.TaskScheduler.Action> CopyRegisterdActions = new List<Microsoft.Win32.TaskScheduler.Action>();                      //Creating a copy of registerd trigger
                    foreach (var item in actionList)                                                //Creating a copy of registerd trigger
                    {
                        CopyRegisterdActions.Add(item);
                    }

                    var RemovableActions = td.Actions.Except(CopyRegisterdActions).ToList();        //Removable Action List
                    foreach (var removable in RemovableActions)
                    {
                        td.Actions.RemoveAt(td.Actions.IndexOf(removable.Id));
                    }
                    foreach (var item in actionList)                                                //Adding new actions
                    {
                        if (!td.Actions.Contains(item))
                            td.Actions.Add(item);
                    }
                    var spectraFolder = ts.RootFolder.SubFolders.Where(sf => sf.Name.Equals(Constant.taskFolderPath));
                    var spectraTaskFolder = spectraFolder.FirstOrDefault() ??
                                            ts.RootFolder.CreateFolder(Constant.taskFolderPath);
                    spectraTaskFolder.RegisterTaskDefinition(_task.Name, td);
                    logger.LogInfo(string.Format("Task {0} updated successfully.", _task.Name));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info,CustomPopup.ePopupTitle.Warning, string.Format("Task may be created from another account so, it can not be edited"), CustomPopup.ePopupButton.OK);
                logger.LogError(string.Format("Exception in EditTask method, Message : {0}, ", ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in EditTask method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// This method is used to check existing task.
        /// </summary>
        /// <param name="taskName">TaskName</param>
        /// <returns></returns>

        public static bool TaskNameExsits(string taskName)
        {
            return GetTaskByName(taskName) != null;
        }

        /// <summary>
        /// This method is used to end the specified task.
        /// </summary>
        /// <param name="task">Microsoft.Win32.TaskScheduler.Task</param>

        internal static void EndTask(Task task)
        {
            task.Stop();
            logger.LogInfo(string.Format("Task {0} Stopped", task.Name));
        }

        /// <summary>
        /// This method is used to export the specified task.
        /// </summary>
        /// <param name="task">Microsoft.Win32.TaskScheduler.Task</param>
        /// 

        internal static bool ExportTask(Task task)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Xml file (*.xml)|*.xml";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.FileName = task.Name + ".xml";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, task.Definition.XmlText);
                    return true;
                }
                logger.LogError(string.Format("Task {0} exported successfully.", task.Name));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in ExportTask method, Message : {0}", ex.Message));
            }
            return false;
        }
    }
}
