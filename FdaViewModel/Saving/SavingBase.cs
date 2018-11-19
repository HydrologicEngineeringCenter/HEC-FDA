using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving
{
    public abstract class SavingBase : BaseViewModel
    {
        private const int MAX_CHANGE_NUMBER = 4;

        public Study.FDACache StudyCacheForSaving { get; set; }

        #region Utilities
        public List<ChildElement> CreateElementsFromRows(string tableName, Func<object[], ChildElement> createElemsFromRowDataAction)
        {
            List<ChildElement> elems = new List<ChildElement>();
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }

            DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(tableName);
            if (dtv != null)
            {
                //add an element based on a row element;
                for (int i = 0; i < dtv.NumberOfRows; i++)
                {
                    elems.Add(createElemsFromRowDataAction(dtv.GetRow(i)));
                }
                //if (Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Close();
            }
            return elems;
        }

        private int GetElementIndexInTable(DataBase_Reader.DataTableView tableView, string name, int nameIndexInTheRow)
        {
            if (tableView != null)
            {
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

        #endregion

        #region save new
        public void SaveNewElementToParentTable(object[] rowData, string tableName, string[] TableColumnNames, Type[] TableColumnTypes)
        {
            //if (!element.SavesToRow()) return;


            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(tableName);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(tableName, TableColumnNames, TableColumnTypes);
                tbl = Storage.Connection.Instance.GetTable(tableName);
            }

            tbl.AddRow(rowData);
            tbl.ApplyEdits();
        }

        public void SaveElementToChangeTable(string elementName, object[] rowData, string changeTableConstant, string[] TableColumnNames, Type[] TableColumnTypes)
        {

            string changeTableName = changeTableConstant + elementName + "-ChangeTable";

            DataBase_Reader.DataTableView changeTable = Storage.Connection.Instance.GetTable(changeTableName);
            if (changeTable == null)
            {
                Storage.Connection.Instance.CreateTable(changeTableName, TableColumnNames, TableColumnTypes);
                changeTable = Storage.Connection.Instance.GetTable(changeTableName);
            }

            changeTable.AddRow(rowData);
            changeTable.ApplyEdits();

        }

        public void SaveCurveTable(Statistics.UncertainCurveDataCollection curve, string changeTableConstant, string lastEditDate)
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            curve.toSqliteTable(changeTableConstant + lastEditDate);
            
        }


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
                DataBase_Reader.DataTableView parentTableView = Storage.Connection.Instance.GetTable(tableName);
                if (parentTableView != null)
                {
                    int parentTableIndex = GetElementIndexInTable(parentTableView, element.Name, 0);
                    if (parentTableIndex != -1)
                    {
                        //object[] parentRow = parentTableView.GetRow(parentTableIndex);
                        parentTableView.DeleteRow(parentTableIndex);
                        parentTableView.ApplyEdits();
                    }
                }
            }

        }

        public void DeleteChangeTableAndAssociatedTables(ChildElement element, string changeTableConstant)
        {
            string changeTableName = changeTableConstant + element.Name + "-ChangeTable";
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
            if (changeTableView != null)
            {
                //loop through all rows and delete all associated tables
                //then delete the change table
                foreach (object[] row in changeTableView.GetRows(0, changeTableView.NumberOfRows-1))
                {
                    string tableName = changeTableConstant + row[1];//this is the edit time
                    Storage.Connection.Instance.DeleteTable(tableName);//do i need to check if these exist?
                }
                Storage.Connection.Instance.DeleteTable(changeTableName);
            }
        }



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

        public void UpdateParentTableRow(string elementName, int changeIndex, object[] rowData, string oldName, string tableName, bool hasChangeTable = false, string changeTableConstant = "")
        {
            if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
            DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(tableName);

            

            bool nameHasChanged = false;
            if (!oldName.Equals(elementName))
            {
                nameHasChanged = true;
            }

            int rowIndex = GetElementIndexInTable(tableView, oldName, 0);
            if (rowIndex != -1)
            {

                //possibly need to change the name in associated table
                if (nameHasChanged && hasChangeTable)
                {
                    Storage.Connection.Instance.RenameTable(changeTableConstant + oldName + "-ChangeTable", changeTableConstant + elementName + "-ChangeTable");
                }


                tableView.EditRow(rowIndex, rowData);
                tableView.ApplyEdits();

                if (hasChangeTable)
                {
                    UpdateElementChangeTable(changeIndex, rowData, elementName, changeTableConstant);
                }
            }
        }

        public bool DidParentTableRowValuesChange(ChildElement element, object[] rowData, string oldName, string tableName)
        {
            if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
            DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(tableName);


            int rowIndex = GetElementIndexInTable(tableView, oldName, 0);
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

        public bool AreCurvesDifferent(Statistics.UncertainCurveDataCollection oldCurve, Statistics.UncertainCurveDataCollection newCurve)
        {
            bool isModified = false;
            if (oldCurve.Distribution != newCurve.Distribution) { isModified = true; }
            if (oldCurve.Count != newCurve.Count) { isModified = true; }
            if (oldCurve.GetType() != newCurve.GetType()) { isModified = true; }
            //are all x values the same
            for (int i = 0; i < oldCurve.XValues.Count(); i++)
            {
                if (oldCurve.XValues[i] != newCurve.XValues[i])
                {
                    isModified = true;
                    break;
                }
            }
            for (int i = 0; i < oldCurve.YValues.Count(); i++)
            {
                if (oldCurve.YValues[i] != newCurve.YValues[i])
                {
                    isModified = true;
                    break;
                }
            }
            return isModified;
        }



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


        /// <summary>
        /// This is where the re sorting and adding and deleting or rows occurs for the change table.
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
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
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


        #endregion






        #region UndoRedo

        abstract internal string ChangeTableConstant { get; }
        abstract public ChildElement CreateElementFromRowData(object[] rowData);


        public ChildElement Undo(ChildElement element, int changeTableIndex)
        {
            ChildElement prevElement = null;
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(ChangeTableConstant + element.Name +  "-ChangeTable");
            if (changeTableIndex < changeTableView.NumberOfRows - 1)
            {
                prevElement = GetPreviousElementFromChangeTable(element, changeTableIndex + 1);
                //if (prevElement != null)// null if out of range index
                //{
                //    ChangeTableIndex += 1;
                //    //UpdateUndoRedoVisibility(ChangeTableIndex, ChangeTableName);
                //    //UpdateIndividualUndoRedoRows();
                //}
            }
            return prevElement;
        }

      


        public ChildElement Redo(ChildElement element, int changeTableIndex)
        {
            ChildElement nextElement = null;
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(ChangeTableConstant + element.Name+ "-ChangeTable");
            if (changeTableView != null)
            {
                //get the previous state
                if (changeTableIndex > 0)
                {
                    nextElement = (ChildElement)GetNextElementFromChangeTable(element, changeTableIndex - 1);
                    //if (nextElement != null)// null if out of range index
                    //{
                    //    ChangeTableIndex -= 1;
                    //    //UpdateUndoRedoVisibility(ChangeIndex, ChangeTableName);
                    //    //UpdateIndividualUndoRedoRows();
                    //}
                }
            }
            return nextElement;
        }

        public ObservableCollection<UndoRedoRowItem> CreateUndoRedoRows(ChildElement element)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }

            string changeTableName = ChangeTableConstant + element.Name +  "-ChangeTable";
            DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(changeTableName);
            if (tableView == null) { return new ObservableCollection<UndoRedoRowItem>(); }

            int nameIndex = 0;
            int dateIndex = 1;
            ObservableCollection<UndoRedoRowItem> retVals = new ObservableCollection<UndoRedoRowItem>();
            foreach (object[] row in tableView.GetRows(0, tableView.NumberOfRows - 1))
            {
                retVals.Add(new UndoRedoRowItem(row[nameIndex].ToString(), row[dateIndex].ToString()));
            }
            return retVals;
        }


        public virtual ChildElement GetPreviousElementFromChangeTable(ChildElement element, int changeTableIndex)
        {
            ChildElement prevElement = null;
            string changeTableName = ChangeTableConstant + element.Name+ "-ChangeTable";

            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
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
            string changeTableName = ChangeTableConstant + element.Name+ "-ChangeTable";

            ChildElement nextElement = null;
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
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


        #endregion

    }
}
