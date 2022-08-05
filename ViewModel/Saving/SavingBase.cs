using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace HEC.FDA.ViewModel.Saving
{
    public abstract class SavingBase : BaseViewModel, IElementManager
    {
        /// <summary>
        /// The FDACache stores all the elements in memory. 
        /// </summary>
        public Study.FDACache StudyCacheForSaving { get; set; }
        public abstract string TableName { get; }

        public const string NAME = "Name";
        public const string XML = "XML";
        public const string ID_COL_NAME = "ID";
        //public const string NAME = "name";
        //public const string ELEMENT_ID_COL_NAME = "elem_id";
        //public const string LAST_EDIT_DATE = "last_edit_date";
        //public const string DESCRIPTION = "description";
        //public const string CURVE_DISTRIBUTION_TYPE = "curve_distribution_type";
        //public const string CURVE_TYPE = "curve_type";
        //public const string CURVE = "curve";

        public const int ID_COL = 0;
        public const int XML_COL = 1;

        public virtual string[] TableColumnNames { get; } = new string[] {XML};
        public virtual Type[] TableColumnTypes { get; } = new Type[] {typeof(string)};
        public virtual object[] GetRowDataFromElement(ChildElement elem)
        {
            return new object[] { elem.ToXML() };
        }

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

        private int GetElementIndexInTable(DataTable tableView, int id)
        {
            if (tableView != null)
            {
                DataRowCollection rows = tableView.Rows;

                for (int i = 0; i < rows.Count; i++)
                {
                    int rowId = Convert.ToInt32(rows[i][0]);

                    if (rowId == id)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        
        #endregion

        public virtual void SaveNew(ChildElement element)
        {
            OpenConnection();
            //update the edit date
            string editDate = DateTime.Now.ToString("G");
            element.LastEditDate = editDate;
            //save to parent table
            SaveNewElementToTable(GetRowDataFromElement(element), TableName, TableColumnNames, TableColumnTypes);
            //add the element to the study cache
            StudyCacheForSaving.AddElement(element);
        }
        public void SaveNewElementToTable(object[] rowData, string tableName, string[] TableColumnNames, Type[] TableColumnTypes)
        {
            OpenConnection();
            DatabaseManager.DataTableView tbl = Connection.Instance.GetTable(tableName);
            if (tbl == null)
            {
                Connection.Instance.CreateTableWithPrimaryKey(tableName, TableColumnNames, TableColumnTypes);
            }

            Connection.Instance.AddRowToTableWithPrimaryKey(rowData, tableName, TableColumnNames);
        }

        public virtual void SaveExisting(ChildElement elementToSave)
        {
            OpenConnection();
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if(IDExistsInDB(TableName, elementToSave.ID, ID_COL_NAME))
            {
                UpdateTableRow(TableName, elementToSave.ID, ID_COL_NAME, TableColumnNames, GetRowDataFromElement(elementToSave));
                StudyCacheForSaving.UpdateElement( elementToSave);
            }
            else
            {
                elementToSave.ID = GetNextAvailableId();
                SaveNew(elementToSave);
            }
        }

        /// <summary>
        /// Check to see if the id still exists. If it doesn't, then we can save it as a new element.
        /// This is here because it was possible to open up an editor and then delete the element from the tree
        /// and then click save on the editor. 
        /// </summary>
        /// <returns></returns>
        private bool IDExistsInDB(string tableName, int id, string idColumnName)
        {
            bool idExists = false;
            OpenConnection();

            StringBuilder sb = new StringBuilder("SELECT 1 FROM ").Append(tableName).Append(" WHERE ").Append(idColumnName)
            .Append(" = '").Append(id).Append("'");

            SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            object returnValue = command.ExecuteScalar();
            int returnValueInt = Convert.ToInt32(returnValue);

            if (returnValueInt > 0)
            {
                idExists = true;
            }

            return idExists;
        }
   
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
        //public void RemoveFromGeopackageTable(string tableName)
        //{
        //    LifeSimGIS.GeoPackageWriter gpw = new LifeSimGIS.GeoPackageWriter(Connection.Instance.Reader);
        //    gpw.DeleteFeatures(tableName);
        //}
        public virtual void RemoveElementFromTable(ChildElement element, string tableName)
        {
            OpenConnection();
            if (Connection.Instance.TableNames().Contains(tableName))
            {
                DatabaseManager.DataTableView parentTableView = Connection.Instance.GetTable(tableName);
                if (parentTableView != null)
                {
                    DataTable dt = Connection.Instance.GetDataTable(tableName);
                    int parentTableIndex = GetElementIndexInTable(dt, element.ID);
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

        private string EscapeSingleQuotes(object value)
        {
            string returnValue = null;
            if (value != null)
            {
                returnValue = value.ToString().Replace("'", "''");
            }
            return returnValue;
        }

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
                sb.Append(columns[i]).Append(" = '").Append(EscapeSingleQuotes(values[i])).Append("' ").Append(",");
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
                sb.Append(columns[i]).Append(" = '").Append(EscapeSingleQuotes(values[i])).Append("' ").Append(",");
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

        #endregion

        abstract public ChildElement CreateElementFromRowData(object[] rowData);

        public int GetNextAvailableId(int idColNumber = 0)
        {
            //make sure the table exists
            OpenConnection();
            DatabaseManager.DataTableView tbl = Connection.Instance.GetTable(TableName);
            if (tbl == null)
            {
                Connection.Instance.CreateTableWithPrimaryKey(TableName, TableColumnNames, TableColumnTypes);
            }
            int retval = -1;
            string tableName = TableName;
            try
            {
                //todo: implement
                SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
                command.CommandText = "SELECT seq FROM SQLITE_SEQUENCE WHERE name = '" + tableName + "' LIMIT 1";
                object id = command.ExecuteScalar();
                if (id == null)
                {
                    //if the id is null, then there is nothing in the table. The first id will be 1.
                    retval = 1;
                }
                else
                {
                    retval = Convert.ToInt32(id) + 1;
                }
            }
            catch (Exception e)
            {
                //todo: some message? Name doesn't exist in the database.
                retval = -1;
            }
            return retval;
            //https://stackoverflow.com/questions/107005/predict-next-auto-inserted-row-id-sqlite#:~:text=Try%20SELECT%20*%20FROM%20SQLITE_SEQUENCE%20WHERE,to%20get%20the%20next%20ID.
        }
      

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

        public virtual void Remove(ChildElement element)
        {
            StudyCacheForSaving.RemoveElement(element);
            RemoveElementFromTable(element, TableName);
        }

        public virtual void Load()
        {
            List<ChildElement> childElems = CreateElementsFromRows(TableName, rowData => CreateElementFromRowData(rowData));
            foreach (ChildElement elem in childElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

    }
}
