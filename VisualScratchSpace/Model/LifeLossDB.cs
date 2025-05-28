using System.Data.SQLite;
using System.Text;

namespace VisualScratchSpace.Model
{
    public class LifeLossDB
    {
        private string _connectionString;

        public LifeLossDB(string dbpath)
        {
            _connectionString = $"Data Source={dbpath}";
        }

        static string[] lifelossColumns = {"Name", "Alternatives", "Time_2", "Time_4",
                                                   "Time_6, Time_8", "Time_10", "Time_12",
                                                   "Time_14", "Time_16", "Time_18", "Time_20", "Time_22"};

        public List<Simulation> UpdateSimulations()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            for (int i = 0; i < lifelossColumns.Length; i++)
            {
                sb.Append(lifelossColumns[i]);
                if (i < lifelossColumns.Length - 1)
                    sb.Append(", ");
            }
            sb.Append(" FROM Simulations_Lookup_Table");
            string query = sb.ToString();
            List<Simulation> simulations = new List<Simulation>();

            try
            {
                using SQLiteConnection connection = new SQLiteConnection(_connectionString);
                connection.Open();
                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Simulation simulation = new Simulation();

                    // Name is 0th selection
                    simulation.Name = reader.GetString(0);
                    // Alternatives are 1st selection
                    simulation.Alternatives = reader.GetString(1).Split(','); // Alternatives are in csv string format
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
    }
}
