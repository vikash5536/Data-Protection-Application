//*******************************************************//
//                                                       //
// CSharp.Net Data Potection Application common Library  //
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
namespace DataProtectionApplication.CommonLibrary.Model
{

    /// <summary>
    /// Task Result code
    /// </summary>
    public enum HRESULT : int
    {
        SCHED_S_TASK_HAS_NOT_RUN_WIN7 = 1,
        SCHED_S_TASK_SUCCESS = 0,
        SCHED_S_TASK_READY = 214775687,
        SCHED_S_TASK_RUNNING = 267009,
        SCHED_S_TASK_DISABLED = 267010,
        SCHED_S_TASK_HAS_NOT_RUN = 267011,
        SCHED_S_TASK_NO_MORE_RUNS = 267012,
        SCHED_S_TASK_NOT_SCHEDULED = 267013,
        SCHED_S_TASK_TERMINATED = 267014,
        SCHED_S_TASK_NO_VALID_TRIGGERS = 267015,
        SCHED_S_EVENT_TRIGGER = 267016,
        SCHED_E_TRIGGER_NOT_FOUND = -2147216631,
        SCHED_E_TASK_NOT_READY = -2147216630,
        SCHED_E_TASK_NOT_RUNNING = -2147216629,
        SCHED_E_SERVICE_NOT_INSTALLED = -2147216628,
        SCHED_E_CANNOT_OPEN_TASK = -2147216627,
        SCHED_E_INVALID_TASK = -2147216626,
        SCHED_E_ACCOUNT_INFORMATION_NOT_SET = -2147216625,
        SCHED_E_ACCOUNT_NAME_NOT_FOUND = -2147216624,
        SCHED_E_ACCOUNT_DBASE_CORRUPT = -2147216623,
        SCHED_E_NO_SECURITY_SERVICES = -2147216622,
        SCHED_E_UNKNOWN_OBJECT_VERSION = -2147216621,
        SCHED_E_UNSUPPORTED_ACCOUNT_OPTION = -2147216620,
        SCHED_E_SERVICE_NOT_RUNNING = -2147216619,
        SCHED_E_UNEXPECTEDNODE = -2147216618,
        SCHED_E_NAMESPACE = -2147216617,
        SCHED_E_INVALIDVALUE = -2147216616,
        SCHED_E_MISSINGNODE = -2147216615,
        SCHED_E_MALFORMEDXML = -2147216614,
        SCHED_S_SOME_TRIGGERS_FAILED = -2147216613,
        SCHED_S_BATCH_LOGON_PROBLEM = -2147216612,
        SCHED_E_TOO_MANY_NODES = -2147216611,
        SCHED_E_PAST_END_BOUNDARY = -2147216610,
        SCHED_E_ALREADY_RUNNING = -2147216609,
        SCHED_E_USER_NOT_LOGGED_ON = -2147216608,
        SCHED_E_INVALID_TASK_HASH = -2147216607,
        SCHED_E_SERVICE_NOT_AVAILABLE = -2147216606,
        SCHED_E_SERVICE_TOO_BUSY = -2147216605,
        SCHED_E_TASK_ATTEMPTED = -2147216604,
        SCHED_S_TASK_QUEUED = -2147216603,
        SCHED_E_TASK_DISABLED = -2147216602,
        SCHED_E_TASK_NOT_V1_COMPAT = -2147216601,
        SCHED_E_START_ON_DEMAND = -2147216600,
        SCHED_E_PATH_NOT_FOUND_Win7 = -2147024893,
        SCHED_E_PATH_NOT_FOUND_Win10 = -2147024894,
        TASK_NAME_NOT_SUPPLIED = 11,
        SOURCE_LOCATION_DOES_NOT_EXISTS = 12,
        UNABLE_TO_PARSE_ACTION_PARAMETERS = 13, //Unable to parse action parameter which was supplied when action created in task.       
        SCHED_F_TASK_FAILED = 15, // Task got failed.
        UNABLE_TO_PARSE_DEST_BP_CONFIG = 16, // Unable to parse destination blackPearl server configuration.
        UNABLE_TO_PARSE_SOURCE_BP_CONFIG = 17,// Unable to parse source blackPearl server configuration.
        FULL_SERVER_BACKUP_FAILED = 18, //Full server backup task got failed.
        FULL_LOCAL_BACKUP_FAILED = 19, //Full local backup task got failed.
        PARTIAL_LOCAL_BACKUP_FAILED = 20,// Partial local backup task got failed.
        PARTIAL_SERVER_BACKUP_FAILED = 21,// Partial server backup task got failed.
        INVALID_ACTION_PARAMETER_COUNT = 22 // Parameters count in action are not valid.
    }

    public enum ErrorCodes { Success = 1, Fail = 2 };
    public class ErrorResult
    {
        public ErrorCodes ErrorCode { get; set; }
        public string  ErrorMessage { get; set; }
    }
}
