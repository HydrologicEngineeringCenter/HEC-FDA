using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace HEC.FDA.Model.LifeLoss.Saving;
public abstract class SQLiteSaverBase<T> : ISQLiteSaver<T>
{
    protected readonly string _connectionString;
    protected readonly SQLiteConnection _connection;

    protected SQLiteSaverBase(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}" ?? throw new ArgumentNullException(nameof(dbPath));

        _connection = new SQLiteConnection(_connectionString);
        _connection.Open();
    }

    public abstract void SaveToSQLite(T item);
    public abstract List<T> ReadFromSQLite(SQLiteFilter filter, bool selectAll = false);

    public void Dispose() => _connection.Dispose();
}