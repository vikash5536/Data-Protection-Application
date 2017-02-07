//*******************************************************//
//                                                       //
// CSharp.Net Data Potection Application common Library  //
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//

namespace DataProtectionApplication.CommonLibrary.Model
{
    /// <summary>
    /// Action type enum : Backup or Restore
    /// </summary>
    public enum ActionTypeEnum { Backup = 1, Restore = 2 };
    /// <summary>
    /// Backup/Restore type : Full or Partial
    /// </summary>
    public enum BackupRestoreTypeEnum { Full = 1, Partial = 2 };
    /// <summary>
    /// Backup/Server location : Local/Network or Server
    /// </summary>
    public enum BackupRestoreLoactionEnum { Local = 1, Server = 2 }
    /// <summary>
    /// BlackPearl server type: Local or Server
    /// </summary>
    public enum ServerType { Source = 1, Destination = 2 };
    /// <summary>
    /// This class is used to pass action parameters in a task.
    /// </summary>
    public class ActionParameters
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Task Name
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// Action Type : Backup or Restore
        /// </summary>
        public ActionTypeEnum ActionType { get; set; }
        /// <summary>
        /// Backup/Restore type  : Full or Partial
        /// </summary>
        public BackupRestoreTypeEnum BackupRestoreType { get; set; }
        /// <summary>
        /// Backup/Restore Location : Local/Network or another BlackPearl server
        /// </summary>
        public BackupRestoreLoactionEnum BackupRestoreLocation { get; set; }
        /// <summary>
        /// Action Path : File transfer application path.
        /// </summary>
        public string ActionPath { get; set; }
        /// <summary>
        /// Source Server details in case of source is BlackPearl Server 
        /// </summary>
        public string SourceServerDetails { get; set; }
        /// <summary>
        /// Destination BlackPearl Server Details.
        /// </summary>
        public string DestinationDetails { get; set; }
        /// <summary>
        /// Bucket name incase source is BlackPearl Server else local folder path.
        /// </summary>
        public string SourceLocation { get; set; }
        /// <summary>
        /// Destination Bucket name 
        /// </summary>
        public string DestinationBucketName { get; set; }
    }
}
