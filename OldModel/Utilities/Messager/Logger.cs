using System.Collections.Generic;
using System.Collections.Concurrent;
using FdaModel.Utilities.Attributes;
using System.Linq;
using System;

namespace FdaModel.Utilities.Messager
{
    public delegate void RequestFlushLogFileHandler(object sender, EventArgs e);
    /// <summary>
    /// This is a static class that should be used to report error messages.  Error messages consist of a message and a log level.
    /// The entry point for logging a message is "ReportMessage", to ensure that all error messages have written to disk, the flush method must be called before closing the application.
    /// </summary>
    [Author("Q0HECWPL", "6 / 14 / 2016 9:17:04 AM")]

    public sealed class Logger
    {
        #region Notes
        // Created By: Q0HECWPL
        // Created Date: 6/14/2016 9:17:04 AM
        /* Would like to add the ability to display message boxes if the error comes from the View or view model.  However, it would require an added reference that is otherwize unessary to the model.
            It is possible I can create similar functionality by raising an event that is caught via the view.         
             */
        #endregion
        #region Fields
        //private static volatile Logger instance;
        private static object _WriteLock = new object();
        //private static DataBase_Reader.SqLiteReader _Logfile;
        private static ConcurrentQueue<ErrorMessage> _queuedMessages; // Queue the messages to be written
        #endregion
        #region Properties
        public static readonly Logger Instance = new Logger();
        public event RequestFlushLogFileHandler RequestFlushLogFile;
        #endregion
        #region Constructors
        private Logger()
        {
            //if (_Logfile == null || _Logfile == "")
            //{
            //    string assmb = System.Reflection.Assembly.GetAssembly(typeof(Logger)).GetName().Name;
            //    _Logfile = System.IO.Path.GetTempPath() + "HEC\\" + assmb + "_LogFile.txt";
            //}
            _queuedMessages = new System.Collections.Concurrent.ConcurrentQueue<ErrorMessage>();
        }
        #endregion
        #region Voids
        /// <summary>
        /// adds a error to the message queue
        /// </summary>
        /// <param name="error">the ErrorMessage you wish to add.</param>
        public void ReportMessage(ErrorMessage error)
        {
            _queuedMessages.Enqueue(error);
            if (_queuedMessages.Count > 100) { Flush(); }
        }
        private void Flush()
        {
            RequestFlushLogFile?.Invoke(this, null);
        }
        /// <summary>
        /// This flushes the message queue.
        /// </summary>
        public void Flush(DatabaseManager.SQLiteManager reader)
        {
            WriteAllMessagesToLogFile(reader);
        }
        private void WriteAllMessagesToLogFile(DatabaseManager.SQLiteManager reader)
        {
            List<ErrorMessage> messages = new List<ErrorMessage>();
            ErrorMessage error;
            if (_queuedMessages.Count == 0) return;
            lock (_WriteLock)
            {
                do
                {
                    if (_queuedMessages.TryDequeue(out error))
                    {
                        messages.Add(error);
                    }
                } while (_queuedMessages.Count > 0);
                //System.IO.FileStream fs;
                //if (!System.IO.File.Exists(_Logfile))
                //{
                //    if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(_Logfile))) System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_Logfile));
                //    fs = new System.IO.FileStream(_Logfile, System.IO.FileMode.Create);
                //}
                //else
                //{
                //    fs = new System.IO.FileStream(_Logfile, System.IO.FileMode.Append);
                //}
                //using (System.IO.StreamWriter sr = new System.IO.StreamWriter(fs))
                //{
                if (reader != null)
                {
                    if (!reader.TableNames.Contains("Messages"))
                    {
                        reader.CreateTable("Messages", new string[] { "Message", "Error Level", "User", "Date", "Source" }, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) });
                    }
                    DatabaseManager.DataTableView dtv = reader.GetTableManager("Messages");
                    foreach (ErrorMessage err in messages)
                    {
                        dtv.AddRow(new object[] { err.Message, err.ErrorLevel, err.User, err.Date, err.ReportedFrom });
                        //switch (err.ErrorLevel)
                        //{
                        //    case ErrorMessageEnum.Report:
                        //        WriteMessageToLogTable(err, reader);
                        //        break;
                        //    case ErrorMessageEnum.Minor:
                        //        WriteMessageToLogTable(err, reader);
                        //        break;
                        //    case ErrorMessageEnum.Major:
                        //        WriteMessageToLogTable(err, reader);
                        //        break;
                        //    case ErrorMessageEnum.Fatal:
                        //        WriteMessageToLogTable(err, reader);
                        //        break;
                        //    default:
                        //        WriteMessageToLogTable(err, reader);
                        //        break;
                        //}
                    }
                    dtv.ApplyEdits();
                }


                //}
            }
        }
        //private void WriteMessageToLogFile(ErrorMessage message, System.IO.StreamWriter sr)
        //{
        //    if (!(message.ErrorLevel == ErrorMessageEnum.Report))
        //    {
        //        sr.WriteLine("[" + message.ErrorLevel.ToString() + "]: " + message.Message);
        //    }
        //    else
        //    {
        //        sr.WriteLine(message.Message);
        //    }
        //}
        //private void WriteMessageToLogTable(ErrorMessage message, DataBase_Reader.SqLiteReader reader)
        //{
        //    if (reader != null)
        //    {
        //        if (!reader.TableNames.Contains("Messages"))
        //        {
        //            reader.CreateTable("Messages", new string[] { "Message", "Error Level", "User", "Date", "Source" }, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) });
        //        }
        //        DataBase_Reader.DataTableView dtv = reader.GetTableManager("Messages");
        //        dtv.AddRow(new object[] { message.Message, message.ErrorLevel, Environment.UserName, DateTime.Now, message.ReportedBy });
        //        dtv.ApplyEdits();
        //    }

        //}
        #endregion
        #region Functions
        #endregion
    }
}
