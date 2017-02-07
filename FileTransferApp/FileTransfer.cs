//*******************************************************//
//                                                       //
// CSharp.Net Data Potection Application FileTransfer App//
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
using DataProtectionApplication.CommonLibrary;
using DataProtectionApplication.CommonLibrary.Model;
using Ds3.Calls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DataProtectionApplication.FileTransferApp
{
    public class FileTransfer
    {
        public static Logger logger = new Logger(typeof(FileTransfer));
        #region Case 1 : Backup
        /// <summary>
        /// This method is used to load action arguments.
        /// </summary>
        /// <param name="args"></param>
        public HRESULT LoadActionArguments(string[] args)
        {
            try
            {
                // Assigning arguments to ActionParameters object
                ActionParameters action = new ActionParameters()
                {
                    TaskName = args[0],
                    ActionType = (ActionTypeEnum)Enum.Parse(typeof(ActionTypeEnum), args[1]),
                    BackupRestoreType = (BackupRestoreTypeEnum)Enum.Parse(typeof(BackupRestoreTypeEnum), args[2]),
                    BackupRestoreLocation = (BackupRestoreLoactionEnum)Enum.Parse(typeof(BackupRestoreLoactionEnum), args[3]),
                    SourceServerDetails = args[4],
                    SourceLocation = args[5],
                    DestinationDetails = args[6],
                    DestinationBucketName = args[7]
                };

                // Check that task should have task name
                if (action.TaskName == "")
                {
                    logger.LogError("Task name can not be Empty");
                    return HRESULT.TASK_NAME_NOT_SUPPLIED;
                }
                if (action.BackupRestoreLocation == BackupRestoreLoactionEnum.Local)
                {
                    //This is used to check if the source local location is exists or not.
                    if (!Directory.Exists(action.SourceLocation))
                    {
                        logger.LogError(string.Format("Source Location ({0}) does not exists", action.SourceLocation));
                        return HRESULT.SOURCE_LOCATION_DOES_NOT_EXISTS;
                    }
                }
                return TransferHandeler(action);                                                                    
            }
            catch (Exception ex)
            {                
                logger.LogError(string.Format("Exception in LoadActionArguments , Message : {0}", ex.Message));
                return HRESULT.UNABLE_TO_PARSE_ACTION_PARAMETERS;
            }

        }
        /// <summary>
        /// This method is used to handle the data to be transfer.
        /// </summary>
        /// <param name="action">Contains all informations about action</param>
        /// <returns></returns>
        private HRESULT TransferHandeler(ActionParameters action)
        {
            try
            {
                BlackPearlConfiguration dConfig = null;
                BlackPearlConfiguration sConfig = null;

                // Deserialize the xml string if destination details exists
                if (action.DestinationDetails != null)
                {
                    dConfig = new BlackPearlConfiguration();
                    XmlSerializer serializer = new XmlSerializer(typeof(BlackPearlConfiguration));
                    byte[] byteArray = Encoding.UTF8.GetBytes(action.DestinationDetails);
                    MemoryStream stream = new MemoryStream(byteArray);
                    dConfig = (BlackPearlConfiguration)serializer.Deserialize(stream);
                }

                // Check destination configuration exists
                if (dConfig == null || string.IsNullOrEmpty(dConfig.AccessId) || string.IsNullOrEmpty(dConfig.SecretKey))
                {
                    string fatalerr = "Must set values for DS3_ENDPOINT, DS3_ACCESS_KEY, and DS3_SECRET_KEY to continue";
                    logger.LogError("FileTransfer:" + fatalerr);
                    return HRESULT.UNABLE_TO_PARSE_DEST_BP_CONFIG;
                }

                // Deserialize the xml string if source details exists
                if (action.BackupRestoreLocation == BackupRestoreLoactionEnum.Server)
                {
                    if (!string.IsNullOrEmpty(action.SourceServerDetails))
                    {
                        sConfig = new BlackPearlConfiguration();
                        XmlSerializer serializer = new XmlSerializer(typeof(BlackPearlConfiguration));
                        byte[] byteArray = Encoding.UTF8.GetBytes(action.SourceServerDetails);
                        MemoryStream stream = new MemoryStream(byteArray);
                        sConfig = (BlackPearlConfiguration)serializer.Deserialize(stream);
                    }
                    else
                        return HRESULT.UNABLE_TO_PARSE_SOURCE_BP_CONFIG;
                }

                ///Destination client creation
                Ds3Client dDs3Client = new Ds3Client(dConfig.GetEndPoint(), dConfig.GetAccessId(), dConfig.GetSecretKey(), "");
                logger.LogInfo("FileTransfer: Destination server connected successfully");

                Ds3Client sDs3Client = null;
                if (sConfig != null)
                {
                    ///Source client creation
                    sDs3Client = new Ds3Client(sConfig.GetEndPoint(), sConfig.GetAccessId(), sConfig.GetSecretKey(), "");
                    logger.LogInfo("FileTransfer: Source server connected successfully");
                }
                logger.LogInfo(string.Format("FileTransfer: Task Name : {0} ", action.TaskName));
                switch (action.ActionType)
                {
                    case ActionTypeEnum.Backup: //Action Type Backup
                        logger.LogInfo("FileTransfer: Action Type: Backup");
                        switch (action.BackupRestoreType)
                        {
                            case BackupRestoreTypeEnum.Full://Backup type Full
                                logger.LogInfo("FileTransfer: Type: Full");
                                switch (action.BackupRestoreLocation)
                                {
                                    case BackupRestoreLoactionEnum.Local://Full Backup from local or network drive
                                        logger.LogInfo("FileTransfer: Location: Local");
                                        return FullLocalBackup(action, dDs3Client);
                                    case BackupRestoreLoactionEnum.Server://Backup from another BlackPearl server
                                        logger.LogInfo("FileTransfer: Location: Server");
                                        return FullServerBackup(action, sDs3Client, dDs3Client);
                                    default:
                                        break;
                                }
                                break;
                            case BackupRestoreTypeEnum.Partial://Backup type partial
                                logger.LogInfo("FileTransfer: Type: Partial");
                                switch (action.BackupRestoreLocation)
                                {
                                    case BackupRestoreLoactionEnum.Local://Partial backup from local or network drive
                                        logger.LogInfo("FileTransfer: Location: Local");
                                        return PartialLocalBackup(action, dDs3Client);
                                    case BackupRestoreLoactionEnum.Server://Partial backup from other Blackpearl server
                                        logger.LogInfo("FileTransfer: Location: Server");
                                        return PartialServerBackup(action, sDs3Client, dDs3Client);
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case ActionTypeEnum.Restore://Action type Restore : Needs to be implemented
                        break;
                    default:
                        break;
                }
                return HRESULT.SCHED_S_TASK_SUCCESS;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in TransferHandeler method ,Message :{0} , StackTrace: {1}", ex.Message,ex.StackTrace));
                return HRESULT.SCHED_F_TASK_FAILED;
            }
        }
        #region Case 1.1 : Backup : Full-Local/Network
        /// <summary>
        /// This method is used to handle Full Local Backup
        /// </summary>
        /// <param name="action">Contain info of action</param>
        /// <param name="dDs3Client">Client for destination Server</param>
        /// <returns>Status of operation</returns>        
        private HRESULT FullLocalBackup(ActionParameters action, Ds3Client dDs3Client)
        {
            try
            {
                //Check if destination client exists or not
                if (dDs3Client == null)
                {
                    logger.LogError("FileTransfer: Destination is unknown");
                    return HRESULT.FULL_LOCAL_BACKUP_FAILED;
                }
                //fetch the current timestamp for making the directory with current timestamp
                string timeStamp = DateTime.Now.ToString("yyyy_MM_ddHHmmssfff");
                foreach (var filename in Ds3Client.ListObjectsForDirectory(action.SourceLocation))
                    // put a file from source location to BlackPearl with taskname and timestamp as prefix
                    dDs3Client.runPut(action.DestinationBucketName, action.SourceLocation, filename.Name, action.TaskName + "/" + timeStamp + "/" + filename.Name);
                logger.LogInfo("Full local backup Successful");
                return HRESULT.SCHED_S_TASK_SUCCESS; // If task executed successfully
            }
            catch (Exception ex)
            {
                logger.LogError("File: FileTransfer Func: FullLocalBackup Message:" + ex.Message);
                return HRESULT.FULL_LOCAL_BACKUP_FAILED; // If task fails with any exception
            }
        }
        #endregion
        #region Case 1.2 : Backup : Full-Server
        /// <summary>
        /// This method is used to handle Full Server Backup
        /// This method use local system as an intermediate to transfer file server to server
        /// </summary>
        /// <param name="action">Contain info of action</param>
        /// <param name="sDs3Client">Client for source Server</param>
        /// <param name="dDs3Client">Client for destination Server</param>
        /// <returns>Status of operation</returns>
        private HRESULT FullServerBackup(ActionParameters action, Ds3Client sDs3Client, Ds3Client dDs3Client)
        {
            try
            {
                //Check if destination client exists or not
                if (dDs3Client == null)
                {
                    logger.LogError("FileTransfer: Destination is unknown");
                    return HRESULT.FULL_SERVER_BACKUP_FAILED;
                }

                // Combine a local temporary path with taskname and current timestamp
                string path = Path.Combine("temp", action.TaskName, DateTime.Now.ToString("yyyy_MM_ddHHmmssfff"));

                // Check if source client is exists or not
                if (sDs3Client == null)
                {
                    logger.LogError("FileTransfer: Source is unknown");
                    return HRESULT.FULL_SERVER_BACKUP_FAILED;
                }

                // put all data in local temporary folder
                sDs3Client.runBulkGet(action.SourceLocation, path, "");

                // put all data from local to BlackPearl
                dDs3Client.runBulkPut(action.DestinationBucketName, "temp", "");

                // Delete the temporary Directory
                Directory.Delete("temp", true);
                logger.LogInfo("Full Server backup Successful");
                return HRESULT.SCHED_S_TASK_SUCCESS; // returns SCHED_S_TASK_SUCCESS if task is successful
            }
            catch (Exception ex)
            {
                logger.LogError("File: FileTransfer Func: FullServerBackup Message:" + ex.Message);
                return HRESULT.FULL_SERVER_BACKUP_FAILED; // returns FULL_SERVER_BACKUP_FAILED if task is unsuccessful
            }
        }
        #endregion
        #region Case 1.3 : Backup : Partial-Local/Network
        /// <summary>
        /// This method is used to handle Partial Local Backup
        /// </summary>
        /// <param name="action">Contain info of action</param>
        /// <param name="dDs3Client">Client for destination Server</param>
        /// <returns>Status of operation</returns>
        private HRESULT PartialLocalBackup(ActionParameters action, Ds3Client dDs3Client)
        {
            try
            {
                // Check if destination client exists or not
                if (dDs3Client == null)
                {
                    logger.LogError("FileTransfer: Destination is unknown");
                    return HRESULT.PARTIAL_LOCAL_BACKUP_FAILED;
                }

                // Make a bucket request to pull the info about that bucket
                var request = new GetBucketRequest(action.DestinationBucketName);
                // Fetch the response corresponds to that request
                GetBucketResponse bucketResponse = dDs3Client.returnInstance().GetBucket(request);
                // Fetch the list of all files of bucket
                var listofobjects = bucketResponse.ResponsePayload.Objects;
                // find the files having present in same taskname folder
                var match = listofobjects.Where(x => x.Key.StartsWith(action.TaskName + "/"));
                
                // check if files exists with same task name or not
                if (match.Count() == 0)
                {
                    // files not exist means this task is executing first time so we have to take full backup
                    string timeStamp = DateTime.Now.ToString("yyyy_MM_ddHHmmssfff");
                    foreach (var filename in Ds3Client.ListObjectsForDirectory(action.SourceLocation))
                        dDs3Client.runPut(action.DestinationBucketName, action.SourceLocation, filename.Name, action.TaskName + "/" + timeStamp + "/" + filename.Name);
                }
                else
                {
                    // files exists of same task
                    int pos = action.TaskName.Length + 1;
                    // select the current timestamp
                    string ts = DateTime.Now.ToString("yyyy_MM_ddHHmmssfff");
                    // set pos at the starting of backup path at server (after task name and timestamp)
                    pos += ts.Length + 1;
                    // fetch source file list from Source Directory
                    var sourceFiles = Ds3Client.ListObjectsForDirectory(action.SourceLocation);
                    var listSourceFiles = sourceFiles;
                    // list source file that is not present on BlackPearl
                    foreach (var item in sourceFiles)
                    {
                        FileInfo fileInfo = new FileInfo(Path.Combine(action.SourceLocation, item.Name));
                        if ((match.Where(x => string.Equals(x.Key.Substring(pos), item.Name) && x.LastModified > fileInfo.LastWriteTime).Count() > 0))
                            listSourceFiles = listSourceFiles.Where(x => x.Name != item.Name);
                    }
                    // put that files that are not present on Blackpearl
                    foreach (var filename in listSourceFiles)
                        dDs3Client.runPut(action.DestinationBucketName, action.SourceLocation, filename.Name, action.TaskName + "/" + ts + "/" + filename.Name);
                }
                logger.LogInfo("Partial Local backup Successful");
                return HRESULT.SCHED_S_TASK_SUCCESS; // returns true if task executed successfully
            }
            catch (Exception ex)
            {
                logger.LogError("File: FileTransfer Func: PartialLocalBackup Message:" + ex.Message);
                return HRESULT.PARTIAL_LOCAL_BACKUP_FAILED; ; // returns false if task execution unsuccessful
            }
        }
        #endregion
        #region Case 1.4 : Backup : Partial-Server
        /// <summary>
        /// This method is used handle Partial Server Backup
        /// This method use local system as an intermediate to transfer file server to server
        /// </summary>
        /// <param name="action">Contain info of action</param>
        /// <param name="sDs3Client">Client for source Server</param>
        /// <param name="dDs3Client">Client for destination Server</param>
        /// <returns>Status of operation</returns>
        private HRESULT PartialServerBackup(ActionParameters action, Ds3Client sDs3Client, Ds3Client dDs3Client)
        {
            try
            {
                // Check if source client exists or not
                if (sDs3Client == null)
                {
                    logger.LogError("FileTransfer: Source is unknown");
                    return HRESULT.PARTIAL_SERVER_BACKUP_FAILED;
                }
                // Check if destination client exists or not
                if (dDs3Client == null)
                {
                    logger.LogError("FileTransfer: Destination is unknown");
                    return HRESULT.PARTIAL_SERVER_BACKUP_FAILED;
                }
                
                // Make a bucket request to pull the info about that bucket
                var request = new GetBucketRequest(action.DestinationBucketName);
                // Fetch the response corresponds to that request
                GetBucketResponse bucketResponse = dDs3Client.returnInstance().GetBucket(request);
                // Fetch the list of all files of bucket
                var listofobjects = bucketResponse.ResponsePayload.Objects;
                // find the files having present in same taskname folder
                var match = listofobjects.Where(x => x.Key.StartsWith(action.TaskName + "/"));

                // check if any file exists of same task
                if (match.Count() == 0)
                {
                    // file not exist means we have to take full backup
                    string path1 = Path.Combine("temp", action.TaskName, DateTime.Now.ToString("yyyy_MM_ddHHmmssfff"));
                    sDs3Client.runBulkGet(action.SourceLocation, path1, "");
                    dDs3Client.runBulkPut(action.DestinationBucketName, "temp", "");
                    Directory.Delete("temp", true);
                }
                else
                {
                    // files exists of same task
                    int pos = action.TaskName.Length + 1;
                    // select the current timestamp
                    string ts = DateTime.Now.ToString("yyyy_MM_ddHHmmssfff");
                    // set pos at the starting of backup path at server (after task name and timestamp)
                    pos += ts.Length + 1;
                    // fetch source file list from Source Bucket
                    var sourceFiles = sDs3Client.ListObjects(action.SourceLocation);
                    var listSourceFiles = sourceFiles;
                    // list source file that is not present on BlackPearl
                    foreach (var item in sourceFiles)
                    {
                        FileInfo fileInfo = new FileInfo(Path.Combine(action.SourceLocation, item.Name));
                        if ((match.Where(x => string.Equals(x.Key.Substring(pos), item.Name) && x.LastModified > fileInfo.LastWriteTime).Count() > 0))
                            listSourceFiles = listSourceFiles.Where(x => x.Name != item.Name);
                    }
                    // put that file that are not present on Blackpearl using local system
                    foreach (var filename in listSourceFiles)
                    {
                        if (filename.Size > 0) // Do not include folders when creating a GET job.
                        {
                            sDs3Client.runGet(action.SourceLocation, "temp", filename.Name);
                            dDs3Client.runPut(action.DestinationBucketName, "temp", filename.Name, action.TaskName + "/" + ts + "/" + filename.Name);
                        }
                    }
                    // delete temporay filder if exists
                    if (Directory.Exists("temp"))
                        Directory.Delete("temp", true);
                }
                logger.LogInfo("Partial Server backup Successful");
                return HRESULT.SCHED_S_TASK_SUCCESS; ; // returns true if task executed successfully
            }
            catch (Exception ex)
            {
                logger.LogError("File: FileTransfer Func: PartialServerBackup Message:" + ex.Message);
                return HRESULT.PARTIAL_SERVER_BACKUP_FAILED; ; // returns true if task execution unsuccessful
            }
        }
        #endregion
        #endregion
    }
}
