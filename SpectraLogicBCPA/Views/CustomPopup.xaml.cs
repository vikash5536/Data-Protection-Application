//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using DataProtectionApplication.CommonLibrary;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DataProtectionApplication.TaskSchedulingApp.Views
{
    /// <summary>
    /// Interaction logic for CustomPopup.xaml
    /// </summary>
    public partial class CustomPopup : Window
    {
        #region Properties and Variables
        ePopupResult result;
        public static Logger logger = new Logger(typeof(CustomPopup));
        public enum ePopupButton { YesNo = 0, OkCancel, OK };
        public enum ePopupImage { Warning = 0, Info, Error };
        public enum ePopupTitle { Warning = 0, Information, Error, Success };
        public enum ePopupResult { OK = 0, Cancel, Yes, No };
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor of CustomPopup Class
        /// </summary>

        public CustomPopup()
        {
            InitializeComponent();
        }
        #endregion
        #region Window Methods
        /// <summary>
        /// Event fires on click of exit button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
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
        #region Custom Popup
        /// <summary>
        /// Method used to create custom popup
        /// </summary>
        /// <param name="image">used to get popup image from ePopupImage enum</param>
        /// <param name="title">used to get popup title as an string</param>
        /// <param name="text">used to get popup message as an string</param>
        /// <param name="btn">used to get popup button result</param>
        /// <returns>ePopupResult</returns>

        public ePopupResult DisplayPopupData(ePopupImage image, ePopupTitle title, string text, ePopupButton btn)
        {
            if (image == ePopupImage.Error)
            {
                PopUpimage.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/PopupError.png") as ImageSource;
            }
            else if (image == ePopupImage.Warning)
            {
                PopUpimage.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/PopupWarning.png") as ImageSource;
            }
            else
                PopUpimage.Source = new ImageSourceConverter().ConvertFromString(@"pack://application:,,,/../Images/PopupInfo.png") as ImageSource;

            if (btn == ePopupButton.OK)
            {
                PopupBtnOk.Visibility = Visibility.Visible;
                PopupBtnFirst.Visibility = Visibility.Hidden;
                PopupBtnSecond.Visibility = Visibility.Hidden;
            }
            else if (btn == ePopupButton.OkCancel)
            {
                PopupBtnOk.Visibility = Visibility.Hidden;
                PopupBtnFirst.Visibility = Visibility.Visible;
                PopupBtnSecond.Visibility = Visibility.Visible;
                PopupBtnFirst.Content = "OK";
                PopupBtnSecond.Content = "Cancel";
            }
            else
            {
                PopupBtnOk.Visibility = Visibility.Hidden;
                PopupBtnFirst.Visibility = Visibility.Visible;
                PopupBtnSecond.Visibility = Visibility.Visible;
                PopupBtnFirst.Content = "Yes";
                PopupBtnSecond.Content = "No";
            }
            PopupTitle.Content = title.ToString();
            PopupText.Text = text;
            this.ShowDialog();

            return result;
        }

        /// <summary>
        /// Event fires on click of PopupBtnFirst button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void PopupBtnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (PopupBtnFirst.Content.ToString() == "OK")
                result = ePopupResult.OK;
            else
                result = ePopupResult.Yes;
            this.Close();
        }

        /// <summary>
        /// Event fires on click of PopupBtnSecond button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void PopupBtnSecond_Click(object sender, RoutedEventArgs e)
        {
            if (PopupBtnFirst.Content.ToString() == "Cancel")
                result = ePopupResult.Cancel;
            else
                result = ePopupResult.No;
            this.Close();
        }

        /// <summary>
        /// Event fires on click of PopupBtnOk button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void PopupBtnOk_Click(object sender, RoutedEventArgs e)
        {
            result = ePopupResult.OK;
            this.Close();
        }
        #endregion
    }
}
