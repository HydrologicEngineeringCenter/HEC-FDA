using HEC.FDA.ViewModel;
using Statistics.Histograms;
using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Importer;
using System.Drawing.Printing;
using Statistics;

namespace ScratchSpace.EntryPoints;
internal class Schonherr
{
    public static void EntryPoint()
    {
        
        string dbpath = @"Data Source=C:\Users\HEC\Downloads\LifeSim\LifeSim\Muncie\Muncie.fia";
        using SQLiteConnection connection = new(dbpath);
        connection.Open();

        string[] cols = { "Iteration", "Population_At_RiskO65" };
        string tableName = "500 Year Sim>Results_By_Iteration>500 Year WOP Alt>14>500yr>Left-Bank";
        string query = RowSumQuery(cols, tableName);

        using SQLiteCommand command = new(query, connection);
        using SQLiteDataReader reader = command.ExecuteReader();
        List<double> vals = new List<double>();
        while (reader.Read())
        {
            double val = (long)reader[0];
            vals.Add(val);
            Console.WriteLine(val);
        }

        ConvergenceCriteria cc = new();
        DynamicHistogram histogram = new DynamicHistogram(vals, cc);
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
            if (i < columns.Length - 1) sb.Append(" + ");
        }
        sb.Append($") FROM \"{tableName}\"");
        return sb.ToString();
    }
}
