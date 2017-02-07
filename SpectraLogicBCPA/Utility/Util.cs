//**********************************************************//
//                                                          //
// CSharp.Net Data Potection Application TaskScheduling App //
// Copyright(c) 2014-2015 Spectra Logic Corporation.        //
//                                                          //
//**********************************************************//
using System;
using System.IO;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.TaskSchedulingApp.Views;
using Ds3.Calls;
using System.Xml;
using DataProtectionApplication.CommonLibrary.Model;
using DataProtectionApplication.CommonLibrary.Constants;

namespace DataProtectionApplication.TaskSchedulingApp.Common
{
    public class Util
    {
        public static Logger logger = new Logger(typeof(Util));

        /// <summary>
        /// This method enables num key press.
        /// </summary>
        /// <param name="inKey" type="Key">it provides the key pressed</param>
        /// <returns type=true/false></returns>

        public bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
            {
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method is used to load xml configuration.
        /// </summary>
        /// <param name="ServerFileName"></param>
        /// <returns></returns>

        public static BlackPearlConfiguration LoadConfigurationFromFile(string serverfilename)
        {
            BlackPearlConfiguration config = new BlackPearlConfiguration();
            XmlSerializer serializer = new XmlSerializer(typeof(BlackPearlConfiguration));
            using (StreamReader reader = new StreamReader(serverfilename))
            {
                config = (BlackPearlConfiguration)serializer.Deserialize(reader);
            }
            return config;
        }

        /// <summary>
        /// This method is used to show loaded xml configuration if exists else show page to create.
        /// </summary>
        /// <param name="serverfilename">BlackPearl server configuration file</param>
        /// <param name="servertype">Source\Destination</param>
        /// <param name="isSaveConfig">Is Config need to save</param>
        public static void ShowBlackPearlConfiguration(ActionParameters actionParameter, ServerType servertype, bool isCreate, bool isSaveConfig)
        {
            try
            {
                if (isCreate)
                {
                    if (File.Exists(Constant.DestinationServerDetails) && servertype == ServerType.Destination)
                    {
                        BlackPearlConfiguration _config = LoadConfigurationFromFile(Constant.DestinationServerDetails);
                        BlackPearlConfig.ShowEdit(_config, isSaveConfig);  // Save Configuration in file
                    }
                    else
                        BlackPearlConfig.ShowCreate(servertype, isSaveConfig); // Save Configuration in file
                }
                else
                {
                    string serverdetails = servertype == ServerType.Source ? actionParameter.SourceServerDetails : actionParameter.DestinationDetails;
                    if (!string.IsNullOrEmpty(serverdetails))
                    {
                        XDocument doc = XDocument.Parse(serverdetails);
                        BlackPearlConfiguration _config = Util.LoadConfigurationFromDocument(doc);
                        if (_config != null)
                            BlackPearlConfig.ShowEdit(_config, isSaveConfig);  // Don't Save Configuration in file
                        else
                            logger.LogInfo(string.Format("Unable to load configuration from document."));
                    }
                    else
                        BlackPearlConfig.ShowCreate(servertype, isSaveConfig); // Save Configuration in file
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in ShowBlackPearlConfiguration method, Message : {0}", ex.Message));
            }


        }

        /// <summary>
        /// This method is used to load configuration from xml document
        /// </summary>
        /// <param name="doc">document name</param>

        internal static BlackPearlConfiguration LoadConfigurationFromDocument(XDocument doc)
        {
            try
            {
                XmlSerializer xS = new XmlSerializer(typeof(BlackPearlConfiguration));
                return (BlackPearlConfiguration)xS.Deserialize(doc.CreateReader());
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in LoadConfigurationFromDocument method, Message : {0}, StackTrace : {1}", ex.Message, ex.StackTrace));
                return null;
            }

        }

        /// <summary>
        /// This Method check whether bucket name exists or not
        /// </summary>
        /// <param name="config">it is object of an class BlackPearlConfiguration</param>
        /// <returns>true or false</returns>

        public static bool IsBucketExist(string bucketName, BlackPearlConfiguration _config)
        {
            try
            {
                Ds3Client client = new Ds3Client(_config.GetEndPoint(), _config.GetAccessId(), _config.GetSecretKey(), "");
                var headResponse = client.HeadBucket(new HeadBucketRequest(bucketName));
                if (headResponse.Status == HeadBucketResponse.StatusType.Exists)
                {
                    logger.LogInfo(string.Format("Bucket exists , Name : {0} ", bucketName));
                    return true;
                }
                else
                {
                    logger.LogInfo(string.Format("Bucket not found , Name : {0} ", bucketName));
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in IsBucketExist method, Message : {0}", ex.Message));
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Info, CustomPopup.ePopupTitle.Information, ex.Message, CustomPopup.ePopupButton.OK);
                return false;
            }
        }

        /// <summary>
        /// This Method is used to save black pearl configurations
        /// </summary>
        /// <param name="config">it is object of an class BlackPearlConfiguration</param>
        /// <param name="servertype">it return the value of enum ServerType</param>
        /// <returns>true or false</returns>

        public static string SaveBlackPearlConfiguration(BlackPearlConfiguration config, ServerType servertype)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(config.GetType());
                string result = string.Empty;
                XmlDocument xd = null;
                using (MemoryStream memStm = new MemoryStream())
                {
                    ser.Serialize(memStm, config);

                    memStm.Position = 0;

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;
                    var xtr = XmlReader.Create(memStm, settings);
                    xd = new XmlDocument();
                    xd.Load(xtr);
                }

                if (servertype == ServerType.Destination)
                {
                    xd.Save(Constant.DestinationServerDetails);
                    return File.ReadAllText(Constant.DestinationServerDetails).Replace("\"", "'").Replace("\r\n", "");
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in SaveBlackPearlConfiguration, message : {0}", ex.Message));
                return "";
            }
        }

        /// <summary>
        /// This method is used to save email configuration.
        /// </summary>
        /// <param name="_currentEmailConfig"></param>

        public static void SaveEmailConfiguration(EmailConfiguration _currentEmailConfig)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(_currentEmailConfig.GetType());
                string result = string.Empty;
                XmlDocument xd = null;
                using (MemoryStream memStm = new MemoryStream())
                {
                    ser.Serialize(memStm, _currentEmailConfig);
                    memStm.Position = 0;
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;

                    var xtr = XmlReader.Create(memStm, settings);
                    xd = new XmlDocument();
                    xd.Load(xtr);

                }
                xd.Save(Constant.EmailConfigurationDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in SaveEmailConfiguration, message : {0}", ex.Message));
            }
        }

        /// <summary>
        /// This method is used to load Email configuration.
        /// </summary>
        /// <param name="emailConfigurationDetails"></param>
        /// <returns></returns>

        public static EmailConfiguration LoadEmailConfiguration(string emailConfigurationDetails)
        {
            try
            {
                EmailConfiguration _emailConfig = new EmailConfiguration();
                XmlSerializer serializer = new XmlSerializer(typeof(EmailConfiguration));
                StreamReader reader = new StreamReader(emailConfigurationDetails);
                _emailConfig = (EmailConfiguration)serializer.Deserialize(reader);
                reader.Close();
                return _emailConfig;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in LoadEmailConfiguration method , Message : {0}", ex.Message));
                return null;
            }

        }

        /// <summary>
        /// This method is used to validate spectra user.
        /// </summary>
        /// <returns></returns>
        public GetUserSpectraS3Response IsValidS3User(BlackPearlConfiguration config)
        {
            try
            {
                if (config != null)
                {
                    Ds3Client client = new Ds3Client(config.GetEndPoint(), config.GetAccessId(), config.GetSecretKey(), "");
                    return client.GetUserSpectraS3(new GetUserSpectraS3Request(config.GetAccessId()));
                }
            }
            catch (Exception ex)
            {
                new CustomPopup().DisplayPopupData(CustomPopup.ePopupImage.Error, CustomPopup.ePopupTitle.Error, ex.Message, CustomPopup.ePopupButton.OK);
                logger.LogError(string.Format("Exception in IsValidS3User method, Message : {0}", ex.Message));
            }
            return null;
        }
    }
}
