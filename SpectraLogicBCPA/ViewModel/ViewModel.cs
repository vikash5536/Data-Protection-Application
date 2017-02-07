//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using MTS = Microsoft.Win32.TaskScheduler;
using System.Collections.Generic;

namespace DataProtectionApplication.TaskSchedulingApp.ViewModel
{
    /// <summary>
    /// Class MonthsViewModel used for MonthsOfYear
    /// </summary>

    public class MonthsViewModel : ViewModelBase
    {
        private Dictionary<string, object> _monthsofyear;
        private Dictionary<string, object> _selectedmonthsofyear;
        public Dictionary<string, object> MonthsOfYear
        {
            get
            {
                return _monthsofyear;
            }
            set
            {
                _monthsofyear = value;
                NotifyPropertyChanged("MonthsOfYear");
            }
        }
        public Dictionary<string, object> SelectedMonthsOfYear
        {
            get
            {
                return _selectedmonthsofyear;
            }
            set
            {
                _selectedmonthsofyear = value;
                NotifyPropertyChanged("SelectedMonthsOfYear");
            }
        }
        /// <summary>
        /// Ctor for MonthsViewModel class.
        /// </summary>
        public MonthsViewModel()
        {
            MonthsOfYear = new Dictionary<string, object>();
            MonthsOfYear.Add(MTS.MonthsOfTheYear.January.ToString(), MTS.MonthsOfTheYear.January);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.February.ToString(), MTS.MonthsOfTheYear.February);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.March.ToString(), MTS.MonthsOfTheYear.March);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.April.ToString(), MTS.MonthsOfTheYear.April);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.May.ToString(), MTS.MonthsOfTheYear.May);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.June.ToString(), MTS.MonthsOfTheYear.June);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.July.ToString(), MTS.MonthsOfTheYear.July);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.August.ToString(), MTS.MonthsOfTheYear.August);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.September.ToString(), MTS.MonthsOfTheYear.September);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.October.ToString(), MTS.MonthsOfTheYear.October);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.November.ToString(), MTS.MonthsOfTheYear.November);
            MonthsOfYear.Add(MTS.MonthsOfTheYear.December.ToString(), MTS.MonthsOfTheYear.December);
        }
    }

    /// <summary>
    /// Class WhichWeekViewModel used for _whichWeek
    /// </summary>

    public class WhichWeekViewModel : ViewModelBase
    {
        private Dictionary<string, object> _whichWeek;
        private Dictionary<string, object> _selectedwhichweek;
        public Dictionary<string, object> WhichWeek
        {
            get
            {
                return _whichWeek;
            }
            set
            {
                _whichWeek = value;
                NotifyPropertyChanged("WhichWeek");
            }
        }
        public Dictionary<string, object> SelectedWhichWeek
        {
            get
            {
                return _selectedwhichweek;
            }
            set
            {
                _selectedwhichweek = value;
                NotifyPropertyChanged("SelectedWhichWeek");
            }
        }
        /// <summary>
        /// Ctor for WhichWeekViewModel class.
        /// </summary>
        public WhichWeekViewModel()
        {
            WhichWeek = new Dictionary<string, object>();
            WhichWeek.Add(MTS.WhichWeek.FirstWeek.ToString(), MTS.WhichWeek.FirstWeek);
            WhichWeek.Add(MTS.WhichWeek.SecondWeek.ToString(), MTS.WhichWeek.SecondWeek);
            WhichWeek.Add(MTS.WhichWeek.ThirdWeek.ToString(), MTS.WhichWeek.ThirdWeek);
            WhichWeek.Add(MTS.WhichWeek.FourthWeek.ToString(), MTS.WhichWeek.FourthWeek);
            WhichWeek.Add(MTS.WhichWeek.LastWeek.ToString(), MTS.WhichWeek.LastWeek);

        }
    }

    /// <summary>
    /// Class DaysOfTheWeekViewModel used for DaysOfTheWeek
    /// </summary>

    public class DaysOfTheWeekViewModel : ViewModelBase
    {
        private Dictionary<string, object> _daysoftheweek;
        private Dictionary<string, object> _selecteddaysoftheweek;
        public Dictionary<string, object> DaysOfTheWeek
        {
            get
            {
                return _daysoftheweek;
            }
            set
            {
                _daysoftheweek = value;
                NotifyPropertyChanged("DaysOfTheWeek");
            }
        }
        public Dictionary<string, object> SelectedDaysOfTheWeek
        {
            get
            {
                return _selecteddaysoftheweek;
            }
            set
            {
                _selecteddaysoftheweek = value;
                NotifyPropertyChanged("DaysOfTheWeek");
            }
        }
        /// <summary>
        /// ctor for DaysOfTheWeekViewModel class
        /// </summary>
        public DaysOfTheWeekViewModel()
        {
            DaysOfTheWeek = new Dictionary<string, object>();
            DaysOfTheWeek.Add(MTS.DaysOfTheWeek.Monday.ToString(), MTS.DaysOfTheWeek.Monday);
            DaysOfTheWeek.Add(MTS.DaysOfTheWeek.Tuesday.ToString(), MTS.DaysOfTheWeek.Tuesday);
            DaysOfTheWeek.Add(MTS.DaysOfTheWeek.Wednesday.ToString(), MTS.DaysOfTheWeek.Wednesday);
            DaysOfTheWeek.Add(MTS.DaysOfTheWeek.Thursday.ToString(), MTS.DaysOfTheWeek.Thursday);
            DaysOfTheWeek.Add(MTS.DaysOfTheWeek.Friday.ToString(), MTS.DaysOfTheWeek.Friday);
            DaysOfTheWeek.Add(MTS.DaysOfTheWeek.Saturday.ToString(), MTS.DaysOfTheWeek.Saturday);
            DaysOfTheWeek.Add(MTS.DaysOfTheWeek.Sunday.ToString(), MTS.DaysOfTheWeek.Sunday);
        }
    }

    /// <summary>
    /// Class DaysOfMonthViewModel used for DaysOfMonth
    /// </summary>

    public class DaysOfMonthViewModel : ViewModelBase
    {
        private Dictionary<string, object> _daysofmonth;
        private Dictionary<string, object> _selecteddaysofmonth;
        public Dictionary<string, object> DaysOfMonth
        {
            get
            {
                return _daysofmonth;
            }
            set
            {
                _daysofmonth = value;
                NotifyPropertyChanged("DaysOfMonth");
            }
        }
        public Dictionary<string, object> SelectedDaysOfMonth
        {
            get
            {
                return _selecteddaysofmonth;
            }
            set
            {
                _selecteddaysofmonth = value;
                NotifyPropertyChanged("SelectedDaysOfMonth");
            }
        }
        /// <summary>
        /// ctor for DaysOfMonthViewModel class
        /// </summary>
        public DaysOfMonthViewModel()
        {
            DaysOfMonth = new Dictionary<string, object>();
            for (int i = 1; i <= 31; i++)
            {
                DaysOfMonth.Add(i.ToString(), i);
            }
            DaysOfMonth.Add("Last Day", "32");
        }
    }
}
