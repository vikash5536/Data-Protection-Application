//*******************************************************//
//                                                       //
// CSharp.Net Data Potection Application common Library  //
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
namespace DataProtectionApplication.CommonLibrary.Model
{
    /// <summary>
    /// Class for Email configuration 
    /// </summary>
    public class EmailConfiguration
    {
        public string SmtpAddress { get; set; }
        public int PortNumber { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Password { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
