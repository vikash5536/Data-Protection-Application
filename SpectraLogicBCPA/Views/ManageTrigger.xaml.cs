//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.TaskSchedulingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MTS = Microsoft.Win32.TaskScheduler;
using DataProtectionApplication.TaskSchedulingApp.ViewModel;
using DataProtectionApplication.TaskSchedulingApp.Model;

namespace DataProtectionApplication.TaskSchedulingApp.Views
{
    /// <summary>
    /// Interaction logic for ManageTrigger.xaml
    /// </summary>
    public partial class ManageTrigger : Window
    {
        #region Properties and Variables        
        public static Logger logger = new Logger(typeof(ManageTrigger));
        private static ManageTrigger _manageTrigger;
        private static bool _isCreate;
        public static CustomTrigger _customtrigger { get; set; }
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor of ManageTrigger class
        /// </summary>
        /// 
        public ManageTrigger()
        {
            InitializeComponent();
            if (_isCreate)
                lbTitle.Content = "Create Trigger";
            else
                lbTitle.Content = "Edit Trigger";
            CanOperate();               // Checking validation at first time on loading.
        }
        #endregion
        #region Window Methods

        /// <summary>
        /// Event used to close application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _customtrigger = null;
            this.Close();
        }

        /// <summary>
        /// Event fired on window load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isCreate)
            {
                FillDataFromTrigger(_customtrigger);
            }

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
        #region Save and Cancel Trigger

        /// <summary>
        /// Event fired on click of save button to save trigger details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnSaveTrigger_Click(object sender, RoutedEventArgs e)
        {
            _customtrigger = new CustomTrigger();
            if (rbtOneTime.IsChecked.HasValue && rbtOneTime.IsChecked.Value)
            {
                _customtrigger._Trigger = new MTS.TimeTrigger
                {
                    StartBoundary = GetStratBoundary()                    
                };
            }
            else if (rbtDaily.IsChecked.HasValue && rbtDaily.IsChecked.Value)
            {
                _customtrigger._Trigger = new MTS.DailyTrigger
                {
                    StartBoundary = GetStratBoundary(),
                    DaysInterval = Convert.ToInt16(txtRecurDay.Text)
                };
            }
            else if (rbtWeekly.IsChecked.HasValue && rbtWeekly.IsChecked.Value)
            {
                _customtrigger._Trigger = new MTS.WeeklyTrigger
                {
                    StartBoundary = GetStratBoundary(),
                    WeeksInterval = Convert.ToInt16(txtRecurWeek.Text),
                    DaysOfWeek = GetDaysOfWeek()
                };
            }
            else
            {
                if (rbDays.IsChecked == true)
                {
                    _customtrigger._Trigger = new MTS.MonthlyTrigger
                    {
                        DaysOfMonth = GetDaysOfMonth(),
                        StartBoundary = GetStratBoundary(),
                        MonthsOfYear = GetMonthOfYear(),
                        RunOnLastDayOfMonth = GetStatusForRunOnLastDayOfMonth()
                    };
                }
                else
                {
                    _customtrigger._Trigger = new MTS.MonthlyDOWTrigger
                    {
                        StartBoundary = GetStratBoundary(),
                        WeeksOfMonth = GetWhichWeek(),
                        DaysOfWeek = GetDaysOfWeek(),
                        MonthsOfYear = GetMonthOfYear(),
                        RunOnLastWeekOfMonth = GetStatusForRunOnLastWeekOfMonth()
                    };
                }
            }
            _customtrigger._Trigger.Enabled = chkEnable.IsChecked.Value;            // Trigger Enable Value Bound
            _customtrigger._Enabled = chkEnable.IsChecked.Value;                    // Trigger Enable Display Value Bound   
            _customtrigger._Trigger.Id = Guid.NewGuid().ToString("N").ToUpper();
            _customtrigger._Details = _customtrigger._Trigger.ToString();
            this.Close();
        }

        /// <summary>
        /// Event fired on click of cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnCancelTrigger_Click(object sender, RoutedEventArgs e)
        {
            _customtrigger = null;
            this.Close();
        }
        #endregion
        #region Manage Triggers
        #region Tiggers Configuration

        /// <summary>
        /// Method used to get DaysOfMonth
        /// </summary>
        /// <returns>int aaray of days of month</returns>

        private int[] GetDaysOfMonth()
        {
            List<int> days = new List<int>();
            foreach (var item in ListDaysInMonth.SelectedItems)
            {
                if (item.Key != "Last Day")
                    days.Add(Convert.ToInt32(item.Value));
            }
            return days.ToArray();
        }

        /// <summary>
        /// Method used to get DaysOfWeek
        /// </summary>
        /// <returns>it return DaysOfTheWeek</returns>

        private MTS.DaysOfTheWeek GetDaysOfWeek()
        {
            MTS.DaysOfTheWeek daysOfTheWeek = 0;
            if (rbtMonthly.IsChecked == true)
            {
                foreach (var item in ListDaysOfTheWeek.SelectedItems)
                {
                    if ((MTS.DaysOfTheWeek)item.Value == MTS.DaysOfTheWeek.Monday) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Monday;
                    if ((MTS.DaysOfTheWeek)item.Value == MTS.DaysOfTheWeek.Tuesday) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Tuesday;
                    if ((MTS.DaysOfTheWeek)item.Value == MTS.DaysOfTheWeek.Wednesday) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Wednesday;
                    if ((MTS.DaysOfTheWeek)item.Value == MTS.DaysOfTheWeek.Thursday) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Thursday;
                    if ((MTS.DaysOfTheWeek)item.Value == MTS.DaysOfTheWeek.Friday) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Friday;
                    if ((MTS.DaysOfTheWeek)item.Value == MTS.DaysOfTheWeek.Saturday) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Saturday;
                    if ((MTS.DaysOfTheWeek)item.Value == MTS.DaysOfTheWeek.Sunday) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Sunday;
                }
            }
            if (rbtWeekly.IsChecked == true)
            {
                if (cbSunWeekly.IsChecked == true) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Sunday;
                if (cbMonWeekly.IsChecked == true) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Monday;
                if (cbTueWeekly.IsChecked == true) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Tuesday;
                if (cbWedWeekly.IsChecked == true) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Wednesday;
                if (cbThuWeekly.IsChecked == true) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Thursday;
                if (cbFriWeekly.IsChecked == true) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Friday;
                if (cbSatWeekly.IsChecked == true) daysOfTheWeek = daysOfTheWeek | MTS.DaysOfTheWeek.Saturday;
            }
            return daysOfTheWeek;
        }

        /// <summary>
        /// Method used to get MonthOfYear
        /// </summary>
        /// <returns>it return MonthsOfTheYear</returns>

        private MTS.MonthsOfTheYear GetMonthOfYear()
        {
            MTS.MonthsOfTheYear monthOfTheYear = 0;
            foreach (var item in ListMonthOfYear.SelectedItems)
            {
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.January) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.January;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.February) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.February;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.March) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.March;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.April) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.April;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.May) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.May;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.June) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.June;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.July) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.July;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.August) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.August;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.September) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.September;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.October) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.October;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.November) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.November;
                if ((MTS.MonthsOfTheYear)item.Value == MTS.MonthsOfTheYear.December) monthOfTheYear = monthOfTheYear | MTS.MonthsOfTheYear.December;
            }
            return monthOfTheYear;
        }

        /// <summary>
        /// Method used to get status for RunOnLastDayOfMonth
        /// </summary>
        /// <returns>true or false</returns>

        private bool GetStatusForRunOnLastDayOfMonth()
        {
            bool lastDayOfMonth = false;
            if (ListDaysInMonth.SelectedItems.ContainsValue("32")) // Last Day of the month
                lastDayOfMonth = true;
            return lastDayOfMonth;
        }

        /// <summary>
        /// Method used to get status for RunOnLastWeekOfMonth
        /// </summary>
        /// <returns>true or false</returns>

        private bool GetStatusForRunOnLastWeekOfMonth()
        {
            bool lastweek = false;
            if (ListWhichWeek.SelectedItems.Values.Contains(MTS.WhichWeek.LastWeek)) lastweek = true;
            return lastweek;
        }

        /// <summary>
        /// Method used to get WhichWeek
        /// </summary>
        /// <returns>it return WhichWeek</returns>

        private MTS.WhichWeek GetWhichWeek()
        {
            MTS.WhichWeek whichweek = 0;
            foreach (var item in ListWhichWeek.SelectedItems)
            {
                if ((MTS.WhichWeek)item.Value == MTS.WhichWeek.FirstWeek) whichweek = whichweek | MTS.WhichWeek.FirstWeek;
                if ((MTS.WhichWeek)item.Value == MTS.WhichWeek.SecondWeek) whichweek = whichweek | MTS.WhichWeek.SecondWeek;
                if ((MTS.WhichWeek)item.Value == MTS.WhichWeek.ThirdWeek) whichweek = whichweek | MTS.WhichWeek.ThirdWeek;
                if ((MTS.WhichWeek)item.Value == MTS.WhichWeek.FirstWeek) whichweek = whichweek | MTS.WhichWeek.FourthWeek;
                if ((MTS.WhichWeek)item.Value == MTS.WhichWeek.LastWeek) whichweek = whichweek | MTS.WhichWeek.LastWeek;
            }
            return whichweek;
        }

        /// <summary>
        /// Method used to get StratBoundary
        /// </summary>
        /// <returns>it return datetime of StratBoundary</returns>

        private DateTime GetStratBoundary()
        {
            return new DateTime(
               dpStartDate.SelectedDate.Value.Year,
               dpStartDate.SelectedDate.Value.Month,
               dpStartDate.SelectedDate.Value.Day,
               tmStartTime.Value.Value.Hour,
               tmStartTime.Value.Value.Minute,
               tmStartTime.Value.Value.Second);
        }

        /// <summary>
        /// Method used to fill data from trigger
        /// </summary>
        /// <param name="_customerTrigger">it is used as an object of class CustomTrigger</param>

        private void FillDataFromTrigger(CustomTrigger _customerTrigger)
        {
            try
            {
                var trigger = _customerTrigger._Trigger;
                var dateTime = new DateTime(
                    trigger.StartBoundary.Year, trigger.StartBoundary.Month, trigger.StartBoundary.Day,
                    trigger.StartBoundary.Hour, trigger.StartBoundary.Minute, trigger.StartBoundary.Second);
                dpStartDate.SelectedDate = dateTime;
                tmStartTime.Value = dateTime;
                chkEnable.IsChecked = _customerTrigger._Enabled;
                if (trigger.TriggerType == MTS.TaskTriggerType.Time)
                {
                    rbtOneTime.IsChecked = true;
                }
                if (trigger.TriggerType == MTS.TaskTriggerType.Daily)
                {
                    GridDailyTrigger.Visibility = Visibility.Visible;
                    var dailyTrigger = trigger as MTS.DailyTrigger;                    
                    txtRecurDay.Text = dailyTrigger.DaysInterval.ToString();
                    rbtDaily.IsChecked = true;
                }
                else if (trigger.TriggerType == MTS.TaskTriggerType.Weekly)
                {
                    GridWeeklyTrigger.Visibility = Visibility.Visible;
                    var weeklyTrigger = trigger as MTS.WeeklyTrigger;                    
                    txtRecurWeek.Text = weeklyTrigger.WeeksInterval.ToString();
                    var days = weeklyTrigger.DaysOfWeek;
                    if ((days & MTS.DaysOfTheWeek.Sunday) == MTS.DaysOfTheWeek.Sunday) cbSunWeekly.IsChecked = true;
                    if ((days & MTS.DaysOfTheWeek.Monday) == MTS.DaysOfTheWeek.Monday) cbMonWeekly.IsChecked = true;
                    if ((days & MTS.DaysOfTheWeek.Tuesday) == MTS.DaysOfTheWeek.Tuesday) cbTueWeekly.IsChecked = true;
                    if ((days & MTS.DaysOfTheWeek.Wednesday) == MTS.DaysOfTheWeek.Wednesday) cbWedWeekly.IsChecked = true;
                    if ((days & MTS.DaysOfTheWeek.Thursday) == MTS.DaysOfTheWeek.Thursday) cbThuWeekly.IsChecked = true;
                    if ((days & MTS.DaysOfTheWeek.Friday) == MTS.DaysOfTheWeek.Friday) cbFriWeekly.IsChecked = true;
                    if ((days & MTS.DaysOfTheWeek.Saturday) == MTS.DaysOfTheWeek.Saturday) cbSatWeekly.IsChecked = true;
                    rbtWeekly.IsChecked = true;
                }
                else if (trigger.TriggerType == MTS.TaskTriggerType.Monthly || trigger.TriggerType == MTS.TaskTriggerType.MonthlyDOW)
                {
                    
                    GridMonthlyTrigger.Visibility = Visibility.Visible;
                    if (trigger is MTS.MonthlyTrigger)
                    {                        
                        var monthlyTrigger = trigger as MTS.MonthlyTrigger;
                        FillMonthOfYears(monthlyTrigger.MonthsOfYear);
                        rbtMonthly.IsChecked = true;
                        FillDaysInMonth(monthlyTrigger);
                        if (monthlyTrigger.RunOnLastDayOfMonth)
                            ListDaysInMonth.SelectedItems.Add("Last Day", 32);
                        rbDays.IsChecked = true;
                    }
                    else
                    {                        
                        var monthlyDOWTrigger = trigger as MTS.MonthlyDOWTrigger;
                        FillMonthOfYears(monthlyDOWTrigger.MonthsOfYear);
                        FillDaysOfWeek(monthlyDOWTrigger.DaysOfWeek);
                        FillWeeksOfMonth(monthlyDOWTrigger.WeeksOfMonth);
                        rbOn.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogInfo(string.Format("Exception in FillDataFromTrigger, Message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// Method used to fill WeeksOfMonth
        /// </summary>
        /// <param name="weeksOfMonth">it is used as an object of enum WhichWeek</param>

        private void FillWeeksOfMonth(MTS.WhichWeek weeksOfMonth)
        {
            WhichWeekViewModel whichweekViewModel = new WhichWeekViewModel();
            whichweekViewModel.SelectedWhichWeek = new Dictionary<string, object>();
            if ((weeksOfMonth & MTS.WhichWeek.FirstWeek) == MTS.WhichWeek.FirstWeek) whichweekViewModel.SelectedWhichWeek.Add(MTS.WhichWeek.FirstWeek.ToString(), MTS.WhichWeek.FirstWeek);
            if ((weeksOfMonth & MTS.WhichWeek.SecondWeek) == MTS.WhichWeek.SecondWeek) whichweekViewModel.SelectedWhichWeek.Add(MTS.WhichWeek.SecondWeek.ToString(), MTS.WhichWeek.SecondWeek);
            if ((weeksOfMonth & MTS.WhichWeek.ThirdWeek) == MTS.WhichWeek.ThirdWeek) whichweekViewModel.SelectedWhichWeek.Add(MTS.WhichWeek.ThirdWeek.ToString(), MTS.WhichWeek.ThirdWeek);
            if ((weeksOfMonth & MTS.WhichWeek.FourthWeek) == MTS.WhichWeek.FourthWeek) whichweekViewModel.SelectedWhichWeek.Add(MTS.WhichWeek.FourthWeek.ToString(), MTS.WhichWeek.FourthWeek);
            if ((weeksOfMonth & MTS.WhichWeek.LastWeek) == MTS.WhichWeek.LastWeek) whichweekViewModel.SelectedWhichWeek.Add(MTS.WhichWeek.LastWeek.ToString(), MTS.WhichWeek.LastWeek);
            ListWhichWeek.SelectedItems = whichweekViewModel.SelectedWhichWeek;
        }

        /// <summary>
        /// Method used to fill daysOfWeek
        /// </summary>
        /// <param name="daysOfWeek">it is used as an object of enum daysOfWeek</param>

        private void FillDaysOfWeek(MTS.DaysOfTheWeek daysOfWeek)
        {
            DaysOfTheWeekViewModel daysofweekViewModel = new DaysOfTheWeekViewModel();
            daysofweekViewModel.SelectedDaysOfTheWeek = new Dictionary<string, object>();
            if ((daysOfWeek & MTS.DaysOfTheWeek.Sunday) == MTS.DaysOfTheWeek.Sunday) daysofweekViewModel.SelectedDaysOfTheWeek.Add(MTS.DaysOfTheWeek.Sunday.ToString(), MTS.DaysOfTheWeek.Sunday);
            if ((daysOfWeek & MTS.DaysOfTheWeek.Monday) == MTS.DaysOfTheWeek.Monday) daysofweekViewModel.SelectedDaysOfTheWeek.Add(MTS.DaysOfTheWeek.Monday.ToString(), MTS.DaysOfTheWeek.Monday);
            if ((daysOfWeek & MTS.DaysOfTheWeek.Tuesday) == MTS.DaysOfTheWeek.Tuesday) daysofweekViewModel.SelectedDaysOfTheWeek.Add(MTS.DaysOfTheWeek.Tuesday.ToString(), MTS.DaysOfTheWeek.Tuesday);
            if ((daysOfWeek & MTS.DaysOfTheWeek.Wednesday) == MTS.DaysOfTheWeek.Wednesday) daysofweekViewModel.SelectedDaysOfTheWeek.Add(MTS.DaysOfTheWeek.Wednesday.ToString(), MTS.DaysOfTheWeek.Wednesday);
            if ((daysOfWeek & MTS.DaysOfTheWeek.Thursday) == MTS.DaysOfTheWeek.Thursday) daysofweekViewModel.SelectedDaysOfTheWeek.Add(MTS.DaysOfTheWeek.Thursday.ToString(), MTS.DaysOfTheWeek.Thursday);
            if ((daysOfWeek & MTS.DaysOfTheWeek.Friday) == MTS.DaysOfTheWeek.Friday) daysofweekViewModel.SelectedDaysOfTheWeek.Add(MTS.DaysOfTheWeek.Friday.ToString(), MTS.DaysOfTheWeek.Friday);
            if ((daysOfWeek & MTS.DaysOfTheWeek.Saturday) == MTS.DaysOfTheWeek.Saturday) daysofweekViewModel.SelectedDaysOfTheWeek.Add(MTS.DaysOfTheWeek.Saturday.ToString(), MTS.DaysOfTheWeek.Saturday);
            ListDaysOfTheWeek.SelectedItems = daysofweekViewModel.SelectedDaysOfTheWeek;
        }

        /// <summary>
        /// Method used to fill MonthsOfTheYear
        /// </summary>
        /// <param name="months">it is used as an object of enum MonthsOfTheYear</param>

        private void FillMonthOfYears(MTS.MonthsOfTheYear months)
        {
            MonthsViewModel monthViewModel = new MonthsViewModel();
            monthViewModel.SelectedMonthsOfYear = new Dictionary<string, object>();
            if ((months & MTS.MonthsOfTheYear.January) == MTS.MonthsOfTheYear.January) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.January.ToString(), MTS.MonthsOfTheYear.January);
            if ((months & MTS.MonthsOfTheYear.February) == MTS.MonthsOfTheYear.February) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.February.ToString(), MTS.MonthsOfTheYear.February);
            if ((months & MTS.MonthsOfTheYear.March) == MTS.MonthsOfTheYear.March) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.March.ToString(), MTS.MonthsOfTheYear.March);
            if ((months & MTS.MonthsOfTheYear.April) == MTS.MonthsOfTheYear.April) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.April.ToString(), MTS.MonthsOfTheYear.April);
            if ((months & MTS.MonthsOfTheYear.May) == MTS.MonthsOfTheYear.May) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.May.ToString(), MTS.MonthsOfTheYear.May);
            if ((months & MTS.MonthsOfTheYear.June) == MTS.MonthsOfTheYear.June) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.June.ToString(), MTS.MonthsOfTheYear.June);
            if ((months & MTS.MonthsOfTheYear.July) == MTS.MonthsOfTheYear.July) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.July.ToString(), MTS.MonthsOfTheYear.July);
            if ((months & MTS.MonthsOfTheYear.August) == MTS.MonthsOfTheYear.August) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.August.ToString(), MTS.MonthsOfTheYear.August);
            if ((months & MTS.MonthsOfTheYear.September) == MTS.MonthsOfTheYear.September) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.September.ToString(), MTS.MonthsOfTheYear.September);
            if ((months & MTS.MonthsOfTheYear.October) == MTS.MonthsOfTheYear.October) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.October.ToString(), MTS.MonthsOfTheYear.October);
            if ((months & MTS.MonthsOfTheYear.November) == MTS.MonthsOfTheYear.November) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.November.ToString(), MTS.MonthsOfTheYear.November);
            if ((months & MTS.MonthsOfTheYear.December) == MTS.MonthsOfTheYear.December) monthViewModel.SelectedMonthsOfYear.Add(MTS.MonthsOfTheYear.December.ToString(), MTS.MonthsOfTheYear.December);
            ListMonthOfYear.SelectedItems = monthViewModel.SelectedMonthsOfYear;
        }

        /// <summary>
        /// Method used to fill DaysInMonth
        /// </summary>
        /// <param name="monthlytrigger">it is used as an object of class MonthlyTrigger</param>

        private void FillDaysInMonth(MTS.MonthlyTrigger monthlytrigger)
        {
            DaysOfMonthViewModel daysViewModel = new DaysOfMonthViewModel();
            daysViewModel.SelectedDaysOfMonth = new Dictionary<string, object>();
            for (int i = 0; i < monthlytrigger.DaysOfMonth.Length; i++)
            {
                daysViewModel.SelectedDaysOfMonth.Add(monthlytrigger.DaysOfMonth[i].ToString(), monthlytrigger.DaysOfMonth[i]);
            }
            if (monthlytrigger.RunOnLastDayOfMonth)
                daysViewModel.SelectedDaysOfMonth.Add("Last Day", 32);
            ListDaysInMonth.SelectedItems = daysViewModel.SelectedDaysOfMonth;
        }

        #endregion

        /// <summary>
        /// Event fired on CbWeek checkbox checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void CbWeek_Checked(object sender, RoutedEventArgs e)
        {

            if (rbtWeekly.IsChecked.Value && (cbSunWeekly.IsChecked.Value || cbMonWeekly.IsChecked.Value || cbTueWeekly.IsChecked.Value || cbWedWeekly.IsChecked.Value || cbThuWeekly.IsChecked.Value || cbFriWeekly.IsChecked.Value || cbSatWeekly.IsChecked.Value))
            {
                btnSaveTrigger.IsEnabled = true;
            }
            else
                btnSaveTrigger.IsEnabled = false;
        }

        /// <summary>
        /// Event fired on CbWeek checkbox Unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void CbWeek_Unchecked(object sender, RoutedEventArgs e)
        {
            if (rbtWeekly.IsChecked.Value && (cbSunWeekly.IsChecked.Value || cbMonWeekly.IsChecked.Value || cbTueWeekly.IsChecked.Value || cbWedWeekly.IsChecked.Value || cbThuWeekly.IsChecked.Value || cbFriWeekly.IsChecked.Value || cbSatWeekly.IsChecked.Value))
            {
                btnSaveTrigger.IsEnabled = true;
            }
            else
                btnSaveTrigger.IsEnabled = false;
        }

        /// <summary>
        /// Event fired when rbDays checkbox is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbDays_Checked(object sender, RoutedEventArgs e)
        {

            ListWhichWeek.IsEnabled = false;
            ListDaysOfTheWeek.IsEnabled = false;
            ListDaysInMonth.IsEnabled = true;
            CanOperate();
        }

        /// <summary>
        /// Event fired when rbOn checkbox is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbOn_Checked(object sender, RoutedEventArgs e)
        {
            ListDaysInMonth.IsEnabled = false;
            ListWhichWeek.IsEnabled = true;
            ListDaysOfTheWeek.IsEnabled = true;
            CanOperate();
        }

        /// <summary>
        /// Event fired on validation selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ValidateSelection(object sender, RoutedEventArgs e)
        {
            CanOperate();
        }

        /// <summary>
        /// Method used to enable/disable save button based on validation
        /// </summary>

        private void CanOperate()
        {
            btnSaveTrigger.IsEnabled = IsValidTrigger();
        }

        /// <summary>
        /// Method used to validate trigger
        /// </summary>
        /// <returns>true or false</returns>

        private bool IsValidTrigger()
        {
            bool status = false;
            if (rbtOneTime.IsChecked.Value)
            {
                return true;
            }
            else if (rbtDaily.IsChecked.Value)
            {
                if (txtRecurDay.Text != "")
                {
                    txtRecurDay.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtRecurDay.ToolTip = txtRecurDay.Text;
                    status = true;
                }
                else
                {
                    txtRecurDay.BorderBrush = new SolidColorBrush(Colors.Red);
                    txtRecurDay.ToolTip = "Recur day can't be empty.";
                    status = false;
                }
            }
            else if (rbtWeekly.IsChecked.Value && (cbSunWeekly.IsChecked.Value || cbMonWeekly.IsChecked.Value || cbTueWeekly.IsChecked.Value || cbWedWeekly.IsChecked.Value || cbThuWeekly.IsChecked.Value || cbFriWeekly.IsChecked.Value || cbSatWeekly.IsChecked.Value))
            {
                if (txtRecurWeek.Text != "")
                {
                    txtRecurWeek.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtRecurWeek.ToolTip = txtRecurWeek.Text;
                    status = true;
                }
                else
                {
                    txtRecurWeek.BorderBrush = new SolidColorBrush(Colors.Black);
                    txtRecurWeek.ToolTip = "Recur week can't be empty.";
                    status = false;
                }
            }
            else if (rbtMonthly.IsChecked.Value && ListMonthOfYear.SelectedItems != null && ListMonthOfYear.SelectedItems.Count > 0)
            {
                if (rbDays.IsChecked.Value && ListDaysInMonth.SelectedItems != null && ListDaysInMonth.SelectedItems.Count > 0)
                {
                    status = true;
                }
                else if (rbOn.IsChecked.Value && ListWhichWeek.SelectedItems != null && ListWhichWeek.SelectedItems.Count > 0 && ListDaysOfTheWeek.SelectedItems != null && ListDaysOfTheWeek.SelectedItems.Count > 0)
                    status = true;
            }
            return status;
        }

        /// <summary>
        /// Method used to show create triggers
        /// </summary>

        public static void ShowCreate()
        {
            _isCreate = true;
            Show();
        }

        /// <summary>
        /// Method used to show edit trigger
        /// </summary>
        /// <param name="editTrigger">it is used as an object of class CustomTrigger</param>

        public static void ShowEdit(CustomTrigger editTrigger)
        {
            _isCreate = false;
            _customtrigger = editTrigger;
            Show();
        }

        /// <summary>
        /// Method used to show manage trigger window
        /// </summary>

        private new static void Show()
        {
            _manageTrigger = new ManageTrigger();
            _manageTrigger.ShowDialog();
        }

        /// <summary>
        /// Event fired on rbtOneTime radiobutton checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbtOneTime_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                GridDailyTrigger.Visibility = Visibility.Hidden;
                GridWeeklyTrigger.Visibility = Visibility.Hidden;
                GridMonthlyTrigger.Visibility = Visibility.Hidden;
                CanOperate();
            }
        }

        /// <summary>
        /// Event fired on rbtDaily radiobutton checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbtDaily_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                GridDailyTrigger.Visibility = Visibility.Visible;
                GridWeeklyTrigger.Visibility = Visibility.Hidden;
                GridMonthlyTrigger.Visibility = Visibility.Hidden;
                CanOperate();
            }
        }

        /// <summary>
        /// Event fired on rbtWeekly radiobutton checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbtWeekly_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                GridDailyTrigger.Visibility = Visibility.Hidden;
                GridWeeklyTrigger.Visibility = Visibility.Visible;
                GridMonthlyTrigger.Visibility = Visibility.Hidden;
                CanOperate();
            }
        }

        /// <summary>
        /// Event fired on rbtMonthly radiobutton checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbtMonthly_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                GridDailyTrigger.Visibility = Visibility.Hidden;
                GridWeeklyTrigger.Visibility = Visibility.Hidden;
                GridMonthlyTrigger.Visibility = Visibility.Visible;
                CanOperate();
            }
        }

        /// <summary>
        ///  Event used to validate key down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ValidateKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((new Util().IsNumberKey(e.Key) || e.Key == Key.Tab) && e.Key != Key.D0)             // Ignore 0
                e.Handled = false;
            else
                e.Handled = true;
        }

        /// <summary>
        /// Event fired on text change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ValidateTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded) // Ignore first time when controls get initialized
                CanOperate();
        }

        /// <summary>
        /// Method used to check whether weekday is selected or not 
        /// </summary>
        /// <returns>true or false</returns>

        private bool IsWeekdaySelected()
        {
            return false;
        }
        #endregion        
    }
    #region Converters
    public class YesBooleanConverter : IValueConverter
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
            return value.ToString();

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
