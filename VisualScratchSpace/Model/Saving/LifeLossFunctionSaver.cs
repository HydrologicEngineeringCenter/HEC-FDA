using HEC.FDA.Model.paireddata;
using Statistics.Histograms;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Transactions;

namespace VisualScratchSpace.Model.Saving;

/// <summary>
/// Object with the ability to save and read life loss functions to and from  SQLite database
/// </summary>
public class LifeLossFunctionSaver : SQLiteSaverBase<LifeLossFunction>
{
    private const string LL_TABLE_NAME = "Life_Loss";
    private static readonly string _createCommandText = 
        $@"CREATE TABLE IF NOT EXISTS ""{LL_TABLE_NAME}"" (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Simulation TEXT NOT NULL,
            Alternative TEXT NOT NULL, 
            Stage REAL NOT NULL,
            Hazard_Time TEXT NULL,
            Summary_Zone TEXT NOT NULL,
            Min REAL NOT NULL,
            Bin_Width REAL NOT NULL,
            Sample_Mean REAL NOT NULL,
            Sample_Variance REAL NOT NULL,
            Sample_Min REAL NOT NULL,
            Sample_Max REAL NOT NULL,
            Bin_Counts TEXT NOT NULL,
            UNIQUE(Simulation, Alternative, Hazard_Time, Summary_Zone) 
        );"; // UNIQUE functionally means we cannot overwrite with this command, and cannot add a duplicate. 
    private static readonly string _insertCommandText =
        $@"INSERT OR IGNORE INTO ""{LL_TABLE_NAME}"" (
            Simulation, 
            Alternative, 
            Stage,
            Hazard_Time,
            Summary_Zone,
            Min,
            Bin_Width,
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
            @s_mean,
            @s_var,
            @s_min,
            @s_max,
            @bin_cts
        )";

    public LifeLossFunctionSaver(string dbpath) : base(dbpath) // calls the base class constructor to initialize the SQLite connection
    {
        CreateTable(_connection); // more efficient for SQL to check if table exists than checking a flag in this class
    }

    /// <summary>
    /// Saves a single life loss function to SQLite. Creates the life loss table in SQLite if it does not exist yet
    /// </summary>
    /// <param name="llf"></param>
    public override void SaveToSQLite(LifeLossFunction llf)
    {
        if (llf == null) return;

        using var transaction = _connection.BeginTransaction();
        using var insertCommand = new SQLiteCommand(_connection) { Transaction = transaction };
        BuildInsertCommand(insertCommand); 
        InsertIntoTable(insertCommand, llf); // we reuse the same command with the same parameter placeholders, changing their values each time the command is executed

        transaction.Commit(); // commit everything at the end instead of many times for each insert command
    }

    /// <summary>
    /// Reads in a list of life loss functions from SQLite based on the specified filter
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="selectAll">False by default. Set to true to select all from the database</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public override List<LifeLossFunction> ReadFromSQLite(SQLiteFilter filter, bool selectAll = false)
    {
        if (filter is not LifeLossFunctionFilter plotFilter) throw new ArgumentException();

        using var selectCommand = new SQLiteCommand(_connection);
        BuildSelectCommand(selectCommand, plotFilter, selectAll);
        using var reader = selectCommand.ExecuteReader();

        List<LifeLossFunction> result = new();
        string? currentSim = null, currentSZ = null, currentHT = null;
        List<string> alternatives = new();
        List<double> stages = new();
        List<DynamicHistogram> histograms = new();

        // local function to push life loss functions to the result list once they have been fully read
        void AddCurrent()
        {
            if (currentSim == null || currentSZ == null || currentHT == null) return; // means we have not read anything yet, do not want to create a life loss function yet

            UncertainPairedData data = new(stages.ToArray(), histograms.ToArray(), new CurveMetaData());
            LifeLossFunction llf = new(data, alternatives.ToArray(), currentSim, currentSZ, currentHT);
            result.Add(llf);

            // reset the lists of life loss function parameters
            // each set of three represents one (x, y) pair where x = stage (alternative) and y = histogram
            // they all share the same simulation, summary zone, and time because they belong to the same function
            alternatives.Clear();
            stages.Clear();
            histograms.Clear();
        }

        while (reader.Read())
        {
            string sim = reader.GetString(reader.GetOrdinal("Simulation"));
            string sz = reader.GetString(reader.GetOrdinal("Summary_Zone"));
            string ht = reader.GetString(reader.GetOrdinal("Hazard_Time"));

            // a lifeloss function is unique iff it's comprised of a unique combination of simulation name, summary zone, and hazard time
            // the data being read in from the DB is ordered by simulation, then summary zone, then hazard time, then stage
            // if the simulation, summary zone, or hazard time changes with a subsequent read, we MUST be reading a new function
            if (sim != currentSim || sz != currentSZ || ht != currentHT)
            {
                AddCurrent(); // we are at a new lifeloss function, so add the previous to the result
                currentSim = sim;
                currentSZ = sz;
                currentHT = ht;
            }
            alternatives.Add(reader.GetString(reader.GetOrdinal("Alternative")));
            stages.Add(reader.GetDouble(reader.GetOrdinal("Stage")));
            histograms.Add(DeserializeSQLiteHistogram(reader));
        }
        AddCurrent(); // add the final function to the result
        return result;
    }

    /// <summary>
    /// Reads the SQLite data back into a histogram object
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    private static DynamicHistogram DeserializeSQLiteHistogram(SQLiteDataReader reader)
    {
        double min = reader.GetDouble(reader.GetOrdinal("Min"));
        double binWidth = reader.GetDouble(reader.GetOrdinal("Bin_Width"));
        double sampleMean = reader.GetDouble(reader.GetOrdinal("Sample_Mean"));
        double sampleVariance = reader.GetDouble(reader.GetOrdinal("Sample_Variance"));
        double sampleMin = reader.GetDouble(reader.GetOrdinal("Sample_Min"));
        double sampleMax = reader.GetDouble(reader.GetOrdinal("Sample_Max"));
        string[] binCountsString = reader.GetString(reader.GetOrdinal("Bin_Counts")).Split(',');
        long[] binCounts = new long[binCountsString.Length];
        for (int i = 0; i < binCountsString.Length; i++)
        {
            if (!long.TryParse(binCountsString[i], out binCounts[i])) throw new FormatException($"{binCountsString[i]} not a valid integer");
        }
        DynamicHistogram histogram = new DynamicHistogram(min, binWidth, binCounts, sampleMean, sampleVariance, sampleMin, sampleMax, new Statistics.ConvergenceCriteria());
        return histogram;
    }

    private void CreateTable(SQLiteConnection connection)
    {
        using var cmd = new SQLiteCommand(connection);
        cmd.CommandText = _createCommandText;
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Insert a single life loss function into the table.
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="llf"></param>
    private void InsertIntoTable(SQLiteCommand cmd, LifeLossFunction llf)
    {
        // reuse the same command each time and just fill in values for each parameter
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
            cmd.Parameters["@s_mean"].Value = histogram.SampleMean;
            cmd.Parameters["@s_var"].Value = histogram.SampleVariance;
            cmd.Parameters["@s_min"].Value = histogram.SampleMin;
            cmd.Parameters["@s_max"].Value = histogram.SampleMax;
            cmd.Parameters["@bin_cts"].Value = string.Join(",", histogram.BinCounts);

            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Constructs the INSERT command data structure 
    /// </summary>
    /// <param name="cmd"></param>
    private void BuildInsertCommand(SQLiteCommand cmd)
    {
        cmd.CommandText = _insertCommandText;
        // each parameter has a placeholder formatted "@param" which gets filled in when this command is executed
        cmd.Parameters.Add("@sim",      DbType.String);
        cmd.Parameters.Add("@alt",      DbType.String);
        cmd.Parameters.Add("@stg",      DbType.Double);
        cmd.Parameters.Add("@hz_t",     DbType.String);
        cmd.Parameters.Add("@s_z",      DbType.String);
        cmd.Parameters.Add("@min",      DbType.Double);
        cmd.Parameters.Add("@bin_w",    DbType.Double);
        cmd.Parameters.Add("@s_mean",   DbType.Double);
        cmd.Parameters.Add("@s_var",    DbType.Double);
        cmd.Parameters.Add("@s_min",    DbType.Double);
        cmd.Parameters.Add("@s_max",    DbType.Double);
        cmd.Parameters.Add("@bin_cts",  DbType.String);
    }

    /// <summary>
    /// Constructs the SELECT command data structure
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="filter"></param>
    /// <param name="selectAll"></param>
    private void BuildSelectCommand(SQLiteCommand cmd, SQLiteFilter filter, bool selectAll)
    {
        StringBuilder querySB = new();
        if (selectAll)
        {
            // simple select all command
            querySB.Append($@"SELECT * FROM {LL_TABLE_NAME}");
        }
        else
        {
            // add parameters to the command if we are only selecting specific entries
            // same idea as adding parameters in BuildInsertCommand(), except here we add parameters and their values simultaneously
            // we are not reusing the command so we do not add parameters once and then set at different points, we just do both at once
            querySB = filter.BuildSelect(LL_TABLE_NAME, out IReadOnlyDictionary<string, object> parameters);
            foreach (var parameterPair in parameters)
                cmd.Parameters.AddWithValue(parameterPair.Key, parameterPair.Value);
        }
        querySB.Append(@" ORDER BY ""Simulation"", ""Summary_Zone"", ""Hazard_Time"" DESC, ""Stage"";"); // hazard time is descending because we want "2" to be read before "14" as strings
        cmd.CommandText = querySB.ToString();
    }

}
