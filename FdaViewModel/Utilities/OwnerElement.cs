using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FdaViewModel.Utilities
{
    public abstract class OwnerElement: OwnedElement
    {
        #region Notes
        #endregion
        #region Fields
        protected ObservableCollection<OwnedElement> _Elements;
        private bool _IsExpanded = true;
        private int _FontSize = 14;
        private bool _IsBold = true;
        private const int MAX_CHANGE_NUMBER = 4;
        //private object _CustomTreeViewHeader;
        #endregion
        #region Properties
        //public object CustomTreeViewHeader
        //{
        //    get { return _CustomTreeViewHeader; }
        //    set { _CustomTreeViewHeader = value; NotifyPropertyChanged(nameof(CustomTreeViewHeader)); }
        //}
        public ObservableCollection<OwnedElement> Elements
        {
            get { return _Elements; }
            set { _Elements = value;  NotifyPropertyChanged(nameof(Elements)); }
        }
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value;  NotifyPropertyChanged(nameof(IsExpanded)); }
        }
        public int FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value;NotifyPropertyChanged(nameof(FontSize)); }
        }
        public bool IsBold
        {
            get { return _IsBold; }
            set { _IsBold = value; NotifyPropertyChanged(nameof(IsBold)); }
        }
        #endregion
        #region Constructors
        public OwnerElement(BaseFdaElement owner): base(owner)
        {
            _Elements = new ObservableCollection<OwnedElement>();
        }
        #endregion
        #region Voids

        public virtual void Rename(object sender, EventArgs e)
        {
            OwnedElement ele = (OwnedElement)sender;
            RenameLoopLogic(ele);
        }

        private void RenameLoopLogic(OwnedElement ele)
        {
            do
            {
                FdaModel.Utilities.Messager.ErrorMessage error = new FdaModel.Utilities.Messager.ErrorMessage("An element of type " + ele.GetType() + " and name " + ele.Name + " was attempted to be added to " + Name + " but " + Name + " already has an element with that name.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Major);
                ReportMessage(error);
                int i = 1;
                string prevname = ele.Name;
                do
                {
                    ele.Name = prevname + "_" + i;
                    i++;
                } while (CheckForNameConflict(ele));
                RenameVM renameViewModel = new RenameVM(ele.Name);
                Navigate(renameViewModel, true, true);
                if (renameViewModel.WasCancled)
                {
                    //user aborted
                    return;
                }
                else
                {
                    ele.Name = renameViewModel.Name;
                }
            }
            while (CheckForNameConflict(ele));
        }

        public void AddElement(OwnedElement ele, bool newElement = true)
        {
            if (CheckForNameConflict(ele))
            {
                // give user opportunity to rename.
                do
                {
                    FdaModel.Utilities.Messager.ErrorMessage error = new FdaModel.Utilities.Messager.ErrorMessage("An element of type " + ele.GetType() + " and name " + ele.Name + " was attempted to be added to " + Name + " but " + Name + " already has an element with that name.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Major);
                    ReportMessage(error);
                    int i = 1;
                    string prevname = ele.Name;
                    do
                    {
                        ele.Name = prevname + "_" + i;
                        i++;
                    } while (CheckForNameConflict(ele));
                    RenameVM renameViewModel = new RenameVM(ele.Name);
                    Navigate(renameViewModel, true, true);
                    if (renameViewModel.WasCancled)
                    {
                        //user aborted
                        return;
                    }
                    else
                    {
                        ele.Name = renameViewModel.Name;
                    }
                }
                while (CheckForNameConflict(ele));
            }

            ele.RequestNavigation += Navigate;
            ele.RequestShapefilePaths += ShapefilePaths;
            ele.RequestShapefilePathsOfType += ShapefilePathsOfType;
            ele.RequestAddToMapWindow += AddToMapWindow;
            ele.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            ele.TransactionEvent += AddTransaction;
            Elements.Add(ele);
            if (newElement)
            {
                SaveNewElement(ele);
            }
            ////special logic for the WSE's because they have a subfolder to the hydraulics folder that needs to be changed as well
            //if (ele.GetType() == typeof(WaterSurfaceElevation.WaterSurfaceElevationElement))
            //{
            //    //rename the directory
            //    string oldDirectoryPath = Storage.Connection.Instance.HydraulicsDirectory + "\\" + this.Name;
            //    string newDirectoryPath = Storage.Connection.Instance.HydraulicsDirectory + "\\";// + ;
            //    if (System.IO.Directory.Exists(oldDirectoryPath))
            //    {
            //        System.IO.Directory.Move(oldDirectoryPath, newDirectoryPath);
            //    }
            //}

            IsExpanded = true;
            
        }
        public abstract void AddBaseElements();
        public override void Save()
        {
            Storage.Connection.Instance.DeleteTable(TableName); // always delete owner tables, and rewrite them.  This simplifies checking for removal, sorting, or adding owned elements.
            string[] names = TableColumnNames();
            Type[] types = TableColumnTypes();
            Storage.Connection.Instance.CreateTable(TableName, names, types);
            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
            foreach (OwnedElement ele in Elements)
            {
                if (ele.SavesToRow()) { tbl.AddRow(ele.RowData()); }
                if (ele.TableContainsGeoData == false)
                {
                    if (ele.SavesToTable()) { ele.Save(); }
                }
                
            }
            tbl.ApplyEdits();
        }
        public void UpdateParentTable()
        {
            Storage.Connection.Instance.DeleteTable(TableName); // always delete owner tables, and rewrite them.  This simplifies checking for removal, sorting, or adding owned elements.
            string[] names = TableColumnNames();
            Type[] types = TableColumnTypes();
            Storage.Connection.Instance.CreateTable(TableName, names, types);
            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
            foreach (OwnedElement ele in Elements)
            {
                if (ele.SavesToRow()) { tbl.AddRow(ele.RowData()); }
            }
            tbl.ApplyEdits();
        }

        /// <summary>
        /// This method is to be used after an editor has closed. If the user changed any information
        /// that gets stored in this ownerElement row data, then we need to update it. If the user
        /// changes the name of the element and that element saves to a table, then update that tables name.
        /// </summary>
        /// <param name="oldName">Name before editing. This is what is used to find the row in this owner's table</param>
        /// <param name="elem"></param>
        /// <param name="nameIndexInRow">This should always be zero, but if it is not, then just tell me what index in the row you are at.</param>
        public void UpdateTableRowIfModified(string oldName, OwnedElement elem, int nameIndexInRow = 0)
        {

            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            if (elem.SavesToRow() == false) { return; }
            DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(TableName);

            int rowIndex = -1;
            object[] elemRowValues = elem.RowData();
            List<object[]> rows = tableView.GetRows(0, Elements.Count - 1);
            //i need to get the row index. It is easy to get the row index based off the Elements of this owner, but
            //i felt that if we later sort the elements or something that the database rows won't be in the same order
            //as the elements list. So i look through the db for the old name. 
            for(int i = 0;i<rows.Count;i++)
            { 
                if(  ((string)rows[i][nameIndexInRow]).Equals(oldName))
                {
                    rowIndex = i;
                    break;
                }
            }
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
                //Type[] colTypes = owner.TableColumnTypes();
                //if (rowIsModified == false)
                //{
                //    object[] dbRow = rows[rowIndex];
                //    for (int j = 0; j < elemRowValues.Length; j++)
                //    {
                //        Type t = rowTypes[j];
                //        if(t == typeof(int))
                //        {

                //        }
                //        (t)Convert.ChangeType(dbRow[j], typeof(t);

                //        if(dbRow[j] is rowTypes[j])
                //        {

                //        }
                //        if ((dbRow[j]).Equals(elemRowValues[j]))
                //        {
                //            rowIsModified = true;
                //            break;
                //        }
                //    }
                //}

                //if (rowIsModified)
                {

                    tableView.EditRow(rowIndex, elemRowValues);
                    tableView.ApplyEdits();
                    //possibly need to change the name in associated table
                    if(nameHasChanged)
                    {
                       // Storage.Connection.Instance.RenameTable(elem.GetTableConstant() + oldName, elem.TableName);
                        Storage.Connection.Instance.RenameTable(elem.GetTableConstant() + oldName + "-ChangeTable", elem.ChangeTableName());
                    }


                    RearangeElementChangeTable(elem.Name, elem);
                  
                    Storage.Connection.Instance.Close();
                }
            }
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
        private void RearangeElementChangeTable(string oldName, OwnedElement elem)
        {
            string changeTableName = elem.GetTableConstant() + oldName + "-ChangeTable";
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);
            System.Data.DataTable table = changeTableView.ExportToDataTable();


            //grab the currently selected row
            DataRow row = table.NewRow();
            for(int j = 0;j< table.Columns.Count;j++)
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
            for(int i = 0;i<newRowData.Length;i++)
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

        /// <summary>
        /// This is where the re sorting and adding and deleting or rows occurs for the change table.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="elem"></param>
        //private void RearangeElementChangeTable_OldVer(string oldName, OwnedElement elem)
        //{

        //    string changeTableName = elem.GetTableConstant() + oldName + "-ChangeTable";
        //    DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(changeTableName);

        //    if (changeTableView.NumberOfRows == 0)//this should never be true
        //    {
        //        changeTableView.AddRow();
        //        AddRowDataToChangeTableAtZero(changeTableView, elem);
        //    }
        //    else
        //    {
        //        MoveAllChangeTableRowsDown(changeTableView);
        //        MoveChangeTableRowFromIndexToZero(changeTableView, elem.ChangeIndex + 1);
        //        //remove elem.index +1
        //        DeleteRowAtIndex(elem.ChangeIndex + 1, changeTableName);

        //        //if(it was actually modified) then add the new one into 0
        //        MoveAllChangeTableRowsDown(changeTableView);

        //        //add the new row
        //        AddRowDataToChangeTableAtZero(changeTableView, elem);

        //        //if the table was full when we started, then there is one extra row.
        //        //delete the last row
        //        if (changeTableView.NumberOfRows > MAX_CHANGE_NUMBER)
        //        {

        //            //delete the table associated with that row
        //            if (elem.SavesToTable())
        //            {
        //                string nameOfChangeTable = elem.GetTableConstant() + changeTableView.GetRow(MAX_CHANGE_NUMBER)[1].ToString();
        //                Storage.Connection.Instance.DeleteTable(nameOfChangeTable);
        //            }
        //            //delete the last row in the change table
        //            DeleteRowAtIndex(MAX_CHANGE_NUMBER, changeTableName);
        //        }
        //        changeTableView.ApplyEdits();
        //    }
            
        //}
        //private void DeleteRowAtIndex(int index, string tableName)
        //{
        //    DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(tableName);
        //    changeTableView.DeleteRow(index);
        //    changeTableView.ApplyEdits();
        //}
        //private void AddRowDataToChangeTableAtZero(DataBase_Reader.DataTableView table, OwnedElement elem)
        //{
        //    //object[] rowToAdd = elem.RowData();
        //    //string lastEditDate = rowToAdd[1].ToString();
        //    //rowToAdd[1] = elem.GetTableConstant() + lastEditDate;
        //    table.EditRow(0, elem.RowData());
        //    //table.ApplyEdits();

        //    if(elem.SavesToTable())
        //    {
        //        elem.Save();
        //    }
        //}

        //private void MoveChangeTableRowFromIndexToZero(DataBase_Reader.DataTableView table, int index)
        //{
        //    //open and close??
        //    object[] rowToMove = table.GetRow(index);
        //    table.EditRow(0, rowToMove);
        //    //table.ApplyEdits();

        //}

       
        //private void MoveAllChangeTableRowsDown(DataBase_Reader.DataTableView table)
        //{
            
        //        table.AddRow();
        //        table.ApplyEdits();
            
            
        //    for (int i = table.NumberOfRows - 1; i > 0; i--)
        //    {
        //        table.EditRow(i, table.GetRow(i - 1));
                
        //    }
        //    //table.ApplyEdits();
        //}
        //private void InsertRowIntoChangeTableAtZero(DataBase_Reader.DataTableView table, object[] rowToInsert)
        //{
        //    table.EditRow(0, rowToInsert);
        //    table.ApplyEdits();
        //    //if it isn't full then add a row to the end
        //    int numRows = table.NumberOfRows;
        //    if (numRows == MAX_CHANGE_NUMBER)
        //    {
        //        //since the table is full, we will need to delete one
        //        //we will delete the last row unless the row that was modified was the last row. (index 4)
        //        // in that case we would delete index 3
        //        //delete the table associated with that row
        //        if (elem.ChangeIndex == MAX_CHANGE_NUMBER - 1)
        //        {
        //            //delete max -2
        //        }
        //        else
        //        {
        //            //delete max -1

        //        }

        //        string nameOfChangeTable = changeTableView.GetRow(4)[0].ToString();
        //        Storage.Connection.Instance.DeleteTable(elem.GetTableConstant() + nameOfChangeTable + 4);
        //        //delete the last row in the change table
        //        changeTableView.DeleteRow(4);
        //        changeTableView.ApplyEdits();
        //    }

        //    changeTableView.AddRow();
        //    numRows += 1;


        //    int numRows = table.NumberOfRows;
        //    for (int i = numRows - 1; i > 0; i--)
        //    {
        //        changeTableView.EditRow(i, changeTableView.GetRow(i - 1));
        //        changeTableView.ApplyEdits();
        //        if (elem.SavesToTable())
        //        {
        //            string nameOfChangeTable = changeTableView.GetRow(i)[0].ToString();
        //            Storage.Connection.Instance.RenameTable(elem.GetTableConstant() + nameOfChangeTable + (i - 1).ToString(), elem.GetTableConstant() + nameOfChangeTable + i);
        //        }
        //    }
        //}

        public void UpdateExistingElement(OwnedElement element)
        {
            //find the row in the parent table that we are dealing with
            //replace that row with the element.rowData
            //if it saves to table then re write that table. Don't update the last edit date?
        }

        public virtual void SaveNewElement(OwnedElement element)
        {
            if (!element.SavesToRow()) return;


            DataBase_Reader.DataTableView tbl = Storage.Connection.Instance.GetTable(TableName);
            if (tbl == null)
            {
                Storage.Connection.Instance.CreateTable(TableName, TableColumnNames(), TableColumnTypes());
                tbl = Storage.Connection.Instance.GetTable(TableName);
            }

            //create this objects change table
            string changeTableName = element.ChangeTableName();
            DataBase_Reader.DataTableView changeTable = Storage.Connection.Instance.GetTable(changeTableName);
            if(changeTable == null)
            {
                Storage.Connection.Instance.CreateTable(changeTableName, TableColumnNames(), TableColumnTypes());
                changeTable = Storage.Connection.Instance.GetTable(changeTableName);
            }

            changeTable.AddRow(element.RowData());
            changeTable.ApplyEdits();

            tbl.AddRow(element.RowData());
            tbl.ApplyEdits();
            if (element.SavesToTable()) { element.Save(); }
        }
        #endregion
        #region Functions
        public abstract string[] TableColumnNames();
        public abstract Type[] TableColumnTypes();
        public bool CheckForNameConflict(OwnedElement ele)
        {
           
            foreach (OwnedElement o in _Elements)
            {
                if (o.Name.Equals(ele.Name)) { return true; }
            }
            return false;
            
        }
        public override BaseFdaElement GetElementOfTypeAndName(Type t, string name)
        {
            foreach(BaseFdaElement ele in _Elements)
            {
                if(ele.GetType()==t & ele.Name.Equals(name))
                {
                    return ele;
                }
            }
            return _Owner.GetElementOfTypeAndName(t, name);
        }
        public override List<T> GetElementsOfType<T>()
        {
            List<T> ret = new List<T>();
            BaseFdaElement root = this as BaseFdaElement;
            OwnerElement owner = _Owner as OwnerElement;
            do
            {
                if (root.GetType().BaseType == typeof(OwnerElement))//this element may not have any, but its kids may be owners who have some..
                {
                    root = owner as BaseFdaElement;
                    owner = owner._Owner as OwnerElement; 
                }
            } while (owner != null);
            ret.AddRange(root.GetElementsOfType<T>());// i may not have any, but my parent may have kids who are owners who have some.
            return ret;
        }
        public List<T> GetOwnedElementsOfType<T>() where T: OwnedElement
        {
            List<T> ret = new List<T>();
            foreach (OwnedElement ele in _Elements)
            {
                T casetedEle = ele as T;
                if (!(casetedEle == null))
                {
                    ret.Add(casetedEle);
                }
                else if (ele.GetType().BaseType == typeof(OwnerElement))//this element may not have any, but its kids may be owners who have some..
                {
                    OwnerElement owner = ele as OwnerElement;
                    ret.AddRange(owner.GetOwnedElementsOfType<T>());
                }
            }
            //ret.AddRange(_Owner.GetElementsOfType<T>());// i may not have any, but my parent may have kids who are owners who have some.
            return ret;
        }
        public override bool SavesToRow()
        {
            return false;
        }
        public override bool SavesToTable()
        {
            return true;
        }
        public override object[] RowData()
        {
            throw new NotImplementedException();
        }
        public virtual OwnedElement CreateElementFromRowData(object[] rowData) { return null; }
      
        public abstract void AddElement(object[] rowData);
        public virtual void AddChildrenFromTable()
        {
            if (SavesToTable())
            {
                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable(TableName);
                if (dtv != null)
                {
                    //add an element based on a row element;
                    if (!Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Open();
                    for(int i = 0; i < dtv.NumberOfRows; i++)
                    {
                        AddElement(dtv.GetRow(i));
                    }
                    if (Storage.Connection.Instance.IsOpen) Storage.Connection.Instance.Close();
                }
            }
            else
            {
                foreach (OwnedElement ele in Elements)
                {
                    if(ele is OwnerElement)
                    {
                        ((OwnerElement)ele).AddChildrenFromTable();
                    }
                }
            }

        }
        #endregion
    }
}
