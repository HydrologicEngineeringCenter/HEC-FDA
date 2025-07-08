using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace VisualScratchSpace.Model.Saving;
public class PlotSaver : ISQLiteSaver
{
    private string _connectionString;
    private List<LifeLossFunction> _lifeLossFunction;

    public PlotSaver(string dbpath, List<LifeLossFunction> funcs)
    {
        _connectionString = $"Data Source={dbpath}"; ;
        _lifeLossFunction = funcs ;
    }

    public void SaveToSQLite()
    {
        CreateTables();
    }

    private void CreateTables()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = new SQLiteCommand(connection) { Transaction = transaction };

        foreach (LifeLossFunction function in _lifeLossFunction )
        {
            string tableName = $"{function.SimulationName}_{function.SummaryZone}_{function.HazardTime}";
            string createTableQuery =
                $@"CREATE TABLE IF NOT EXISTS ""{tableName}""(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Alternative TEXT NOT NULL, 
                Stage REAL NOT NULL);";
            command.CommandText = createTableQuery;
            command.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    private void InsertIntoTable(string tableName)
    {

    }
}
