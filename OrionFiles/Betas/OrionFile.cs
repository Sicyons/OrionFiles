using System;
using System.IO;
using OrionCore.ErrorManagement;

namespace OrionFiles
{
    public abstract class OrionFile : IOrionErrorLogManager
    {
        #region Properties
        public String FilePath { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a <see cref="OrionFile"/> object with a specified file path.
        /// </summary>
        /// <param name="filePath">Path of the file. If a single file name is provided, this file name is appended to calling assembly location directory.</param>
        /// <exception cref="OrionException">The <b>filePath</b> parameter is missing or unreachable. The exception <b>Data</b> directory contains a <i>FilePath</i> entry with the file path.</exception>
        protected OrionFile(String filePath)
        {
            this.FilePath = OrionFile.ValidPath(filePath);
        }// OrionFile()
        #endregion

        #region Parent method implementations
        public abstract Boolean LogError(StructOrionErrorLogInfos errorLog);
        #endregion

        #region Protected interface
        protected static String ValidPath(String filePath)
        {
            String strDirectoryPath, strFileName;

            if (String.IsNullOrWhiteSpace(filePath) == false)
            {
                try
                {
                    //** A single file name or a relative path has been provided. It is completed with calling assembly file path. **
                    if (Path.IsPathRooted(filePath) == false)
                    {
                        strDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
                        strFileName = filePath;
                    }
                    else
                    {
                        strDirectoryPath = Path.GetDirectoryName(filePath);
                        strFileName = Path.GetFileName(filePath);
                    }

                    if (strFileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1) throw new OrionException("File name is not valid.", "FileName=" + strFileName);
                    if (Directory.Exists(strDirectoryPath) == false) throw new OrionException("Directory path has not been found.", "FilePath=" + strDirectoryPath);

                    return Path.Combine(strDirectoryPath, strFileName);
                }
                catch (ArgumentException ex)
                {
                    throw new OrionException("Directory path is not valid.", ex, "FilePath=" + filePath);
                }
                catch (PathTooLongException ex)
                {
                    throw new OrionException("File path is too long.", ex, "FilePath=" + filePath);
                }
            }
            else
                throw new OrionException("File path can't be null.");
        }// ValidPath()
        #endregion
    }
}
