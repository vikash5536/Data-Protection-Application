//*******************************************************//
//                                                       //
// CSharp.Net Data Potection Application common Library  //
// Copyright(c) 2014-2015 Spectra Logic Corporation.     //
//                                                       //
//*******************************************************//
using Ds3;
using Ds3.Calls;
using Ds3.Helpers;
using Ds3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataProtectionApplication.CommonLibrary
{
    public class Ds3Client
    {
        protected IDs3Client _client;
        protected IDs3ClientHelpers _helpers;
        public static Logger logger = new Logger(typeof(Ds3Client));

        /// <summary>
        /// Parametized Constructor
        /// </summary>
        /// <param name="endpoint">it contains endpoint</param>
        /// <param name="accessId">it contains accessId</param>
        /// <param name="secretkey">it contains secretkey</param>
        /// <param name="proxy">it contains ip address</param>
        public Ds3Client(string endpoint, string accessId, string secretkey, string proxy)
        {
            try
            {
                Ds3Builder builder = new Ds3Builder(endpoint, new Credentials(accessId, secretkey));
                if (!string.IsNullOrEmpty(proxy))
                {
                    builder.WithProxy(new Uri(proxy));
                }
                _client = builder.Build();

                // Set up the high-level abstractions.
                _helpers = new Ds3ClientHelpers(_client);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in Ds3Client ctor , Message : {0}", ex.Message));
            }

        }

        #region Autherization
        /// <summary>
        /// Method used to get response for spectras3
        /// </summary>
        /// <param name="request">object of class GetUserSpectraS3Request</param>
        /// <returns>GetUserSpectraS3Response</returns>
        public GetUserSpectraS3Response GetUserSpectraS3(GetUserSpectraS3Request request)
        {
            return _client.GetUserSpectraS3(request);
        }
        #endregion
        #region Bucket

        /// <summary>
        /// This Method is used to get head bucket response
        /// </summary>
        /// <param name="request">HeadBucketRequest</param>
        /// <returns>HeadBucketResponse</returns>
        public HeadBucketResponse HeadBucket(HeadBucketRequest request)
        {
            return _client.HeadBucket(request);
        }

        /// <summary>
        /// This method is used to add prefix to file path.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="prefix">Prefic to add</param>
        /// <returns></returns>
        private static string PrependPrefix(string path, string prefix)
        {
            try
            {
                var fileName = Path.GetFileName(path);
                var fixedPath = path.Substring(0, path.Length - fileName.Length) + prefix + fileName;
                return fixedPath;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Exception in PrependPrefix method , Message : {0}", ex.Message));
                return "";
            }
        }

        /// <summary>
        /// This method is used to replace window directory separator with Linux directory separator
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns></returns>
        private static string ConvertPathToKey(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        /// <summary>
        /// This method is used to list all objects of local directory.
        /// </summary>
        /// <param name="root">Path of source folder</param>
        /// <param name="prefix">Prefix if exists</param>
        /// <returns></returns>
        public static IEnumerable<Ds3Object> ListObjectsForDirectory(string root, string prefix = "")
        {
            // remove trailing slash (it works but spoil the count)
            if (root.EndsWith("\\") || root.EndsWith("/"))
            {
                root = root.Substring(0, root.Length - 1);
            }
            var rootDirectory = new DirectoryInfo(root);
            var rootSize = rootDirectory.FullName.Length;
            if (!rootDirectory.FullName.EndsWith("\\") && !rootDirectory.FullName.EndsWith("/"))
                rootSize++;
            return rootDirectory
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Select(file => new Ds3Object(
                PrependPrefix(ConvertPathToKey(file.FullName.Substring(rootSize)), prefix),
                file.Length
                ));
        }
        /// <summary>
        /// This Method is used to runPut
        /// </summary>
        /// <param name="bucket">it consists of bucket</param>
        /// <param name="srcDirectory">it consists of source directory</param>
        /// <param name="sFilename">it consists of source file name</param>
        /// <param name="dFilename">it consist destination file name</param>
        public void runPut(string bucket, string srcDirectory, string sFilename, string dFilename)
        {
            // get file size, instantiate Ds3Object, add to list
            FileInfo fileInfo = new FileInfo(Path.Combine(srcDirectory, sFilename));
            string path = Path.Combine(srcDirectory, sFilename);
            var ds3Obj = new Ds3Object(dFilename, fileInfo.Length);
            var ds3Objs = new List<Ds3Object>();
            ds3Objs.Add(ds3Obj);

            // Creates a bulk job with the server based on the files in a directory (recursively).
            IJob job = _helpers.StartWriteJob(bucket, ds3Objs);
            logger.LogInfo(string.Format("runPut({1}): Job id {0}", job.JobId, sFilename));

            // Provide Func<string, stream> to be called on each object
            job.Transfer(
                key =>
                    new DisposableFileStream(
                        File.OpenRead(path)));
            //job.Transfer(FileHelpers.BuildFilePutter(path));
        }

        /// <summary>
        /// This Method is used to set data to bucket
        /// </summary>
        /// <param name="bucket">it consists of bucket</param>
        /// <param name="srcDirectory">it consists of source directory</param>
        /// <param name="prefix">it contains prefix</param>
        public void runBulkPut(string bucket, string srcDirectory, string prefix = "")
        {
            List<Ds3Object> ds3Objs = new List<Ds3Object>();
            foreach (var filename in ListObjectsForDirectory(srcDirectory))
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(srcDirectory, filename.Name));
                var ds3Obj = new Ds3Object(prefix + filename.Name, fileInfo.Length);
                ds3Objs.Add(ds3Obj);
            }

            // Creates a bulk job with the server based on the files in a directory (recursively).
            IJob job = _helpers.StartWriteJob(bucket, ds3Objs);
            logger.LogInfo(string.Format("runBulkPut(): Job id {0}", job.JobId));

            // Transfer all of the files.
            job.Transfer(FileHelpers.BuildFilePutter(srcDirectory));
        }

        /// <summary>
        /// This Method is used to get all the files or folder
        /// </summary>
        /// <param name="bucket">it consists of bucket</param>
        /// <param name="directory">it consists of directory</param>
        /// <param name="prefix">it contains prefix</param>
        /// <returns>true or false</returns>
        public bool runBulkGet(string bucket, string directory, string prefix)
        {
            // Creates a bulk job with all of the objects in the bucket.
            // Same as: IJob job = _helpers.StartReadAllJob(bucket);
            var listOfObjects = _helpers.ListObjects(bucket).Where(item => item.Size > 0);
            IJob job = _helpers.StartReadJob(bucket, listOfObjects);
            logger.LogInfo(string.Format("runBulkGet(): Job id {0}", job.JobId));

            // Transfer all of the files.
            job.Transfer(FileHelpers.BuildFileGetter(directory, prefix));

            return true;
        }

        /// <summary>
        /// This Method is used to get all the files or folder
        /// </summary>
        /// <param name="bucket">bucket name </param>
        /// <param name="directory">dirctory</param>
        /// <param name="filename">file name</param>
        /// <returns></returns>
        public bool runGet(string bucket, string directory, string filename)
        {
            // find the desired object 
            var objects = _helpers.ListObjects(bucket);
            var targetobj = (from o in objects
                             where o.Name == filename
                             select o);

            // get it
            IJob job = _helpers.StartReadJob(bucket, targetobj);
            logger.LogInfo(string.Format("runGet({1}): Job id {0}", job.JobId, filename));

            // Transfer all of the files.
            job.Transfer(FileHelpers.BuildFileGetter(directory, string.Empty));

            return true;
        }

        /// <summary>
        /// This Method is used to list objects
        /// </summary>
        /// <param name="bucket">it is the bucket name to be deleted</param>
        /// <returns>collelction of Ds3objects</returns>
        public IEnumerable<Ds3Object> ListObjects(string bucket)
        {
            return _helpers.ListObjects(bucket);
        }

        /// <summary>
        /// This function returns the IDs3Client instance of current Ds3Client
        /// </summary>
        /// <returns></returns>
        public IDs3Client returnInstance()
        {
            return _client;
        }

        #endregion
    }
}
