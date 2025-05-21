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
            return null;
        }
    }
}
