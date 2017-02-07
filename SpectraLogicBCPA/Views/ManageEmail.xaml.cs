using DataProtectionApplication.CommonLibrary.Model;
using DataProtectionApplication.CommonLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataProtectionApplication.TaskSchedulingApp.Common;
using DataProtectionApplication.CommonLibrary.Constants;

namespace DataProtectionApplication.TaskSchedulingApp.Views
{
    /// <summary>
    /// Interaction logic for ManageEmail.xaml
    /// </summary>
    public partial class ManageEmail : Window
    {
        public static Logger logger = new Logger(typeof(ManageEmail));
        private static EmailConfiguration _currentEmailConfig;
        private static bool _isCreate;
        private static ManageEmail _emailConfig;

        public ManageEmail()
        {
            InitializeComponent();
            if (!_isCreate)
                FillDataFromEmailConfig(_currentEmailConfig);
            CanOperate();
        }

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
        #region ManageEmail Methods

        /// <summary>
        /// Method used to handle validation
        /// </summary>
        /// 
        private void CanOperate()
        {
            btnSaveEmailConfig.IsEnabled = IsValidEmailConfig();
        }
        private void FillDataFromEmailConfig(EmailConfiguration _currentEmailConfig)
        {
            txtSmtpAddress.Text = _currentEmailConfig.SmtpAddress;
            txtServerPort.Text = _currentEmailConfig.PortNumber.ToString();
            txtEmailFrom.Text = _currentEmailConfig.EmailFrom;
            txtEmailTo.Text = _currentEmailConfig.EmailTo;
            txtPassword.Password = _currentEmailConfig.Password;
        }
        private bool IsValidEmailConfig()
        {
            if (string.IsNullOrWhiteSpace(txtSmtpAddress.Text))
            {
                txtSmtpAddress.BorderBrush = new SolidColorBrush(Colors.Red);
                txtSmtpAddress.ToolTip = "SmtpAddress should not be empty.";
                lblTooltipSMTPAddress.Visibility = Visibility.Visible;
                return false;
            }
            else
            {
                txtSmtpAddress.BorderBrush = new SolidColorBrush(Colors.Black);
                txtSmtpAddress.ToolTip = "SMTP Address";
                lblTooltipSMTPAddress.Visibility = Visibility.Hidden;
            }
            if (string.IsNullOrWhiteSpace(txtServerPort.Text))
            {
                txtServerPort.BorderBrush = new SolidColorBrush(Colors.Red);
                txtServerPort.ToolTip = "SMTP Port should not be empty";
                lblTooltipPort.Visibility = Visibility.Visible;
                return false;
            }
            else
            {
                txtServerPort.BorderBrush = new SolidColorBrush(Colors.Black);
                txtServerPort.ToolTip = "SMTP Port";
                lblTooltipPort.Visibility = Visibility.Hidden;
            }
            if (string.IsNullOrWhiteSpace(txtEmailFrom.Text))
            {
                txtEmailFrom.BorderBrush = new SolidColorBrush(Colors.Red);
                txtEmailFrom.ToolTip = "Email From should not be empty.";
                lblTooltipEmailFrom.Visibility = Visibility.Visible;
                return false;
            }
            else if (Constant.validEmailRegex.IsMatch(txtEmailFrom.Text))
            {
                txtEmailFrom.BorderBrush = new SolidColorBrush(Colors.Black);
                txtEmailFrom.ToolTip = "Email From";
                lblTooltipEmailFrom.Visibility = Visibility.Hidden;
            }
            else
            {
                txtEmailFrom.BorderBrush = new SolidColorBrush(Colors.Red);
                txtEmailFrom.ToolTip = "Email From should be in proper format.";
                lblTooltipEmailFrom.Visibility = Visibility.Visible;
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmailTo.Text))
            {
                txtEmailTo.BorderBrush = new SolidColorBrush(Colors.Red);
                txtEmailTo.ToolTip = "Email To should not be empty.";
                lblTooltipEmailTo.Visibility = Visibility.Visible;
                return false;
            }
            else if (Constant.validEmailRegex.IsMatch(txtEmailTo.Text))
            {
                txtEmailTo.BorderBrush = new SolidColorBrush(Colors.Black);
                txtEmailTo.ToolTip = "Email To";
                lblTooltipEmailTo.Visibility = Visibility.Hidden;
            }
            else
            {
                txtEmailTo.BorderBrush = new SolidColorBrush(Colors.Red);
                txtEmailTo.ToolTip = "Email To should be in proper format.";
                lblTooltipEmailTo.Visibility = Visibility.Visible;
                return false;
            }
            //if (string.IsNullOrWhiteSpace(txtUsername.Text))
            //{
            //    txtUsername.BorderBrush = new SolidColorBrush(Colors.Red);
            //    txtUsername.ToolTip = "Username should not be empty.";
            //    lblTooltipUsername.Visibility = Visibility.Visible;
            //    return false;
            //}
            //else
            //{
            //    txtUsername.BorderBrush = new SolidColorBrush(Colors.Black);
            //    txtUsername.ToolTip = "Username";
            //    lblTooltipUsername.Visibility = Visibility.Hidden;
            //}
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Red);
                txtPassword.ToolTip = "Password should not be empty.";
                lblTooltipPassword.Visibility = Visibility.Visible;
                return false;
            }
            else
            {
                txtPassword.BorderBrush = new SolidColorBrush(Colors.Black);
                txtPassword.ToolTip = "Password";
                lblTooltipPassword.Visibility = Visibility.Hidden;
            }
            return true;
        }
        internal static void ShowCreate()
        {
            _isCreate = true;
            Show();
        }
        internal static void ShowEdit(EmailConfiguration currentEmaiConfig)
        {
            _currentEmailConfig = currentEmaiConfig;
            _isCreate = false;
            Show();
        }
        private new static void Show()
        {
            _emailConfig = new ManageEmail();
            _emailConfig.ShowDialog();
        }

        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                CanOperate();
            }
        }
        private void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
                tb.SelectAll();
        }
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
        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CanOperate();
        }
        #endregion
        #region Save Email Configuration
        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _currentEmailConfig = new EmailConfiguration()
                {
                    SmtpAddress = txtSmtpAddress.Text,
                    PortNumber = Convert.ToInt16(txtServerPort.Text),
                    EmailFrom = txtEmailFrom.Text,
                    EmailTo = txtEmailTo.Text,
                    Password = txtPassword.Password
                };
                Util.SaveEmailConfiguration(_currentEmailConfig);
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Success, "Email configuration saved successfully.", CustomPopup.ePopupButton.OK);
                this.Close();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in btnSaveConfig_Click event, Message : {0}", ex.Message));
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Error, CustomPopup.ePopupTitle.Error, "Unable to save email configuration.", CustomPopup.ePopupButton.OK);
            }
        }
        #endregion

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
