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
        using var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", _connection); // have to do this for every connection if using multiple, enforces foregin key rules
        pragma.ExecuteNonQuery();
    }

    public abstract void SaveToSQLite(T item);
    public abstract List<T> ReadFromSQLite(SQLiteFilter filter, bool selectAll = false);
    public abstract void DeleteFromSQLite(SQLiteFilter filter);

    public void Dispose() => _connection.Dispose();
}