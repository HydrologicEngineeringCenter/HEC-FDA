using System;
using System.Collections.Generic;

namespace HEC.FDA.Model.SQLite;

public interface ISQLiteSaver<T> : IDisposable
{
    public void SaveToSQLite(T item);

    public List<T> ReadFromSQLite(SQLiteFilter filter);

    public void DeleteFromSQLite(SQLiteFilter filter);
}