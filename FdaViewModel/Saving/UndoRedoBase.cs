using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving
{
    public abstract class UndoRedoBase:SavingBase
    {
        private const int MAX_CHANGE_NUMBER = 4;

        

        /// <summary>
        /// The index of the "name" column in the change table
        /// </summary>
        public const int CHANGE_TABLE_NAME_INDEX = 1;
        public const string STATE_INDEX_COL_NAME = "state_index";
        public const int STATE_INDEX = 7;


       
        public abstract object[] GetRowDataForChangeTable(ChildElement element);
        public abstract string ChangeTableName { get; }
        public abstract string[] ChangeTableColumnNames { get; }
        public abstract Type[] ChangeTableColumnTypes { get; }

        //todo: not sure if i need this one
        public virtual int ChangeTableNameColIndex { get { return CHANGE_TABLE_NAME_INDEX; } }
        public virtual string ChangeTableStateIndexColName { get { return STATE_INDEX_COL_NAME; } }
        public virtual int ChangeTableLastEditDateIndex { get { return 2; } }
        public virtual string ChangeTableElementIdColName { get {return  ELEMENT_ID_COL_NAME; } }


        public void SaveToChangeTable(ChildElement element)
        {
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(ChangeTableName);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(ChangeTableName, ChangeTableColumnNames, ChangeTableColumnTypes);
                tbl = Storage.Connection.Instance.GetTable(ChangeTableName);
            }
            object[] row = GetRowDataForChangeTable(element);

            tbl.AddRow(row);
            tbl.ApplyEdits();

        }

        public void UpdateChangeTable(ChildElement element, int stateIndex)
        {
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(ChangeTableName);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(ChangeTableName, ChangeTableColumnNames, ChangeTableColumnTypes);
            }
            int elemId = GetElementId(TableName, element.Name);
            int highestStateId = Storage.Connection.Instance.GetMaxStateIndex(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, STATE_INDEX_COL_NAME);

            //if the user was on the newest then we just add a new entry after it
            //if the user had gone back (undo) then we move that row to the newest, shift others down and then add the
            //new elem to the newest

            if (highestStateId == stateIndex)
            {
                SaveToChangeTable(element);
            }
            else
            {
                //find the item with the state index and change its index to be the max
                //in order to do this, i need to pull it out while i make the other changes so i give it "-1"
                UpdateChangeTableRow(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, stateIndex, -1, STATE_INDEX_COL_NAME);
                //move any that are in between and move them down 1
                for (int i = stateIndex + 1; i <= highestStateId; i++)
                {
                    UpdateChangeTableRow(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, i, i - 1, STATE_INDEX_COL_NAME);

                }

                //now i can put it in its rightful place. Change from "-1" to highest
                UpdateChangeTableRow(ChangeTableName, elemId, ELEMENT_ID_COL_NAME, -1, highestStateId, STATE_INDEX_COL_NAME);

                //put the new one at max plus 1
                SaveToChangeTable(element);
            }

        }

        public void Remove(ChildElement element)
        {
            //important to get the element id before removing it from the parent table or else you wont get it.
            int id = GetElementId(TableName, element.Name);
            RemoveElementFromChangeTable(ChangeTableName, id, ELEMENT_ID_COL_NAME);
            RemoveFromParentTable(element, TableName);
            //DeleteChangeTableAndAssociatedTables(element, ChangeTableConstant);
            StudyCacheForSaving.RemoveElement(element, id);

        }

        /// <summary>
        /// Removes any row that has the given element id
        /// </summary>
        /// <param name="tableName">The name of the change table</param>
        /// <param name="id">The element id that should be removed</param>
        /// <param name="idColumnName">The column header that holds the element id's</param>
        public void RemoveElementFromChangeTable(string tableName, int id, string idColumnName)
        {
            StringBuilder sb = new StringBuilder("DELETE FROM ").Append(tableName).Append(" WHERE ")
                .Append(idColumnName).Append(" = ").Append(id);

            SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();
        }


        /// <summary>
        /// This is where the re sorting and adding and deleting of rows occurs for the change table.
        /// I use a data table to do all the rearanging because it is faster than dealing directly with the 
        /// table in the database. If a row gets added and the elem saves to table, then it will save out its 
        /// associated table as well. If a row gets deleted and the elem saves to table, then the associated
        /// table will also get deleted.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="elem"></param>
        private void UpdateElementChangeTable(int changeIndex, object[] rowData, string elementName, string changeTableConstant)
        {
            string changeTableName = changeTableConstant + elementName + "-ChangeTable";
            DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
            System.Data.DataTable table = changeTableView.ExportToDataTable();


            //grab the currently selected row
            DataRow row = table.NewRow();
            for (int j = 0; j < table.Columns.Count; j++)
            {
                row[j] = table.Rows[changeIndex][j];
            }

            //remove the row from the table
            table.Rows.RemoveAt(changeIndex);
            //insert this row back into table at 0
            table.Rows.InsertAt(row, 0);

            //now insert the new row at zero
            object[] newRowData = rowData;
            DataRow newRow = table.NewRow();
            for (int i = 0; i < newRowData.Length; i++)
            {
                newRow[i] = newRowData[i];
            }
            table.Rows.InsertAt(newRow, 0);

            //if the table was full when we started, then there is one extra row.
            //delete the last row

            if (table.Rows.Count > MAX_CHANGE_NUMBER)
            {
                //delete the table associated with that row
                //if (elem.SavesToTable())
                {
                    string nameOfChangeTable = changeTableConstant + table.Rows[MAX_CHANGE_NUMBER][1].ToString();
                    Storage.Connection.Instance.DeleteTable(nameOfChangeTable);//do i need to check if this table exists first?
                }
                table.Rows.RemoveAt(MAX_CHANGE_NUMBER);
            }

            //*************************************    ??????
            //if (elem.SavesToTable())
            //{
            //    elem.Save();
            //}

            //now that we have the table where we want it. Replace the old change table with this data table
            Storage.Connection.Instance.DeleteTable(changeTableName);
            table.TableName = changeTableName;
            Storage.Connection.Instance.Reader.SaveDataTable(table);

        }

        private int GetNextChangeTableStateNumber(string tableName, string elementName)
        {
            int retval = -1;
            try
            {
                SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
                command.CommandText = "select ID from " + tableName + " where Name = '" + elementName + "'";
                retval = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception e)
            {
                //some message? Name doesn't exist in the database.
                retval = -1;
            }
            return retval;
        }

        public DataTable GetChangeTableRow(int id, string idColumnName, int stateIndex,
            string stateIndexColumnName, string tableName)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            DataTable tab = new DataTable();
            if (Storage.Connection.Instance.Reader.TableNames.Contains(tableName))
            {
                List<object[]> rows = new List<object[]>();
                SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
                command.CommandText = "select * from " + tableName + " where " + idColumnName + " = " + id +
                    " and " + stateIndexColumnName + " = " + stateIndex;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(tab);
            }
            return tab;
        }

        public void UpdateChangeTableRow(string tableName, int elementId, string elementIdColName, int oldStateValue, int newStateValue, string stateValueColName)
        {
            //columns and values need to be corespond to each other, you don't have to update columns that don't need it
            StringBuilder sb = new StringBuilder("update ").Append(tableName).Append(" set ");

            sb.Append(stateValueColName).Append(" = '").Append(newStateValue).Append("' ")
            .Append(" where ").Append(elementIdColName).Append(" = ").Append(elementId).Append(" AND ")
            .Append(stateValueColName).Append(" = ").Append(oldStateValue);

            SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();

        }

        

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave, int changeTableIndex)
        {
            //string editDate = DateTime.Now.ToString("G");
            //elementToSave.LastEditDate = editDate;
            //save to parent table
            SaveExisting(oldElement, elementToSave);
            //if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((RatingCurveElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve,elementToSave.Curve))
            //{
                //UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((RatingCurveElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                //this updates the parent table
                //UpdateTableRow(TableName, GetElementId(TableName, oldElement.Name), ID_COL_NAME, TableColumnNames, GetRowDataFromElement((RatingCurveElement)elementToSave));
                //SaveToChangeTable((RatingCurveElement)elementToSave);
                UpdateChangeTable(elementToSave, changeTableIndex);
                //StudyCacheForSaving.UpdateRatingCurve((RatingCurveElement)oldElement, (RatingCurveElement)elementToSave);
            //}

            //Log(FdaLogging.LoggingLevel.Info, "Saved rating curve: " + elementToSave.Name, elementToSave.Name);

            //if (!oldElement.Name.Equals(elementToSave.Name))
            //{
            //    Log(FdaLogging.LoggingLevel.Info, "Rating curve name changed from " + oldElement.Name +
            //        " to " + elementToSave.Name + ".", elementToSave.Name);

            //}

        }

        //public virtual ObservableCollection<UndoRedoRowItem> CreateUndoRedoRows(ChildElement element)
        //{
        //    if (Storage.Connection.Instance.IsOpen != true)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }

        //    string changeTableName = ChangeTableConstant;// + element.Name +  "-ChangeTable";
        //    DatabaseManager.DataTableView tableView = Storage.Connection.Instance.GetTable(changeTableName);
        //    if (tableView == null) { return new ObservableCollection<UndoRedoRowItem>(); }

        //    int nameIndex = 0;
        //    int dateIndex = 1;
        //    ObservableCollection<UndoRedoRowItem> retVals = new ObservableCollection<UndoRedoRowItem>();
        //    foreach (object[] row in tableView.GetRows(0, tableView.NumberOfRows - 1))
        //    {
        //        retVals.Add(new UndoRedoRowItem(row[nameIndex].ToString(), row[dateIndex].ToString()));
        //    }
        //    return retVals;
        //}

        public ObservableCollection<UndoRedoRowItem> CreateUndoRedoRows(ChildElement element)
        {
            //retrieve the table from the database with the id of this element. Loop over the rows and create undo-redo rows.
            int elemId = GetElementId(TableName, element.Name);
            DataTable table = Storage.Connection.Instance.GetRowsWithIDValue(elemId, ChangeTableElementIdColName, ChangeTableName);

            ObservableCollection<UndoRedoRowItem> retVals = new ObservableCollection<UndoRedoRowItem>();

            foreach (DataRow row in table.Rows)
            {
                retVals.Add(new UndoRedoRowItem(row[ChangeTableNameColIndex].ToString(),
                    row[ChangeTableLastEditDateIndex].ToString()));
            }

            return retVals;
        }

        public virtual ChildElement GetPreviousElementFromChangeTable(ChildElement element, int changeTableIndex)
        {
            ChildElement prevElement = null;
            string changeTableName = ChangeTableConstant + element.Name + "-ChangeTable";

            DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
            if (changeTableIndex < changeTableView.NumberOfRows)
            {
                if (changeTableView != null)
                {
                    if (!Storage.Connection.Instance.IsOpen)
                    {
                        Storage.Connection.Instance.Open();
                    }
                    object[] rowData = changeTableView.GetRow(changeTableIndex);

                    prevElement = CreateElementFromRowData(rowData);

                    Storage.Connection.Instance.Close();
                }
            }
            return prevElement;
        }


        public virtual ChildElement GetNextElementFromChangeTable(ChildElement element, int changeTableIndex)
        {
            string changeTableName = ChangeTableConstant + element.Name + "-ChangeTable";

            ChildElement nextElement = null;
            DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
            if (changeTableIndex >= 0)
            {
                if (changeTableView != null)
                {
                    if (!Storage.Connection.Instance.IsOpen)
                    {
                        Storage.Connection.Instance.Open();
                    }
                    object[] rowData = changeTableView.GetRow(changeTableIndex);

                    nextElement = CreateElementFromRowData(rowData);

                    Storage.Connection.Instance.Close();
                }
            }
            return nextElement;
        }

        public virtual ChildElement Undo(ChildElement element, int changeTableIndex)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }

            int elemId = GetElementId(TableName, element.Name);
            DataTable table = GetChangeTableRow(elemId, ChangeTableElementIdColName, changeTableIndex, ChangeTableStateIndexColName, ChangeTableName);

            if (table.Rows.Count == 1)
            {
                DataRow row = table.Rows[0];
                return CreateElementFromRowData(row.ItemArray);
            }
            else
            {
                //something went wrong
                return null;
            }
        }

        public virtual ChildElement Redo(ChildElement element, int changeTableIndex)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }

            int elemId = GetElementId(TableName, element.Name);
            DataTable table = GetChangeTableRow(elemId, ChangeTableElementIdColName, changeTableIndex, ChangeTableStateIndexColName, ChangeTableName);

            if (table.Rows.Count == 1)
            {
                DataRow row = table.Rows[0];
                return CreateElementFromRowData(row.ItemArray);
            }
            else
            {
                //something went wrong
                return null;
            }
        }

    }
}
