using Statistics;
using Statistics.Histograms;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace HEC.FDA.Model.LifeLoss
{
    /// <summary>
    /// Handles operations for querying .fia databases imported from LifeSim
    /// </summary>
    public class LifeSimDatabase
    {
        private string _connectionString;
        private static string[] _lifelossColumns = {"Name", "Alternatives", "Time_2", "Time_4",
                                                   "Time_6, Time_8", "Time_10", "Time_12",
                                                   "Time_14", "Time_16", "Time_18", "Time_20", "Time_22"};

        public LifeSimDatabase(string dbpath)
        {
            _connectionString = $"Data Source={dbpath}";
        }

        /// <summary>
        /// Change the list of simulations available to the user to select from
        /// </summary>
        /// <returns></returns>
        public List<LifeSimSimulation> UpdateSimulations()
        {
            StringBuilder sb = new();
            sb.Append("SELECT ");
            for (int i = 0; i < _lifelossColumns.Length; i++)
            {
                sb.Append(_lifelossColumns[i]);
                if (i < _lifelossColumns.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(" FROM Simulations_Lookup_Table");
            string query = sb.ToString();
            List<LifeSimSimulation> simulations = [];

            try
            {
                using SQLiteConnection connection = new SQLiteConnection(_connectionString);
                connection.Open();
                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string name = reader.GetString(0); // Name is 0th selection
                    LifeSimSimulation simulation = new LifeSimSimulation(name);

                    simulation.Alternatives = reader.GetString(1).Split(',').ToList(); // Alternatives are 1st selection and in csv string format

                    // Hazard Times are in the 2nd through 12th selections                                                           
                    for (int i = 2; i <= 12; i++)
                    {

                        if (reader.GetBoolean(i))
                            simulation.HazardTimes.Add($"{(i - 1) * 2}"); // convert time to integer string, starting from 2 and incrementing by 2 until 22
                    }
                    simulations.Add(simulation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error trying to open LifeSim database", ex);
            }
            return simulations;
        }

        /// <summary>
        /// Create a histogram of life loss data from a given table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DynamicHistogram QueryLifeLossTable(string tableName)
        {
            using SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();
            string query = $"SELECT (LL_In_StructuresU65 + LL_In_StructuresO65 + LL_Caught) FROM \"{tableName}\";";
            using SQLiteCommand command = new(query, connection);
            using SQLiteDataReader reader = command.ExecuteReader();
            List<double> vals = [];
            while (reader.Read())
            {
                double val = (long)reader[0];
                vals.Add(val);
            }
            ConvergenceCriteria cc = new();
            return new DynamicHistogram(vals, cc);
        }

        /// <summary>
        /// Create map of alternative names to their associated hydraulics folders
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> CreateAlternativeHydraulicsPairs()
        {
            using SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = "SELECT Name, Hydraulic_Scenario FROM Alternatives_Lookup_Table";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            Dictionary<string, string> res = new();
            while (reader.Read())
            {
                string alternativeName = reader.GetString(0);
                string hydraulicsName = reader.GetString(1);
                res[alternativeName] = hydraulicsName;
            }
            return res;
        }

        /// <summary>
        /// Get the summary set name for a given simulation
        /// </summary>
        /// <param name="simulationName"></param>
        /// <returns></returns>
        public string SummarySetName(string simulationName)
        {

            string prefix = simulationName + ">Summary_Polygon_Set>";
            string query = "SELECT name FROM sqlite_master WHERE type='table' AND name LIKE @pattern;";

            using SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using SQLiteCommand command = new(query, connection);
            command.Parameters.AddWithValue("@pattern", prefix + "%");
            using SQLiteDataReader reader = command.ExecuteReader();
            string summarySetName = "";
            while (reader.Read())
            {
                string tableName = reader.GetString(0);
                summarySetName = tableName.Split('>').Last();
            }
            return summarySetName;
        }
    }
}