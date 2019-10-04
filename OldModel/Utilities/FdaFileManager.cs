using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaModel.Utilities
{
    public sealed class FdaFileManager
    {
        #region Notes
        #endregion
        #region Fields
        private static object _WriteLock = new object();
        private static DataBase_Reader.SqLiteReader _SqliteReader; // consider a list of readers so i can create a reader pool.
        #endregion
        #region Properties
        public string ProjectFile
        {
            get { return _SqliteReader.DataBasePath; }//check if _sqliteReader exists?
            set
            {
                if (_SqliteReader == null)//the likelihood of this should be nil.
                {
                    if (!System.IO.File.Exists(value))
                    {
                        DataBase_Reader.SqLiteReader.CreateSqLiteFile(value);
                    }else
                    {
                        //do nothing.
                    }
                    
                }else
                {
                    //dispose of the old guy? and link to the new one?
                    //if the filepath is new, and the old filepath existed should i perform the copy here?
                    if (!System.IO.File.Exists(value))
                    {
                        //
                        System.IO.File.Copy(_SqliteReader.DataBasePath, value);
                        System.IO.File.Delete(_SqliteReader.DataBasePath);
                        //DataBase_Reader.SqLiteReader.CreateSqLiteFile(value);
                    }
                    else
                    {
                        //??
                        
                    }
                }
                _SqliteReader = new DataBase_Reader.SqLiteReader(value);
                _SqliteReader.EditsSaved += _SqliteReader_EditsSaved;
                //assign table edit event handler
            }
        }

        public bool IsOpen { get { return _SqliteReader.DataBaseOpen; } }


        public bool IsLocatedInTempDirectory
        {
            get { return ProjectFile == createTempFile(); }
        }
        public static readonly FdaFileManager Instance = new FdaFileManager();
        #endregion
        #region Constructors
        private FdaFileManager()
        {
            //create a temporary sqlite file
            
            if (_SqliteReader == null)
            {
                string tmpfile = createTempFile();
                if (System.IO.File.Exists(tmpfile)) System.IO.File.Delete(tmpfile);
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(tmpfile))) System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(tmpfile));
                DataBase_Reader.SqLiteReader.CreateSqLiteFile(tmpfile);
                _SqliteReader = new DataBase_Reader.SqLiteReader(tmpfile);
                _SqliteReader.EditsSaved += _SqliteReader_EditsSaved;
                //add a handler for datatable editing.
            }
        }
        #endregion
        #region Voids
        public void Open()
        {
            _SqliteReader.Open();
        }
        private void _SqliteReader_EditsSaved(string tableName)
        {
            if (IsLocatedInTempDirectory)
            {
                Utilities.Messager.Logger.Instance.ReportMessage(new Messager.ErrorMessage("Saving edits to " + tableName + " from the temp file at " + _SqliteReader.DataBasePath, Messager.ErrorMessageEnum.Model | Messager.ErrorMessageEnum.Minor));
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
        private string createTempFile()
        {
            string assmb = System.Reflection.Assembly.GetAssembly(typeof(FdaFileManager)).GetName().Name;
            assmb = System.IO.Path.GetTempPath() + "HEC\\" + assmb + "_Temp.sqlite";
            return assmb;
        }
        public string[] TableNames()
        {
            return _SqliteReader.GetTableNames();
        }
        public DataBase_Reader.DataTableView GetTable(string TableName)
        {
            
            if (IsLocatedInTempDirectory)
            {
                Utilities.Messager.Logger.Instance.ReportMessage(new Messager.ErrorMessage("Getting the table " + TableName + " from the temp file at " + _SqliteReader.DataBasePath, Messager.ErrorMessageEnum.Model | Messager.ErrorMessageEnum.Minor));
            }
            return _SqliteReader.GetTableManager(TableName);
        }
        #endregion
    }
}
