using DataProtectionApplication.CommonLibrary.Model;
using DataProtectionApplication.FileTransferApp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataProtectionApplication.DPA.Test
{
    /// <summary>
    /// Test class to test all backup functionalities
    /// </summary>
    [TestFixture]
    public class FileTransferTest
    {
        private const string dirName = "DPA_TestData"; // Test Local Directory
        private const string fileName = "DPATestData"; // Test Local FileName prefix

        /// <summary>
        /// Create new file for testing
        /// </summary>
        /// <param name="fileCount">No. of files has to be created</param>
        /// <returns></returns>
        private bool CreateNewFile(int fileCount)
        {
            try
            {
                int i = 0; // File suffix starts to 0
                if (!Directory.Exists(dirName)) // check whether directory exists or not
                    Directory.CreateDirectory(dirName); // create test local directory if not exists
                // find the next creation file suffix
                for (i = 1; ; i++)
                {
                    if (!File.Exists(Path.Combine(dirName, fileName + i.ToString() + ".txt")))
                        break;
                }

                // create test files
                for (int count = 0; count < fileCount; count++, i++)
                {
                    using (StreamWriter sw = File.CreateText(Path.Combine(dirName, fileName + i.ToString() + ".txt")))
                    {
                        sw.WriteLine("This is a demo transfer file.");
                    }
                }
                return true; // returns true in case of success
            }
            catch (System.Exception)
            {
                return false; // returns false if any exception occurs
            }
        }

        /// <summary>
        /// Fetch the Local Test Action Parameters from csv file
        /// </summary>
        /// <returns>return the array of action parameters</returns>
        private IEnumerable<string[]> GetTestDataLocal()
        {
            using (var fs = File.Open(@"Data\Data_Local.csv", FileMode.Open, FileAccess.Read)) // open file in read mode
            {
                using (var reader = new StreamReader(fs)) // read file content with streamreader
                {
                    while (!reader.EndOfStream) // loop till filecontent exists
                    {
                        var line = reader.ReadLine(); // read one line
                        var values = line.Split(','); // split the content of line
                        bool result = false;
                        if (!bool.TryParse(values[9], out result)) // check for header
                            continue;
                        // Assigning
                        string TaskName = values[0];
                        string ActionType = values[1];
                        string BackupRestoreType = values[2];
                        string BackupRestoreLoaction = values[3];
                        string SourceServerDetails = values[4];
                        string SourceLocation = values[5];
                        string DestinationDetails = values[6];
                        string DestinationBucketName = values[7];
                        string ExpectedResult = values[8];
                        string IsRun = values[9];
                        // make a array to return
                        yield return new[] { TaskName, ActionType, BackupRestoreType, BackupRestoreLoaction, SourceServerDetails, DestinationDetails, SourceLocation, DestinationBucketName, ExpectedResult, IsRun };
                    }
                }
            }
        }

        /// <summary>
        /// Fetch the Server Test Action Parameters from csv file
        /// </summary>
        /// <returns>return the array of action parameters</returns>
        private IEnumerable<string[]> GetTestDataServer()
        {
            using (var fs = File.Open(@"Data\Data_Server.csv", FileMode.Open, FileAccess.Read))// open file in read mode
            {
                using (var reader = new StreamReader(fs)) // read file content with streamreader
                {
                    while (!reader.EndOfStream) // loop till filecontent exists
                    {
                        var line = reader.ReadLine(); // read one line
                        var values = line.Split(','); // split the content of line
                        bool result = false;
                        if (!bool.TryParse(values[9], out result)) // check for header
                            continue;
                        // Assigning
                        string TaskName = values[0];
                        string ActionType = values[1];
                        string BackupRestoreType = values[2];
                        string BackupRestoreLoaction = values[3];
                        string SourceServerDetails = values[4];
                        string SourceLocation = values[5];
                        string DestinationDetails = values[6];
                        string DestinationBucketName = values[7];
                        string ExpectedResult = values[8];
                        string IsRun = values[9];
                        // make a array to return
                        yield return new[] { TaskName, ActionType, BackupRestoreType, BackupRestoreLoaction, SourceServerDetails, DestinationDetails, SourceLocation, DestinationBucketName, ExpectedResult, IsRun };
                    }
                }
            }
        }

        /// <summary>
        /// Fetch the Network Test Action Parameters from csv file
        /// </summary>
        /// <returns>return the array of action parameters</returns>
        private IEnumerable<string[]> GetTestDataNetwork()
        {
            using (var fs = File.Open(@"Data\Data_Network.csv", FileMode.Open, FileAccess.Read))// open file in read mode
            {
                using (var reader = new StreamReader(fs)) // read file content with streamreader
                {
                    while (!reader.EndOfStream) // loop till filecontent exists
                    {
                        var line = reader.ReadLine(); // read one line
                        var values = line.Split(','); // split the content of line
                        bool result = false;
                        if (!bool.TryParse(values[9], out result)) // check for header
                            continue;
                        // Assigning
                        string TaskName = values[0];
                        string ActionType = values[1];
                        string BackupRestoreType = values[2];
                        string BackupRestoreLoaction = values[3];
                        string SourceServerDetails = values[4];
                        string SourceLocation = values[5];
                        string DestinationDetails = values[6];
                        string DestinationBucketName = values[7];
                        string ExpectedResult = values[8];
                        string IsRun = values[9];
                        // make a array to return
                        yield return new[] { TaskName, ActionType, BackupRestoreType, BackupRestoreLoaction, SourceServerDetails, DestinationDetails, SourceLocation, DestinationBucketName, ExpectedResult, IsRun };
                    }
                }
            }
        }

        /// <summary>
        /// Test method for creating new tast files
        /// </summary>
        [Test]
        public void CreateNewFileTest()
        {
            bool actual = CreateNewFile(4);
            Assert.AreEqual(actual, true); // check if test case pass or not
        }

        /// <summary>
        /// Test method for testing local to blackpearl backup
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="ActionType"></param>
        /// <param name="BackupRestoreType"></param>
        /// <param name="BackupRestoreLoaction"></param>
        /// <param name="SourceServerDetails"></param>
        /// <param name="DestinationDetails"></param>
        /// <param name="SourceLocation"></param>
        /// <param name="DestinationBucketName"></param>
        /// <param name="ExpectedResult"></param>
        /// <param name="IsRun"></param>
        [Test, TestCaseSource("GetTestDataLocal")]
        public void FileTransferLocalTest(string TaskName, string ActionType, string BackupRestoreType, string BackupRestoreLoaction, string SourceServerDetails, string DestinationDetails, string SourceLocation, string DestinationBucketName, string ExpectedResult, string IsRun)
        {

            // Check for run
            if (bool.Parse(IsRun))
            {
                if (!Directory.Exists(dirName)) // create file if any file notexist (directory not exists)
                    CreateNewFile(1);
                // Arrange
                string[] args = new string[]
            {
                TaskName,
                ActionType,
                BackupRestoreType,
                BackupRestoreLoaction,
                SourceServerDetails,
                SourceLocation,
                DestinationDetails,
                DestinationBucketName
            };
                HRESULT Expected = (HRESULT)Enum.Parse(typeof(HRESULT), ExpectedResult);
                if (BackupRestoreType == "Partial")
                {
                    CreateNewFile(1);
                    Thread.Sleep(1000);
                }
                // Act
                FileTransfer testobj = new FileTransfer();
                // Assert
                HRESULT actualResult = testobj.LoadActionArguments(args);
                Assert.AreEqual(actualResult, Expected);
            }
        }

        /// <summary>
        /// Test method for testing blackpearl to blackpearl backup
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="ActionType"></param>
        /// <param name="BackupRestoreType"></param>
        /// <param name="BackupRestoreLoaction"></param>
        /// <param name="SourceServerDetails"></param>
        /// <param name="DestinationDetails"></param>
        /// <param name="SourceLocation"></param>
        /// <param name="DestinationBucketName"></param>
        /// <param name="ExpectedResult"></param>
        /// <param name="IsRun"></param>
        [Test, TestCaseSource("GetTestDataServer")]
        public void FileTransferServerTest(string TaskName, string ActionType, string BackupRestoreType, string BackupRestoreLoaction, string SourceServerDetails, string DestinationDetails, string SourceLocation, string DestinationBucketName, string ExpectedResult, string IsRun)
        {

            // Check for run
            if (bool.Parse(IsRun))
            {
                // Arrange
                string[] args = new string[]
                {
                    TaskName,
                    ActionType,
                    BackupRestoreType,
                    BackupRestoreLoaction,
                    SourceServerDetails,
                    SourceLocation,
                    DestinationDetails,
                    DestinationBucketName
                };
                HRESULT Expected = (HRESULT)Enum.Parse(typeof(HRESULT), ExpectedResult);
                // Act
                FileTransfer testobj = new FileTransfer();
                // Assert
                HRESULT actualResult = testobj.LoadActionArguments(args);
                Assert.AreEqual(actualResult, Expected);
            }
        }

        /// <summary>
        /// Test method for testing network to blackpearl backup
        /// </summary>
        /// <param name="TaskName"></param>
        /// <param name="ActionType"></param>
        /// <param name="BackupRestoreType"></param>
        /// <param name="BackupRestoreLoaction"></param>
        /// <param name="SourceServerDetails"></param>
        /// <param name="DestinationDetails"></param>
        /// <param name="SourceLocation"></param>
        /// <param name="DestinationBucketName"></param>
        /// <param name="ExpectedResult"></param>
        /// <param name="IsRun"></param>
        [Test, TestCaseSource("GetTestDataNetwork")]
        public void FileTransferNetworkTest(string TaskName, string ActionType, string BackupRestoreType, string BackupRestoreLoaction, string SourceServerDetails, string DestinationDetails, string SourceLocation, string DestinationBucketName, string ExpectedResult, string IsRun)
        {

            // Check for run
            if (bool.Parse(IsRun))
            {
                // Arrange
                string[] args = new string[]
                {
                    TaskName,
                    ActionType,
                    BackupRestoreType,
                    BackupRestoreLoaction,
                    SourceServerDetails,
                    SourceLocation,
                    DestinationDetails,
                    DestinationBucketName
                };
                HRESULT Expected = (HRESULT)Enum.Parse(typeof(HRESULT), ExpectedResult);
                // Act
                FileTransfer testobj = new FileTransfer();
                // Assert
                HRESULT actualResult = testobj.LoadActionArguments(args);
                Assert.AreEqual(actualResult, Expected);
            }
        }
    }
}
