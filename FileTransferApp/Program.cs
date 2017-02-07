//*******************************************************//
//                                                       //
// CSharp.Net Data Potection Application FileTransfer App//
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.CommonLibrary.Constants;
using DataProtectionApplication.CommonLibrary.Model;
using DataProtectionApplication.CommonLibrary.SendEmail;
using System;

namespace DataProtectionApplication.FileTransferApp
{
    class Program
    {
        public static Logger logger = new Logger(typeof(Program));

        /// <summary>
        /// Entry point for File transfer app.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == Constant.ActionParameterCount)
                {
                    logger.LogInfo("Starting FileTransfer main()");
                    var result = new FileTransfer().LoadActionArguments(args);
                    EmailConfiguration emailConfig = new SendEmail().LoadEmailConfiguration();
                    logger.LogInfo(result.ToString());
                    switch (result)
                    {
                        case HRESULT.SCHED_S_TASK_SUCCESS:
                        case HRESULT.TASK_NAME_NOT_SUPPLIED:
                        case HRESULT.SOURCE_LOCATION_DOES_NOT_EXISTS:
                        case HRESULT.UNABLE_TO_PARSE_ACTION_PARAMETERS:                        
                        case HRESULT.SCHED_F_TASK_FAILED:
                        case HRESULT.UNABLE_TO_PARSE_DEST_BP_CONFIG:
                        case HRESULT.UNABLE_TO_PARSE_SOURCE_BP_CONFIG:
                        case HRESULT.FULL_SERVER_BACKUP_FAILED:
                        case HRESULT.FULL_LOCAL_BACKUP_FAILED:
                        case HRESULT.PARTIAL_LOCAL_BACKUP_FAILED:
                        case HRESULT.PARTIAL_SERVER_BACKUP_FAILED:
                            {
                                try
                                {
                                    if (emailConfig != null)
                                    {
                                        SendEmail.UpdateEmailConfig(args, result, emailConfig);
                                        new SendEmail().SendMail(emailConfig);
                                    }
                                    else
                                        logger.LogInfo(string.Format("Unable to load email configuration."));
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError(string.Format("Exception in Send Email, Message : {0}", ex.Message));
                                }
                                Environment.Exit((int)result);
                            }
                            break;
                        default:
                            break;
                    }
                    logger.LogInfo("Completed FileTransfer main()");
                }
                else
                {
                    logger.LogError("FileTransfer: Arguments are not correct");
                    Environment.Exit((int)HRESULT.INVALID_ACTION_PARAMETER_COUNT);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in Main method, Message : {0} , StackTrace : {1} ", ex.Message, ex.StackTrace));
                Environment.Exit((int)HRESULT.SCHED_F_TASK_FAILED);
            }
        }
    }
}
