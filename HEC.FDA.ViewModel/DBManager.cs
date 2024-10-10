using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel;
public class DBManager
{
    private SQLiteConnection _dbConnection;

    public void SetDatabaseConnection(string fileName, SQLiteConnectionStringBuilder connectionBuilder = null)
    {
        connectionBuilder.DataSource = fileName;
        _dbConnection = new SQLiteConnection(connectionBuilder.ToString());
    }
}
