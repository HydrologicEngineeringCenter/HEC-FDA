using HEC.FDA.Model.alternatives;
using Microsoft.Extensions.Primitives;
using SciChart.Core.Extensions;
using Statistics.Histograms;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Markup;

namespace VisualScratchSpace.Model.Saving;
public class PlotSaver : ISQLiteSaver
{
    private string _connectionString;
    private List<LifeLossFunction> _lifeLossFunctions;

    public PlotSaver(string dbpath, List<LifeLossFunction> funcs)
    {
        _connectionString = $"Data Source={dbpath}"; ;
        _lifeLossFunctions = funcs ;
    }

    public void SaveToSQLite()
    {
        if (_lifeLossFunctions.IsEmpty()) return;

        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        using var createCommand = new SQLiteCommand(connection) { Transaction = transaction };
        foreach (LifeLossFunction function in _lifeLossFunctions) { }
        CreateTable(_lifeLossFunctions[0], createCommand);

        using var insertCommand = new SQLiteCommand(connection) { Transaction = transaction };
        insertCommand.Parameters.Add("@alt", DbType.String);
        insertCommand.Parameters.Add("@stg", DbType.Double);
        for (int i = 0; i < 12; i++)
        {
            insertCommand.Parameters.Add($"@bin{i}", DbType.Int64);
        }
        InsertIntoTable(_lifeLossFunctions[0], insertCommand);

        transaction.Commit();
    }

    private void CreateTable(LifeLossFunction llf, SQLiteCommand cmd)
    {
        string tableName = GetTableName(llf);
        string createTableQuery = BuildCreateTableQuery(llf);
        cmd.CommandText = createTableQuery;
        cmd.ExecuteNonQuery();
    }

    private string BuildCreateTableQuery(LifeLossFunction llf)
    {
        string tableName = GetTableName(llf);
        StringBuilder sb = new StringBuilder();
        string baseQuery =
            $@"CREATE TABLE IF NOT EXISTS ""{tableName}""(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Alternative TEXT NOT NULL, 
                    Stage REAL NOT NULL,";
        sb.Append(baseQuery);
        DynamicHistogram histogram = (DynamicHistogram)llf.Data.Yvals[0]; // asserting that every histogram for each plot will have the same # of bins
        for (int i = 0; i < histogram.BinCounts.Length; i++)
        {
            sb.Append($"Bin_{i} INTEGER NOT NULL,");
        }
        sb.Append("UNIQUE(Alternative));");
        return sb.ToString();
    }

    private void InsertIntoTable(LifeLossFunction llf, SQLiteCommand cmd)
    {
        string insertQuery = BuildInsertQuery(llf);
        cmd.CommandText = insertQuery;
        for (int i = 0; i < llf.AlternativeNames.Length; i++)
        {
            cmd.Parameters["@alt"].Value = llf.AlternativeNames[i];
            cmd.Parameters["@stg"].Value = llf.Data.Xvals[i];
            DynamicHistogram histogram = (DynamicHistogram)llf.Data.Yvals[i];
            for (int j = 0; j < histogram.BinCounts.Length; j++)
            {
                cmd.Parameters[$"@bin{j}"].Value = histogram.BinCounts[j];
            }
            cmd.ExecuteNonQuery();
        }
    }

    private string BuildInsertQuery(LifeLossFunction llf)
    {
        string tableName = GetTableName(llf);
        StringBuilder sb = new StringBuilder();
        sb.Append($@"INSERT OR IGNORE INTO ""{tableName}""(Alternative, Stage,");
        DynamicHistogram histogram = (DynamicHistogram)llf.Data.Yvals[0]; // asserting that every histogram for each plot will have the same # of bins
        for (int i = 0; i < histogram.BinCounts.Length; i++)
        {
            sb.Append(i < histogram.BinCounts.Length - 1 ? $"Bin_{i}," : $"Bin_{i}");
        }
        
        sb.Append(") VALUES (@alt, @stg,");
        for (int i = 0; i < histogram.BinCounts.Length; i++)
        {
            sb.Append(i < histogram.BinCounts.Length - 1 ? $"@bin{i}," : $"@bin{i}");
        }
        sb.Append(");");

        string q = sb.ToString();
        return q;
    }

    private string GetTableName(LifeLossFunction function)
    {
        return $"{function.SimulationName}_{function.SummaryZone}_{function.HazardTime}";
    }
}
