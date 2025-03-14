using Utility.Memory;

namespace HEC.FDA.Model.Spatial.Extensions;
public static class Utility
{
    /// <summary>
    /// Tries to get the value of the specified column as the specified type. If the type is wrong, or column doesn't exist, returns fallback value
    /// </summary>
    public static T TryGetValueAs<T>(this TableRow row, string columnName, T fallbackValue = default) {
		try
		{
			if(string.IsNullOrWhiteSpace(columnName))
                return fallbackValue;
            return row.ValueAs(columnName, fallbackValue);
        }
		catch (System.Exception)
		{
			return fallbackValue;
		}
    }
}
