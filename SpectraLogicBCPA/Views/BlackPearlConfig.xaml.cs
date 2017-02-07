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
using System.Xml.Serialization;
using Ds3.Helpers;
using Ds3.Calls;
using System.Net;
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.TaskSchedulingApp.Common;
using DataProtectionApplication.CommonLibrary.Model;
using System.Text;
using DataProtectionApplication.CommonLibrary.Constants;

namespace DataProtectionApplication.TaskSchedulingApp.Views
{
    /// <summary>
    /// Interaction logic for BlackPearlConfig.xaml
    /// </summary>
    public partial class BlackPearlConfig : Window
    {
        #region Properties and Variables

        /// <summary>
        /// Button OK content for blackPearlConfiguration Page
        /// </summary>
        public const string btnContentOK = "OK";
        public static Logger logger = new Logger(typeof(BlackPearlConfig));
        private static BlackPearlConfig _blackPearlConfig;
        private static bool _isCreate = true;
        private static ServerType _serverType;
        public static string _destinationServerBucketName = "";
        public static string _sourceServerBucketName = "";
        public static string _sourceServerDetails = "";
        public static string _destinationServerDetails = "";
        private static BlackPearlConfiguration _config;
        private static bool isSaveConfiguration = false; // This is use to store value if configuration will be save or not.
        #endregion
        #region Constructor

        /// <summary>
        /// Constructor of BlackPearlConfig class
        /// </summary>

        public BlackPearlConfig()
        {
            InitializeComponent();
            if (!_isCreate)
            {
                FillDataFromConfig(_config);
            }
            if (!isSaveConfiguration)
            {
                btnSaveConfig.Content = btnContentOK;
                lblTitle.Content = string.Format("{0} BlackPearl Configuration", _serverType);
            }
            else
                lblTitle.Content = string.Format("Default {0} BlackPearl Configuration", _serverType);
            CanOperate();
        }
        #endregion
        #region texbox validation
        /// <summary>
        /// Event fires on text change of txtPort.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                CanOperate();
            }
        }

        /// <summary>
        ///  Event fires on text change of txtIP.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        ///  Event fires on text change of txtSecretKey.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtSecretKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        ///  Event fires on text change of txtAcceddID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtAccessID_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        ///  Event fires on text change of txtBucketName.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBucketName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CanOperate();
        }
        #endregion
        #region Save and Cancel Configuration
        /// <summary>
        /// Event fire on click of Cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnCancelConfig_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event fires on click on Test Connection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnTestConn_Click(object sender, RoutedEventArgs e)
        {
            _config = new BlackPearlConfiguration
            {
                IP = txtIP.Text,
                Port = Convert.ToInt16(txtPort.Text),
                BucketName = txtBucketName.Text,
                AccessId = txtAccessID.Text,
                SecretKey = txtSecretKey.Text,
                ServerType = _serverType
            };
            //Validate S3 user for provided BlackPearl configuration
            GetUserSpectraS3Response UserSpectraS3Response = new Util().IsValidS3User(_config);
            if (UserSpectraS3Response != null && UserSpectraS3Response.ResponsePayload.AuthId == _config.GetAccessId())
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Success, "Connected with BlackPearl server.", CustomPopup.ePopupButton.OK);
            else
                logger.LogInfo(string.Format("Access Denied."));
        }

        /// <summary>
        /// Event fires on click of save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                _config = new BlackPearlConfiguration
                {
                    IP = txtIP.Text,
                    Port = Convert.ToInt16(txtPort.Text),
                    BucketName = txtBucketName.Text,
                    AccessId = txtAccessID.Text,
                    SecretKey = txtSecretKey.Text,
                    ServerType = _serverType
                };
                //Validate S3 user for provided BlackPearl configuration
                GetUserSpectraS3Response UserSpectraS3Response = new Util().IsValidS3User(_config);
                if (UserSpectraS3Response != null && UserSpectraS3Response.ResponsePayload.AuthId == _config.AccessId)
                {
                    // Not need to save configuration.
                    if (!isSaveConfiguration)
                    {
                        XmlSerializer ser = new XmlSerializer(_config.GetType());
                        MemoryStream memStm = new MemoryStream();
                        ser.Serialize(memStm, _config);
                        memStm.Position = 0;
                        if (_serverType == ServerType.Source)
                        {
                            _sourceServerBucketName = _config.BucketName;
                            _sourceServerDetails = Encoding.UTF8.GetString(memStm.GetBuffer()).Replace("\"", "'").Replace("\r\n", "");
                        }
                        else
                        {
                            _destinationServerBucketName = _config.BucketName;
                            _destinationServerDetails = Encoding.UTF8.GetString(memStm.GetBuffer()).Replace("\"", "'").Replace("\r\n", "");
                        }
                    }
                    else if (Util.IsBucketExist(_config.GetBucketName(), _config)) // If bucket exists
                    {
                        if (_serverType == ServerType.Destination)
                        {
                            _destinationServerBucketName = _config.BucketName;
                            _destinationServerDetails = Util.SaveBlackPearlConfiguration(_config, _serverType); // Save Default BlackPearl configuration.
                        }
                        new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Information, string.Format("BlackPearl {0} server configured successfully.", _serverType), CustomPopup.ePopupButton.OK);
                    }
                    else
                    {
                        new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Warning, CustomPopup.ePopupTitle.Warning, string.Format("Bucket does not exists , Name : {0} ", _config.GetBucketName()), CustomPopup.ePopupButton.OK);
                        return;
                    }
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in btnSaveConfig_Click, message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// Method used to handle validation for input controls
        /// </summary>

        private void CanOperate()
        {
            btnTestConn.IsEnabled = IsValidConfig();
            btnSaveConfig.IsEnabled = btnTestConn.IsEnabled && IsValidBucketName();
        }

        /// <summary>
        /// Method used to validate bucket name
        /// </summary>
        /// <returns>true or false</returns>

        private bool IsValidBucketName()
        {
            if (string.IsNullOrWhiteSpace(txtBucketName.Text))
            {
                txtBucketName.BorderBrush = new SolidColorBrush(Colors.Red);
                txtBucketName.ToolTip = "Bucket Name Should not be empty";
                lblTooltipBucketName.Visibility = Visibility.Visible;
                lblTooltipBucketName.ToolTip = "Bucket Name Should not be empty";
                return false;
            }
            else
            {
                txtBucketName.BorderBrush = new SolidColorBrush(Colors.Black);
                txtBucketName.ToolTip = "Bucket Name";
                lblTooltipBucketName.Visibility = Visibility.Hidden;
                lblTooltipBucketName.ToolTip = "Bucket Name";

            }
            return true;
        }

        /// <summary>
        ///  Method used to validate data path address
        /// </summary>
        /// <returns>true or false</returns>

        private bool IsValidConfig()
        {
            if (!string.IsNullOrWhiteSpace(txtIP.Text))
            {
                IPAddress ipAddr;
                if (Constant.validHostnameRegex.IsMatch(txtIP.Text.Trim()) || IPAddress.TryParse(txtIP.Text, out ipAddr))
                {
                    txtIP.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtIP.ToolTip = "Data Path Address";
                    lblTooltipIP.Visibility = Visibility.Hidden;
                    lblTooltipIP.ToolTip = "Data Path Address";
                }
                else
                {
                    txtIP.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtIP.ToolTip = "Data Path Address should be in proper format.";
                    lblTooltipIP.Visibility = Visibility.Visible;
                    lblTooltipIP.ToolTip = "Data Path Address should be in proper format.";
                    return false;
                }
            }
            else
            {
                txtIP.BorderBrush = new SolidColorBrush(Colors.Red);
                txtIP.ToolTip = "Data Path Address Should not be empty";
                lblTooltipIP.Visibility = Visibility.Visible;
                lblTooltipIP.ToolTip = "Data Path Address Should not be empty";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPort.Text))
            {
                txtPort.BorderBrush = new SolidColorBrush(Colors.Red);
                txtPort.ToolTip = "Port Should not be empty";
                lblTooltipPort.Visibility = Visibility.Visible;
                lblTooltipPort.ToolTip = "Port Should not be empty";
                return false;
            }
            else
            {
                txtPort.BorderBrush = new SolidColorBrush(Colors.Black);
                txtPort.ToolTip = "Port";
                lblTooltipPort.Visibility = Visibility.Hidden;
                lblTooltipPort.ToolTip = "Port";
            }
            if (string.IsNullOrWhiteSpace(txtAccessID.Text))
            {
                txtAccessID.BorderBrush = new SolidColorBrush(Colors.Red);
                txtAccessID.ToolTip = "Access ID should not be empty";
                lblTooltipAccessID.Visibility = Visibility.Visible;
                lblTooltipAccessID.ToolTip = "Access ID should not be empty";
                return false;
            }
            else
            {
                txtAccessID.BorderBrush = new SolidColorBrush(Colors.Black);
                txtAccessID.ToolTip = "Access ID";
                lblTooltipAccessID.Visibility = Visibility.Hidden;
                lblTooltipAccessID.ToolTip = "Access ID";
            }
            if (string.IsNullOrWhiteSpace(txtSecretKey.Text))
            {
                txtSecretKey.BorderBrush = new SolidColorBrush(Colors.Red);
                txtSecretKey.ToolTip = "Secret Key Should not be empty";
                lblTooltipSecretKey.Visibility = Visibility.Visible;
                lblTooltipSecretKey.ToolTip = "Secret Key Should not be empty";
                return false;
            }
            else
            {
                txtSecretKey.BorderBrush = new SolidColorBrush(Colors.Black);
                txtSecretKey.ToolTip = "Secret Key";
                lblTooltipSecretKey.Visibility = Visibility.Hidden;
                lblTooltipSecretKey.ToolTip = "Secret Key";

            }
            return true;
        }

        /// <summary>
        /// Event fires on key down of textPort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txtPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (new Util().IsNumberKey(e.Key) || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }


        #endregion
        #region BlackPearlConfig

        /// <summary>
        /// Method used to edit BlackPearl configurations
        /// </summary>
        /// <param name="config">config used as an object of BlackPearlConfiguration class</param>

        internal static void ShowEdit(BlackPearlConfiguration config, bool isSaveConfig)
        {
            _isCreate = false;
            _config = config;
            _serverType = config.ServerType;
            isSaveConfiguration = isSaveConfig;
            Show();
        }

        /// <summary>
        /// Method used to create configuration
        /// </summary>
        /// <param name="servertype">servertype return the enum value of ServerType</param>

        internal static void ShowCreate(ServerType servertype, bool isSaveConfig)
        {
            _serverType = servertype;
            _isCreate = true;
            isSaveConfiguration = isSaveConfig;
            Show();
        }

        /// <summary>
        /// Method used to show blackpearlConfig screen
        /// </summary>

        private new static void Show()
        {
            _blackPearlConfig = new BlackPearlConfig();
            _blackPearlConfig.ShowDialog();
        }

        /// <summary>
        /// Method used to load data from xml to textboxes.
        /// </summary>
        /// <param name="config">config used as an object of BlackPearlConfiguration Screen</param>

        private void FillDataFromConfig(BlackPearlConfiguration config)
        {
            _serverType = config.ServerType;
            txtIP.Text = config.IP;
            txtPort.Text = config.Port.ToString();
            txtSecretKey.Text = config.SecretKey;
            txtAccessID.Text = config.AccessId;
            txtBucketName.Text = config.BucketName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
                tb.SelectAll();
        }
        #endregion
        #region Window Methods
        /// <summary>
        /// Event fire on click of exit button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //_destinationServerBucketName = "";
            this.Close();
        }

        /// <summary>
        /// Event used to make screen movable.
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

    }
}
