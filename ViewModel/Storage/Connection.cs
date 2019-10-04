using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Storage
{
    public sealed class Connection
    {
        #region Notes
        #endregion
        #region Fields
        private static object _WriteLock = new object();
        private static DatabaseManager.SQLiteManager _SqliteReader = null; // consider a list of readers so i can create a reader pool.
        private const string _TerrainFolderName = "Terrains";
        private const string _HydraulicsFolderName = "Hydraulic Data";
        private static string _ProjectDirectory = "";
        #endregion
        #region Properties
        public string ProjectFile
        {
            get { return _SqliteReader.DataBasePath; }
            set
            {
                if (_SqliteReader == null)
                {
                    if (!System.IO.File.Exists(value))
                    {
                        SetUpForNewStudy(value);
                    }
                    else
                    {
                        SetUpForExistingStudy(value);
                    }
                    _SqliteReader = new DatabaseManager.SQLiteManager(value);
                    _SqliteReader.EditsSaved += _SqliteReader_EditsSaved;
                }
                else
                {
                    if (!System.IO.File.Exists(value))
                    {
                        SetUpForNewStudy(value);
                    
                    }
                    else
                    {
                        SetUpForExistingStudy(value);
                        
                    }                    
                    _SqliteReader = new DatabaseManager.SQLiteManager(value);
                    _SqliteReader.EditsSaved += _SqliteReader_EditsSaved;
                }
                //add a logging target for the sqlite db.
                FdaLogging.Initializer.Initialize(_SqliteReader);
                //NLogDataBaseHelper.CreateDBTargets(value);
            }
        }

        private void SetUpForExistingStudy(string value)
        {
            _ProjectDirectory = System.IO.Path.GetDirectoryName(value);
            if (!Directory.Exists(ProjectDirectory)) { Directory.CreateDirectory(ProjectDirectory); }
            if (!Directory.Exists(TerrainDirectory)) { Directory.CreateDirectory(TerrainDirectory); }
            if (!Directory.Exists(HydraulicsDirectory)) { Directory.CreateDirectory(HydraulicsDirectory); }
        }
        private void SetUpForNewStudy(string value)
        {
            _ProjectDirectory = System.IO.Path.GetDirectoryName(value);
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
            get { return _ProjectDirectory + "\\" + _TerrainFolderName; }
        }
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
            get { return _ProjectDirectory + "\\" + _HydraulicsFolderName; }
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
        public void RenameGeoPackageTable(string oldName, string newName)
        {
            if (_SqliteReader.TableNames.Contains(oldName))
            {
                LifeSimGIS.GeoPackageWriter geoWriter = new LifeSimGIS.GeoPackageWriter(_SqliteReader);
                geoWriter.RenameFeatures(oldName, newName);
            }
        }
        private void _SqliteReader_EditsSaved(string tableName)
        {
            //if (IsConnectionNull)
            //{
            //    FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Saving edits failed, the directory and file name have not been set",
            //        FdaModel.Utilities.Messager.ErrorMessageEnum.Model | FdaModel.Utilities.Messager.ErrorMessageEnum.Minor));
            //}
        }
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
           // string text = "CREATE TABLE people (person_id INTEGER PRIMARY KEY AUTOINCREMENT, first_name text NOT NULL,last_name text NOT NULL);";

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
                sb.Append("'").Append(obj).Append("',");
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

        public void GetRowDataFromTable(string tablename, int row)
        {
            //SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
            //command.CommandText = InsertIntoTableText(rowData, tablename, colnames);
            //command.ExecuteNonQuery();
        }

        /// <summary>
        /// This is here to act in a very similar way to "GetTable" below which was not working well
        /// with tables that were using the autoincrementing id column.
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataTable(string tablename)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            System.Data.DataTable tab = new System.Data.DataTable();
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

        public int GetMaxStateIndex(string tableName, int elementId, string elementIdColName, string stateIndexName)
        {
            DataTable tab = new DataTable();

            SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
            command.CommandText = "select MAX(" + stateIndexName + ") AS maxValue from " +
                tableName + " where " + elementIdColName + " = " + elementId;
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            adapter.Fill(tab);
            if (tab.Rows.Count == 1 && tab.Rows[0].ItemArray[0] != DBNull.Value)
            {
                object[] row = tab.Rows[0].ItemArray;
                return Convert.ToInt32(row[0]);
            }
            else
            {
                //There is nothing in the db. 
                return -1;
            }
        }

        public DataTable GetRowsWithIDValue(int value, string columnName, string tableName)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            DataTable tab = new DataTable();
            if (_SqliteReader.TableNames.Contains(tableName))
            {
                List<object[]> rows = new List<object[]>();
                SQLiteCommand command = _SqliteReader.DbConnection.CreateCommand();
                command.CommandText = "select * from " + tableName + " where " + columnName + " = " + value;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(tab);
            }
            return tab;
        }

        

        

        public string[] TableNames()
        {
            return _SqliteReader.GetTableNames();
        }
        public DatabaseManager.DataTableView GetTable(string TableName)
        {

            if (IsConnectionNull)
            {
                FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Table " + TableName + " requested from a null project", FdaModel.Utilities.Messager.ErrorMessageEnum.Model | FdaModel.Utilities.Messager.ErrorMessageEnum.Minor));
                return null;
            }
            if (_SqliteReader.TableNames.Contains(TableName))
            {
                return _SqliteReader.GetTableManager(TableName);
            }else
            {
                FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Table " + TableName + " requested from a database that doesnt contain the table", FdaModel.Utilities.Messager.ErrorMessageEnum.Model | FdaModel.Utilities.Messager.ErrorMessageEnum.Minor));
                return null;
            }

        }
        #endregion
    }
}
