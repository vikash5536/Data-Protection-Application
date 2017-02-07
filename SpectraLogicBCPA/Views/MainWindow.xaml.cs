//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.TaskSchedulingApp.Common;
using DataProtectionApplication.TaskSchedulingApp.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MTS = Microsoft.Win32.TaskScheduler;
using DataProtectionApplication.TaskSchedulingApp.Convertors;
using System.Windows.Media;
using DataProtectionApplication.CommonLibrary.Model;
using System.Diagnostics;
using Microsoft.Win32;
using DataProtectionApplication.CommonLibrary.Constants;

namespace DataProtectionApplication.TaskSchedulingApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties and Variables
        List<string> paths = new List<string>();
        private static BlackPearlConfiguration _config;
        private object dummyNode = null;
        CollectionView view;
        public static Logger logger = new Logger(typeof(MainWindow));
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor of MainWindow Class
        /// </summary>

        public MainWindow()
        {



            InitializeComponent();

            RefreshTasksToTable();

        }
        #endregion
        #region Window Events

        /// <summary>
        /// Command which check when to execute refresh command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RefreshCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Command to be executed in order to perform refresh on F5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RefreshTasksToTable();
        }

        /// <summary>
        /// Command which check when to execute enable/disable command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DisabledCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Command to be executed in order to perform enable/disable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DisabledCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedTask = dgTasklist.SelectedItem as MTS.Task;
            if (selectedTask.State.ToString() == "Disabled")
            {
                if (selectedTask != null)
                    TaskServiceUtils.EnableTask(TaskServiceUtils.GetTaskByName(selectedTask.Name));
            }
            else
            {
                if (selectedTask != null)
                    TaskServiceUtils.DisableTask(TaskServiceUtils.GetTaskByName(selectedTask.Name));
            }

            RefreshTasksToTable();

        }

        /// <summary>
        /// Command which check when to execute exit command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        ///  Command to be executed in order to perform exit throught alt+f4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                // if (MessageBox.Show("Do you want to close the application?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                if (new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Warning, CustomPopup.ePopupTitle.Warning, "Do you want to close the application?", CustomPopup.ePopupButton.YesNo) == CustomPopup.ePopupResult.Yes)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("File: MainWindow Method: btnclose_Click . Message : " + ex.Message);
            }
        }

        /// <summary>
        /// To Drag and Move main Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                logger.LogInfo(ex.ToString());
            }
        }
        #endregion
        #region Main Window
        /// <summary>
        /// Event fires on click on refresh button to refresh task list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnTaskListRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTasksToTable();
        }

        /// <summary>
        /// Event fires on click on bucket button to view bucket.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnBucket_Click(object sender, RoutedEventArgs e)
        {
            gridMain.Visibility = Visibility.Hidden;
            gridConfig.Visibility = Visibility.Visible;
            if (IsConfigurationSet())
                GetBucketDetails();
        }

        /// <summary>
        /// Event fires on click on Task list button to load task list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnTaskList_Click(object sender, RoutedEventArgs e)
        {
            gridMain.Visibility = Visibility.Visible;
            gridConfig.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderName = Environment.GetFolderPath(
             Environment.SpecialFolder.ApplicationData);

                string path = System.IO.Path.Combine(folderName + "\\Spectra_Logic");

                if (Directory.Exists(path))
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = path;
                    if (openFileDialog.ShowDialog() == true)
                    {
                        var file = openFileDialog.FileName;
                        Process.Start(file);
                    }
                }
                else
                {
                    new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Warning, CustomPopup.ePopupTitle.Warning, "Log folder has been deleted", CustomPopup.ePopupButton.OK);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Inside btnLogs_Click" + ex);
            }
        }

        private void btnBucketRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetBucketDetails();
        }
        #endregion
        #region Tasks
        /// <summary>
        /// Refresh the task list.
        /// </summary>

        private void RefreshTasksToTable()
        {
            try
            {
                IEnumerable<Microsoft.Win32.TaskScheduler.Task> tasks = TaskServiceUtils.GetAllTasks();
                dgTasklist.ItemsSource = tasks;
                dgTasklist.Items.Refresh();
                view = (CollectionView)CollectionViewSource.GetDefaultView(dgTasklist.ItemsSource);
                logger.LogInfo(string.Format("Task Count : {0}", tasks.Count()));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in RefreshTasksToTable method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// Event fired on click of create new task button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void CreatNewTask_Click(object sender, RoutedEventArgs e)
        {
            ManageTask.ShowCreate();
            RefreshTasksToTable();
        }

        /// <summary>
        /// Event fired on mouse double click on selecting a task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void dgTasklist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var selectedTask = dgTasklist.SelectedItem as MTS.Task;
                if (selectedTask != null)
                {
                    ManageTask.ShowEdit(TaskServiceUtils.GetTaskByName(selectedTask.Name));
                    RefreshTasksToTable();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in EditContext_Click method, Message : {0}", ex.Message));
            }
        }

        #region Task Context
        /// <summary>
        /// Event fired to edit task on click of edit of context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void EditContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTask = dgTasklist.SelectedItem as MTS.Task;
                if (selectedTask != null)
                {
                    ManageTask.ShowEdit(TaskServiceUtils.GetTaskByName(selectedTask.Name));
                    RefreshTasksToTable();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in EditContext_Click method, Message : {0}", ex.Message));
            }

        }

        /// <summary>
        /// Event fired to delete selected task on click of delete of context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DeleteContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < dgTasklist.SelectedItems.Count; i++)
                {
                    var selectedTask = dgTasklist.SelectedItems[i] as MTS.Task;
                    if (selectedTask != null)
                    {
                        using (MTS.TaskService ts = new MTS.TaskService())
                        {
                            var spectraFolder = ts.RootFolder.SubFolders.Where(sf => sf.Name.Equals(Constant.taskFolderPath));
                            var spectraTaskFolder = spectraFolder.FirstOrDefault() ??
                                                    ts.RootFolder.CreateFolder(Constant.taskFolderPath);

                            if (ts.GetTask(Constant.taskFolderPath + "\\" + selectedTask.Name) != null)
                            {
                                spectraTaskFolder.DeleteTask(selectedTask.Name);
                            }
                        }
                    }
                }
                RefreshTasksToTable();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in DeleteContext_Click method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// Event fired to run task on click of run of context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void RunContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTask = dgTasklist.SelectedItem as MTS.Task;
                if (selectedTask != null)
                    TaskServiceUtils.RunTask(TaskServiceUtils.GetTaskByName(selectedTask.Name));
                RefreshTasksToTable();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in RunContext_Click method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// Event fired to stop task on click of end of context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void EndContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTask = dgTasklist.SelectedItem as MTS.Task;
                if (selectedTask != null)
                    TaskServiceUtils.EndTask(TaskServiceUtils.GetTaskByName(selectedTask.Name));
                RefreshTasksToTable();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in EndContext_Click method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// Event fired to enable/disable task on click of enable/disable of context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void DisableContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTask = dgTasklist.SelectedItem as MTS.Task;
                if (selectedTask != null)
                    TaskServiceUtils.DisableTask(TaskServiceUtils.GetTaskByName(selectedTask.Name));
                RefreshTasksToTable();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in DisableContext_Click method, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// Event fired to export task on click of export from context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ExportContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTask = dgTasklist.SelectedItem as MTS.Task;
                if (selectedTask != null)
                    if (TaskServiceUtils.ExportTask(selectedTask))
                    {
                        new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Information, string.Format("Task {0} exported successfully.", selectedTask.Name), CustomPopup.ePopupButton.OK);
                    }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in ExportContext_Click method, Message : {0}", ex.Message));
            }

        }
        #endregion
        #endregion
        #region BlackPearl Configuration
        /// <summary>
        /// Event fired on click of blackpearl configuration button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void BlackPearlConfiguration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Util.ShowBlackPearlConfiguration(null, ServerType.Destination, true, true);
                if (IsConfigurationSet())
                    GetBucketDetails();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in BlackPearlConfiguration_Click method, Message : {0}", ex.Message));
            }
        }
        /// <summary>
        /// Method used to check configurations
        /// </summary>
        /// <returns></returns>

        private bool IsConfigurationSet()
        {
            if (File.Exists(Constant.DestinationServerDetails))
            {
                return true;
            }
            else
            {
                gridBucketSyncIcon.Visibility = Visibility.Visible;
                gridTree.Visibility = Visibility.Hidden;
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Information, "BlackPearl server not configured.", CustomPopup.ePopupButton.OK);
                return false;
            }
        }
        #endregion
        #region Files Tree view

        /// <summary>
        /// Method used to get bucket details
        /// </summary>

        public void GetBucketDetails()
        {
            try
            {
                if (File.Exists(Constant.DestinationServerDetails))
                {
                    _config = Util.LoadConfigurationFromFile(Constant.DestinationServerDetails);
                    Ds3Client ds3client = new Ds3Client(_config.GetEndPoint(), _config.AccessId, _config.SecretKey, "");
                    var listofobjects = ds3client.ListObjects(_config.BucketName);
                    paths.Clear();
                    foreach (var itm in listofobjects)
                    {
                        paths.Add(_config.BucketName + "/" + itm.Name);
                    }
                    HeaderToImageConverter.bucketFlag = 0;
                    TreeViewItem item = new TreeViewItem();
                    item.Header = _config.BucketName;
                    item.Tag = _config.BucketName;
                    item.FontWeight = FontWeights.Normal;
                    item.Items.Add(dummyNode);
                    item.Expanded += new RoutedEventHandler(folder_Expanded);
                    foldersItem.Items.Clear();
                    foldersItem.Items.Add(item);
                    gridBucketSyncIcon.Visibility = Visibility.Hidden;
                    gridTree.Visibility = Visibility.Visible;
                }
                else
                {
                    gridBucketSyncIcon.Visibility = Visibility.Visible;
                    gridTree.Visibility = Visibility.Hidden;
                    btnBucket.IsEnabled = false;
                    ImageBucket.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/BucketDisable.png") as ImageSource;
                    new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Information, "BlackPearl server not configured.", CustomPopup.ePopupButton.OK);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in GetBucketDetails, Message : {0}", ex.Message));
            }

        }

        /// <summary>
        /// Method used to convert path into key
        /// </summary>
        /// <param name="path">provide path that is to be converted</param>
        /// <returns>key as string</returns>

        private static string ConvertPathToKey(string path)
        {
            return path.Replace(System.IO.Path.DirectorySeparatorChar, '/');
        }

        /// <summary>
        /// Method used for PrependPrefix
        /// </summary>
        /// <param name="path">it returns the path of the selected file</param>
        /// <param name="prefix">it return prefix</param>
        /// <returns></returns>

        private static string PrependPrefix(string path, string prefix)
        {
            var fileName = System.IO.Path.GetFileName(path);
            var fixedPath = path.Substring(0, path.Length - fileName.Length) + prefix + fileName;
            return fixedPath;
        }

        /// <summary>
        /// Event fired in order to expand folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    var match = paths.Where(stringToCheck => stringToCheck.Contains(item.Tag.ToString() + "/"));
                    List<string> dirList = new List<string>();
                    foreach (string s in match)
                    {
                        int sIndex = s.IndexOf(item.Tag.ToString() + "/") + (item.Tag.ToString() + "/").Length;
                        int eLen = (s.IndexOf("/", sIndex) - sIndex);
                        if (eLen > 0 && sIndex + eLen < s.Length)
                            dirList.Add(s.Substring(sIndex, eLen));
                        else
                            dirList.Add(s.Substring(s.LastIndexOf("/") + 1));
                    }
                    dirList = dirList.Distinct().ToList();
                    foreach (string s in dirList)
                    {
                        if (s.Contains('.'))
                        {
                            TreeViewItem subitem = new TreeViewItem();
                            subitem.Header = s;
                            subitem.Tag = item.Tag + "/" + s;
                            subitem.FontWeight = FontWeights.Normal;
                            //subitem.IsExpanded = true;
                            subitem.Items.Add(dummyNode);
                            subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                            subitem.ExpandSubtree();
                            item.Items.Add(subitem);
                        }
                        else
                        {
                            TreeViewItem subitem = new TreeViewItem();
                            subitem.Header = s;
                            subitem.Tag = item.Tag + "/" + s;
                            subitem.FontWeight = FontWeights.Normal;
                            subitem.Items.Add(dummyNode);
                            subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                            item.Items.Add(subitem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(string.Format("Exception in folder_Expanded method, Message : {0} ", ex.Message));
                }
            }
        }
        #endregion        
        #region Email Configuration
        /// <summary>
        /// This event is used to open email configuration window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmailConfig_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Constant.EmailConfigurationDetails))
            {
                EmailConfiguration _emailConfig = Util.LoadEmailConfiguration(Constant.EmailConfigurationDetails);
                if (_emailConfig != null)
                    ManageEmail.ShowEdit(_emailConfig);
                else
                    new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Error, CustomPopup.ePopupTitle.Error, "Unable to load Email configuration.", CustomPopup.ePopupButton.OK);
            }
            else
                ManageEmail.ShowCreate();
        }
        #endregion
    }
    #region CustomKeyInput
    /// <summary>
    /// Commands used to perform enable/disable and exit operations
    /// </summary>

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Disabled = new RoutedUICommand
                ();
        public static readonly RoutedUICommand Exit = new RoutedUICommand
                       (
                               "Exit",
                               "Exit",
                               typeof(CustomCommands),
                               new InputGestureCollection()
                               {
                                        new KeyGesture(Key.F4, ModifierKeys.Alt)
                               }
                       );
        public static readonly RoutedUICommand Refresh = new RoutedUICommand
                       (
                               "Refresh",
                               "Refresh",
                               typeof(CustomCommands),
                               new InputGestureCollection()
                               {
                                        new KeyGesture(Key.F5)
                               }
                       );
    }
    #endregion  
}
