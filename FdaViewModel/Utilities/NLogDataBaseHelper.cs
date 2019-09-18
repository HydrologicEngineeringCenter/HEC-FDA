using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    /// <summary>
    /// This class is used to attach a "target" to the NLog config so that logs can be stored in the projects
    /// SqLite database.
    /// </summary>
    static class NLogDataBaseHelper
    {
       
        private const string FATAL_LOG_TABLE_NAME = "Log_Fatal";
        private const string ERROR_LOG_TABLE_NAME = "Log_Error";
        private const string WARN_LOG_TABLE_NAME = "Log_Warn";
        private const string INFO_LOG_TABLE_NAME = "Log_Info";


        private const string TIMESTAMP_COL = "Timestamp";
        private const string USER_COL = "User";
        private const string LOGLEVEL_COL = "Loglevel";
        private const string LOGGER_COL = "Logger";
        private const string CALLSITE_COL = "CallSite";
        private const string MESSAGE_COL = "Message";


        private static void MakeSureLogTableExists(string tableName)
        {
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
            if (tbl == null)
            {
                string[] tableColumnNames = new string[] { TIMESTAMP_COL, USER_COL, LOGLEVEL_COL, LOGGER_COL, CALLSITE_COL, MESSAGE_COL };
                Type[] tableColumnTypes = new Type[] { typeof(String),typeof(String), typeof(String), typeof(String), typeof(String), typeof(String) };
                Storage.Connection.Instance.CreateTable(tableName, tableColumnNames, tableColumnTypes);
            }
        }

        /// <summary>
        /// Creates a "target" and adds it to the NLog config so that logs gets stored in the "Log" table in the project's SqLite.
        /// </summary>
        /// <param name="sqliteDBPath"></param>
        public static void CreateDBTargets(String sqliteDBPath)
        {
            var config = LogManager.Configuration;
            if(config == null)
            {
                //this happens when unit testing
                return;
            }
            //set up fatal target
            DatabaseTarget fatalDBTarget = new DatabaseTarget(FATAL_LOG_TABLE_NAME);
            SetUpTarget(fatalDBTarget, sqliteDBPath, FATAL_LOG_TABLE_NAME);
            config.AddTarget(fatalDBTarget);
            var dbRule = new LoggingRule("*", LogLevel.Fatal, LogLevel.Fatal, fatalDBTarget);
            config.LoggingRules.Add(dbRule);

            //set up error target
            DatabaseTarget errorDBTarget = new DatabaseTarget(ERROR_LOG_TABLE_NAME);
            SetUpTarget(errorDBTarget, sqliteDBPath, ERROR_LOG_TABLE_NAME);
            config.AddTarget(errorDBTarget);
            dbRule = new LoggingRule("*", LogLevel.Error, LogLevel.Error, errorDBTarget);
            config.LoggingRules.Add(dbRule);

            //set up Warn target
            DatabaseTarget warnDBTarget = new DatabaseTarget(WARN_LOG_TABLE_NAME);
            SetUpTarget(warnDBTarget, sqliteDBPath, WARN_LOG_TABLE_NAME);
            config.AddTarget(warnDBTarget);
            dbRule = new LoggingRule("*", LogLevel.Warn, LogLevel.Warn, warnDBTarget);
            config.LoggingRules.Add(dbRule);

            //set up Info target
            DatabaseTarget infoDBTarget = new DatabaseTarget(INFO_LOG_TABLE_NAME);
            SetUpTarget(infoDBTarget, sqliteDBPath, INFO_LOG_TABLE_NAME);
            config.AddTarget(infoDBTarget);
            dbRule = new LoggingRule("*", LogLevel.Info, LogLevel.Info, infoDBTarget);
            config.LoggingRules.Add(dbRule);


            LogManager.Configuration = config;
        }

        private static void SetUpTarget(DatabaseTarget target, string sqliteDBPath, string tableName)
        {
            MakeSureLogTableExists(tableName);

            target.DBProvider = "System.Data.SQLite";
            target.KeepConnection = false;
            target.ConnectionString = "Data Source=" + sqliteDBPath + "; Version=3";
            target.CommandText = "INSERT into " + tableName + "(" + TIMESTAMP_COL +"," + USER_COL + "," + 
                LOGLEVEL_COL + "," + LOGGER_COL + ","+CALLSITE_COL + "," + MESSAGE_COL+ 
                ") values(@Timestamp, @User, @Loglevel, @Logger, @Callsite, @Message)";


            DatabaseParameterInfo param;

            param = new DatabaseParameterInfo();
            param.Name = "@Timestamp";
            param.Layout = "${longdate}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@User";
            param.Layout = "${environment-user}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@Loglevel";
            param.Layout = "${level:uppercase=true}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@Logger";
            param.Layout = "${logger}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@Callsite";
            param.Layout = "${callsite:filename=true}";
            target.Parameters.Add(param);

            param = new DatabaseParameterInfo();
            param.Name = "@Message";
            param.Layout = "${message}";
            target.Parameters.Add(param);


            MemoryTarget memTarget = new MemoryTarget("CurveEditorVM");
            memTarget.Layout = "${message}";
            SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Fatal);

        }

        //it would be pretty easy to get logs based on level and user, level and class, date etc
        public static List<Object[]> GetLogs(LogLevel level)
        {

            string tableName = GetTableName(level);

            List<String> logs = new List<string>();
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }

            DatabaseManager.DataTableView dtv = Storage.Connection.Instance.GetTable(tableName);
            if (dtv != null)
            {
                return dtv.GetRows(0, dtv.NumberOfRows - 1);
            }
            else
            {
                return new List<object[]>();
            }
        }

        private static string GetTableName(LogLevel level)
        {
            string tableName = "";
            if (level == LogLevel.Fatal)
            {
                tableName = FATAL_LOG_TABLE_NAME;
            }
            else if (level == LogLevel.Error)
            {
                tableName = ERROR_LOG_TABLE_NAME;
            }
            else if (level == LogLevel.Warn)
            {
                tableName = WARN_LOG_TABLE_NAME;
            }
            else if (level == LogLevel.Info)
            {
                tableName = INFO_LOG_TABLE_NAME;
            }
            return tableName;
        }

        //private static string GetLogString(Object[] row)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach(Object obj in row)
        //    {
        //        sb.Append(obj.ToString());
        //    }
        //    return sb.ToString();
        //}
        public static List<MessageRowItem> GetMessageRows(LogLevel level)
        {
            List<MessageRowItem> messages = new List<MessageRowItem>();
            List<Object[]> logs = GetLogs(level);
            foreach(Object[] log in logs)
            {
                MessageRowItem row = new MessageRowItem(log[0].ToString(), log[5].ToString(), log[1].ToString());
                messages.Add(row);
            }
            return messages;
        }
    }
}
