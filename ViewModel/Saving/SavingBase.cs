using ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Saving
{
    public abstract class SavingBase : BaseViewModel
    {
        //private const int MAX_CHANGE_NUMBER = 4;
        /// <summary>
        /// The FDACache stores all the elements in memory. 
        /// </summary>
        public Study.FDACache StudyCacheForSaving { get; set; }
        public abstract string TableName { get; }
        public const string ID_COL_NAME = "id";
        public const string NAME = "name";
        //todo: move these out of here and put in the rating element persistence manager which is what is using it
        public const string ELEMENT_ID_COL_NAME = "elem_id";

        public const string LAST_EDIT_DATE = "last_edit_date";

        public const string DESCRIPTION = "description";
        public const string CURVE_DISTRIBUTION_TYPE = "curve_distribution_type";
        //todo: some elems use the curve type in their tables and some don't. Am i using the curve type for something? investigate?
        public const string CURVE_TYPE = "curve_type";
        public const string CURVE = "curve";


        public abstract string[] TableColumnNames { get; }
        public abstract Type[] TableColumnTypes { get; }
        public abstract object[] GetRowDataFromElement(ChildElement elem);

        #region Utilities

        public static async void SaveElementOnBackGroundThread(ChildElement elementToSave, BaseFdaElement elementToChangeActionsAndHeader, Action<ChildElement> SavingAction, string savingMessage) 
        {
            elementToChangeActionsAndHeader.CustomTreeViewHeader = new CustomHeaderVM(elementToChangeActionsAndHeader.Name, "", savingMessage, true);//need to create a method to just append the note and gif

            //clear the actions while it is saving
            List<NamedAction> actions = new List<NamedAction>();
            foreach (NamedAction act in elementToChangeActionsAndHeader.Actions)
            {
                actions.Add(act);
            }
            elementToChangeActionsAndHeader.Actions = new List<NamedAction>();

            await Task.Run(() =>
            {
                if (!Storage.Connection.Instance.IsConnectionNull)
                {
                    if (!Storage.Connection.Instance.IsOpen)
                    {
                        Storage.Connection.Instance.Open();
                    }
                    System.Threading.Thread.Sleep(5000);

                    SavingAction(elementToSave);

                    
                    elementToChangeActionsAndHeader.CustomTreeViewHeader = new CustomHeaderVM(elementToChangeActionsAndHeader.Name);

                    //restore the actions
                    elementToChangeActionsAndHeader.Actions = actions;
                }
            });
        }

        public static async void SaveElementsOnBackGroundThread(List<ChildElement> elementsToSave, BaseFdaElement elementToChangeActionsAndHeader, Action<ChildElement> SavingAction, string savingMessage)
        {
            elementToChangeActionsAndHeader.CustomTreeViewHeader = new CustomHeaderVM(elementToChangeActionsAndHeader.Name, "", savingMessage, true);//need to create a method to just append the note and gif

            //clear the actions while it is saving
            List<NamedAction> actions = new List<NamedAction>();
            foreach (NamedAction act in elementToChangeActionsAndHeader.Actions)
            {
                actions.Add(act);
            }
            elementToChangeActionsAndHeader.Actions = new List<NamedAction>();

            await Task.Run(() =>
            {
                if (!Storage.Connection.Instance.IsConnectionNull)
                {
                    if (!Storage.Connection.Instance.IsOpen)
                    {
                        Storage.Connection.Instance.Open();
                    }
                    //System.Threading.Thread.Sleep(5000);

                    foreach (ChildElement elem in elementsToSave)
                    {
                        SavingAction(elem);
                    }


                    elementToChangeActionsAndHeader.CustomTreeViewHeader = new CustomHeaderVM(elementToChangeActionsAndHeader.Name);

                    //restore the actions
                    elementToChangeActionsAndHeader.Actions = actions;
                }
            });
        }

        public List<ChildElement> CreateElementsFromRows(string tableName, Func<object[], ChildElement> createElemsFromRowDataAction)
        {
            List<ChildElement> elems = new List<ChildElement>();
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }

            DataTable table = Storage.Connection.Instance.GetDataTable(tableName);
           // if (dtv != null)
            {
                //add an element based on a row element;
                //for (int i = 1; i < table.Rows.Count; i++)
                foreach(DataRow row in table.Rows)
                {
                   // Storage.Connection.Instance.GetRowQueryText(tableName);
                    //object[] row = dtv.GetRow(i);
                    elems.Add(createElemsFromRowDataAction(row.ItemArray));
                }
                //if (Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Close();
            }
            return elems;
        }

        private int GetElementIndexInTable(DatabaseManager.DataTableView tableView, string name, int nameIndexInTheRow)
        {
            if (tableView != null)
            {
                tableView.GetRows(0, tableView.NumberOfRows - 1);
                List<object[]> rows = tableView.GetRows(0, tableView.NumberOfRows - 1);

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

        private int GetElementIndexInTable(System.Data.DataTable tableView, string name, int nameIndexInTheRow)
        {
            if (tableView != null)
            {
                DataRowCollection rows = tableView.Rows;// GetRows(0, tableView.NumberOfRows - 1);

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

        public void UpdateRow(string table)
        {

        }

        #endregion

        #region save new
        public void SaveNewElement(ChildElement element)
        {
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
            DatabaseManager.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTableWithPrimaryKey(tableName, TableColumnNames, TableColumnTypes);
                //tbl = Storage.Connection.Instance.GetTable(tableName);
            }

            Storage.Connection.Instance.AddRowToTableWithPrimaryKey(rowData, tableName, TableColumnNames);

        }

        public void SaveExisting(ChildElement oldElement, ChildElement elementToSave)
        {
            string editDate = DateTime.Now.ToString("G");
            elementToSave.LastEditDate = editDate;

            //if (DidParentTableRowValuesChange(elementToSave, GetRowDataFromElement((RatingCurveElement)elementToSave), oldElement.Name, TableName) || AreCurvesDifferent(oldElement.Curve,elementToSave.Curve))
            {
                //UpdateParentTableRow(elementToSave.Name, changeTableIndex, GetRowDataFromElement((RatingCurveElement)elementToSave), oldElement.Name, TableName, true, ChangeTableConstant);
                //this updates the parent table
                int id = GetElementId(TableName, oldElement.Name);
                UpdateTableRow(TableName, id, ID_COL_NAME, TableColumnNames, GetRowDataFromElement(elementToSave));
                //SaveToChangeTable((RatingCurveElement)elementToSave);
                //UpdateChangeTable(elementToSave, changeTableIndex);

                //make this so that i can pass in "childElement" and have it updated
                StudyCacheForSaving.UpdateElement(oldElement, elementToSave, id);
            }

            //Log(FdaLogging.LoggingLevel.Info, "Saved rating curve: " + elementToSave.Name, elementToSave.Name);

            //if (!oldElement.Name.Equals(elementToSave.Name))
            //{
            //    Log(FdaLogging.LoggingLevel.Info, "Rating curve name changed from " + oldElement.Name +
            //        " to " + elementToSave.Name + ".", elementToSave.Name);

            //}

        }

        //public void SaveElementToChangeTable(string elementName, object[] rowData, string changeTableConstant, string[] TableColumnNames, Type[] TableColumnTypes)
        //{

        //    string changeTableName = changeTableConstant + elementName + "-ChangeTable";

        //    DatabaseManager.DataTableView changeTable = Storage.Connection.Instance.GetTable(changeTableName);
        //    if (changeTable == null)
        //    {
        //        Storage.Connection.Instance.CreateTable(changeTableName, TableColumnNames, TableColumnTypes);
        //        changeTable = Storage.Connection.Instance.GetTable(changeTableName);
        //    }

        //    changeTable.AddRow(rowData);
        //    changeTable.ApplyEdits();

        //}

        //public void SaveCurveTable(Statistics.UncertainCurveDataCollection curve, string changeTableConstant, string lastEditDate)
        //{
        //    if (!Storage.Connection.Instance.IsOpen)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }
        //    curve.toSqliteTable(changeTableConstant + lastEditDate);

        //}


        #endregion

        #region Remove element

        public virtual void RemoveFromParentTable(ChildElement element, string tableName)
        {

            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }

            element.RemoveElementFromMapWindow(this, new EventArgs());

            if (Storage.Connection.Instance.TableNames().Contains(tableName))
            {
                DatabaseManager.DataTableView parentTableView = Storage.Connection.Instance.GetTable(tableName);
                if (parentTableView != null)
                {
                    //int parentTableIndex = GetElementIndexInTable(parentTableView, element.Name, 1);
                    DataTable dt = Storage.Connection.Instance.GetDataTable(tableName);
                    int parentTableIndex = GetElementIndexInTable(dt, element.Name, 1);
                    if (parentTableIndex != -1)
                    {
                        //object[] parentRow = parentTableView.GetRow(parentTableIndex);
                        parentTableView.DeleteRow(parentTableIndex);
                        parentTableView.ApplyEdits();
                    }
                }
            }

        }

        //public void DeleteChangeTableAndAssociatedTables(ChildElement element, string changeTableConstant)
        //{
        //    string changeTableName = changeTableConstant + element.Name + "-ChangeTable";
        //    DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
        //    if (changeTableView != null)
        //    {
        //        //loop through all rows and delete all associated tables
        //        //then delete the change table
        //        foreach (object[] row in changeTableView.GetRows(0, changeTableView.NumberOfRows-1))
        //        {
        //            string tableName = changeTableConstant + row[1];//this is the edit time
        //            Storage.Connection.Instance.DeleteTable(tableName);//do i need to check if these exist?
        //        }
        //        Storage.Connection.Instance.DeleteTable(changeTableName);
        //    }
        //}



        #endregion




        #region save existing

        ///////////////////////   save existing  //////////////////////////////

        //update parent row
        //SaveElementToChangeTable


        //public void SaveExistingElement(string oldName, Statistics.UncertainCurveDataCollection oldCurve, ChildElement element, string tableName)
        //{


        //    //update parent table row
        //    UpdateTableRowIfModified(oldName,tableName,  element);
        //    //update its own table
        //    if (element.SavesToTable())
        //    {
        //        element.UpdateTableIfModified(oldName, oldCurve, element.Curve);
        //    }

        //}

        //public void UpdateParentTableRow(string elementName, int changeIndex, object[] rowData, string oldName, string tableName, bool hasChangeTable = false, string changeTableConstant = "")
        //{
        //    if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
        //    DatabaseManager.DataTableView tableView = Storage.Connection.Instance.GetTable(tableName);

            

        //    bool nameHasChanged = false;
        //    if (!oldName.Equals(elementName))
        //    {
        //        nameHasChanged = true;
        //    }

        //    //int rowIndex = GetElementIndexInTable(tableView, oldName, 0);
        //    DataTable dt = Storage.Connection.Instance.GetDataTable(tableName);
        //    int rowIndex = GetElementIndexInTable(dt, oldName, 1);
        //    if (rowIndex != -1)
        //    {

        //        //possibly need to change the name in associated table
        //        if (nameHasChanged && hasChangeTable)
        //        {
        //            Storage.Connection.Instance.RenameTable(changeTableConstant + oldName + "-ChangeTable", changeTableConstant + elementName + "-ChangeTable");
        //        }


        //        tableView.EditRow(rowIndex, rowData);
        //        tableView.ApplyEdits();

        //        //if (hasChangeTable)
        //        //{
        //        //    UpdateElementChangeTable(changeIndex, rowData, elementName, changeTableConstant);
        //        //}
        //    }
        //}

        /// <summary>
        /// This sends a sql "update" command to the database.
        /// </summary>
        /// <param name="tableName">The name of the sqlite table</param>
        /// <param name="primaryKey">The id of the element. The column that the id is in must be "ID"</param>
        /// <param name="columns">The columns that you want to update</param>
        /// <param name="values">The values that you want in the columns listed in "columns"</param>
        public void UpdateTableRow(string tableName, int primaryKey, string primaryKeyColName, string[] columns, object[] values)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            //columns and values need to be corespond to each other, you don't have to update columns that don't need it
            StringBuilder sb = new StringBuilder("update ").Append(tableName).Append(" set ");
            for(int i = 0;i<columns.Length;i++)
            {
                sb.Append(columns[i]).Append(" = '").Append(values[i]).Append("' ").Append(",");
            }
            //get rid of last comma
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" where ").Append(primaryKeyColName).Append(" = ").Append(primaryKey);

            SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
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
        public void UpdateTableRowWithCompoundKey(string tableName, int[] primaryKeys, string[] primaryKeyColNames, string[] columns, object[] values)
        {
            //this sql query looks like this:
            //update occupancy_types set Name = 'codyistesting' where GroupID = 1 and OcctypeID = 1
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            //columns and values need to be corespond to each other, you don't have to update columns that don't need it
            StringBuilder sb = new StringBuilder("update ").Append(tableName).Append(" set ");
            for (int i = 0; i < columns.Length; i++)
            {
                sb.Append(columns[i]).Append(" = '").Append(values[i]).Append("' ").Append(",");
            }
            //get rid of last comma
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" where ");
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                sb.Append(primaryKeyColNames[i]).Append(" = ").Append(primaryKeys[i]).Append(" and ");
            }
            //remove the last "and"
            sb.Remove(sb.Length - 4, 4);

            SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();

        }

        public void DeleteRowWithCompoundKey(string tableName, int[] primaryKeys, string[] primaryKeyColNames)
        {
            //this sql query looks like this:
            //delete from occupancy_types where GroupID = 1 and OcctypeID = 27
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            StringBuilder sb = new StringBuilder("delete from ").Append(tableName).Append(" where ");
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                sb.Append(primaryKeyColNames[i]).Append(" = ").Append(primaryKeys[i]).Append(" and ");
            }
            //remove the last "and"
            sb.Remove(sb.Length - 4, 4);

            SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();

        }

        public void DeleteRowWithKey(string tableName, int key, string keyColName)
        {
            //this sql query looks like this:
            //delete from occupancy_types where GroupID = 1
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            //if the table doesn't exist, then there is nothing to delete
            if(Storage.Connection.Instance.GetTable(tableName) == null)
            {
                return;
            }

            StringBuilder sb = new StringBuilder("delete from ").Append(tableName).Append(" where ").Append(keyColName).Append(" = ").Append(key);
            SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();

        }

        //public void UpdateChangeTableRow(string tableName, int elementId, string elementIdColName, int oldStateValue, int newStateValue, string stateValueColName)
        //{
        //    //columns and values need to be corespond to each other, you don't have to update columns that don't need it
        //    StringBuilder sb = new StringBuilder("update ").Append(tableName).Append(" set ");

        //        sb.Append(stateValueColName).Append(" = '").Append(newStateValue).Append("' ")
        //        .Append(" where ").Append(elementIdColName).Append(" = ").Append(elementId).Append(" AND ")
        //        .Append(stateValueColName).Append(" = ").Append(oldStateValue);

        //    SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
        //    command.CommandText = sb.ToString();
        //    command.ExecuteNonQuery();

        //}

        public bool DidParentTableRowValuesChange(ChildElement element, object[] rowData, string oldName, string tableName)
        {
            if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
            DatabaseManager.DataTableView tableView = Storage.Connection.Instance.GetTable(tableName);
            DataTable dt = Storage.Connection.Instance.GetDataTable(tableName);
            int rowIndex = GetElementIndexInTable(dt, oldName, 1);
            //int rowIndex = GetElementIndexInTable(tableView, oldName, 1);
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

        //public void UpdateTableIfModified(string oldName, Statistics.UncertainCurveDataCollection oldCurve, Statistics.UncertainCurveDataCollection newCurve)
        //{
        //    bool isModified = AreCurvesDifferent(oldCurve, newCurve);
        //    if (isModified && SavesToTable())
        //    {
        //        //if the name has changed then we need to delete the old table
        //        Storage.Connection.Instance.DeleteTable(TableName);
        //        Save();
        //    }
        //}

            //todo: Refactor: CO
        //public bool AreCurvesDifferent(Statistics.UncertainCurveDataCollection oldCurve, Statistics.UncertainCurveDataCollection newCurve)
        //{
        //    bool isModified = false;
        //    if (oldCurve.Distribution != newCurve.Distribution) { isModified = true; }
        //    if (oldCurve.Count != newCurve.Count) { isModified = true; }
        //    if (oldCurve.GetType() != newCurve.GetType()) { isModified = true; }
        //    //are all x values the same
        //    for (int i = 0; i < oldCurve.XValues.Count(); i++)
        //    {
        //        if (oldCurve.XValues[i] != newCurve.XValues[i])
        //        {
        //            isModified = true;
        //            break;
        //        }
        //    }
        //    for (int i = 0; i < oldCurve.YValues.Count(); i++)
        //    {
        //        if (oldCurve.YValues[i] != newCurve.YValues[i])
        //        {
        //            isModified = true;
        //            break;
        //        }
        //    }
        //    return isModified;
        //}



        /// <summary>
        /// This method is to be used after an editor has closed. If the user changed any information
        /// that gets stored in this ownerElement row data, then we need to update it. If the user
        /// changes the name of the element and that element saves to a table, then update that tables name.
        /// </summary>
        /// <param name="oldName">Name before editing. This is what is used to find the row in this owner's table</param>
        /// <param name="elem"></param>
        /// <param name="nameIndexInTheRow">This should always be zero, but if it is not, then just tell me what index in the row you are at.</param>
        //public void UpdateTableRowIfModified( string oldName, string TableName, ChildElement elem, int nameIndexInTheRow = 0)
        //{


        //        if (rowIsModified)
        //        {

        //            //possibly need to change the name in associated table
        //            if (nameHasChanged)
        //            {
        //                Storage.Connection.Instance.RenameTable(elem.GetTableConstant() + oldName + "-ChangeTable", elem.ChangeTableName());
        //            }

        //            tableView.EditRow(rowIndex, elem.RowData());
        //            tableView.ApplyEdits();

        //            RearangeElementChangeTable(elem.Name, elem);

        //            //Storage.Connection.Instance.Close();
        //        }

        //}


        private bool AreListsDifferent(object[] a, object[] b)
        {

            for (int i = 0; i < a.Length; i++)
            {
                //don't evaluate the last edit time which is the second one. - i am getting rid of this which means it will always save because
                //the conditions (1) is the description.
               // if (i == 1) { continue; }
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

        ///// <summary>
        ///// Removes any row that has the given element id
        ///// </summary>
        ///// <param name="tableName">The name of the change table</param>
        ///// <param name="id">The element id that should be removed</param>
        ///// <param name="idColumnName">The column header that holds the element id's</param>
        //public void RemoveElementFromChangeTable(string tableName, int id, string idColumnName)
        //{
        //    StringBuilder sb = new StringBuilder("DELETE FROM ").Append(tableName).Append(" WHERE ")
        //        .Append(idColumnName).Append(" = ").Append(id);

        //    SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
        //    command.CommandText = sb.ToString();
        //    command.ExecuteNonQuery();
        //}

        ///// <summary>
        ///// This is where the re sorting and adding and deleting of rows occurs for the change table.
        ///// I use a data table to do all the rearanging because it is faster than dealing directly with the 
        ///// table in the database. If a row gets added and the elem saves to table, then it will save out its 
        ///// associated table as well. If a row gets deleted and the elem saves to table, then the associated
        ///// table will also get deleted.
        ///// </summary>
        ///// <param name="oldName"></param>
        ///// <param name="elem"></param>
        //private void UpdateElementChangeTable(int changeIndex, object[] rowData, string elementName, string changeTableConstant)
        //{
        //    string changeTableName = changeTableConstant + elementName + "-ChangeTable";
        //    DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
        //    System.Data.DataTable table = changeTableView.ExportToDataTable();


        //    //grab the currently selected row
        //    DataRow row = table.NewRow();
        //    for (int j = 0; j < table.Columns.Count; j++)
        //    {
        //        row[j] = table.Rows[changeIndex][j];
        //    }

        //    //remove the row from the table
        //    table.Rows.RemoveAt(changeIndex);
        //    //insert this row back into table at 0
        //    table.Rows.InsertAt(row, 0);

        //    //now insert the new row at zero
        //    object[] newRowData = rowData;
        //    DataRow newRow = table.NewRow();
        //    for (int i = 0; i < newRowData.Length; i++)
        //    {
        //        newRow[i] = newRowData[i];
        //    }
        //    table.Rows.InsertAt(newRow, 0);

        //    //if the table was full when we started, then there is one extra row.
        //    //delete the last row

        //    if (table.Rows.Count > MAX_CHANGE_NUMBER)
        //    {
        //        //delete the table associated with that row
        //        //if (elem.SavesToTable())
        //        {
        //            string nameOfChangeTable = changeTableConstant + table.Rows[MAX_CHANGE_NUMBER][1].ToString();
        //            Storage.Connection.Instance.DeleteTable(nameOfChangeTable);//do i need to check if this table exists first?
        //        }
        //        table.Rows.RemoveAt(MAX_CHANGE_NUMBER);
        //    }

        //    //*************************************    ??????
        //    //if (elem.SavesToTable())
        //    //{
        //    //    elem.Save();
        //    //}

        //    //now that we have the table where we want it. Replace the old change table with this data table
        //    Storage.Connection.Instance.DeleteTable(changeTableName);
        //    table.TableName = changeTableName;
        //    Storage.Connection.Instance.Reader.SaveDataTable(table);

        //}


        #endregion






        #region UndoRedo

        abstract public ChildElement CreateElementFromRowData(object[] rowData);


        //public virtual ChildElement Undo(ChildElement element, int changeTableIndex)
        //{
        //    ChildElement prevElement = null;
        //    if (Storage.Connection.Instance.IsOpen != true)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }
        //    DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(ChangeTableConstant + element.Name +  "-ChangeTable");
        //    if (changeTableIndex < changeTableView.NumberOfRows - 1)
        //    {
        //        prevElement = GetPreviousElementFromChangeTable(element, changeTableIndex + 1);
        //        //if (prevElement != null)// null if out of range index
        //        //{
        //        //    ChangeTableIndex += 1;
        //        //    //UpdateUndoRedoVisibility(ChangeTableIndex, ChangeTableName);
        //        //    //UpdateIndividualUndoRedoRows();
        //        //}
        //    }
        //    return prevElement;
        //}

      


        //public virtual ChildElement Redo(ChildElement element, int changeTableIndex)
        //{
        //    ChildElement nextElement = null;
        //    if (Storage.Connection.Instance.IsOpen != true)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }
        //    DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(ChangeTableConstant + element.Name+ "-ChangeTable");
        //    if (changeTableView != null)
        //    {
        //        //get the previous state
        //        if (changeTableIndex > 0)
        //        {
        //            nextElement = (ChildElement)GetNextElementFromChangeTable(element, changeTableIndex - 1);
        //            //if (nextElement != null)// null if out of range index
        //            //{
        //            //    ChangeTableIndex -= 1;
        //            //    //UpdateUndoRedoVisibility(ChangeIndex, ChangeTableName);
        //            //    //UpdateIndividualUndoRedoRows();
        //            //}
        //        }
        //    }
        //    return nextElement;
        //}

        //public virtual ChildElement Undo(ChildElement element, int changeTableIndex)
        //{
        //    if (Storage.Connection.Instance.IsOpen != true)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }

        //    int elemId = GetElementId(TABLE_NAME, element.Name);
        //    DataTable table = GetChangeTableRow(elemId, EXT_INT_ID, changeTableIndex, STATE_INDEX_COL_NAME, CHANGE_TABLE_NAME);

        //    if (table.Rows.Count == 1)
        //    {
        //        DataRow row = table.Rows[0];
        //        return CreateElementFromRowData(row.ItemArray);
        //    }
        //    else
        //    {
        //        //something went wrong
        //        return null;
        //    }
        //}

        //public virtual ChildElement Redo(ChildElement element, int changeTableIndex)
        //{
        //    if (Storage.Connection.Instance.IsOpen != true)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }

        //    int elemId = GetElementId(TableName, element.Name);
        //    DataTable table = GetChangeTableRow(elemId, EXT_INT_ID, changeTableIndex, STATE_INDEX_COL_NAME, CHANGE_TABLE_NAME);

        //    if (table.Rows.Count == 1)
        //    {
        //        DataRow row = table.Rows[0];
        //        return CreateElementFromRowData(row.ItemArray);
        //    }
        //    else
        //    {
        //        //something went wrong
        //        return null;
        //    }
        //}

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


        //public virtual ChildElement GetPreviousElementFromChangeTable(ChildElement element, int changeTableIndex)
        //{
        //    ChildElement prevElement = null;
        //    string changeTableName = ChangeTableConstant + element.Name+ "-ChangeTable";

        //    DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
        //    if (changeTableIndex < changeTableView.NumberOfRows)
        //    {
        //        if (changeTableView != null)
        //        {
        //            if (!Storage.Connection.Instance.IsOpen)
        //            {
        //                Storage.Connection.Instance.Open();
        //            }
        //            object[] rowData = changeTableView.GetRow(changeTableIndex);

        //            prevElement = CreateElementFromRowData(rowData);

        //            Storage.Connection.Instance.Close();
        //        }
        //    }
        //    return prevElement;
        //}

        //public virtual ChildElement GetNextElementFromChangeTable(ChildElement element, int changeTableIndex)
        //{
        //    string changeTableName = ChangeTableConstant + element.Name+ "-ChangeTable";

        //    ChildElement nextElement = null;
        //    DatabaseManager.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
        //    if (changeTableIndex >= 0)
        //    {
        //        if (changeTableView != null)
        //        {
        //            if (!Storage.Connection.Instance.IsOpen)
        //            {
        //                Storage.Connection.Instance.Open();
        //            }
        //            object[] rowData = changeTableView.GetRow(changeTableIndex);

        //            nextElement = CreateElementFromRowData(rowData);

        //            Storage.Connection.Instance.Close();
        //        }
        //    }
        //    return nextElement;
        //}


        #endregion

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
                SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
                command.CommandText = "select ID from " + tableName + " where Name = '" + elementName + "'";
                retval = Convert.ToInt32(command.ExecuteScalar());
            }
            catch(Exception e)
            {
                //some message? Name doesn't exist in the database.
                retval = -1;
            }
            return retval;
        }

        //private int GetNextChangeTableStateNumber(string tableName, string elementName)
        //{
        //    int retval = -1;
        //    try
        //    {
        //        SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
        //        command.CommandText = "select ID from " + tableName + " where Name = '" + elementName + "'";
        //        retval = Convert.ToInt32(command.ExecuteScalar());
        //    }
        //    catch (Exception e)
        //    {
        //        //some message? Name doesn't exist in the database.
        //        retval = -1;
        //    }
        //    return retval;
        //}

        //public DataTable GetChangeTableRow(int id, string idColumnName, int stateIndex,
        //    string stateIndexColumnName, string tableName)
        //{
        //    if (Storage.Connection.Instance.IsOpen != true)
        //    {
        //        Storage.Connection.Instance.Open();
        //    }
        //    DataTable tab = new DataTable();
        //    if (Storage.Connection.Instance.Reader.TableNames.Contains(tableName))
        //    {
        //        List<object[]> rows = new List<object[]>();
        //        SQLiteCommand command = Storage.Connection.Instance.Reader.DbConnection.CreateCommand();
        //        command.CommandText = "select * from " + tableName + " where " + idColumnName + " = " + id +
        //            " and " + stateIndexColumnName + " = " + stateIndex;
        //        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
        //        adapter.Fill(tab);
        //    }
        //    return tab;
        //}


    }
}
