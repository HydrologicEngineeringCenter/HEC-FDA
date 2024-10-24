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
}
