using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrionCore.ErrorManagement;
using OrionFiles;

namespace OrionFilesTests
{
    [TestClass]
    public class OrionFilesTests
    {
        #region Fields
        private static String strTestDirectory, strTestFilePath;// strApplicationFileName, , strTestFilename, ;
        private static OrionHistoryFile xOrionHistoryFile;
        #endregion

        #region Initializations
        [TestInitialize]
        public void InitializeBeforeTests()
        {
            OrionFilesTests.strTestDirectory = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            OrionFilesTests.strTestFilePath = Path.Combine(OrionFilesTests.strTestDirectory, "Errors.log");

            File.Delete(OrionFilesTests.strTestFilePath);

            OrionFilesTests.xOrionHistoryFile = new OrionHistoryFile(OrionFilesTests.strTestFilePath);
        }// InitializeBeforeTests()
        #endregion

        #region Test methods
        #region OrionHistoryFile creation tests
        [TestCategory("OrionHistoryFile")]
        [TestMethod]
        public void CreateOrionHistoryFile_FileName_FilePath()
        {
            OrionHistoryFile xOrionHistoryFile;

            xOrionHistoryFile = new OrionHistoryFile("Errors.Log");

            Assert.AreEqual(xOrionHistoryFile.FilePath, Path.Combine(OrionFilesTests.strTestDirectory, "Errors.Log"));
        }// CreateOrionHistoryFile_FileName_FilePath()
        [TestCategory("OrionHistoryFile")]
        [TestMethod]
        public void CreateOrionHistoryFile_AbsolutePath_FilePath()
        {
            String strPath;
            OrionHistoryFile xOrionHistoryFile;

            strPath = Path.Combine(OrionFilesTests.strTestFilePath);

            xOrionHistoryFile = new OrionHistoryFile(strPath);

            Assert.AreEqual(xOrionHistoryFile.FilePath, strPath);
        }// CreateOrionHistoryFile_AbsolutePath_FilePath()
        [TestCategory("OrionHistoryFile")]
        [TestMethod]
        public void CreateOrionHistoryFile_DirectoryPathNotFound_DirectoryNotFoundException()
        {
            OrionHistoryFile xOrionHistoryFile;
            Exception xException;

            xException = null;

            try
            {
                xOrionHistoryFile = new OrionHistoryFile(Path.Combine("Z:\\", "Temp", "Errors.Log"));
            }
            catch (Exception ex)
            {
                xException = ex;
            }

            Assert.IsInstanceOfType(xException, typeof(OrionException));
            Assert.AreEqual(xException.Message, "Directory path has not been found.");
        }// CreateOrionHistoryFile_DirectoryPathNotFound_DirectoryNotFoundException()
        [TestCategory("OrionHistoryFile")]
        [TestMethod]
        public void CreateXHistoryFile_NullPath_XException()
        {
            OrionHistoryFile xOrionHistoryFile;
            Exception xException;

            xException = null;

            try
            {
                xOrionHistoryFile = new OrionHistoryFile(null);
            }
            catch (Exception ex)
            {
                xException = ex;
            }

            Assert.IsInstanceOfType(xException, typeof(OrionException));
            Assert.AreEqual(xException.Message, "File path can't be null.");
        }// CreateOrionHistoryFile_NullPath_XException()
        [TestCategory("OrionHistoryFile")]
        [TestMethod]
        public void CreateOrionHistoryFile_InvalidFileName_XException()
        {
            OrionHistoryFile xOrionHistoryFile;
            Exception xException;

            xException = null;

            try
            {
                xOrionHistoryFile = new OrionHistoryFile(":#yyyejdlz\\");
            }
            catch (Exception ex)
            {
                xException = ex;
            }

            Assert.IsInstanceOfType(xException, typeof(OrionException));
            Assert.AreEqual(xException.Message, "File name is not valid.");
        }// CreateOrionHistoryFile_InvalidPath_XExceptionArgumentException()
        [TestCategory("OrionHistoryFile")]
        [TestMethod]
        public void CreateOrionHistoryFile_InvalidDirectoryPath_XException()
        {
            OrionHistoryFile xOrionHistoryFile;
            Exception xException;

            xException = null;

            try
            {
                xOrionHistoryFile = new OrionHistoryFile("C:\\<TempErrors.log");
            }
            catch (Exception ex)
            {
                xException = ex;
            }

            Assert.IsInstanceOfType(xException, typeof(OrionException));
            Assert.IsInstanceOfType(xException.InnerException, typeof(ArgumentException));
            Assert.AreEqual(xException.Message, "Directory path is not valid.");
        }// CreateOrionHistoryFile_InvalidDirectoryPath_XException()
        [TestCategory("OrionHistoryFile")]
        [TestMethod]
        public void CreateXHistoryFile_PathTooLong_XExceptionPathTooLongException()
        {
            OrionHistoryFile xOrionHistoryFile;
            Exception xException;

            xException = null;

            try
            {
                xOrionHistoryFile = new OrionHistoryFile(Path.Combine("c:", "Temp", new String('A', 500)));
            }
            catch (Exception ex)
            {
                xException = ex;
            }

            Assert.IsInstanceOfType(xException, typeof(OrionException));
            Assert.IsInstanceOfType(xException.InnerException, typeof(PathTooLongException));
        }// CreateXHistoryFile_InvalidPath_XExceptionArgumentException()
        #endregion
        #endregion
    }
}