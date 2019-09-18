using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
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
                NLogDataBaseHelper.CreateDBTargets(value);
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
            if (IsConnectionNull)
            {
                FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Saving edits failed, the directory and file name have not been set",
                    FdaModel.Utilities.Messager.ErrorMessageEnum.Model | FdaModel.Utilities.Messager.ErrorMessageEnum.Minor));
            }
        }
        public void DeleteTable(string tableName)
        {
            _SqliteReader.DeleteTable(tableName);
        }
        public void CreateTable(string tablename, string[] colnames, Type[] coltypes)
        {
            _SqliteReader.CreateTable(tablename, colnames, coltypes);
        }
        #endregion
        #region Functions
        //private string createTempFile()
        //{
        //    string assmb = System.Reflection.Assembly.GetAssembly(typeof(Connection)).GetName().Name;
        //    assmb = System.IO.Path.GetTempPath() + "HEC\\" + assmb + "_Temp.sqlite";
        //    return assmb;
        //}
        public string[] TableNames()
        {
            return _SqliteReader.GetTableNames();
        }
        public DatabaseManager.DataTableView GetTable(string TableName)
        {

            if (IsConnectionNull)
            {
                FdaModel.Utilities.Messager.Logger.Instance.ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("Table " + TableName + " requested from a null project", FdaModel.Utilities.Messager.ErrorMessageEnum.Model | FdaModel.Utilities.Messager.ErrorMessageEnum.Minor));
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
