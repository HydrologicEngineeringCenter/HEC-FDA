using System;

namespace HEC.FDA.Model.SQLite;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class SQLColumnAttribute : Attribute
{
    public string ColumnName { get; }

    public SQLColumnAttribute(string columnName)
    {
        ColumnName = columnName;
    }
}
