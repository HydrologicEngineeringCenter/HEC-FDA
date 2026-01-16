using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace HEC.FDA.ViewModel.Storage
{
    public sealed class Connection
    {
        #region Fields
        private static SQLiteManager _SqliteReader = null;
        private const string TERRAIN_DIRECTORY = "Terrains";
        private const string HYDRAULIC_DIRECTORY = "Hydraulic Data";
        private const string PROJECTION_DIRECTORY = "Projection";
        private const string IMPACT_AREA_DIRECTORY = "Impact Areas";
        private const string INVENTORY_DIRECTORY = "Structure Inventories";
        private const string INDEX_POINTS_DIRECTORY = "Index Points";
        private const string STRUCTURE_STAGE_DAMAGE_DETAILS = "StructureStageDamageDetails";
        private const string LIFESIM_DIRECTORY = "LifeSim Data";

        private static string _ProjectDirectory = "";
        #endregion
        #region Properties
        public string ProjectFile
        {
            get
            {
                string projectPath = null;
                if (_SqliteReader != null)
                {
                    projectPath = _SqliteReader.DataBasePath;
                }
                return projectPath;
            }
            set
            {
                if (!File.Exists(value))
                {
                    SetUpForNewStudy(value);
                }
                else
                {
                    SetUpForExistingStudy(value);
                }
                _SqliteReader = new SQLiteManager(value);
            }
        }

        private void SetUpForExistingStudy(string value)
        {
            EnforceFolderStructure(value);
        }
        private void SetUpForNewStudy(string value)
        {
            EnforceFolderStructure(value);
            SQLiteManager.CreateSqLiteFile(value);
        }

        // value is the sqlite file path
        private void EnforceFolderStructure(string value)
        {
            _ProjectDirectory = Path.GetDirectoryName(value);
            if (!Directory.Exists(ProjectDirectory)) { Directory.CreateDirectory(ProjectDirectory); }
            if (!Directory.Exists(TerrainDirectory)) { Directory.CreateDirectory(TerrainDirectory); }
            if (!Directory.Exists(HydraulicsDirectory)) { Directory.CreateDirectory(HydraulicsDirectory); }
            if (!Directory.Exists(ProjectionDirectory)) { Directory.CreateDirectory(ProjectionDirectory); }
        }

        public SQLiteManager Reader
        {
            get { return _SqliteReader; }
        }
        public string ProjectDirectory
        {
            get { return _ProjectDirectory; }
        }

        public string TerrainDirectory
        {
            get { return _ProjectDirectory + "\\" + TERRAIN_DIRECTORY; }
        }
        public string HydraulicsDirectory
        {
            get { return _ProjectDirectory + "\\" + HYDRAULIC_DIRECTORY; }
        }
        public string ProjectionDirectory
        {
            get { return _ProjectDirectory + "\\" + PROJECTION_DIRECTORY; }
        }
        public string ProjectionFile
        {
            get
            {
                string[] projContents = Directory.GetFiles(ProjectionDirectory);
                if (projContents.Length > 0)
                {
                    return projContents[0];
                }
                else { return ""; }
            }
        }
        public string ImpactAreaDirectory
        {
            get { return _ProjectDirectory + "\\" + IMPACT_AREA_DIRECTORY; }
        }
        public string InventoryDirectory
        {
            get { return _ProjectDirectory + "\\" + INVENTORY_DIRECTORY; }
        }
        public string IndexPointsDirectory
        {
            get { return _ProjectDirectory + "\\" + INDEX_POINTS_DIRECTORY; }
        }

        public string LifeSimDirectory
        {
            get { return _ProjectDirectory + "\\" + LIFESIM_DIRECTORY; }
        }

        public string GetStructureStageDamageDetailsDirectory
        {
            get { return _ProjectDirectory + "\\" + STRUCTURE_STAGE_DAMAGE_DETAILS; }
        }

        public bool IsOpen { get { return _SqliteReader.DataBaseOpen; } }

        public bool IsConnectionNull
        {
            get { return _SqliteReader == null; }
        }

        public static readonly Connection Instance = new Connection();
        #endregion
        #region Constructors
        private Connection()
        {
        }
        #endregion
        #region Voids
        public void Open()
        {
            _SqliteReader.Open();
        }
        public void Close()
        {
            _SqliteReader.Close();
        }

        #region Cody's DB queries
        public void CreateTableWithPrimaryKey(string tablename, string[] colnames, Type[] coltypes)
        {
            if (_SqliteReader.GetTableNames().Contains(tablename))
            {
                throw new Exception("table already exists.");
            }
            SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
            command.CommandText = GetCreateTableWithPrimaryKeyText(tablename, colnames, coltypes);
            command.ExecuteNonQuery();
        }

        public void AddRowToTableWithPrimaryKey(object[] rowData, string tablename, string[] colnames)
        {
            SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
            command.CommandText = InsertIntoTableText(rowData, tablename, colnames);
            command.ExecuteNonQuery();
        }

        private string EscapeSingleQuotes(object value)
        {
            string returnValue = null;
            if (value != null)
            {
                string stringValue = value.ToString();
                returnValue = stringValue.Replace("'", "''");
            }
            return returnValue;
        }

        private string InsertIntoTableText(object[] rowData, string tablename, string[] colnames)
        {
            StringBuilder sb = new StringBuilder("INSERT INTO ")
            .Append(tablename).Append(" ( ");

            foreach (string colName in colnames)
            {
                sb.Append(colName).Append(",");

            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(") VALUES (");
            foreach (object obj in rowData)
            {
                sb.Append("'").Append(EscapeSingleQuotes(obj)).Append("',");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(");");
            return sb.ToString();
        }

        private string GetCreateTableWithPrimaryKeyText(string tablename, string[] colnames, Type[] coltypes)
        {
            // set up autoincrement ID
            StringBuilder sb = new StringBuilder("CREATE TABLE ")
            .Append(tablename).Append(" ( ")
            .Append("ID INTEGER PRIMARY KEY AUTOINCREMENT");
            // add column names + their types
            for (int i = 0; i < colnames.Length; i++)
            {
                sb.Append(", ").Append(colnames[i]).Append(" ");
                Type type = coltypes[i];
                if (type == typeof(string))
                {
                    sb.Append("TEXT");
                    continue;
                }

                if (type == typeof(DateTime))
                {
                    sb.Append("DATETIME");
                    continue;
                }

                if (type == typeof(byte) || type == typeof(sbyte))
                {
                    sb.Append("INT1");
                    continue;
                }

                if (type == typeof(short) || type == typeof(ushort))
                {
                    sb.Append("INT2");
                    continue;
                }

                if (type == typeof(int) || type == typeof(uint))
                {
                    sb.Append("INT4");
                    continue;
                }

                if (type == typeof(long) || type == typeof(ulong))
                {
                    sb.Append("INT8");
                    continue;
                }

                if (type == typeof(float))
                {
                    sb.Append("FLOAT");
                    continue;
                }

                if (type == typeof(double))
                {
                    sb.Append("DOUBLE");
                    continue;
                }

                if (type == typeof(decimal))
                {
                    sb.Append("NUMBER");
                    continue;
                }

                if (type == typeof(char))
                {
                    sb.Append("CHAR");
                    continue;
                }

                if (type == typeof(bool))
                {
                    sb.Append("BOOLEAN");
                    continue;
                }

                if (type == typeof(object) || type == typeof(byte[]))
                {
                    sb.Append("BLOB");
                    continue;
                }
                throw new Exception(coltypes[i].ToString() + " Not implemented, Column: " + colnames[i]);
            }
            sb.Append(");");

            return sb.ToString();

        }

        /// <summary>
        /// This is here to act in a very similar way to "GetTable" below which was not working well
        /// with tables that were using the autoincrementing id column.
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string tablename)
        {
            if (Connection.Instance.IsOpen != true)
            {
                Connection.Instance.Open();
            }
            DataTable tab = new DataTable();
            if (_SqliteReader.TableNames.Contains(tablename))
            {
                SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
                command.CommandText = "select * from " + tablename;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(tab);
            }

            return tab;
        }


        #endregion

        #endregion
        #region Functions

        public string[] TableNames()
        {
            return _SqliteReader.GetTableNames();
        }

        public object GetTable(string TableName)
        {

            if (IsConnectionNull)
            {
                return null;
            }
            if (_SqliteReader.GetTableNames().Contains(TableName))
            {
                // we just need to return something not null, there is definitely a better way
                return new object();
            }
            else
            {
                return null;
            }

        }

        public bool TableExists(string tableName)
        {
            return GetTable(tableName) != null;
        }
        #endregion
    }
}
