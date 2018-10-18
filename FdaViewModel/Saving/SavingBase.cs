using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Saving
{
    public abstract class SavingBase
    {
        private const int MAX_CHANGE_NUMBER = 4;

        public Study.FDACache StudyCache { get; set; }
        public List<ChildElement> CreateElementsFromRows( string tableName, Func<object[],ChildElement> createElemsFromRowDataAction)
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


        public void SaveNewElementToParentTable(ChildElement element, string TableName, string[] TableColumnNames, Type[] TableColumnTypes)
        {
            if (!element.SavesToRow()) return;


            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(TableName, TableColumnNames, TableColumnTypes);
                tbl = Storage.Connection.Instance.GetTable(TableName);
            }

            //create this objects change table
            string changeTableName = element.ChangeTableName();
            DataBase_Reader.DataTableView changeTable = Storage.Connection.Instance.GetTable(changeTableName);
            if (changeTable == null)
            {
                Storage.Connection.Instance.CreateTable(changeTableName, TableColumnNames, TableColumnTypes);
                changeTable = Storage.Connection.Instance.GetTable(changeTableName);
            }

            changeTable.AddRow(element.RowData());
            changeTable.ApplyEdits();

            tbl.AddRow(element.RowData());
            tbl.ApplyEdits();
        }




     
        ///////////////////////   save existing  //////////////////////////////
       



        public void SaveExistingElement(string oldName, Statistics.UncertainCurveDataCollection oldCurve, ChildElement element, string tableName)
        {
            //only need to check for name conflict if the name has changed. We will always fail if
            //we check with the original name because the original name is already in the elements list.
            //if it is a new name, then we need to make sure that it is an available name.

            //update parent table row
            UpdateTableRowIfModified(oldName,tableName,  element);
            //update its own table
            if (element.SavesToTable())
            {
                element.UpdateTableIfModified(oldName, oldCurve, element.Curve);
            }

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

        /// <summary>
        /// This method is to be used after an editor has closed. If the user changed any information
        /// that gets stored in this ownerElement row data, then we need to update it. If the user
        /// changes the name of the element and that element saves to a table, then update that tables name.
        /// </summary>
        /// <param name="oldName">Name before editing. This is what is used to find the row in this owner's table</param>
        /// <param name="elem"></param>
        /// <param name="nameIndexInTheRow">This should always be zero, but if it is not, then just tell me what index in the row you are at.</param>
        public void UpdateTableRowIfModified( string oldName, string TableName, ChildElement elem, int nameIndexInTheRow = 0)
        {
            if (!Storage.Connection.Instance.IsOpen) { Storage.Connection.Instance.Open(); }
            if (elem.SavesToRow() == false) { return; }

            DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(TableName);

            int rowIndex = GetElementIndexInTable(tableView, oldName, nameIndexInTheRow);
            if (rowIndex != -1)
            {

                //is anything in this row modified from the original
                bool rowIsModified = false;
                bool nameHasChanged = false;

                //has the name changed
                if (!oldName.Equals(elem.Name))
                {
                    nameHasChanged = true;
                    rowIsModified = true;
                }


                //has anything else in the row changed
                rowIsModified = AreListsDifferent(elem.RowData(), tableView.GetRow(rowIndex));

                if (rowIsModified)
                {

                    //possibly need to change the name in associated table
                    if (nameHasChanged)
                    {
                        Storage.Connection.Instance.RenameTable(elem.GetTableConstant() + oldName + "-ChangeTable", elem.ChangeTableName());
                    }

                    tableView.EditRow(rowIndex, elem.RowData());
                    tableView.ApplyEdits();

                    RearangeElementChangeTable(elem.Name, elem);

                    //Storage.Connection.Instance.Close();
                }
            }
        }


        private bool AreListsDifferent(object[] a, object[] b)
        {

            for (int i = 0; i < a.Length; i++)
            {
                //don't evaluate the last edit time which is the second one
                if (i == 1) { continue; }
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
        private void RearangeElementChangeTable(string oldName, ChildElement elem)
        {
            string changeTableName = elem.GetTableConstant() + oldName + "-ChangeTable";
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
            System.Data.DataTable table = changeTableView.ExportToDataTable();


            //grab the currently selected row
            DataRow row = table.NewRow();
            for (int j = 0; j < table.Columns.Count; j++)
            {
                row[j] = table.Rows[elem.ChangeIndex][j];
            }

            //remove the row from the table
            table.Rows.RemoveAt(elem.ChangeIndex);
            //insert this row back into table at 0
            table.Rows.InsertAt(row, 0);

            //now insert the new row at zero
            object[] newRowData = elem.RowData();
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
                if (elem.SavesToTable())
                {
                    string nameOfChangeTable = elem.GetTableConstant() + table.Rows[MAX_CHANGE_NUMBER][1].ToString();
                    Storage.Connection.Instance.DeleteTable(nameOfChangeTable);
                }
                table.Rows.RemoveAt(MAX_CHANGE_NUMBER);
            }

            //*************************************
            if (elem.SavesToTable())
            {
                elem.Save();
            }

            //now that we have the table where we want it. Replace the old change table with this data table
            Storage.Connection.Instance.DeleteTable(changeTableName);
            table.TableName = changeTableName;
            Storage.Connection.Instance.Reader.SaveDataTable(table);

        }





    }
}
