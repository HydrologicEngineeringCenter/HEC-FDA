using System.Data.SQLite;

namespace VisualScratchSpace.Model
{
    public class LifeLossDB
    {
        private string _connectionString;

        public LifeLossDB(string dbpath)
        {
            _connectionString = $"Data Source={dbpath}";
        }

        public List<Simulation> UpdateSimulations()
        {
            string query = "SELECT Name, Alternatives FROM Simulations_Lookup_Table";
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
                    // Alternatives are 1st selection
                    simulation.Name = reader.GetString(0);
                    simulation.Alternatives = reader.GetString(1).Split(','); // Alternatives are in csv string format                   

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
