﻿using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Saving
{
    public class SavingBase<Element> : BaseViewModel, IElementManager
        where Element : ChildElement
    {

        public const string NAME = "Name";
        public const string XML = "XML";
        public const string ID_COL_NAME = "ID";

        public const int ID_COL = 0;
        public const int XML_COL = 1;

        /// <summary>
        /// The FDACache stores all the elements in memory. 
        /// </summary>
        public Study.FDACache StudyCacheForSaving { get; set; }
        private string _TableName;

        public virtual string[] TableColumnNames { get; } = new string[] { XML };
        public virtual Type[] TableColumnTypes { get; } = new Type[] { typeof(string) };

        public SavingBase(FDACache studyCache, string tableName)
        {
            StudyCacheForSaving = studyCache;
            _TableName = tableName;
        }

        public virtual object[] GetRowDataFromElement(ChildElement elem)
        {
            return new object[] { elem.ToXML() };
        }

        #region Utilities

        public List<ChildElement> CreateElementsFromRows(Func<object[], ChildElement> createElemsFromRowDataAction)
        {
            OpenConnection();
            List<ChildElement> elems = new List<ChildElement>();

            DataTable table = Connection.Instance.GetDataTable(_TableName);
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

        public Element CreateElementFromRowData(object[] rowData)
        {
            int id = Convert.ToInt32(rowData[ID_COL]);
            string xmlString = (string)rowData[XML_COL];
            XDocument doc = XDocument.Parse(xmlString);
            return (Element)Activator.CreateInstance(typeof(Element), doc.Root, id);
        }

        #endregion

        public virtual void SaveNew(ChildElement element)
        {
            OpenConnection();
            //update the edit date
            string editDate = DateTime.Now.ToString("G");
            element.LastEditDate = editDate;
            //save to parent table
            SaveNewElementToTable(GetRowDataFromElement(element), TableColumnNames, TableColumnTypes);
            //add the element to the study cache
            StudyCacheForSaving.AddElement(element);
        }

        public void SaveNewElementToTable(object[] rowData, string[] TableColumnNames, Type[] TableColumnTypes)
        {
            OpenConnection();
            if (!Connection.Instance.TableExists(_TableName))
            {
                Connection.Instance.CreateTableWithPrimaryKey(_TableName, TableColumnNames, TableColumnTypes);
            }

            Connection.Instance.AddRowToTableWithPrimaryKey(rowData, _TableName, TableColumnNames);
        }

        public virtual void SaveExisting(ChildElement elementToSave)
        {
            OpenConnection();
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            if (IDExistsInDB(_TableName, elementToSave.ID, ID_COL_NAME))
            {
                UpdateTableRow(elementToSave.ID, ID_COL_NAME, TableColumnNames, GetRowDataFromElement(elementToSave));
                StudyCacheForSaving.UpdateElement(elementToSave);
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

        public void RemoveElementFromTable(ChildElement element)
        {
            OpenConnection();
            bool tableExists = Connection.Instance.TableNames().Contains(_TableName);
            if (tableExists)
            {
                //Check that the element exists 
                //Search the ID column of the database table for the ID. Return the index
                //assume first column of the table is ID
                //delete the row at that index. 

                DataTable dt = Connection.Instance.GetDataTable(_TableName);
                int parentTableIndex = GetElementIndexInTable(dt, element.ID);
                if (parentTableIndex != -1)
                {
                    string query = $"DELETE FROM {_TableName} WHERE ID = {element.ID}";
                    SQLiteCommand command = Connection.Instance.Reader.DbConnection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();
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
        public void UpdateTableRow(int primaryKey, string primaryKeyColName, string[] columns, object[] values)
        {
            OpenConnection();
            //columns and values need to be corespond to each other, you don't have to update columns that don't need it
            StringBuilder sb = new StringBuilder("update ").Append(_TableName).Append(" set ");
            for (int i = 0; i < columns.Length; i++)
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
   

        #endregion

        public int GetNextAvailableId()
        {
            //make sure the table exists
            OpenConnection();
            object tbl = Connection.Instance.GetTable(_TableName);
            if (!Connection.Instance.TableExists(_TableName))
            {
                Connection.Instance.CreateTableWithPrimaryKey(_TableName, TableColumnNames, TableColumnTypes);
            }
            int retval = -1;
            string tableName = _TableName;
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
            catch
            {
                //todo: some message? Name doesn't exist in the database.
                retval = -1;
            }
            return retval;
            //https://stackoverflow.com/questions/107005/predict-next-auto-inserted-row-id-sqlite#:~:text=Try%20SELECT%20*%20FROM%20SQLITE_SEQUENCE%20WHERE,to%20get%20the%20next%20ID.
        }

        public virtual void Remove(ChildElement element)
        {
            StudyCacheForSaving.RemoveElement(element);
            RemoveElementFromTable(element);
        }

        public virtual void Load()
        {
            List<ChildElement> childElems = CreateElementsFromRows(rowData => CreateElementFromRowData(rowData));
            foreach (ChildElement elem in childElems)
            {
                StudyCacheForSaving.AddElement(elem);
            }
        }

    }
}
