using System;
using System.IO;
using System.Security;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OrionCore.ErrorManagement;
using OrionCore.LogManagement;

namespace OrionFiles
{
    public class OrionHistoryFile : OrionFile
    {
        #region Fields
        private const Int32 iMARGIN = 25;
        #endregion

        #region Constructors
        public OrionHistoryFile(String filePath)
            : base(filePath)
        {
        }// OrionHistoryFile()
        #endregion

        #region Parent methods implementation
        public override Boolean SaveLog(OrionLogInfos log)
        {
            OrionException xException;
            StreamReader xStreamReader;
            StreamWriter xStreamWriter;
            List<String> strContentLines;
            Collection<String> strNewLines;

            xException = null;
            strContentLines = null;
            xStreamReader = null;
            xStreamWriter = null;

            try
            {
                if (File.Exists(this.FilePath) == true)
                {
                    strContentLines = new List<String>();

                    xStreamReader = File.OpenText(this.FilePath);
                    xStreamReader.ReadLine();
                    xStreamReader.ReadLine();
                    while (xStreamReader.EndOfStream == false)
                        strContentLines.Add(xStreamReader.ReadLine());

                    xStreamReader.Close();
                    xStreamReader = null;
                }
                
                xStreamWriter = File.CreateText(this.FilePath);

                xStreamWriter.WriteLine("********** " + log.EventType.ToString().ToUpperInvariant() + " **********");
                xStreamWriter.WriteLine("Application source : " + log.SourceApplicationName);

                xStreamWriter.WriteLine();
                xStreamWriter.WriteLine(new String('-', 70));
                xStreamWriter.WriteLine((log.LogDate.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture) + ": ").PadLeft(OrionHistoryFile.iMARGIN) + log.LogMessage);

                if (log.SourceException != null)
                {
                    strNewLines = this.ParseException(0, log.SourceException, false);
                    for (Int32 iLineCounter = 0; iLineCounter < strNewLines.Count; iLineCounter++)
                        xStreamWriter.WriteLine(strNewLines[iLineCounter]);
                }

                //** Append previous content to new file **
                if (strContentLines != null)
                    for (Int32 iLineCounter = 0; iLineCounter < strContentLines.Count; iLineCounter++)
                        xStreamWriter.WriteLine(strContentLines[iLineCounter]);

                xStreamWriter.Flush();
                xStreamWriter.Close();
                xStreamWriter = null;
            }
            catch (System.Text.EncoderFallbackException ex)
            {
                xException = new OrionException("An encoding error occured.", ex, "FilePath=" + this.FilePath);
            }
            catch (DirectoryNotFoundException ex)
            {
                xException = new OrionException("Directory path is not found.", ex, "FilePath=" + this.FilePath);
            }
            catch (FileNotFoundException ex)
            {
                xException = new OrionException("File path is not found.", ex, "FilePath=" + this.FilePath);
            }
            catch (IOException ex)
            {
                xException = new OrionException("An I/O error occured.", ex, "FilePath=" + this.FilePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                xException = new OrionException("The File is readonly, or the calling has no authorization to perform this operation.", ex, "FilePath=" + this.FilePath);
            }
            catch (SecurityException ex)
            {
                xException = new OrionException("The calling has no authorization to perform this operation.", ex, "FilePath=" + this.FilePath);
            }
            finally
            {
                if (xStreamReader != null) xStreamReader.Close();
                if (xStreamWriter != null)
                {
                    xStreamWriter.Flush();
                    xStreamWriter.Close();
                }
            }

            if (xException != null) throw xException;

            return true;
        }// SaveLog()
        #endregion

        #region Private procedures
        private Collection<String> ParseException(Int32 indent, Exception sourceException, Boolean innerException)
        {
            Int32 iDataCounter;
            String strIndent, strKeyTemp;
            Collection<String> strExceptionLines, strStackTraceLines;

            strIndent = indent > 0 ? new String(' ', indent) : null;
            strStackTraceLines = null;

            strExceptionLines = new Collection<String>();
            strExceptionLines.Add(strIndent + (innerException == false ? "Exception: ".PadLeft(OrionHistoryFile.iMARGIN) : "Inner exception: ".PadLeft(OrionHistoryFile.iMARGIN)) + sourceException.GetType().Name);
            strExceptionLines.Add(strIndent + "Message: ".PadLeft(OrionHistoryFile.iMARGIN + 17) + (sourceException.Message != null ? sourceException.Message : "-"));
            strExceptionLines.Add(strIndent + "Source: ".PadLeft(OrionHistoryFile.iMARGIN + 17) + (sourceException.Source != null ? sourceException.Source : "-"));
            strExceptionLines.Add(strIndent + "Method: ".PadLeft(OrionHistoryFile.iMARGIN + 17) + (sourceException.TargetSite != null ? sourceException.TargetSite.DeclaringType.Name + "." + sourceException.TargetSite.Name : "-"));
            if (sourceException.Data != null && sourceException.Data.Count > 0)
            {
                iDataCounter = 0;
                foreach (Object objKeyTemp in sourceException.Data.Keys)
                {
                    strKeyTemp = objKeyTemp.ToString();

                    if (iDataCounter == 0)
                        strExceptionLines.Add(strIndent + "Data: ".PadLeft(OrionHistoryFile.iMARGIN + 17) + strKeyTemp + "=" + sourceException.Data[strKeyTemp]);
                    else
                        strExceptionLines.Add((strKeyTemp + "=").PadLeft(OrionHistoryFile.iMARGIN + 17 + 6) + sourceException.Data[strKeyTemp]);
                    iDataCounter++;
                }
            }

            if (Environment.StackTrace != null)
            {
                //strStackTraceLines = OrionLogManager.ParseStackTrace();
                if (strStackTraceLines.Count > 0)
                {
                    strExceptionLines.Add(strIndent + "Stack Trace: ".PadLeft(OrionHistoryFile.iMARGIN + 17) + strStackTraceLines[0]);
                    for (Int32 iLineCounter = 1; iLineCounter < strStackTraceLines.Count; iLineCounter++)
                        strExceptionLines.Add(strStackTraceLines[iLineCounter].PadLeft(indent + OrionHistoryFile.iMARGIN + 17 + strStackTraceLines[iLineCounter].Length));
                }
            }

            if (strStackTraceLines == null) strExceptionLines.Add(strIndent + "Stack Trace: ".PadLeft(OrionHistoryFile.iMARGIN + 17) + "-");

            //** Inner exception. **
            if (sourceException.InnerException != null)
                foreach (String strNewLineTemp in this.ParseException(17, sourceException.InnerException, true))
                    strExceptionLines.Add(strNewLineTemp);
            else
                strExceptionLines.Add(strIndent + "Inner exception: ".PadLeft(OrionHistoryFile.iMARGIN + 17) + "-");

            return strExceptionLines;
        }// ParseException()
        #endregion
    }
}
