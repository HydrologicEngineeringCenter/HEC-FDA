//using NLog;
//using NLog.Config;
//using NLog.Targets;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FdaViewModel.Utilities
//{
//    /// <summary>
//    /// This class is used to attach a "target" to the NLog config so that logs can be stored in the projects
//    /// SqLite database.
//    /// </summary>
//    static class NLogDataBaseHelper
//    {
//        private const int DELETE_LOGS_OLDER_THAN_DAYS = 1;
//        private const int MAX_NUM_LOGS_IN_TABLE = 5;

//        private const string FATAL_LOG_TABLE_NAME = "Log_Fatal";
//        private const string ERROR_LOG_TABLE_NAME = "Log_Error";
//        private const string WARN_LOG_TABLE_NAME = "Log_Warn";
//        private const string INFO_LOG_TABLE_NAME = "Log_Info";


//        private const string TIMESTAMP_COL = "Timestamp";
//        private const int TIMESTAMP_COL_INDEX = 0;
//        private const string USER_COL = "User";
//        private const string LOGLEVEL_COL = "Loglevel";
//        private const string LOGGER_COL = "Logger";
//        private const string CALLSITE_COL = "CallSite";
//        private const string MESSAGE_COL = "Message";
//        private const string CLASS_COL = "Class";


//        //private static void MakeSureLogTableExists(string tableName)
//        //{
//        //    DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
//        //    if (tbl == null)
//        //    {
//        //        string[] tableColumnNames = new string[] { TIMESTAMP_COL, USER_COL, LOGLEVEL_COL, LOGGER_COL, CALLSITE_COL, MESSAGE_COL, CLASS_COL };
//        //        Type[] tableColumnTypes = new Type[] { typeof(String),typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String) };
//        //        Storage.Connection.Instance.CreateTable(tableName, tableColumnNames, tableColumnTypes);
//        //    }
//        //}

//        private static void MakeSureLogTableExists(string tableName)
//        {
//            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
//            if (tbl == null)
//            {
//                string[] tableColumnNames = new string[] { TIMESTAMP_COL, USER_COL, LOGLEVEL_COL, LOGGER_COL, CALLSITE_COL, MESSAGE_COL, "Element", "Name" };
//                Type[] tableColumnTypes = new Type[] { typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String), typeof(String) };
//                Storage.Connection.Instance.CreateTable(tableName, tableColumnNames, tableColumnTypes);
//            }
//        }

//        /// <summary>
//        /// Creates a "target" and adds it to the NLog config so that logs gets stored in the "Log" table in the project's SqLite.
//        /// </summary>
//        /// <param name="sqliteDBPath"></param>
//        public static void CreateDBTargets(String sqliteDBPath)
//        {
//            var config = LogManager.Configuration;
//            if(config == null)
//            {
//                //this happens when unit testing
//                return;
//            }
//            //set up fatal target
//            DatabaseTarget fatalDBTarget = new DatabaseTarget(FATAL_LOG_TABLE_NAME);
//            SetUpTarget(fatalDBTarget, sqliteDBPath, FATAL_LOG_TABLE_NAME);
//            config.AddTarget(fatalDBTarget);
//            var dbRule = new LoggingRule("*", LogLevel.Fatal, LogLevel.Fatal, fatalDBTarget);
//            config.LoggingRules.Add(dbRule);

//            //set up error target
//            DatabaseTarget errorDBTarget = new DatabaseTarget(ERROR_LOG_TABLE_NAME);
//            SetUpTarget(errorDBTarget, sqliteDBPath, ERROR_LOG_TABLE_NAME);
//            config.AddTarget(errorDBTarget);
//            dbRule = new LoggingRule("*", LogLevel.Error, LogLevel.Error, errorDBTarget);
//            config.LoggingRules.Add(dbRule);

//            //set up Warn target
//            DatabaseTarget warnDBTarget = new DatabaseTarget(WARN_LOG_TABLE_NAME);
//            SetUpTarget(warnDBTarget, sqliteDBPath, WARN_LOG_TABLE_NAME);
//            config.AddTarget(warnDBTarget);
//            dbRule = new LoggingRule("*", LogLevel.Warn, LogLevel.Warn, warnDBTarget);
//            config.LoggingRules.Add(dbRule);

//            //set up Info target
//            DatabaseTarget infoDBTarget = new DatabaseTarget(INFO_LOG_TABLE_NAME);
//            SetUpTarget(infoDBTarget, sqliteDBPath, INFO_LOG_TABLE_NAME);
//            config.AddTarget(infoDBTarget);
//            dbRule = new LoggingRule("*", LogLevel.Info, LogLevel.Info, infoDBTarget);
//            config.LoggingRules.Add(dbRule);

//            //set up Test target
//            //DatabaseTarget testDBTarget = new DatabaseTarget("testStructuredLoggingTable");
//            //SetUpTargetTest(testDBTarget, sqliteDBPath, "testStructuredLoggingTable");
//            //config.AddTarget(testDBTarget);
//            //dbRule = new LoggingRule("*", LogLevel.Fatal, LogLevel.Fatal, testDBTarget);
//            //config.LoggingRules.Add(dbRule);

//            LogManager.Configuration = config;
//        }

//        //private static void SetUpTarget(DatabaseTarget target, string sqliteDBPath, string tableName)
//        //{
//        //    MakeSureLogTableExists(tableName);

//        //    target.DBProvider = "System.Data.SQLite";
//        //    target.KeepConnection = false;
//        //    target.ConnectionString = "Data Source=" + sqliteDBPath + "; Version=3";
//        //    target.CommandText = "INSERT into " + tableName + "(" + TIMESTAMP_COL +"," + USER_COL + "," + 
//        //        LOGLEVEL_COL + "," + LOGGER_COL + ","+CALLSITE_COL + "," + MESSAGE_COL+ "," + CLASS_COL +
//        //        ") values(@Timestamp, @User, @Loglevel, @Logger, @Callsite, @Message, @Class)";


//        //    DatabaseParameterInfo param;

//        //    param = new DatabaseParameterInfo();
//        //    param.Name = "@Timestamp";
//        //    param.Layout = "${longdate}";
//        //    target.Parameters.Add(param);

//        //    param = new DatabaseParameterInfo();
//        //    param.Name = "@User";
//        //    param.Layout = "${environment-user}";
//        //    target.Parameters.Add(param);

//        //    param = new DatabaseParameterInfo();
//        //    param.Name = "@Loglevel";
//        //    param.Layout = "${level:uppercase=true}";
//        //    target.Parameters.Add(param);

//        //    param = new DatabaseParameterInfo();
//        //    param.Name = "@Logger";
//        //    param.Layout = "${logger}";
//        //    target.Parameters.Add(param);

//        //    param = new DatabaseParameterInfo();
//        //    param.Name = "@Callsite";
//        //    param.Layout = "${callsite:filename=true}";
//        //    target.Parameters.Add(param);

//        //    param = new DatabaseParameterInfo();
//        //    param.Name = "@Message";
//        //    param.Layout = "${message}";
//        //    target.Parameters.Add(param);

//        //    param = new DatabaseParameterInfo();
//        //    param.Name = "@Class";
//        //    param.Layout = "${event-properties:item=ID}";
//        //    target.Parameters.Add(param);

//        //    MemoryTarget memTarget = new MemoryTarget("CurveEditorVM");
//        //    memTarget.Layout = "${message}";
//        //    SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Fatal);

//        //}

//        private static void SetUpTarget(DatabaseTarget target, string sqliteDBPath, string tableName)
//        {
//            MakeSureLogTableExists(tableName);

//            target.DBProvider = "System.Data.SQLite";
//            target.KeepConnection = false;
//            target.ConnectionString = "Data Source=" + sqliteDBPath + "; Version=3";
//            target.CommandText = "INSERT into " + tableName + "(" + TIMESTAMP_COL + "," + USER_COL + "," +
//                LOGLEVEL_COL + "," + LOGGER_COL + "," + CALLSITE_COL + "," + MESSAGE_COL + "," + "Element" + "," + "Name" +
//                ") values(@Timestamp, @User, @Loglevel, @Logger, @Callsite, @Message, @Element, @Name)";


//            DatabaseParameterInfo param;

//            param = new DatabaseParameterInfo();
//            param.Name = "@Timestamp";
//            param.Layout = "${longdate}";
//            target.Parameters.Add(param);

//            param = new DatabaseParameterInfo();
//            param.Name = "@User";
//            param.Layout = "${environment-user}";
//            target.Parameters.Add(param);

//            param = new DatabaseParameterInfo();
//            param.Name = "@Loglevel";
//            param.Layout = "${level:uppercase=true}";
//            target.Parameters.Add(param);

//            param = new DatabaseParameterInfo();
//            param.Name = "@Logger";
//            param.Layout = "${logger}";
//            target.Parameters.Add(param);

//            param = new DatabaseParameterInfo();
//            param.Name = "@Callsite";
//            param.Layout = "${callsite:filename=true}";
//            target.Parameters.Add(param);

//            param = new DatabaseParameterInfo();
//            param.Name = "@Message";
//            param.Layout = "${message}";
//            target.Parameters.Add(param);

//            param = new DatabaseParameterInfo();
//            param.Name = "@Element";
//            param.Layout = "${event-properties:Element}";
//            target.Parameters.Add(param);

//            param = new DatabaseParameterInfo();
//            param.Name = "@Name";
//            param.Layout = "${event-properties:Name}";
//            target.Parameters.Add(param);

//        }

//        //it would be pretty easy to get logs based on level and user, level and class, date etc
//        public static List<Object[]> GetLogs(LogLevel level)
//        {

//            string tableName = GetTableName(level);

//            List<String> logs = new List<string>();
//            if (!Storage.Connection.Instance.IsOpen)
//            {
//                Storage.Connection.Instance.Open();
//            }

//            DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(tableName);
//            if (dtv != null)
//            {
//                return dtv.GetRows(0, dtv.NumberOfRows - 1);
//            }
//            else
//            {
//                return new List<object[]>();
//            }
//        }

//        private static string GetTableName(LogLevel level)
//        {
//            string tableName = "";
//            if (level == LogLevel.Fatal)
//            {
//                tableName = FATAL_LOG_TABLE_NAME;
//            }
//            else if (level == LogLevel.Error)
//            {
//                tableName = ERROR_LOG_TABLE_NAME;
//            }
//            else if (level == LogLevel.Warn)
//            {
//                tableName = WARN_LOG_TABLE_NAME;
//            }
//            else if (level == LogLevel.Info)
//            {
//                tableName = INFO_LOG_TABLE_NAME;
//            }
//            return tableName;
//        }

//        //private static string GetLogString(Object[] row)
//        //{
//        //    StringBuilder sb = new StringBuilder();
//        //    foreach(Object obj in row)
//        //    {
//        //        sb.Append(obj.ToString());
//        //    }
//        //    return sb.ToString();
//        //}
//        public static ObservableCollection<MessageRowItem> GetMessageRows(LogLevel level)
//        {
//            ObservableCollection<MessageRowItem> messages = new ObservableCollection<MessageRowItem>();
//            List<Object[]> logs = GetLogs(level);
//            //flip the rows around so that i am getting the most recent first
//            for (int i = logs.Count - 1; i >= 0; i--)
//            //foreach(Object[] log in logs)
//            {
//                Object[] log = logs[i];
//                MessageRowItem row = new MessageRowItem(log[0].ToString(), log[5].ToString(), log[1].ToString(), log[2].ToString(), log[3].ToString());
//                messages.Add(row);
//            }

//            return messages;
//        }

//        public static ObservableCollection<MessageRowItem> GetMessageRowsForType(Type elementType)
//        {
//            ObservableCollection<MessageRowItem> messages = new ObservableCollection<MessageRowItem>();
//            GetMessageRowsForType(elementType, LogLevel.Fatal).ToList().ForEach(messages.Add);
//            GetMessageRowsForType(elementType, LogLevel.Error).ToList().ForEach(messages.Add);
//            GetMessageRowsForType(elementType, LogLevel.Warn).ToList().ForEach(messages.Add);
//            GetMessageRowsForType(elementType, LogLevel.Info).ToList().ForEach(messages.Add);
//            return messages;
//        }


//        public static ObservableCollection<MessageRowItem> GetMessageRowsForType(Type elementType, LogLevel level)
//        {
//            ObservableCollection<MessageRowItem> messages = new ObservableCollection<MessageRowItem>();
//            List<Object[]> logs = GetLogs(level);
//            //flip the rows around so that i am getting the most recent first
//            for (int i = logs.Count - 1; i >= 0; i--)
//            //foreach(Object[] log in logs)
//            {
//                Object[] log = logs[i];
//                string typeString = log[6].ToString();
//                if (elementType.ToString().Equals(typeString))
//                {
//                    MessageRowItem row = new MessageRowItem(log[0].ToString(), log[5].ToString(), log[1].ToString(), log[2].ToString(), log[3].ToString());
//                    messages.Add(row);
//                }

//            }

//            return messages;
//        }

//        public static ObservableCollection<MessageRowItem> GetMessageRowsForType(Type elementType, string name)
//        {
//            ObservableCollection<MessageRowItem> messages = new ObservableCollection<MessageRowItem>();

//        }

//        public static ObservableCollection<MessageRowItem> GetMessageRowsForType(Type elementType, string name, )
//        {
//            ObservableCollection<MessageRowItem> messages = new ObservableCollection<MessageRowItem>();

//        }

//        /// <summary>
//        /// Deletes any logs from the table older than the defined cut off date. 
//        /// </summary>
//        public static void DeleteOldLogs()
//        {
//            DeleteOldLogs(FATAL_LOG_TABLE_NAME);
//            DeleteOldLogs(ERROR_LOG_TABLE_NAME);
//            DeleteOldLogs(INFO_LOG_TABLE_NAME);
//            DeleteOldLogs(WARN_LOG_TABLE_NAME);
//        }
//        private static void DeleteOldLogs(string tableName)
//        {
//            if(!Storage.Connection.Instance.IsOpen)
//            {
//                Storage.Connection.Instance.Open();
//            }
//            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
//            if (tbl != null)
//            {
//                List<int> rowsToDelete = new List<int>();
//                DateTime cutOffDate = DateTime.Now.AddDays(-1 * DELETE_LOGS_OLDER_THAN_DAYS);
//                //get the index of all the rows that need to be deleted
//                for (int i = 0; i < tbl.NumberOfRows; i++)
//                {
//                    Object[] row = tbl.GetRow(i);
//                    DateTime rowDate = Convert.ToDateTime(row[TIMESTAMP_COL_INDEX]);
//                    if (rowDate != null)
//                    {
//                        if (rowDate < cutOffDate)
//                        {
//                            rowsToDelete.Add(i);
//                        }
//                    }
//                }
//                //delete the rows
//                tbl.DeleteRows(rowsToDelete.ToArray());
//                tbl.ApplyEdits();
//            }
//        }

//        /// <summary>
//        /// Deletes logs over the max allowed number of logs from the table starting with the oldest logs.
//        /// </summary>
//        public static void DeleteLogsOverMaxNumber()
//        {
//            DeleteLogsOverMaxNumber(FATAL_LOG_TABLE_NAME);
//            DeleteLogsOverMaxNumber(ERROR_LOG_TABLE_NAME);
//            DeleteLogsOverMaxNumber(INFO_LOG_TABLE_NAME);
//            DeleteLogsOverMaxNumber(WARN_LOG_TABLE_NAME);
//        }

//        private static void DeleteLogsOverMaxNumber(string tableName)
//        {
//            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
//            if (tbl != null)
//            {
//                int numRows = tbl.NumberOfRows;
//                if (numRows > MAX_NUM_LOGS_IN_TABLE)
//                {
//                    int numRowsToDelete = numRows - MAX_NUM_LOGS_IN_TABLE;
//                    List<int> rowsToDelete = new List<int>();
//                    //delete starting at zero because these are the oldest.
//                    for (int i = 0; i < numRowsToDelete; i++)
//                    {
//                        rowsToDelete.Add(i);
//                    }
//                    //delete the rows
//                    tbl.DeleteRows(rowsToDelete.ToArray());
//                    tbl.ApplyEdits();
//                }
//            }
//        }

//    }
//}
