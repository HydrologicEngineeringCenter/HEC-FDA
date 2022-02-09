using FdaLogging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using ViewModel.Storage;
using ViewModel.Utilities;

namespace ViewModel.Saving
{
    public abstract class SavingBase : BaseViewModel, IElementManager
    {
        /// <summary>
        /// The FDACache stores all the elements in memory. 
        /// </summary>
        public Study.FDACache StudyCacheForSaving { get; set; }
        public abstract string TableName { get; }
        public const string ID_COL_NAME = "id";
        public const string NAME = "name";
        public const string ELEMENT_ID_COL_NAME = "elem_id";
        public const string LAST_EDIT_DATE = "last_edit_date";
        public const string DESCRIPTION = "description";
        public const string CURVE_DISTRIBUTION_TYPE = "curve_distribution_type";
        public const string CURVE_TYPE = "curve_type";
        public const string CURVE = "curve";

        public const int NAME_COL = 1;

        public abstract string[] TableColumnNames { get; }
        public abstract Type[] TableColumnTypes { get; }
        public abstract object[] GetRowDataFromElement(ChildElement elem);

        #region Utilities

        public List<ChildElement> CreateElementsFromRows(string tableName, Func<object[], ChildElement> createElemsFromRowDataAction)
        {
            OpenConnection();
            List<ChildElement> elems = new List<ChildElement>();

            DataTable table = Connection.Instance.GetDataTable(tableName);
            foreach (DataRow row in table.Rows)
            {
                elems.Add(createElemsFromRowDataAction(row.ItemArray));
            }
            return elems;
        }

        private int GetElementIndexInTable(DataTable tableView, string name, int nameIndexInTheRow)
        {
            if (tableView != null)
            {
                DataRowCollection rows = tableView.Rows;

                for (int i = 0; i < rows.Count; i++)
                {
                    if (((string)rows[i][nameIndexInTheRow]).Equals(name))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        #endregion

        #region save new
        public void SaveNew(ChildElement element)
        {
            OpenConnection();
            //update the edit date
            string editDate = DateTime.Now.ToString("G");
            element.LastEditDate = editDate;
            //save to parent table
            SaveNewElementToParentTable(GetRowDataFromElement(element), TableName, TableColumnNames, TableColumnTypes);
            //add the element to the study cache
            StudyCacheForSaving.AddElement(element);
        }
        public void SaveNewElementToParentTable(object[] rowData, string tableName, string[] TableColumnNames, Type[] TableColumnTypes)
        {
            OpenConnection();
            DatabaseManager.DataTableView tbl = Connection.Instance.GetTable(tableName);
            if (tbl == null)
            {
                Connection.Instance.CreateTableWithPrimaryKey(tableName, TableColumnNames, TableColumnTypes);
            }

            Connection.Instance.AddRowToTableWithPrimaryKey(rowData, tableName, TableColumnNames);
        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave)
        {
            OpenConnection();
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            //this updates the parent table
            int id = GetElementId(TableName, oldElement.Name);
            UpdateTableRow(TableName, id, ID_COL_NAME, TableColumnNames, GetRowDataFromElement(elementToSave));

            //make this so that i can pass in "childElement" and have it updated
            StudyCacheForSaving.UpdateElement(oldElement, elementToSave);
        }

        #endregion

        private void OpenConnection()
        {
            if (!Connection.Instance.IsOpen)
            {
                Connection.Instance.Open();
            }
        }

        #region Remove element
        public void RemoveTable(string tableName)
        {
            OpenConnection();
            if (Connection.Instance.TableNames().Contains(tableName))
            {
                Connection.Instance.DeleteTable(tableName);                
            }
        }
        public void RemoveFromGeopackageTable(string tableName)
        {
            LifeSimGIS.GeoPackageWriter gpw = new LifeSimGIS.GeoPackageWriter(Connection.Instance.Reader);
            gpw.DeleteFeatures(tableName);
        }
        public virtual void RemoveFromParentTable(ChildElement element, string tableName)
        {
            OpenConnection();

            element.RemoveElementFromMapWindow(this, new EventArgs());

            if (Connection.Instance.TableNames().Contains(tableName))
            {
                DatabaseManager.DataTableView parentTableView = Connection.Instance.GetTable(tableName);
                if (parentTableView != null)
                {
                    DataTable dt = Connection.Instance.GetDataTable(tableName);
                    int parentTableIndex = GetElementIndexInTable(dt, element.Name, 1);
                    if (parentTableIndex != -1)
                    {
                        parentTableView.DeleteRow(parentTableIndex);
                        parentTableView.ApplyEdits();
                    }
                }
            }
        }

        #endregion

        #region save existing

        /// <summary>
        /// This sends a sql "update" command to the database.
        /// </summary>
        /// <param name="tableName">The name of the sqlite table</param>
        /// <param name="primaryKey">The id of the element. The column that the id is in must be "ID"</param>
        /// <param name="columns">The columns that you want to update</param>
        /// <param name="values">The values that you want in the columns listed in "columns"</param>
        public void UpdateTableRow(string tableName, int primaryKey, string primaryKeyColName, string[] columns, object[] values)
        {
            OpenConnection();
            //columns and values need to be corespond to each other, you don't have to update columns that don't need it
            StringBuilder sb = new StringBuilder("update ").Append(tableName).Append(" set ");
            for(int i = 0;i<columns.Length;i++)
            {
                sb.Append(columns[i]).Append(" = '").Append(values[i]).Append("' ").Append(",");
            }
            //get rid of last comma
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" where ").Append(primaryKeyColName).Append(" = ").Append(primaryKey);

            SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// This sends a sql "update" command to the database.
        /// </summary>
        /// <param name="tableName">The name of the sqlite table</param>
        /// <param name="primaryKey">The id of the element. The column that the id is in must be "ID"</param>
        /// <param name="columns">The columns that you want to update</param>
        /// <param name="values">The values that you want in the columns listed in "columns"</param>
        public void UpdateTableRowWithCompoundKey(string tableName, object[] primaryKeys, string[] primaryKeyColNames, string[] columns, object[] values)
        {
            //this sql query looks like this:
            //update occupancy_types set Name = 'codyistesting' where GroupID = 1 and OcctypeID = 1
            OpenConnection();
            //columns and values need to be corespond to each other, you don't have to update columns that don't need it
            StringBuilder sb = new StringBuilder("update ").Append("'").Append(tableName).Append("' set ");
            for (int i = 0; i < columns.Length; i++)
            {
                sb.Append(columns[i]).Append(" = '").Append(values[i]).Append("' ").Append(",");
            }
            //get rid of last comma
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" where ");
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                sb.Append(primaryKeyColNames[i]).Append(" = ").Append("'").Append(primaryKeys[i]).Append("' and ");
            }
            //remove the last "and"
            sb.Remove(sb.Length - 4, 4);

            SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();
        }

        public void DeleteRowWithCompoundKey(string tableName, int[] primaryKeys, string[] primaryKeyColNames)
        {
            //this sql query looks like this:
            //delete from occupancy_types where GroupID = 1 and OcctypeID = 27
            OpenConnection();
            StringBuilder sb = new StringBuilder("delete from ").Append(tableName).Append(" where ");
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                sb.Append(primaryKeyColNames[i]).Append(" = ").Append(primaryKeys[i]).Append(" and ");
            }
            //remove the last "and"
            sb.Remove(sb.Length - 4, 4);

            SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();
        }

        public void DeleteRowWithKey(string tableName, int key, string keyColName)
        {
            //this sql query looks like this:
            //delete from occupancy_types where GroupID = 1
            OpenConnection();
            //if the table doesn't exist, then there is nothing to delete
            if(Connection.Instance.GetTable(tableName) == null)
            {
                return;
            }

            StringBuilder sb = new StringBuilder("delete from ").Append(tableName).Append(" where ").Append(keyColName).Append(" = ").Append(key);
            SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();
        }

        public bool DidParentTableRowValuesChange(ChildElement element, object[] rowData, string oldName, string tableName)
        {
            if (!Connection.Instance.IsOpen) { Connection.Instance.Open(); }
            DatabaseManager.DataTableView tableView = Connection.Instance.GetTable(tableName);
            DataTable dt = Connection.Instance.GetDataTable(tableName);
            int rowIndex = GetElementIndexInTable(dt, oldName, 1);
            if (rowIndex != -1)
            {
                //has the name changed
                if (!oldName.Equals(element.Name))
                {                
                    return true;
                }
                //has anything else in the row changed
                return AreListsDifferent(rowData, tableView.GetRow(rowIndex));
            }
            return true;
        }

        private bool AreListsDifferent(object[] a, object[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                //don't evaluate the last edit time which is the second one. - i am getting rid of this which means it will always save because
                //the conditions (1) is the description.
                if (a[i] == null)
                {
                    a[i] = "";
                }
                if (!a[i].ToString().Equals(b[i].ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        abstract public ChildElement CreateElementFromRowData(object[] rowData);

        internal virtual string ChangeTableConstant { get { return ""; } }

        /// <summary>
        /// Gets the ID for the element with the name provided. Note that the table column name
        /// must be "Name" for this to work.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public int GetElementId(string tableName, string elementName)
        {
            int retval = -1;
            try
            {
                SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
                command.CommandText = "select ID from " + tableName + " where Name = '" + elementName + "'";
                object id = command.ExecuteScalar();
                if (id == null)
                {
                    retval = -1;
                }
                else
                {
                    retval = Convert.ToInt32(id);
                }
                return retval;
            }
            catch(Exception e)
            {
                //todo: some message? Name doesn't exist in the database.
                retval = -1;
            }
            return retval;
        }

        public void Remove(ChildElement element)
        {
            StudyCacheForSaving.RemoveElement(element);
            RemoveFromParentTable(element, TableName);
        }

        public abstract void Load();

        public virtual  void Log(LoggingLevel level, string message, string elementName)
        {

        }

        public virtual ObservableCollection<LogItem> GetLogMessages(string elementName)
        {
            return new ObservableCollection<LogItem>();
        }

        public virtual ObservableCollection<LogItem> GetLogMessagesByLevel(LoggingLevel level, string elementName)
        {
            return new ObservableCollection<LogItem>();
        }

    }
}
