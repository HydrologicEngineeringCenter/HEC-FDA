using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace HEC.FDA.ViewModel;

// code adopted from DBManager, stripped down to fit our needs
public class SQLiteManager
{
    private string _dataBasePath;
    private string[] _tableNames;
    private bool _dataBaseOpen;
    private SQLiteConnection _dbConnection;

    public string DataBasePath => _dataBasePath;

    public string[] TableNames => _tableNames; 
    public bool DataBaseOpen => _dataBaseOpen;

    public SQLiteConnection DbConnection => _dbConnection;

    public static SQLiteConnectionStringBuilder DefaultConnectionBuilder
    {
        get
        {
            SQLiteConnectionStringBuilder sQLiteConnectionStringBuilder = new SQLiteConnectionStringBuilder();
            sQLiteConnectionStringBuilder.Version = 3;
            sQLiteConnectionStringBuilder.DefaultTimeout = 5000;
            sQLiteConnectionStringBuilder.BusyTimeout = 5000;
            sQLiteConnectionStringBuilder.PageSize = 65536;
            sQLiteConnectionStringBuilder.CacheSize = 16777216;
            sQLiteConnectionStringBuilder.SyncMode = SynchronizationModes.Off;
            sQLiteConnectionStringBuilder.JournalMode = SQLiteJournalModeEnum.Memory;
            sQLiteConnectionStringBuilder.FailIfMissing = false;
            sQLiteConnectionStringBuilder.ReadOnly = false;
            return sQLiteConnectionStringBuilder;
        }
    }

    public SQLiteManager(string dataBaseFile, SQLiteConnectionStringBuilder connectionBuilder = null)
    {
        _dataBasePath = dataBaseFile;
        SetDatabaseConnection(dataBaseFile, connectionBuilder);
        _tableNames = GetTableNames();
    }

    public void SetDatabaseConnection(string fileName, SQLiteConnectionStringBuilder connectionBuilder = null)
    {
        if (connectionBuilder == null)
        {
            connectionBuilder = DefaultConnectionBuilder;
        }
        connectionBuilder.DataSource = fileName;
        _dbConnection = new SQLiteConnection(connectionBuilder.ToString());
    }

    public void Open()
    {
        _dbConnection.Open();
        _dataBaseOpen = true;
    }

    public void Close()
    {
        _dbConnection.Close();
        SQLiteConnection.ClearAllPools();
        _dataBaseOpen = false;
    }

    public static void CreateSqLiteFile(string databaseFile)
    {
        SQLiteConnection.CreateFile(databaseFile);
    }

    public void RenameTable(string oldTableName, string newTableName)
    {
        bool dataBaseOpen = _dataBaseOpen;
        if (!_dataBaseOpen)
        {
            Open();
        }

        if (!_tableNames.Contains(oldTableName))
        {
            throw new Exception("Table '" + oldTableName + "' does not exist in the database.");
        }

        using (SQLiteCommand sQLiteCommand = new SQLiteCommand("ALTER TABLE [" + oldTableName + "] RENAME TO [" + newTableName + "]", _dbConnection))
        {
            sQLiteCommand.ExecuteNonQuery();
        }

        if (!dataBaseOpen)
        {
            Close();
        }

        _tableNames = GetTableNames();
    }

    public string[] GetTableNames()
    {
        bool dataBaseOpen = _dataBaseOpen;
        if (!_dataBaseOpen)
        {
            Open();
        }

        List<string> list = new List<string>();
        using (SQLiteCommand sQLiteCommand = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", _dbConnection))
        {
            using SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader();
            if (sQLiteDataReader.HasRows)
            {
                while (sQLiteDataReader.Read())
                {
                    list.Add(sQLiteDataReader[0].ToString());
                }
            }
        }

        if (!dataBaseOpen)
        {
            Close();
        }

        _tableNames = list.ToArray();
        return list.ToArray();
    }

    public void CreateTable(string newTableName, string[] tableColumnNames, Type[] newColumnTypes)
    {
        bool dataBaseOpen = _dataBaseOpen;
        if (!_dataBaseOpen)
        {
            Open();
        }

        StringBuilder stringBuilder = new StringBuilder(500);
        stringBuilder.Append("Create Table [").Append(newTableName).Append("] (");
        checked
        {
            int num = tableColumnNames.Count() - 1;
            for (int i = 0; i <= num; i++)
            {
                stringBuilder.Append("[").Append(tableColumnNames[i]).Append("] ");
                Type type = newColumnTypes[i];
                if (type == typeof(string))
                {
                    stringBuilder.Append("TEXT,");
                    continue;
                }

                if (type == typeof(DateTime))
                {
                    stringBuilder.Append("DATETIME,");
                    continue;
                }

                if (type == typeof(byte) || type == typeof(sbyte))
                {
                    stringBuilder.Append("INT1,");
                    continue;
                }

                if (type == typeof(short) || type == typeof(ushort))
                {
                    stringBuilder.Append("INT2,");
                    continue;
                }

                if (type == typeof(int) || type == typeof(uint))
                {
                    stringBuilder.Append("INT4,");
                    continue;
                }

                if (type == typeof(long) || type == typeof(ulong))
                {
                    stringBuilder.Append("INT8,");
                    continue;
                }

                if (type == typeof(float))
                {
                    stringBuilder.Append("FLOAT,");
                    continue;
                }

                if (type == typeof(double))
                {
                    stringBuilder.Append("DOUBLE,");
                    continue;
                }

                if (type == typeof(decimal))
                {
                    stringBuilder.Append("NUMBER,");
                    continue;
                }

                if (type == typeof(char))
                {
                    stringBuilder.Append("CHAR,");
                    continue;
                }

                if (type == typeof(bool))
                {
                    stringBuilder.Append("BOOLEAN,");
                    continue;
                }

                if (type == typeof(object) || type == typeof(byte[]))
                {
                    stringBuilder.Append("BLOB,");
                    continue;
                }

                throw new Exception(newColumnTypes[i].ToString() + " Not implemented, Column: " + tableColumnNames[i]);
            }

            if (tableColumnNames.Count() > 0)
            {
                stringBuilder[stringBuilder.Length - 1] = ')';
            }
            else
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            using (SQLiteCommand sQLiteCommand = _dbConnection.CreateCommand())
            {
                sQLiteCommand.CommandText = stringBuilder.ToString();
                sQLiteCommand.ExecuteNonQuery();
            }

            _tableNames = GetTableNames();
            if (!dataBaseOpen)
            {
                Close();
            }
        }
    }

    public void DeleteTable(string tableName)
    {
        if (_tableNames.Contains(tableName))
        {
            bool dataBaseOpen = _dataBaseOpen;
            if (!_dataBaseOpen)
            {
                Open();
            }

            using (SQLiteCommand sQLiteCommand = _dbConnection.CreateCommand())
            {
                sQLiteCommand.CommandText = "DROP TABLE [" + tableName + "]";
                sQLiteCommand.ExecuteNonQuery();
            }

            _tableNames = GetTableNames();
            Close();
            if (dataBaseOpen)
            {
                Open(); 
            }
        }
    }
}
