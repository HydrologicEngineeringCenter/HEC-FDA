using Statistics;
using Statistics.Histograms;
using System.Data.SQLite;
using System.Text;

namespace VisualScratchSpace.Model
{
    public class LifeLossDB
    {
        private string _connectionString;

        private static string[] lifelossColumns = {"Name", "Alternatives", "Time_2", "Time_4",
                                                   "Time_6, Time_8", "Time_10", "Time_12",
                                                   "Time_14", "Time_16", "Time_18", "Time_20", "Time_22"};

        public LifeLossDB(string dbpath)
        {
            _connectionString = $"Data Source={dbpath}";
        }

        public static List<string> GetSimulationTablePrefixes(string simulation, string[] alternatives, string[] hazardTimes)
        {
            List<string> prefixes = [];
            for (int i = 0; i < alternatives.Length; i++)
            {
                for (int j = 0; j < hazardTimes.Length; j++)
                {
                    prefixes.Add($"{simulation}>Results_By_Iteration>{alternatives[i]}>{hazardTimes[j]}");
                }
            }
            return prefixes;
        }

        public List<Simulation> UpdateSimulations()
        {
            StringBuilder sb = new();
            sb.Append("SELECT ");
            for (int i = 0; i < lifelossColumns.Length; i++)
            {
                sb.Append(lifelossColumns[i]);
                if (i < lifelossColumns.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(" FROM Simulations_Lookup_Table");
            string query = sb.ToString();
            List<Simulation> simulations = [];

            try
            {
                using SQLiteConnection connection = new SQLiteConnection(_connectionString);
                connection.Open();
                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Simulation simulation = new()
                    {
                        // Name is 0th selection
                        Name = reader.GetString(0),
                        // Alternatives are 1st selection
                        Alternatives = reader.GetString(1).Split(',') // Alternatives are in csv string format
                    };
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
                // debugging
                Simulation simulation = new Simulation();
                simulation.Name = ex.Message;
                simulations.Add(simulation);
            }
            return simulations;
        }

        public void QueryMatchingTables(string[] prefixes)
        {
            List<string> allMatchingTables = [];
            using SQLiteConnection connection = new SQLiteConnection(_connectionString);
            connection.Open();

            // get every matching table for each prefix and put them in one final list
            foreach (string prefix in prefixes)
            {
                List<string> tables = GetMatchingTables(prefix, connection);
                allMatchingTables.AddRange(tables);
            }
            
            // query each table in the final list for life loss
            foreach (string tableName in allMatchingTables)
            {
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
                DynamicHistogram histogram = new DynamicHistogram(vals, cc);
            }
        }

        private List<string> GetMatchingTables(string prefix, SQLiteConnection connection)
        {
            string findTablesQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name LIKE @pattern;";
            List<string> matchingTables = [];
            using SQLiteCommand command = new(findTablesQuery, connection);

            // pattern match the simnulation + alternative + time and find all matching tables
            command.Parameters.AddWithValue("@pattern", prefix + "%");
            using SQLiteDataReader reader = command.ExecuteReader();   
            while (reader.Read())
                matchingTables.Add(reader.GetString(0));

            return matchingTables;
        }   
    }
}
