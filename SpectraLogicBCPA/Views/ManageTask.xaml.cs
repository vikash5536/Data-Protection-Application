//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.CommonLibrary.Model;
using DataProtectionApplication.TaskSchedulingApp.Model;
using DataProtectionApplication.TaskSchedulingApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MTS = Microsoft.Win32.TaskScheduler;

namespace DataProtectionApplication.TaskSchedulingApp.Views
{
    /// <summary>
    /// Interaction logic for ManageTask.xaml
    /// </summary>
    public partial class ManageTask : Window
    {
        #region Properties and Variables
        public static List<ActionParameters> ActionParamList { get; private set; }
        public static Logger logger = new Logger(typeof(ManageTask));
        public static List<CustomTrigger> TriggerList { get; private set; }
        private static ManageTask _manageTask;
        public static List<MTS.Action> ActionList { get; private set; }
        private static bool _isCreate = true;
        public static MTS.Task _task { get; private set; }
        public static MTS.TriggerCollection _triggers { get; private set; }
        #endregion
        #region Constructor

        /// <summary>
        /// Constructor of ManageTask Screen
        /// </summary>

        public ManageTask()
        {
            InitializeComponent();
            ActionParamList = new List<ActionParameters>();
            TriggerList = new List<CustomTrigger>();
            ActionList = new List<MTS.Action>();
            const string Text = " Task";
            if (_isCreate)
            {
                const string operation = "Create";
                lbTitle.Content = string.Concat(operation, Text);
            }
            else
            {
                const string operation = "Edit";
                lbTitle.Content = string.Concat(operation, Text);
                txtTaskName.IsEnabled = false;
                FillDataFromTask(_task);
            }
            CanOperate();
        }
        #endregion
        #region Window Methods

        /// <summary>
        /// Event fired on click of exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event used to make screen movable
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
        #region Mangage Task Methods

        /// <summary>
        /// Method used to enable/disable save button based on validation
        /// </summary>

        private void CanOperate()
        {
            btnSaveTask.IsEnabled = IsValidTaskName(); //& IsValidDatabaseLocation();
        }

        /// <summary>
        /// Method used to check whether taskname is valid or not
        /// </summary>
        /// <returns>true or false</returns>

        private bool IsValidTaskName()
        {
            if (string.IsNullOrWhiteSpace(txtTaskName.Text))
            {
                txtTaskName.BorderBrush = new SolidColorBrush(Colors.Red);
                txtTaskName.ToolTip = "Task Name Should not be empty";
                lblTooltipTaskName.Visibility = Visibility.Visible;
                lblTooltipTaskName.ToolTip = "Task Name Should not be empty";
                return false;
            }
            else if (TaskServiceUtils.TaskNameExsits(txtTaskName.Text) && _isCreate)
            {
                txtTaskName.BorderBrush = new SolidColorBrush(Colors.Red);
                txtTaskName.ToolTip = "Task with that name already exsits.";
                lblTooltipTaskName.Visibility = Visibility.Visible;
                lblTooltipTaskName.ToolTip = "Task with that name already exsits.";
                return false;
            }
            else
            {
                txtTaskName.BorderBrush = new SolidColorBrush(Colors.Black);
                txtTaskName.ToolTip = "Task Name";
                lblTooltipTaskName.Visibility = Visibility.Hidden;
                lblTooltipTaskName.ToolTip = "Task Name";
            }
            if (string.IsNullOrWhiteSpace(txtTaskDesc.Text))
            {
                txtTaskDesc.BorderBrush = new SolidColorBrush(Colors.Red);
                txtTaskDesc.ToolTip = "Task description Should not be empty";
                lblTooltipTaskDesc.Visibility = Visibility.Visible;
                lblTooltipTaskDesc.ToolTip = "Task description Should not be empty";
                return false;
            }
            else
            {
                txtTaskDesc.BorderBrush = new SolidColorBrush(Colors.Black);
                txtTaskDesc.ToolTip = "Task description";
                lblTooltipTaskDesc.Visibility = Visibility.Hidden;
                lblTooltipTaskDesc.ToolTip = "Task description";
            }
            return true;
        }

        /// <summary>
        /// Method used to show ManageTask window
        /// </summary>

        private new static void Show()
        {
            _manageTask = new ManageTask();
            _manageTask.ShowDialog();
        }
        #endregion
        #region Trigger    

        /// <summary>
        /// Event used to create new trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnNewTrigger_Click(object sender, RoutedEventArgs e)
        {
            ManageTrigger.ShowCreate();
            if (ManageTrigger._customtrigger != null)
            {
                if (!TriggerList.Contains(ManageTrigger._customtrigger))
                    TriggerList.Add(ManageTrigger._customtrigger);
            }
            dgTriggerlist.ItemsSource = TriggerList;
            dgTriggerlist.Items.Refresh();

        }

        /// <summary>
        /// Event used to edit selected trigger from trigger list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnEditTrigger_Click(object sender, RoutedEventArgs e)
        {
            var _selectedTrigger = dgTriggerlist.SelectedItem as CustomTrigger;
            ManageTrigger.ShowEdit(_selectedTrigger);
            if (ManageTrigger._customtrigger != null)
            {
                TriggerList.RemoveAll(x => x._Trigger.Id == _selectedTrigger._Trigger.Id);
                if (!TriggerList.Contains(ManageTrigger._customtrigger))
                    TriggerList.Add(ManageTrigger._customtrigger);
            }
            dgTriggerlist.ItemsSource = TriggerList;
            dgTriggerlist.Items.Refresh();
        }

        /// <summary>
        /// Event used to delete selected trigger from trigger list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnDeleteTrigger_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgTriggerlist.SelectedItems.Count; i++)
            {
                var _selectedTriggers = dgTriggerlist.SelectedItems[i] as CustomTrigger;
                TriggerList.RemoveAll(x => x._Trigger.Id == _selectedTriggers._Trigger.Id);
            }
            dgTriggerlist.ItemsSource = TriggerList;
            dgTriggerlist.Items.Refresh();
        }

        /// <summary>
        /// Event fired on selection change of triggers from trigger list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void dgTriggerlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgTriggerlist.SelectedItems.Count > 0)
            {
                btnEditTrigger.IsEnabled = true;
                btnDeleteTrigger.IsEnabled = true;
                ImageEditTrigger.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/EditTriggerOn.png") as ImageSource;
                ImageDeleteTrigger.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/DeleteOn.png") as ImageSource;
            }
            else
            {
                btnEditTrigger.IsEnabled = false;
                btnDeleteTrigger.IsEnabled = false;
                ImageEditTrigger.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/EditTriggerOff.png") as ImageSource;
                ImageDeleteTrigger.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/DeleteOff.png") as ImageSource;
            }
        }

        /// <summary>
        /// Event fired on mouse double click on selected trigger from trigger list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void dgTriggerlist_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var _selectedTrigger = dgTriggerlist.SelectedItem as CustomTrigger;
            if (_selectedTrigger != null)
            {
                ManageTrigger.ShowEdit(_selectedTrigger);
                if (ManageTrigger._customtrigger != null)
                {
                    TriggerList.RemoveAll(x => x._Trigger.Id == _selectedTrigger._Trigger.Id);
                    //if (_task != null && _task.Definition.Triggers.Contains(_selectedTrigger._Trigger))
                    //    _task.Definition.Triggers.Remove(_selectedTrigger._Trigger);
                    if (!TriggerList.Contains(ManageTrigger._customtrigger))
                        TriggerList.Add(ManageTrigger._customtrigger);
                }
                dgTriggerlist.ItemsSource = TriggerList;
                dgTriggerlist.Items.Refresh();
            }
        }
        #endregion
        #region Create Task and Edit Task

        /// <summary>
        /// Event fired on text changed of txtTaskName textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtTaskName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        /// Event fired on text changed of txtTaskDesc textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtTaskDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        /// Methos is used to highlight the text of textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        /// <summary>
        /// Method used to create show
        /// </summary>

        public static void ShowCreate()
        {
            _isCreate = true;
            Show();
        }

        /// <summary>
        /// Method used to create task
        /// </summary>
        /// <param name="taskname">it contain name of the task</param>
        /// <param name="taskdesc">it contain description of the task</param>
        /// <param name="triggerlist">it contain list of triggers for a task</param>
        /// <param name="actionList">it contain list of actions for a task</param>
        /// <returns>true or false</returns>

        private bool CreateTask(string taskname, string taskdesc, List<CustomTrigger> triggerlist, List<MTS.Action> actionList)
        {
            try
            {
                TaskServiceUtils.CreateNewTask(taskname, taskdesc, triggerlist, actionList);
                this.Close();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in CreateTask , Message : {0} ", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Method used to show edit task
        /// </summary>
        /// <param name="task">it refers to the object to task class</param>

        public static void ShowEdit(MTS.Task task)
        {
            _isCreate = false;
            _task = task;
            Show();
        }

        /// <summary>
        /// Method used to fill data from task
        /// </summary>
        /// <param name="_task">it refers to the object to task class</param>

        private void FillDataFromTask(MTS.Task _task)
        {

            txtTaskName.Text = _task.Name;
            txtTaskDesc.Text = _task.Definition.RegistrationInfo.Description;
            var _currenttriggers = _task.Definition.Triggers;
            foreach (var item in _currenttriggers)
            {
                TriggerList.Add(new CustomTrigger(item, item.Enabled, item.ToString()));

            }
            dgTriggerlist.ItemsSource = TriggerList;
            dgTriggerlist.Items.Refresh();

            var _currentactions = _task.Definition.Actions;
            foreach (var item in _currentactions)
            {
                ActionList.Add(item);
            }
            //throw new Not
            BindActionToActionParam(ActionList);

        }
        /// <summary>
        /// Method used to update task
        /// </summary>
        /// <param name="taskdesc">it contains the task description</param>
        /// <param name="triggerlist">it contain trigger list of task</param>
        /// <param name="actionList">it contain action list of task</param>
        /// <returns>true or false</returns>

        private bool UpdateTask(string taskdesc, List<CustomTrigger> triggerlist, List<MTS.Action> actionList)
        {
            try
            {
                TaskServiceUtils.EditTask(_task, taskdesc, triggerlist, ActionList);
                this.Close();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in CreateTask , Message : {0} ", ex.Message));
                return false;
            }

        }
        #endregion
        #region Save and Cancel

        /// <summary>
        /// Event fired on click of save button,used to save task details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnSaveTask_Click(object sender, RoutedEventArgs e)
        {
            if (_isCreate && ActionList.Count > 0)
            {
                CreateTask(txtTaskName.Text, txtTaskDesc.Text, TriggerList, ActionList);
            }
            else if (ActionList.Count > 0)
            {
                UpdateTask(txtTaskDesc.Text, TriggerList, ActionList);
            }
            else
            {
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Warning, CustomPopup.ePopupTitle.Warning, " A task must be registered with at least one action", CustomPopup.ePopupButton.OK);
            }
        }

        /// <summary>
        /// Event fired on click of cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnCancelTask_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
        #region Actions

        /// <summary>
        /// Event used to add new action of a task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnNewAction_Click(object sender, RoutedEventArgs e)
        {
            if (txtTaskName.Text != "")
            {
                ManageAction.ShowCreate(txtTaskName.Text);
                if (ManageAction._action != null)
                {
                    if (!ActionList.Contains(ManageAction._action))
                    {
                        ActionList.Add(ManageAction._action);
                    }
                }
                BindActionToActionParam(ActionList);
            }
            else
            {
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Information, "Please enter task name first to add new action.", CustomPopup.ePopupButton.OK);
            }
        }

        /// <summary>
        /// Event used to delete selected action of a task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnDeleteAction_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dgActionlist.SelectedItems.Count; i++)
            {
                var _selectedAction = dgActionlist.SelectedItems[i] as ActionParameters;
                ActionList.RemoveAll(x => x.Id == _selectedAction.Id);
            }
            BindActionToActionParam(ActionList);
        }

        /// <summary>
        /// Event used to edit selected action of a task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnEditAction_Click(object sender, RoutedEventArgs e)
        {
            var _selectedAction = dgActionlist.SelectedItem as ActionParameters;

            // ManageAction.ShowEdit(ActionList.Find(x => x.Id == _selectedAction.Id), _task.Name);
            ManageAction.ShowEdit(_selectedAction);
            if (ManageAction._action != null)
            {
                ActionList.RemoveAll(x => x.Id == _selectedAction.Id);
                if (!ActionList.Contains(ManageAction._action))
                {
                    ActionList.Add(ManageAction._action);
                }
            }
            BindActionToActionParam(ActionList);
        }

        /// <summary>
        /// Event fired on selection change of dgActionlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void dgActionlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgActionlist.SelectedItems.Count > 0)
            {
                ImageEditAction.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/EditTriggerOn.png") as ImageSource;
                ImageDeleteAction.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/DeleteOn.png") as ImageSource;
                btnDeleteAction.IsEnabled = true;
                btnEditAction.IsEnabled = true;
            }
            else
            {
                ImageEditAction.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/EditTriggerOff.png") as ImageSource;
                ImageDeleteAction.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/DeleteOff.png") as ImageSource;
                btnDeleteAction.IsEnabled = false;
                btnEditAction.IsEnabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionList"></param>
        private void BindActionToActionParam(List<MTS.Action> actionList)
        {
            ActionParamList.Clear();
            foreach (var action in actionList)
            {
                if (action.ActionType == MTS.TaskActionType.Execute)
                {
                    var executeAction = action as MTS.ExecAction;
                    string[] args = executeAction.Arguments.Split('\"').Where((s, i) => i % 2 != 0).ToArray();
                    ActionParamList.Add(new ActionParameters()
                    {
                        Id = action.Id,
                        TaskName = args[0],
                        ActionType = (ActionTypeEnum)Enum.Parse(typeof(ActionTypeEnum), args[1]),
                        BackupRestoreType = (BackupRestoreTypeEnum)Enum.Parse(typeof(BackupRestoreTypeEnum), args[2]),
                        BackupRestoreLocation = (BackupRestoreLoactionEnum)Enum.Parse(typeof(BackupRestoreLoactionEnum), args[3]),
                        SourceServerDetails = args[4],
                        SourceLocation = args[5],
                        DestinationDetails = args[6],
                        DestinationBucketName = args[7]

                    });
                }
            }
            dgActionlist.ItemsSource = ActionParamList;
            dgActionlist.Items.Refresh();
        }

        /// <summary>
        ///  Event fired on mouse double click on item of dgActionlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void dgActionlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var _selectedAction = dgActionlist.SelectedItem as ActionParameters;
            if (_selectedAction != null)
            {
                ManageAction.ShowEdit(_selectedAction);
                //ManageAction.ShowEdit(ActionList.Find(x => x.Id == _selectedAction.Id), txtTaskName.Text);
                if (ManageAction._action != null)
                {
                    ActionList.RemoveAll(x => x.Id == _selectedAction.Id);
                    if (!ActionList.Contains(ManageAction._action))
                        ActionList.Add(ManageAction._action);
                }
                BindActionToActionParam(ActionList);
            }
        }
        #endregion      
    }
}

