//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MTS = Microsoft.Win32.TaskScheduler;
using DataProtectionApplication.TaskSchedulingApp.Common;
using DataProtectionApplication.CommonLibrary;
using System.Xml.Linq;
using Ds3.Calls;
using DataProtectionApplication.CommonLibrary.Model;
using DataProtectionApplication.TaskSchedulingApp.Model;
using DataProtectionApplication.CommonLibrary.Constants;

namespace DataProtectionApplication.TaskSchedulingApp.Views
{
    /// <summary>
    /// Interaction logic for ManageAction.xaml
    /// </summary>
    public partial class ManageAction : Window
    {
        #region Properties and variables
        public static Logger logger = new Logger(typeof(ManageAction));
        public static ManageAction _manageAction;
        //public static string _taskName;
        public static MTS.Action _action { get; private set; }
        private static ActionParameters _actionParameter;
        private static bool _isCreate = true;
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor of ManageAction screen
        /// </summary>

        public ManageAction()
        {
            InitializeComponent();
            if (!_isCreate)
            {
                lbTitle.Content = "Edit Action";
                //FillActionFromTask(_action);
                FillActionFromTask(_actionParameter);
            }
            else
            {
                lbTitle.Content = "Create Action";
                cmbBackupRestoreType.SelectedIndex = 1;
                LoadDestinationBlackPearlConfiguration();
            }
            CanOperate();
        }

        #endregion
        #region Window Methods
        /// <summary>
        /// Method used to load configuration of DestinationBlackPearl
        /// </summary>

        private void LoadDestinationBlackPearlConfiguration()
        {
            if (File.Exists(Constant.DestinationServerDetails))
            {
                BlackPearlConfiguration _config = Util.LoadConfigurationFromFile(Constant.DestinationServerDetails);
                try
                {
                    //Ds3Client _client = new Ds3Client(_config.GetEndPoint(), _config.AccessId, _config.SecretKey, "");
                    //HeadBucketResponse response = _client.HeadBucket(new Ds3.Calls.HeadBucketRequest(_config.BucketName));
                    //if (response.Status == HeadBucketResponse.StatusType.Exists)
                    //{
                    txtFullDestinationBPBucket.Text = _config.BucketName;

                    _actionParameter.DestinationBucketName = _config.BucketName;
                    _actionParameter.DestinationDetails = File.ReadAllText(Constant.DestinationServerDetails);
                    logger.LogInfo(string.Format("Destination file , file name : {0}, exists and loaded successfully.", Constant.DestinationServerDetails));
                    //}
                }
                catch (Exception ex)
                {
                    logger.LogError(string.Format("Exception in LoadDestinationBlackPearlConfiguration method, Message : {0}", ex.Message));
                    new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Error, CustomPopup.ePopupTitle.Error, ex.Message, CustomPopup.ePopupButton.OK);
                }

            }
            CanOperate();
        }

        /// <summary>
        /// This method is used to fill action parameters from task
        /// </summary>
        /// <param name="actionParameter"></param>
        private void FillActionFromTask(ActionParameters actionParameter)
        {
            if (_actionParameter.BackupRestoreType == BackupRestoreTypeEnum.Full)
                cmbBackupRestoreType.SelectedValue = BackupRestoreTypeEnum.Full;
            else
                cmbBackupRestoreType.SelectedValue = BackupRestoreTypeEnum.Partial;
            if (_actionParameter.BackupRestoreLocation == BackupRestoreLoactionEnum.Local)
            {
                rbtBackUpFullFromLocal.IsChecked = true;
                txtSourceFolder.Text = actionParameter.SourceLocation;
            }
            else
            {
                rbtBackUpFullFromServer.IsChecked = true;
                txtFullSourceServerBucket.Text = actionParameter.SourceLocation;
            }
            txtFullDestinationBPBucket.Text = actionParameter.DestinationBucketName;
        }

        /// <summary>
        /// Event fired on window loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Event fired on click of exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _action = null;
            this.Close();
        }

        /// <summary>
        /// Method used to show created task 
        /// </summary>
        /// <param name="currentTaskName">it returns the name of selected task</param>

        public static void ShowCreate(string currentTaskName)
        {
            _actionParameter = new ActionParameters();
            _isCreate = true;
            _actionParameter.Id = Guid.NewGuid().ToString("N").ToUpper();
            _actionParameter.TaskName = currentTaskName;
            _actionParameter.ActionType = ActionTypeEnum.Backup;
            _actionParameter.BackupRestoreType = BackupRestoreTypeEnum.Partial;
            Show();
        }

        /// <summary>
        /// Method used to show edit details.
        /// </summary>
        /// <param name="actionparameter">it return the object of actionparameter class </param>
        /// <param name="currentTaskName">it return the name of selected task</param>
        public static void ShowEdit(ActionParameters actionparameter)
        {
            _actionParameter = new ActionParameters();
            _isCreate = false;
            _actionParameter = actionparameter;
            Show();
        }

        /// <summary>
        /// Method used to show manage action window
        /// </summary>

        private new static void Show()
        {
            _manageAction = new ManageAction();
            _manageAction.ShowDialog();
        }
        #endregion
        #region Save and Cancel Action

        /// <summary>
        /// Event fired on click of save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnSaveAction_Click(object sender, RoutedEventArgs e)
        {
            ErrorResult _result = IsValidSource();
            if (_result.ErrorCode == ErrorCodes.Success)
            {
                _result = IsValidDestination();
                if (_result.ErrorCode == ErrorCodes.Success)
                {
#if DEBUG
                    _actionParameter.ActionPath = @"E:\Spectra logic\SpectraLogicBCPA2_25Jan\SpectraLogicBCPA2\FileTransferApp\bin\x64\Debug\FileTransferApp.exe";
#else
                    string path = System.AppDomain.CurrentDomain.BaseDirectory + "File Transfer App\\FileTransferApp.exe";
                    logger.LogInfo(path);
                    _actionParameter.ActionPath = path;
#endif
                    var arg0 = _actionParameter.TaskName; //Task Name
                    var arg1 = _actionParameter.ActionType;                              // Action Type : Backup\Restore
                    var arg2 = _actionParameter.BackupRestoreType;                       // Backup/Restore Type : Full\Partial
                    var arg3 = _actionParameter.BackupRestoreLocation;                   // Location : Local\Server
                    var arg4 = _actionParameter.SourceServerDetails != null ? _actionParameter.SourceServerDetails.Replace("\"", "'") : "";
                    var arg5 = _actionParameter.SourceLocation;
                    var arg6 = _actionParameter.DestinationDetails.Replace("\"", "'");
                    var arg7 = _actionParameter.DestinationBucketName;
                    var arguments = $"\"{arg0}\" \"{arg1}\" \"{arg2}\" \"{arg3}\" \"{arg4}\" \"{arg5}\" \"{arg6}\" \"{arg7}\"";
                    _action = new MTS.ExecAction(_actionParameter.ActionPath, arguments);
                    _action.Id = Guid.NewGuid().ToString("N").ToUpper();
                    this.Close();
                }
                else
                {
                    new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Warning, CustomPopup.ePopupTitle.Warning, _result.ErrorMessage, CustomPopup.ePopupButton.OK);
                }
            }
            else
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Warning, CustomPopup.ePopupTitle.Warning, _result.ErrorMessage, CustomPopup.ePopupButton.OK);
        }

        /// <summary>
        /// Event fired on click of cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnCancelAction_Click(object sender, RoutedEventArgs e)
        {
            _action = null;
            this.Close();
        }
        #endregion
        #region Manage Action

        /// <summary>
        /// Method used to enable/disable save button
        /// </summary>

        private void CanOperate()
        {
            btnSaveAction.IsEnabled = IsValidAction(); //& IsValidDatabaseLocation();
        }

        /// <summary>
        /// Method used to check whether action is valid or not
        /// </summary>
        /// <returns>true or false</returns>

        private bool IsValidAction()
        {
            if (_actionParameter.BackupRestoreLocation == BackupRestoreLoactionEnum.Local)
            {
                if (!string.IsNullOrWhiteSpace(txtSourceFolder.Text) && !string.IsNullOrWhiteSpace(txtFullDestinationBPBucket.Text))
                {
                    txtFullDestinationBPBucket.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtSourceFolder.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtFullDestinationBPBucket.ToolTip = txtFullDestinationBPBucket.Text;
                    txtSourceFolder.ToolTip = txtSourceFolder.Text;
                    lbltooltipFullDestinationBPBucket.Visibility = Visibility.Hidden;
                    lbltooltipLocalSourceFolder.Visibility = Visibility.Hidden;
                    return true;
                }
                else if (string.IsNullOrWhiteSpace(txtFullDestinationBPBucket.Text))
                {
                    txtFullDestinationBPBucket.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtFullDestinationBPBucket.ToolTip = "Destination bucket Should not be empty";
                    lbltooltipFullDestinationBPBucket.Visibility = Visibility.Visible;
                    lbltooltipFullDestinationBPBucket.ToolTip = "Destination bucket Should not be empty";
                    return false;
                }
                else if (string.IsNullOrWhiteSpace(txtSourceFolder.Text))
                {
                    txtSourceFolder.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtSourceFolder.ToolTip = "Source folder path Should not be empty";
                    lbltooltipLocalSourceFolder.Visibility = Visibility.Visible;
                    lbltooltipLocalSourceFolder.ToolTip = "Source folder path Should not be empty";
                    return false;
                }
            }
            else if (_actionParameter.BackupRestoreLocation == BackupRestoreLoactionEnum.Server)
            {
                if (!string.IsNullOrWhiteSpace(txtFullSourceServerBucket.Text) && !string.IsNullOrWhiteSpace(txtFullDestinationBPBucket.Text))
                {
                    txtFullDestinationBPBucket.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtFullDestinationBPBucket.ToolTip = txtFullDestinationBPBucket.Text;
                    lbltooltipFullDestinationBPBucket.Visibility = Visibility.Hidden;

                    txtFullSourceServerBucket.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtFullSourceServerBucket.ToolTip = txtFullSourceServerBucket.Text;
                    lbltooltipFullSourceServerBucket.Visibility = Visibility.Hidden;
                    return true;
                }
                else if (string.IsNullOrWhiteSpace(txtFullSourceServerBucket.Text))
                {
                    txtFullSourceServerBucket.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtFullSourceServerBucket.ToolTip = "Source Server bucket Should not be empty";
                    lbltooltipFullSourceServerBucket.Visibility = Visibility.Visible;
                    lbltooltipFullDestinationBPBucket.ToolTip = "Source Server bucket Should not be empty";
                    return false;
                }
                else if (string.IsNullOrWhiteSpace(txtFullDestinationBPBucket.Text))
                {
                    txtFullDestinationBPBucket.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtFullDestinationBPBucket.ToolTip = "Destination bucket Should not be empty";
                    lbltooltipFullDestinationBPBucket.Visibility = Visibility.Visible;
                    lbltooltipFullDestinationBPBucket.ToolTip = "Destination bucket Should not be empty";
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Event fired on selection change of combo box item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void cmbActionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_actionParameter != null && lblBackUpType != null)
            {
                ComboBoxItem cmbItem = (ComboBoxItem)cmbActionType.SelectedItem;
                if (cmbItem.Content.ToString() == ActionTypeEnum.Backup.ToString())
                    _actionParameter.ActionType = ActionTypeEnum.Backup;
                else
                    _actionParameter.ActionType = ActionTypeEnum.Restore;
                lblBackUpType.Content = _actionParameter.ActionType.ToString() + " Type";
            }
        }

        /// <summary>
        /// Event fired on selection change of combo box item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void cmbBackupRestoreType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_actionParameter != null)
            {
                if ((BackupRestoreTypeEnum)cmbBackupRestoreType.SelectedItem == BackupRestoreTypeEnum.Full)
                    _actionParameter.BackupRestoreType = BackupRestoreTypeEnum.Full;
                else
                    _actionParameter.BackupRestoreType = BackupRestoreTypeEnum.Partial;
            }
        }

        /// <summary>
        /// Method used to update task
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
        /// Method used to validate whether bucket already exists or not
        /// </summary>
        /// <param name="IsBucketExists">Is Bucket exists or not</param>
        /// /// <param name="serverType">Source/Destination</param>
        /// <returns>true or false</returns>

        private bool BucketExistsValidation(bool isBucketExists, ServerType serverType)
        {
            if (serverType == ServerType.Source)
            {
                if (isBucketExists)
                {
                    txtFullSourceServerBucket.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtFullSourceServerBucket.ToolTip = txtFullSourceServerBucket.Text;
                    lbltooltipFullSourceServerBucket.Visibility = Visibility.Hidden;
                    return true;
                }
                else
                {
                    txtFullSourceServerBucket.BorderBrush = new SolidColorBrush(Colors.Red);
                    lbltooltipFullSourceServerBucket.Visibility = Visibility.Visible;
                    txtFullSourceServerBucket.ToolTip = string.Format("Bucket {0} does not Exists.", txtFullSourceServerBucket.Text);
                    return false;
                }
            }
            else
            {
                if (isBucketExists)
                {
                    txtFullDestinationBPBucket.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtFullDestinationBPBucket.ToolTip = txtFullDestinationBPBucket.Text;
                    lbltooltipFullDestinationBPBucket.Visibility = Visibility.Hidden;
                    return true;
                }
                else
                {
                    txtFullDestinationBPBucket.BorderBrush = new SolidColorBrush(Colors.Red);
                    lbltooltipFullDestinationBPBucket.Visibility = Visibility.Visible;
                    txtFullDestinationBPBucket.ToolTip = string.Format("Bucket {0} does not Exists.", txtFullDestinationBPBucket.Text);
                    return false;
                }
            }
        }

        /// <summary>
        /// This method is use to validate source folder or bucket name is valid or not
        /// </summary>
        /// <returns></returns>

        private ErrorResult IsValidSource()
        {            
            ErrorResult _response = new ErrorResult();
            try
            {
                if (_actionParameter.BackupRestoreLocation == BackupRestoreLoactionEnum.Server)
                {
                    if (_actionParameter.SourceServerDetails != "")
                    {
                        XDocument doc = XDocument.Parse(_actionParameter.SourceServerDetails);
                        BlackPearlConfiguration _config = Util.LoadConfigurationFromDocument(doc);
                        GetUserSpectraS3Response userSpectraS3 = new Util().IsValidS3User(_config);
                        if (userSpectraS3 != null && userSpectraS3.ResponsePayload.AuthId == _config.GetAccessId())
                        {
                            if (!BucketExistsValidation(Util.IsBucketExist(txtFullSourceServerBucket.Text, _config), ServerType.Source))
                            {
                                _response.ErrorCode = ErrorCodes.Fail;
                                _response.ErrorMessage = "Source bucket does not exists.";
                            }
                            else
                            {

                                _actionParameter.SourceServerDetails = _actionParameter.SourceServerDetails.Replace("<BucketName>" + _actionParameter.SourceLocation + "</BucketName>", "<BucketName>" + txtFullSourceServerBucket.Text + "</BucketName>");
                                _actionParameter.SourceLocation = txtFullSourceServerBucket.Text;
                                _response.ErrorCode = ErrorCodes.Success;
                                _response.ErrorMessage = "Source bucket exists";
                            }
                        }
                        else
                            logger.LogInfo(string.Format("Access Denied."));
                    }
                    else
                    {
                        _response.ErrorCode = ErrorCodes.Fail;
                        _response.ErrorMessage = "Source BlackPearl Server not configured.";
                    }
                }
                else
                {
                    if (!DirectoryExistsValidation(Directory.Exists(txtSourceFolder.Text)))
                    {
                        _response.ErrorCode = ErrorCodes.Fail;
                        _response.ErrorMessage = "Source location is not valid.";
                    }
                    else
                    {
                        _actionParameter.SourceLocation = MappedDriveResolver.ResolveToUNC(txtSourceFolder.Text);
                        //if (_actionParameter.SourceLocation.EndsWith(@"\"))
                        _actionParameter.SourceLocation = _actionParameter.SourceLocation.TrimEnd('\\');
                        _response.ErrorCode = ErrorCodes.Success;
                        _response.ErrorMessage = "Source bucket exists";
                    }
                }
            }
            catch (Exception ex)
            {
                _response.ErrorCode = ErrorCodes.Fail;
                _response.ErrorMessage = "Something went wrong, Unable to process request.";
                logger.LogError(string.Format("Exception in IsValidSource menthod, Message : {0}", ex.Message));
            }
            return _response;
        }

        /// <summary>
        /// This method is used to validate Source folder exist or not
        /// </summary>
        /// <param name="isDirectoryExists"></param>
        /// <returns></returns>
        private bool DirectoryExistsValidation(bool isDirectoryExists)
        {
            try
            {
                if (isDirectoryExists)
                {
                    txtSourceFolder.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtSourceFolder.ToolTip = txtSourceFolder.Text;
                    lbltooltipLocalSourceFolder.Visibility = Visibility.Hidden;
                    return true;
                }
                else
                {
                    txtSourceFolder.BorderBrush = new SolidColorBrush(Colors.Red);
                    lbltooltipLocalSourceFolder.Visibility = Visibility.Visible;
                    txtSourceFolder.ToolTip = string.Format("Source folder {0} does not Exists.", txtSourceFolder.Text);
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// This method is used to validate Destination bucket.
        /// </summary>
        /// <returns></returns>

        private ErrorResult IsValidDestination()
        {
            ErrorResult _response = new ErrorResult();
            try
            {
                if (_actionParameter.DestinationDetails != "")
                {
                    XDocument doc = XDocument.Parse(_actionParameter.DestinationDetails);
                    BlackPearlConfiguration _config = Util.LoadConfigurationFromDocument(doc);                
                    GetUserSpectraS3Response userSpectraS3 = new Util().IsValidS3User(_config);
                    if (userSpectraS3 != null && userSpectraS3.ResponsePayload.AuthId == _config.GetAccessId())
                        if (!BucketExistsValidation(Util.IsBucketExist(txtFullDestinationBPBucket.Text, _config), ServerType.Destination))
                        {
                            _response.ErrorCode = ErrorCodes.Fail;
                            _response.ErrorMessage = "Destination bucket does not exists.";
                        }
                        else
                        {
                            _actionParameter.DestinationDetails = _actionParameter.DestinationDetails.Replace("<BucketName>" + _actionParameter.DestinationBucketName + "</BucketName>", "<BucketName>" + txtFullDestinationBPBucket.Text + "</BucketName>");
                            _actionParameter.DestinationBucketName = txtFullDestinationBPBucket.Text;
                            _response.ErrorCode = ErrorCodes.Success;
                            _response.ErrorMessage = "Destination bucket exists.";
                        }
                    else
                        logger.LogInfo(string.Format("Access Denied."));
                }
                else
                {
                    _response.ErrorCode = ErrorCodes.Fail;
                    _response.ErrorMessage = "Destination BlackPearl Server not configured.";
                }
            }
            catch (Exception ex)
            {
                _response.ErrorCode = ErrorCodes.Fail;
                _response.ErrorMessage = "Something went wrong, Unable to process request.";
                logger.LogError(string.Format("Exception in IsValidSource menthod, Message : {0}", ex.Message));
            }
            return _response;
        }

        #endregion
        #region Case 1 : Backup - full - Local/Server
        /// <summary>
        /// Event used to change destination blackpearl configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnchangeDesBPConfig_Click(object sender, RoutedEventArgs e)
        {
            Util.ShowBlackPearlConfiguration(_actionParameter, ServerType.Destination, _isCreate, false);
            if (BlackPearlConfig._destinationServerBucketName != "")
            {
                txtFullDestinationBPBucket.Text = BlackPearlConfig._destinationServerBucketName;
                _actionParameter.DestinationBucketName = txtFullDestinationBPBucket.Text;
                _actionParameter.DestinationDetails = BlackPearlConfig._destinationServerDetails;
            }
            CanOperate();
        }

        #region Case 1.1 : Backup - Full/Partial - Local

        /// <summary>
        /// Event fired on text changed of txtFullDestinationBPBucket textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtFullDestinationBPBucket_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        /// Event used to broswer folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtSourceFolder.Text = dialog.SelectedPath;
            _actionParameter.SourceLocation = txtSourceFolder.Text;
            CanOperate();
        }

        /// <summary>
        /// Event fired when rbtBackUpFullFromLocal checkbox in checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbtBackUpFullFromLocal_Checked(object sender, RoutedEventArgs e)
        {
            if (GridBackupFullServer != null && GridBackupFullLocal != null)
            {
                GridFullDestinationBucketName.Visibility = Visibility.Visible;
                GridBackupFullServer.Visibility = Visibility.Hidden;
                GridBackupFullLocal.Visibility = Visibility.Visible;
                _actionParameter.BackupRestoreLocation = BackupRestoreLoactionEnum.Local;
                _actionParameter.SourceServerDetails = "";
            }
            CanOperate();
        }

        /// <summary>
        /// Event fired on text changed of txtLocalSourceFolder textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtLocalSourceFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
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

        #endregion Case 1.1 : Backup - full - Local

        #region Case 1.2 : Back - Full/Partial - Server

        /// <summary>
        /// Event fired when rbtBackUpFullFromServer checkbox in checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbtBackUpFullFromServer_Checked(object sender, RoutedEventArgs e)
        {
            _actionParameter.BackupRestoreLocation = BackupRestoreLoactionEnum.Server;
            GridFullDestinationBucketName.Visibility = Visibility.Visible;
            GridBackupFullServer.Visibility = Visibility.Visible;
            GridBackupFullLocal.Visibility = Visibility.Hidden;
            CanOperate();
        }

        /// <summary>
        /// Event fired on text changed of txtFullSourceServerBucket textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtFullSourceServerBucket_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        /// Event used to change source blackpearl configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnchangeSourceBPConfig_Click(object sender, RoutedEventArgs e)
        {

            Util.ShowBlackPearlConfiguration(_actionParameter, ServerType.Source, _isCreate, false);
            if (BlackPearlConfig._sourceServerBucketName != "")
            {
                txtFullSourceServerBucket.Text = BlackPearlConfig._sourceServerBucketName;
                _actionParameter.SourceLocation = txtFullSourceServerBucket.Text;
                _actionParameter.SourceServerDetails = BlackPearlConfig._sourceServerDetails;
            }
            CanOperate();
        }

        #endregion

        #endregion

    }
}
