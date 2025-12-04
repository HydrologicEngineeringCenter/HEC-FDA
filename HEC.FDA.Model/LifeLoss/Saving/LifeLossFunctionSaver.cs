using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.SQLite;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace HEC.FDA.Model.LifeLoss.Saving;

/// <summary>
/// Object with the ability to save and read life loss functions to and from  SQLite database
/// </summary>
public class LifeLossFunctionSaver : SQLiteSaverBase<LifeLossFunction>
{
    private Dictionary<string, int> _impactAreaIDByName;

    private static readonly string _createCommandText =
        $@"
            CREATE TABLE IF NOT EXISTS {LifeLossStringConstants.LL_TABLE_NAME} (
            {LifeLossStringConstants.ID_HEADER}           INTEGER PRIMARY KEY AUTOINCREMENT,
            {LifeLossStringConstants.ELEMENT_ID_HEADER}   INTEGER NOT NULL,
            {LifeLossStringConstants.FUNCTION_ID_HEADER}  INTEGER NOT NULL,
            {LifeLossStringConstants.SIMULATION_HEADER}   TEXT    NOT NULL,
            {LifeLossStringConstants.ALTERNATIVE_HEADER}  TEXT    NOT NULL,
            {LifeLossStringConstants.STAGE_HEADER}        REAL    NOT NULL,
            {LifeLossStringConstants.HAZARD_TIME_HEADER}  TEXT    NULL,
            {LifeLossStringConstants.SUMMARY_ZONE_HEADER} TEXT    NOT NULL,
            {LifeLossStringConstants.MIN_HEADER}          REAL    NOT NULL,
            {LifeLossStringConstants.BIN_WIDTH_HEADER}    REAL    NOT NULL,
            {LifeLossStringConstants.SAMPLE_MEAN_HEADER}  REAL    NOT NULL,
            {LifeLossStringConstants.SAMPLE_VARIANCE_HEADER} REAL NOT NULL,
            {LifeLossStringConstants.SAMPLE_MIN_HEADER}   REAL    NOT NULL,
            {LifeLossStringConstants.SAMPLE_MAX_HEADER}   REAL    NOT NULL,
            {LifeLossStringConstants.BIN_COUNTS_HEADER}   TEXT    NOT NULL,
            FOREIGN KEY({LifeLossStringConstants.ELEMENT_ID_HEADER}) 
                REFERENCES {LifeLossStringConstants.LL_LOOKUP_TABLE_NAME}(ID) ON DELETE CASCADE,
            UNIQUE(
                {LifeLossStringConstants.ID_HEADER},
                {LifeLossStringConstants.SIMULATION_HEADER},
                {LifeLossStringConstants.ALTERNATIVE_HEADER},
                {LifeLossStringConstants.HAZARD_TIME_HEADER},
                {LifeLossStringConstants.SUMMARY_ZONE_HEADER}
            )
        );"; // UNIQUE functionally means we cannot overwrite with this command, and cannot add a duplicate. 
    private static readonly string _insertCommandText =
        $@"
            INSERT OR IGNORE INTO {LifeLossStringConstants.LL_TABLE_NAME} (
                {LifeLossStringConstants.ELEMENT_ID_HEADER},
                {LifeLossStringConstants.FUNCTION_ID_HEADER},
                {LifeLossStringConstants.SIMULATION_HEADER},
                {LifeLossStringConstants.ALTERNATIVE_HEADER},
                {LifeLossStringConstants.STAGE_HEADER},
                {LifeLossStringConstants.HAZARD_TIME_HEADER},
                {LifeLossStringConstants.SUMMARY_ZONE_HEADER},
                {LifeLossStringConstants.MIN_HEADER},
                {LifeLossStringConstants.BIN_WIDTH_HEADER},
                {LifeLossStringConstants.SAMPLE_MEAN_HEADER},
                {LifeLossStringConstants.SAMPLE_VARIANCE_HEADER},
                {LifeLossStringConstants.SAMPLE_MIN_HEADER},
                {LifeLossStringConstants.SAMPLE_MAX_HEADER},
                {LifeLossStringConstants.BIN_COUNTS_HEADER}
            )
            VALUES (
                {LifeLossStringConstants.ELEMENT_ID_PARAMETER},
                {LifeLossStringConstants.FUNCTION_ID_PARAMETER},
                {LifeLossStringConstants.SIMULATION_PARAMETER},
                {LifeLossStringConstants.ALTERNATIVE_PARAMETER},
                {LifeLossStringConstants.STAGE_PARAMETER},
                {LifeLossStringConstants.HAZARD_TIME_PARAMETER},
                {LifeLossStringConstants.SUMMARY_ZONE_PARAMETER},
                {LifeLossStringConstants.MIN_PARAMETER},
                {LifeLossStringConstants.BIN_WIDTH_PARAMETER},
                {LifeLossStringConstants.SAMPLE_MEAN_PARAMETER},
                {LifeLossStringConstants.SAMPLE_VARIANCE_PARAMETER},
                {LifeLossStringConstants.SAMPLE_MIN_PARAMETER},
                {LifeLossStringConstants.SAMPLE_MAX_PARAMETER},
                {LifeLossStringConstants.BIN_COUNTS_PARAMETER}
            );";

    public LifeLossFunctionSaver(string dbpath, Dictionary<string, int> impactAreaIDByName) : base(dbpath) // calls the base class constructor to initialize the SQLite connection
    {
        CreateTable(_connection); // more efficient for SQL to check if table exists than checking a flag in this class
        _impactAreaIDByName = impactAreaIDByName;
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
    public override List<LifeLossFunction> ReadFromSQLite(SQLiteFilter filter)
    {
        if (filter is not LifeLossFunctionFilter plotFilter) throw new ArgumentException();

        using var selectCommand = new SQLiteCommand(_connection);
        BuildSelectCommand(selectCommand, plotFilter);
        using var reader = selectCommand.ExecuteReader();

        int functionID = -1;
        List<LifeLossFunction> result = [];
        List<string> alternatives = [];
        List<double> stages = [];
        List<DynamicHistogram> histograms = [];
        string currentSim = null, currentSZ = null, currentHT = null;

        // local function to push life loss functions to the result list once they have been fully read
        void AddCurrent()
        {
            if (functionID == -1) return; // means we have not read anything yet, do not want to create a life loss function yet

            UncertainPairedData data = new(stages.ToArray(), histograms.ToArray(), new CurveMetaData("Stage", "Life Loss", $"{currentSim}_{currentSZ}_{currentHT}", "LifeLoss", _impactAreaIDByName[currentSZ], "LifeLoss"));
            LifeLossFunction llf = new(-1, functionID, data, alternatives.ToArray(), currentSim, currentSZ, currentHT);
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
            int currentFunctionID = reader.GetInt32(reader.GetOrdinal(LifeLossStringConstants.FUNCTION_ID_HEADER));

            // the rows we read in are ordered by function ID (look at BuildSelect to see the SELECT command)
            // so, when a new function ID is read, we know we are reading a new life loss function
            if (currentFunctionID != functionID)
            {
                AddCurrent(); // we are at a new lifeloss function, so add the previous to the result
                currentSim = reader.GetString(reader.GetOrdinal(LifeLossStringConstants.SIMULATION_HEADER));
                currentSZ = reader.GetString(reader.GetOrdinal(LifeLossStringConstants.SUMMARY_ZONE_HEADER));
                currentHT = reader.GetString(reader.GetOrdinal(LifeLossStringConstants.HAZARD_TIME_HEADER));
                functionID = currentFunctionID;
            }
            alternatives.Add(reader.GetString(reader.GetOrdinal(LifeLossStringConstants.ALTERNATIVE_HEADER)));
            stages.Add(reader.GetDouble(reader.GetOrdinal(LifeLossStringConstants.STAGE_HEADER)));
            histograms.Add(DeserializeSQLiteHistogram(reader));
        }
        AddCurrent(); // add the final function to the result
        return result;
    }

    public override void DeleteFromSQLite(SQLiteFilter filter)
    {
        using var deleteCommand = new SQLiteCommand(_connection);
        BuildDeleteCommand(deleteCommand, filter);
        deleteCommand.ExecuteNonQuery();
    }

    /// <summary>
    /// Reads the SQLite data back into a histogram object
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    private static DynamicHistogram DeserializeSQLiteHistogram(SQLiteDataReader reader)
    {
        double min = reader.GetDouble(reader.GetOrdinal(LifeLossStringConstants.MIN_HEADER));
        double binWidth = reader.GetDouble(reader.GetOrdinal(LifeLossStringConstants.BIN_WIDTH_HEADER));
        double sampleMean = reader.GetDouble(reader.GetOrdinal(LifeLossStringConstants.SAMPLE_MEAN_HEADER));
        double sampleVariance = reader.GetDouble(reader.GetOrdinal(LifeLossStringConstants.SAMPLE_VARIANCE_HEADER));
        double sampleMin = reader.GetDouble(reader.GetOrdinal(LifeLossStringConstants.SAMPLE_MIN_HEADER));
        double sampleMax = reader.GetDouble(reader.GetOrdinal(LifeLossStringConstants.SAMPLE_MAX_HEADER));
        string[] binCountsString = reader.GetString(reader.GetOrdinal(LifeLossStringConstants.BIN_COUNTS_HEADER)).Split(',');
        long[] binCounts = new long[binCountsString.Length];
        for (int i = 0; i < binCountsString.Length; i++)
        {
            if (!long.TryParse(binCountsString[i], out binCounts[i])) throw new FormatException($"{binCountsString[i]} not a valid integer");
        }
        DynamicHistogram histogram = new DynamicHistogram(min, binWidth, binCounts, sampleMean, sampleVariance, sampleMin, sampleMax, new Statistics.ConvergenceCriteria());
        return histogram;
    }

    private static void CreateTable(SQLiteConnection connection)
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
    private static void InsertIntoTable(SQLiteCommand cmd, LifeLossFunction llf)
    {
        // reuse the same command each time and just fill in values for each parameter
        for (int i = 0; i < llf.AlternativeNames.Length; i++)
        {
            cmd.Parameters[LifeLossStringConstants.ELEMENT_ID_PARAMETER].Value = llf.ElementID;
            cmd.Parameters[LifeLossStringConstants.FUNCTION_ID_PARAMETER].Value = llf.FunctionID;
            cmd.Parameters[LifeLossStringConstants.SIMULATION_PARAMETER].Value = llf.SimulationName;
            cmd.Parameters[LifeLossStringConstants.ALTERNATIVE_PARAMETER].Value = llf.AlternativeNames[i];
            cmd.Parameters[LifeLossStringConstants.STAGE_PARAMETER].Value = llf.Data.Xvals[i];
            cmd.Parameters[LifeLossStringConstants.HAZARD_TIME_PARAMETER].Value = llf.HazardTime;
            cmd.Parameters[LifeLossStringConstants.SUMMARY_ZONE_PARAMETER].Value = llf.SummaryZone;

            DynamicHistogram histogram = (DynamicHistogram)llf.Data.Yvals[i];
            cmd.Parameters[LifeLossStringConstants.MIN_PARAMETER].Value = histogram.Min;
            cmd.Parameters[LifeLossStringConstants.BIN_WIDTH_PARAMETER].Value = histogram.BinWidth;
            cmd.Parameters[LifeLossStringConstants.SAMPLE_MEAN_PARAMETER].Value = histogram.SampleMean;
            cmd.Parameters[LifeLossStringConstants.SAMPLE_VARIANCE_PARAMETER].Value = histogram.SampleVariance;
            cmd.Parameters[LifeLossStringConstants.SAMPLE_MIN_PARAMETER].Value = histogram.SampleMin;
            cmd.Parameters[LifeLossStringConstants.SAMPLE_MAX_PARAMETER].Value = histogram.SampleMax;
            cmd.Parameters[LifeLossStringConstants.BIN_COUNTS_PARAMETER].Value = string.Join(",", histogram.BinCounts);

            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Constructs the INSERT command data structure 
    /// </summary>
    /// <param name="cmd"></param>
    private static void BuildInsertCommand(SQLiteCommand cmd)
    {
        cmd.CommandText = _insertCommandText;

        // each parameter has a placeholder formatted "@param" which gets filled in when this command is executed
        cmd.Parameters.Add(
            LifeLossStringConstants.ELEMENT_ID_PARAMETER, DbType.Int32);
        cmd.Parameters.Add(
            LifeLossStringConstants.FUNCTION_ID_PARAMETER, DbType.Int32);
        cmd.Parameters.Add(
            LifeLossStringConstants.SIMULATION_PARAMETER, DbType.String);
        cmd.Parameters.Add(
            LifeLossStringConstants.ALTERNATIVE_PARAMETER, DbType.String);
        cmd.Parameters.Add(
            LifeLossStringConstants.STAGE_PARAMETER, DbType.Double);
        cmd.Parameters.Add(
            LifeLossStringConstants.HAZARD_TIME_PARAMETER, DbType.String);
        cmd.Parameters.Add(
            LifeLossStringConstants.SUMMARY_ZONE_PARAMETER, DbType.String);
        cmd.Parameters.Add(
            LifeLossStringConstants.MIN_PARAMETER, DbType.Double);
        cmd.Parameters.Add(
            LifeLossStringConstants.BIN_WIDTH_PARAMETER, DbType.Double);
        cmd.Parameters.Add(
            LifeLossStringConstants.SAMPLE_MEAN_PARAMETER, DbType.Double);
        cmd.Parameters.Add(
            LifeLossStringConstants.SAMPLE_VARIANCE_PARAMETER, DbType.Double);
        cmd.Parameters.Add(
            LifeLossStringConstants.SAMPLE_MIN_PARAMETER, DbType.Double);
        cmd.Parameters.Add(
            LifeLossStringConstants.SAMPLE_MAX_PARAMETER, DbType.Double);
        cmd.Parameters.Add(
            LifeLossStringConstants.BIN_COUNTS_PARAMETER, DbType.String);
    }

    /// <summary>
    /// Constructs the SELECT command data structure
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="filter"></param>
    /// <param name="selectAll"></param>
    private static void BuildSelectCommand(SQLiteCommand cmd, SQLiteFilter filter)
    {
        StringBuilder querySB = new();

        // add parameters to the command if we are only selecting specific entries
        // same idea as adding parameters in BuildInsertCommand(), except here we add parameters and their values simultaneously
        // we are not reusing the command so we do not add parameters once and then set at different points, we just do both at once
        querySB = filter.BuildSelect(LifeLossStringConstants.LL_TABLE_NAME, out IReadOnlyDictionary<string, object> parameters);
        foreach (var parameterPair in parameters)
            cmd.Parameters.AddWithValue(parameterPair.Key, parameterPair.Value);

        // order the rows first by function ID to group functions together, then by stage to sort the histograms in ascending order
        querySB.Append($@"
                            ORDER BY
                            {LifeLossStringConstants.FUNCTION_ID_HEADER},
                            {LifeLossStringConstants.STAGE_HEADER};
                        ");
        cmd.CommandText = querySB.ToString();
    }

    private static void BuildDeleteCommand(SQLiteCommand cmd, SQLiteFilter filter)
    {
        StringBuilder sql = new();
        sql = filter.BuildDelete(LifeLossStringConstants.LL_TABLE_NAME, out IReadOnlyDictionary<string, object> parameters);
        foreach (var parameterPair in parameters)
            cmd.Parameters.AddWithValue(parameterPair.Key, parameterPair.Value);
        cmd.CommandText = sql.ToString();
    }

}
