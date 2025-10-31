using HEC.FDA.Model.Spatial;
using System.Data.SQLite;

namespace ScratchSpace.EntryPoints;
internal class Schonherr
{
    public static void EntryPoint()
    {
        SQLiteConnection conn = new(@"Data Source=C:\FDA_Test_Data\WKS20230525\WKS20230525\save-test.db");
        conn.Open();
        using var pragma = new SQLiteCommand("PRAGMA foreign_keys = ON;", conn); // have to do this for every connection if using multiple
        pragma.ExecuteNonQuery();

        Insert(conn);
        //DeleteFromMetadata(conn);

        conn.Close();
    }

    private static void Insert(SQLiteConnection conn)
    {
        using var createcmd = new SQLiteCommand(conn);
        createcmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Metadata (
                Id   INTEGER PRIMARY KEY AUTOINCREMENT,
                Xml  TEXT    NOT NULL
            );";
        createcmd.ExecuteNonQuery();

        for (int i = 0; i < 8; i++)
        {
            InsertIntoMetadata(conn, $"xml string {i}");
        }

        var rnd = new Random();
        for (int i = 1; i <= 16; i++)
        {
            int id = (i + 1) / 2;
            SaveCurves(conn, id, rnd.NextDouble(), rnd.NextDouble());
        }
    }

    private static void InsertIntoMetadata(SQLiteConnection conn, string xml)
    {
        using var insertcmd = new SQLiteCommand(conn);
        insertcmd.CommandText = @"INSERT INTO Metadata (Xml) VALUES(@xml)";
        insertcmd.Parameters.AddWithValue("@xml", xml);
        insertcmd.ExecuteNonQuery();
    }

    private static void SaveCurves(SQLiteConnection conn, int id, double x, double y)
    {
        using var createcmd = new SQLiteCommand(conn);
        createcmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Curves (
                CurveId     INTEGER PRIMARY KEY AUTOINCREMENT,
                MetadataId  INTEGER NOT NULL,
                X           REAL    NOT NULL,
                Y           REAL    NOT NULL,
                FOREIGN KEY(MetadataId) REFERENCES Metadata(Id) ON DELETE CASCADE
            );";
        createcmd.ExecuteNonQuery();

        using var insertcmd = new SQLiteCommand(conn);
        insertcmd.CommandText = @"INSERT INTO Curves (MetadataId, X, Y) VALUES (@mid, @x, @y);";
        insertcmd.Parameters.AddWithValue("@mid", id);
        insertcmd.Parameters.AddWithValue("@x", x);
        insertcmd.Parameters.AddWithValue("@y", y);
        insertcmd.ExecuteNonQuery();
    }

    private static void DeleteFromMetadata(SQLiteConnection conn)
    {
        using var deletecmd = new SQLiteCommand(conn);
        deletecmd.CommandText = "DELETE FROM Metadata WHERE Id IN (1,2);";
        deletecmd.ExecuteNonQuery();
    }

    private void FDABadOutput()
    {
        ShapefileHelper bad = new(@"C:\FDA_Test_Data\WKS20230525\WKS20230525\re-exported.shp");
        var badColumns = bad.GetColumns();
        foreach (string kvp in badColumns)
        {
            Console.WriteLine(kvp);
        }

        Console.WriteLine("\r\nName:");
        var nameValues = bad.GetColumnValues("Name");
        foreach (string name in nameValues) Console.WriteLine(name);

        Console.WriteLine("\r\nfid:");
        var fidValues = bad.GetColumnValues("fid");
        foreach (string fid in fidValues) Console.WriteLine(fid);
    }
}
