using System.Reflection;
using System.Text;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnAttribute : Attribute
{
    public ColumnAttribute(string name) => Name = name;
    public string Name { get; }
}

public abstract class SQLiteFilter
{
    /// <summary>
    /// Builds a SELECT * query on <paramref name="tableName"/> that restricts each
    /// column to the values contained in the corresponding array property.
    /// Returns the SQL text and a dictionary ready to be turned into parameters.
    /// </summary>
    public string BuildSelect(string tableName,
                              out IReadOnlyDictionary<string, object> parameters)
    {
        var sql = new StringBuilder($"SELECT * FROM \"{tableName}\"");
        var dict = new Dictionary<string, object>();

        bool firstClause = true;
        int paramIndex = 0;

        foreach (PropertyInfo prop in GetType().GetProperties(
                     BindingFlags.Public | BindingFlags.Instance))
        {
            // we only care about T[] or IReadOnlyCollection<T>
            if (!prop.PropertyType.IsArray) continue;

            var array = (Array?)prop.GetValue(this);
            if (array is null || array.Length == 0) continue;

            string colName = prop.GetCustomAttribute<ColumnAttribute>()?.Name
                           ?? prop.Name;

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
            sql.Append('"').Append(colName).Append("\" IN (")
               .Append(string.Join(',', placeholders)).Append(')');
        }

        parameters = dict;
        return sql.ToString();
    }
}