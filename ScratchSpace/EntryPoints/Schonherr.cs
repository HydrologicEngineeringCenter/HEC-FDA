 using Statistics.Histograms;
using System.Data.SQLite;
using System.Text;
using Statistics;

namespace ScratchSpace.EntryPoints;
internal class Schonherr
{
    public static void EntryPoint()
    {
        
        string dbpath = @"Data Source=C:\Users\HEC\Downloads\Kelly_Barnes.fia"; //user
        using SQLiteConnection connection = new(dbpath);
        connection.Open();

        string[] cols = { "LL_In_StructuresU65", "LL_In_StructuresO65", "LL_Caught" }; 
        string tableName = "EPZ_Sensitivity>Results_By_Iteration>EPZ_Sensitivity_Slow>2>Toccoa_Falls_SensitivityEPZ>Pepsny_House"; //from user.
        string query = RowSumQuery(cols, tableName);

        using SQLiteCommand command = new(query, connection);
        using SQLiteDataReader reader = command.ExecuteReader();
        List<double> vals = new();
        while (reader.Read())
        {
            double val = (long)reader[0];
            vals.Add(val);
        }

        ConvergenceCriteria cc = new();
        DynamicHistogram histogram = new(vals, cc); // bin size of 1 because we are dealing with ints?
        //histogram.AddObservationsToHistogram(vals.ToArray());
        Console.WriteLine(histogram.Print());
        connection.Close();
    }

    private static string RowSumQuery(string[] columns, string tableName)
    {
        if (columns.Length < 1) throw new ArgumentException("Invalid number of columns");

        StringBuilder sb = new();
        sb.Append("SELECT (");
        for (int i = 0; i < columns.Length; i++)
        {
            sb.Append($"\"{columns[i]}\"");
            if (i < columns.Length - 1) 
                sb.Append(" + ");
        }
        sb.Append($") FROM \"{tableName}\"");
        return sb.ToString();
    }
}
