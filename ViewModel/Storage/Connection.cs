using System;
using System.Collections.Generic;
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
        private static DatabaseManager.SQLiteManager _SqliteReader = null;
        private const string TERRAIN_DIRECTORY = "Terrains";
        private const string HYDRAULIC_DIRECTORY = "Hydraulic Data";
        private const string IMPACT_AREA_DIRECTORY = "Impact Areas";
        private const string INDEX_POINTS_DIRECTORY = "Index Points";

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
                if (_SqliteReader == null)
                {
                    if (!File.Exists(value))
                    {
                        SetUpForNewStudy(value);
                    }
                    else
                    {
                        SetUpForExistingStudy(value);
                    }
                    _SqliteReader = new DatabaseManager.SQLiteManager(value);
                }
                else
                {
                    if (!File.Exists(value))
                    {
                        SetUpForNewStudy(value);
                    
                    }
                    else
                    {
                        SetUpForExistingStudy(value);
                        
                    }                    
                    _SqliteReader = new DatabaseManager.SQLiteManager(value);
                }
            }
        }

        private void SetUpForExistingStudy(string value)
        {
            _ProjectDirectory = Path.GetDirectoryName(value);
            if (!Directory.Exists(ProjectDirectory)) { Directory.CreateDirectory(ProjectDirectory); }
            if (!Directory.Exists(TerrainDirectory)) { Directory.CreateDirectory(TerrainDirectory); }
            if (!Directory.Exists(HydraulicsDirectory)) { Directory.CreateDirectory(HydraulicsDirectory); }
        }
        private void SetUpForNewStudy(string value)
        {
            _ProjectDirectory = Path.GetDirectoryName(value);
            if (!Directory.Exists(ProjectDirectory)) { Directory.CreateDirectory(ProjectDirectory); }
            if (!Directory.Exists(TerrainDirectory)) { Directory.CreateDirectory(TerrainDirectory); }
            if (!Directory.Exists(HydraulicsDirectory)) { Directory.CreateDirectory(HydraulicsDirectory); }
            DatabaseManager.SQLiteManager.CreateSqLiteFile(value);         
        }

        public DatabaseManager.SQLiteManager Reader
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
        /// <summary>
        /// Returns the first file in the terrain directory with the name provided. If it can't
        /// find it, it returns null;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetTerrainFile(string name)
        {
            string[] files = Directory.GetFiles(Connection.Instance.TerrainDirectory, name + ".*");
            if (files.Length > 0)
            {
                return files[0];
            }
            else
            {
                return null;
            }
        }
        public string HydraulicsDirectory
        {
            get { return _ProjectDirectory + "\\" + HYDRAULIC_DIRECTORY; }
        }
        public string ImpactAreaDirectory
        {
            get { return _ProjectDirectory + "\\" + IMPACT_AREA_DIRECTORY; }

        }
        public string IndexPointsDirectory
        {
            get { return _ProjectDirectory + "\\" + INDEX_POINTS_DIRECTORY; }

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
        public void RenameTable(string oldTableName, string newTableName)
        {
            //check table exists
            if (_SqliteReader.TableNames.Contains(oldTableName))
            {
                _SqliteReader.RenameTable(oldTableName, newTableName);

            }
        }
        //public void RenameGeoPackageTable(string oldName, string newName)
        //{
        //    if (_SqliteReader.TableNames.Contains(oldName))
        //    {
        //        LifeSimGIS.GeoPackageWriter geoWriter = new LifeSimGIS.GeoPackageWriter(_SqliteReader);
        //        geoWriter.RenameFeatures(oldName, newName);
        //    }
        //}
        
        public void DeleteTable(string tableName)
        {
            _SqliteReader.DeleteTable(tableName);
        }
        public void CreateTable(string tablename, string[] colnames, Type[] coltypes)
        {
            _SqliteReader.CreateTable(tablename, colnames, coltypes);
        }
        #region Cody's DB queries
        public void CreateTableWithPrimaryKey(string tablename, string[] colnames, Type[] coltypes)
        {
            if(_SqliteReader.GetTableNames().Contains(tablename))
            {
                throw new Exception("table already exists.");
            }
            SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
            command.CommandText = GetCreateTableWithPrimaryKeyText(tablename, colnames, coltypes);
            command.ExecuteNonQuery();   
        }

        public void AddRowToTableWithPrimaryKey(object[] rowData, string tablename,  string[] colnames)
        {
            SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
            command.CommandText = InsertIntoTableText(rowData, tablename, colnames);
            command.ExecuteNonQuery();
        }

        //public void AddNewColumnWithValues(string tableName, string colName, string colType, object defaultValue = null)
        //{
        //    string cmdText = "";
        //    if (defaultValue == null)
        //    {
        //        cmdText = "ALTER TABLE " + tableName + " ADD " + colName + " " + colType;

        //    }
        //    else
        //    {
        //        cmdText = "ALTER TABLE " + tableName + " ADD " + colName + " " + colType + " DEFAULT '" + defaultValue + "'";
        //    }
        //    SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
        //    command.CommandText = cmdText;
        //    command.ExecuteNonQuery();
        //}

        private string EscapeSingleQuotes(object value)
        {
            string returnValue = null;
            if (value != null)
            {
                returnValue = value.ToString().Replace("'", "''");
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
            foreach(object obj in rowData)
            {
                sb.Append("'").Append(EscapeSingleQuotes(obj)).Append("',");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(");");
            return sb.ToString();
        }

        private string GetCreateTableWithPrimaryKeyText(string tablename, string[] colnames, Type[] coltypes)
        {
            StringBuilder sb = new StringBuilder("CREATE TABLE ")
            .Append(tablename).Append(" ( ")
            .Append("ID INTEGER PRIMARY KEY AUTOINCREMENT");
            //todo: change id to use constant
            foreach (string colName in colnames)
            {
                sb.Append(", ").Append(colName);

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
        public DatabaseManager.DataTableView GetTable(string TableName)
        {

            if (IsConnectionNull)
            {
                return null;
            }
            if (_SqliteReader.GetTableNames().Contains(TableName))
            {
                return _SqliteReader.GetTableManager(TableName);
            }else
            {
                return null;
            }

        }
        #endregion
    }
}
