using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseManager;

namespace FdaModel
{
    public sealed class Project
    {
        #region Fields
        private static Project _Instance;
        private string _FilePath;
        private string _Name;
        private string _Description;
        private string _User;
        private DateTime _CreationDate;
        private List<string> _Notes;
        #endregion

        #region Properties
        public static Project Instance
        {
            get
            {
                return _Instance;
            }
        }
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }
        public string User
        {
            get
            {
                return _Name;
            }
            private set
            {
                _Name = value;
            }
        }
        public DateTime CreationDate
        {
            get
            {
                return _CreationDate;
            }
            private set
            {
                _CreationDate = value;
            }
        }
        public List<string> Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                _Notes = value;
            }
        }
        #endregion

        #region Constructor
        private Project(string filePath, string projectName,  string projectDescription, List<string> projectNotes)
        {
            FilePath = filePath;
            Name = projectName;
            Description = projectDescription;
            User = Environment.UserName;
            CreationDate = DateTime.Now;
            Notes = projectNotes;
        }
        private Project(DataTableView dataTable)
        {
            foreach (var property in GetType().GetProperties())
            {
                property.SetValue(this, ReadPropertyData(dataTable, property, property.GetType()));
            }
        }
        #endregion


        #region Functions
        public static Project ProjectSingleton(string filePath, string projectName, string projectDescription = "", List<string> projectNotes = null)
        {
            if (_Instance == null)
            {
                if (projectNotes == null)
                {
                    projectNotes = new List<string>();
                }

                _Instance = new Project(filePath, projectName, projectDescription, projectNotes);
            }
            return _Instance;
        }
        public static Project ProjectSingleton(DataTableView dataTable)
        {
            if (_Instance == null)
            {
                _Instance = new Project(dataTable);
            }
            return _Instance;
        }
        #endregion


        private dynamic ReadPropertyData(DataTableView table, PropertyInfo property, dynamic type)
        {
            int i = Array.IndexOf(table.ColumnNames, property.Name);
            return table.GetColumn(i);
        }

        public void WriteData()
        {
            SQLiteManager database = new SQLiteManager(FilePath + Name + ".sqlite");
            string[] tableNames = new string[2] 
            {
                GetType().ToString(),
                GetType().ToString()+"Notes"
            };

            for (int i = 0; i < tableNames.Length; i++)
            {
                if (database.GetTableNames().Contains(tableNames[i]))
                {
                    database.DeleteTable(tableNames[i]);
                }

                object[][] metaData = new object[2][];
                List<object[]> tableData = new List<object[]>();
                switch (i)
                {
                    case 0:
                        PropertyInfo[] mainTableProperties = new PropertyInfo[5]
                        {
                            GetType().GetProperty(nameof(FilePath)),
                            GetType().GetProperty(nameof(Name)),
                            GetType().GetProperty(nameof(Description)),
                            GetType().GetProperty(nameof(User)),
                            GetType().GetProperty(nameof(CreationDate))
                        };
                        tableData = GetTableRows(mainTableProperties, out metaData);
                        break;
                    case 1:
                        PropertyInfo[] studyNotesTableProperties = new PropertyInfo[1]
                        {
                            GetType().GetProperty(nameof(Notes))
                        };
                        tableData = GetTableRows(studyNotesTableProperties, out metaData);
                        break;
                }

                database.CreateTable(tableNames[i], metaData[0].Cast<string>().ToArray(), metaData[1].Cast<Type>().ToArray());
                DataTableView table = database.GetTableManager(tableNames[i]);
                table.AddRows(tableData);
            }
        }

        public List<object[]> GetTableRows(PropertyInfo[] tableProperties, out object[][] metaData)
        {
            metaData = new object[2][];
            List<object[]> tableData = new List<object[]>();
            
            object[] rowData = new object[tableProperties.Length];                      // length of row is one entry per column (e.g. property)
            int nRows = ((Array)tableProperties[0].GetValue(this)).Length;              // number of rows depends on length of the property data.

            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < tableProperties.Length; j++)
                {
                    if (i == 0)
                    {
                        if (nRows != ((Array)tableProperties[j].GetValue(this)).Length) // make sure the result will be a rectangular matrix.
                        {
                            throw new Exception("The properties being written the sqlite database are not of equal length, therefore a rectangular table cannot be constructed.");
                        }
                        else
                        {
                            metaData[0][j] = tableProperties[j].Name;
                            metaData[1][j] = tableProperties[j].GetType();
                        }
                    }
                    rowData[j] = tableProperties[j].GetValue(i);
                }
                tableData.Add(rowData);
            }
            return tableData;
        }

        public List<object[]> GetTableColumns(PropertyInfo[] tableProperties, out object[][] metaData)
        {
            Array columnData;
            metaData = new object[2][];
            List<object[]> columnDatas = new List<object[]>();

            int length = ((Array)tableProperties[0].GetValue(this)).Length;
            for (int i = 0; i < tableProperties.Length; i++)
            {
                columnData = (Array)tableProperties[i].GetValue(this);
                if (length == columnData.Length)
                {
                    metaData[0][i] = tableProperties[i].Name;
                    metaData[1][i] = tableProperties[i].GetType();
                    for (int j = 0; j < columnData.Length; j++)
                    {
                        columnDatas[i][j] = columnData.GetValue(j);
                    }
                    i++;
                }
                else
                {
                    //error not a rectangular matrix.
                }
            }
            return columnDatas;
        }

        

        public System.Data.DataTable GetDataTable()
        {
            using (System.Data.DataTable table = new System.Data.DataTable())
            {
                throw new NotImplementedException();
            }
        }

    }
}
