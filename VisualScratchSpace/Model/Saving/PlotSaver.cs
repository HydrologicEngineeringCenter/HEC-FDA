using SciChart.Core.Extensions;
using Statistics.Histograms;
using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace VisualScratchSpace.Model.Saving;
public class PlotSaver : ISQLiteSaver<LifeLossFunction>
{
    private const string LL_TABLE_NAME = "Life_Loss";
    private static readonly string _createQuery = 
        $@"CREATE TABLE IF NOT EXISTS ""{LL_TABLE_NAME}"" (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Simulation TEXT NOT NULL,
            Alternative TEXT NOT NULL, 
            Stage REAL NOT NULL,
            Hazard_Time TEXT NULL,
            Summary_Zone TEXT NOT NULL,
            Min REAL NOT NULL,
            Bin_Width REAL NOT NULL,
            Sample_Size INTEGER NOT NULL,
            Sample_Mean REAL NOT NULL,
            Sample_Variance REAL NOT NULL,
            Sample_Min REAL NOT NULL,
            Sample_Max REAL NOT NULL,
            Bin_Counts TEXT NOT NULL,
            UNIQUE(Simulation, Alternative, Hazard_Time, Summary_Zone)
        );";
    private static readonly string _insertQuery =
        $@"INSERT OR IGNORE INTO ""{LL_TABLE_NAME}"" (
            Simulation, 
            Alternative, 
            Stage,
            Hazard_Time,
            Summary_Zone,
            Min,
            Bin_Width,
            Sample_Size,
            Sample_Mean,
            Sample_Variance,
            Sample_Min,
            Sample_Max,
            Bin_Counts
        )
        VALUES (
            @sim,
            @alt,
            @stg,
            @hz_t,
            @s_z,
            @min,
            @bin_w,
            @s_size,
            @s_mean,
            @s_var,
            @s_min,
            @s_max,
            @bin_cts
        )";

    private string _connectionString;
    private SQLiteConnection _connection;

    public PlotSaver(string dbpath)
    {
        _connectionString = $"Data Source={dbpath}";
        _connection = new SQLiteConnection(_connectionString);
        _connection.Open(); 
    }

    public void SaveToSQLite(LifeLossFunction llf)
    {
        if (llf == null) return;

        using var transaction = _connection.BeginTransaction();

        CreateTable(_connection, transaction); // more efficient for SQL to check if table exists than checking a flag in this class

        using var insertCommand = new SQLiteCommand(_connection) { Transaction = transaction };
        BuildInsertCommand(insertCommand); 
        InsertIntoTable(insertCommand, llf);

        transaction.Commit();
    }

    public List<LifeLossFunction> ReadFromSQLite(SQLiteFilter filter, bool selectAll = false)
    {
        if (filter is not PlotFilter pf) throw new ArgumentException();

        using var selectCommand = new SQLiteCommand(_connection);
        string query;
        if (selectAll)
        {
            query = $@"SELECT * FROM {LL_TABLE_NAME}";
        }
        else
        {
            query = pf.BuildSelect(LL_TABLE_NAME, out IReadOnlyDictionary<string, object> parameters);
            foreach (var parameterPair in parameters) 
                selectCommand.Parameters.AddWithValue(parameterPair.Key, parameterPair.Value);
        }
        selectCommand.CommandText = query;    

        return null;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    private void CreateTable(SQLiteConnection connection, SQLiteTransaction transaction)
    {
        using var cmd = new SQLiteCommand(connection) { Transaction = transaction };
        cmd.CommandText = _createQuery;
        cmd.ExecuteNonQuery();
    }

    private void InsertIntoTable(SQLiteCommand cmd, LifeLossFunction llf)
    {
        for (int i = 0; i < llf.AlternativeNames.Length; i++)
        {
            cmd.Parameters["@sim"].Value = llf.SimulationName;
            cmd.Parameters["@alt"].Value = llf.AlternativeNames[i];
            cmd.Parameters["@stg"].Value = llf.Data.Xvals[i];
            cmd.Parameters["@hz_t"].Value = llf.HazardTime;
            cmd.Parameters["@s_z"].Value = llf.SummaryZone;

            DynamicHistogram histogram = (DynamicHistogram)llf.Data.Yvals[i];
            cmd.Parameters["@min"].Value = histogram.Min;
            cmd.Parameters["@bin_w"].Value = histogram.BinWidth;
            cmd.Parameters["@s_size"].Value = histogram.SampleSize;
            cmd.Parameters["@s_mean"].Value = histogram.SampleMean;
            cmd.Parameters["@s_var"].Value = histogram.SampleVariance;
            cmd.Parameters["@s_min"].Value = histogram.SampleMin;
            cmd.Parameters["@s_max"].Value = histogram.SampleMax;
            cmd.Parameters["@bin_cts"].Value = string.Join(",", histogram.BinCounts);

            cmd.ExecuteNonQuery();
        }
    }

    private void BuildInsertCommand(SQLiteCommand cmd)
    {
        cmd.CommandText = _insertQuery;
        cmd.Parameters.Add("@sim",      DbType.String);
        cmd.Parameters.Add("@alt",      DbType.String);
        cmd.Parameters.Add("@stg",      DbType.Double);
        cmd.Parameters.Add("@hz_t",     DbType.String);
        cmd.Parameters.Add("@s_z",      DbType.String);
        cmd.Parameters.Add("@min",      DbType.Double);
        cmd.Parameters.Add("@bin_w",    DbType.Double);
        cmd.Parameters.Add("@s_size",   DbType.Int64);
        cmd.Parameters.Add("@s_mean",   DbType.Double);
        cmd.Parameters.Add("@s_var",    DbType.Double);
        cmd.Parameters.Add("@s_min",     DbType.Double);
        cmd.Parameters.Add("@s_max",    DbType.Double);
        cmd.Parameters.Add("@bin_cts",  DbType.String);
    }
}
