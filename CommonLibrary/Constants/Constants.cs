//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace DataProtectionApplication.CommonLibrary.Constants
{
    /// <summary>
    /// BlackPearlConfig.xaml Save button content.
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// Count for action parameters of filetransfer
        /// </summary>
        public const int ActionParameterCount = 8;
        /// <summary>
        /// Host name validation regex
        /// </summary>
        public static readonly Regex validHostnameRegex = new Regex(@"^(([a-z]|[a-z][a-z0-9-]*[a-z0-9]).)*([a-z]|[a-z][a-z0-9-]*[a-z0-9])$", RegexOptions.IgnoreCase);
        /// <summary>
        /// Email Validation regex
        /// </summary>
        public static readonly Regex validEmailRegex = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        /// <summary>
        /// Destination server xml file path
        /// </summary>
        public static readonly string DestinationServerDetails = AppDomain.CurrentDomain.BaseDirectory + "DestinationBlackPearlConfiguration.xml";
        /// <summary>
        /// Email configuration xml file path
        /// </summary>
        public static readonly string EmailConfigurationDetails = AppDomain.CurrentDomain.BaseDirectory + "EmailConfiguration.xml";
        /// <summary>
        /// Task scheduler folder path.
        /// </summary>
        public static readonly string taskFolderPath = "DataProtectionApp";
    }
}
