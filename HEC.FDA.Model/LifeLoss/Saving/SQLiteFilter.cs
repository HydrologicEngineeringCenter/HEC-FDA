using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HEC.FDA.Model.LifeLoss.Saving;
/// <summary>
/// Abstract class for providing a filter to a SQLite command
/// </summary>
public abstract class SQLiteFilter
{
    /// <summary>
    /// Builds a SELECT * query on <paramref name="tableName"/> that restricts each
    /// column to the values contained in the corresponding array property.
    /// Returns the SQL text and a dictionary ready to be turned into parameters.
    /// </summary>
    public StringBuilder BuildSelect(string tableName, out IReadOnlyDictionary<string, object> parameters)
    {
        var sql = new StringBuilder($"SELECT * FROM \"{tableName}\"");
        var dict = new Dictionary<string, object>();

        bool firstClause = true;
        int paramIndex = 0;

        foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.PropertyType.IsArray) continue; // we only want array properties
            var array = (Array?)prop.GetValue(this);
            if (array is null || array.Length == 0) continue;

            string colName = prop.Name;

            // final return is formatted "WHERE col1 IN (@p1) AND col2 in (@p2, @p3) AND col3..."
            if (firstClause)
            {
                sql.Append(" WHERE ");
                firstClause = false;
            }
            else
            {
                sql.Append(" AND ");
            }

            // build "col IN (@p0,@p1,…)"
            var placeholders = new List<string>();
            foreach (object? value in array)
            {
                string p = $"@p{paramIndex++}";
                placeholders.Add(p);
                dict[p] = value ?? DBNull.Value;
            }
            sql.Append('"').Append(colName).Append("\" IN (").Append(string.Join(',', placeholders)).Append(')');
        }
        parameters = dict;
        return sql;
    }

    public StringBuilder BuildDelete(string tableName, out IReadOnlyDictionary<string, object> parameters)
    {
        var sql = new StringBuilder($"DELETE FROM \"{tableName}\"");
        var dict = new Dictionary<string, object>();

        AppendClauses(sql, dict);

        parameters = dict;
        return sql;
    }

    private void AppendClauses(StringBuilder sql, Dictionary<string, object> dict)
    {
        bool firstClause = true;
        int paramIndex = 0;

        foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.PropertyType.IsArray) continue; // we only want array properties
            var array = (Array?)prop.GetValue(this);
            if (array is null || array.Length == 0) continue;

            string colName = prop.Name;

            // final return is formatted "WHERE col1 IN (@p1) AND col2 in (@p2, @p3) AND col3..."
            if (firstClause)
            {
                sql.Append(" WHERE ");
                firstClause = false;
            }
            else
            {
                sql.Append(" AND ");
            }

            // build "col IN (@p0,@p1,…)"
            var placeholders = new List<string>();
            foreach (object? value in array)
            {
                string p = $"@p{paramIndex++}";
                placeholders.Add(p);
                dict[p] = value ?? DBNull.Value;
            }
            sql.Append('"').Append(colName).Append("\" IN (").Append(string.Join(',', placeholders)).Append(')');
        }
    }
}