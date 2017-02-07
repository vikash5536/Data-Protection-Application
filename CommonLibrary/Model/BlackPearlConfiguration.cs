//*******************************************************//
//                                                       //
// CSharp.Net Data Potection Application common Library  //
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
namespace DataProtectionApplication.CommonLibrary.Model
{
    public class BlackPearlConfiguration
    {
        public string IP { get; set; }
        public int Port { get;  set; }
        public string BucketName { get;  set; }
        public string AccessId { get;  set; }
        public string SecretKey { get; set; }
        public ServerType ServerType { get; set; }

        /// <summary>
        /// Method used to get end point
        /// </summary>
        /// <returns>end point in string type</returns>
        public string GetEndPoint()
        {
            return "http://" + IP + ":" + Port;
        }

        /// <summary>
        /// Method used to get Bucket name
        /// </summary>
        /// <returns>Bucket name in string type</returns>
        public string GetBucketName()
        {
            return BucketName;
        }

        /// <summary>
        /// Method used to get Acces ID
        /// </summary>
        /// <returns>Acces ID in string type</returns>
        public string GetAccessId()
        {
            return AccessId;
        }

        /// <summary>
        /// Method used to get Secret Key
        /// </summary>
        /// <returns>Secret Key in string type</returns>
        public string GetSecretKey()
        {
            return SecretKey;
        }
    }
}
