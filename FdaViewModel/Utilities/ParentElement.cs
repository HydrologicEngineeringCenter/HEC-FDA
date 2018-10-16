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
        #endregion
        #region Properties
     
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
            string newName = ele.Name;
            RenameLoopLogic(ref newName);
        }
        /// <summary>
        /// Returns a boolean that indicates if the user clicked cancel or not.
        /// Essentially the bool tells you if the name is unique. 
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        private bool RenameLoopLogic(ref string newName)
        {
            do
            {
                FdaModel.Utilities.Messager.ErrorMessage error = new FdaModel.Utilities.Messager.ErrorMessage("An element with name " + newName + " was attempted to be added to " + Name + " but " + Name + " already has an element with that name.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Major);
                ReportMessage(error);
                int i = 1;
                string prevname = newName;
                do
                {
                    newName = prevname + "_" + i;
                    i++;
                } while (CheckForNameConflict(newName));
                RenameVM renameViewModel = new RenameVM(newName);
                Navigate(renameViewModel, true, true);
                if (renameViewModel.WasCanceled)
                {
                    //user aborted
                    return false;
                }
                else
                {
                    newName = renameViewModel.Name;
                }
            }
            while (CheckForNameConflict(newName));
            return true;
        }
        /// <summary>
        /// bool tells you if the name is unique.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool HandleNameConflict(ref string newName)
        {
            bool isNameUnique = true;
            if (CheckForNameConflict(newName))
            {
                // give user opportunity to rename.
                isNameUnique = RenameLoopLogic(ref newName);
            }
            return isNameUnique;
        }

        public void AddElement(OwnedElement ele, bool newElement = true)
        {
            string newName = ele.Name;
            if( HandleNameConflict(ref newName) == false)
            {
                //user canceled the rename
                return;
            }
            else
            {
                //the name possibly changed so assign it to the element
            ele.Name = newName;
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

        public void AddOwnerRules(BaseViewModel editorVM, string originalName = null)
        {
            bool excludeOldNameFromSearch = false;
            if(originalName != null)
            {
                excludeOldNameFromSearch = true;
            }
            List<string> existingElements = new List<string>();
            foreach (OwnedElement elem in Elements)
            {
                if (excludeOldNameFromSearch == true && elem.Name.Equals(originalName))
                {
                    continue;
                }
                else
                {
                    existingElements.Add(elem.Name);
                }
            }
            //if(excludeOldNameFromSearch == true && editorVM)

            foreach (string existingName in existingElements)
            {
                editorVM.AddRule(nameof(Name), () =>
                {
                    return editorVM.Name != existingName;
                }, "This name is already used. Names must be unique.");
            }

        }
        /// <summary>
        /// This is here to be overridden by all the editors that can save while editing. 
        /// </summary>
        /// <param name="editorVM"></param>
        /// <returns></returns>
        public virtual OwnedElement CreateElementFromEditor(Editors.BaseEditorVM editorVM)
        {
            return null;
        }

        public virtual void AssignValuesFromElementToEditor(Editors.BaseEditorVM editorVM, OwnedElement element)
        {
            //return null;
        }
        public virtual void AssignValuesFromEditorToElement(Editors.BaseEditorVM editorVM, OwnedElement element)
        {
            //return null;
        }

        //public void SaveNewElement(ISaveUndoRedo editorVM)
        //{
        //    string newName = editorVM.Name;
        //    bool isNameUnique = HandleNameConflict(ref newName);
        //    if (isNameUnique == false)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        editorVM.UpdateNameWithNewValue(newName);
        //    }

        //    editorVM.CurrentElement = CreateElementFromEditor(editorVM);//editorVM.CreateNewElement();
        //    //this is wierd because "AddElement" will run the same repeat name check, but i need to be able to grab
        //    //the new name chosen and update the editor, so i do it beforehand.


        //    AddElement(editorVM.CurrentElement);//this will save it and add it to its parent
        //    editorVM.SaveAction = (foo) => SaveExistingElement(foo);
        //}

        public void SaveNewElement(Editors.SaveUndoRedoHelper saveHelper, OwnedElement element)
        {
            //editorVM.CreateNewElement();
            //this is wierd because "AddElement" will run the same repeat name check, but i need to be able to grab
            //the new name chosen and update the editor, so i do it beforehand.


            AddElement(element);//this will save it and add it to its parent
            saveHelper.SaveAction = (helper,elem) => SaveExistingElement(helper,elem);
        }

        //public void SaveExistingElement(ISaveUndoRedo editorVM)
        //{
        //    //only need to check for name conflict if the name has changed. We will always fail if
        //    //we check with the original name because the original name is already in the elements list.
        //    //if it is a new name, then we need to make sure that it is an available name.
        //    string newName = editorVM.Name;
        //    string originalName = editorVM.CurrentElement.Name;
        //    bool nameHasChanged = !originalName.Equals(newName);

        //    if (nameHasChanged)
        //    {

        //        {
        //            bool isNameUnique = HandleNameConflict(ref newName);
        //            if (isNameUnique == false)
        //            {
        //                return;
        //            }
        //            else
        //            {
        //                editorVM.UpdateNameWithNewValue(newName);
        //            }
        //        }
        //    }

        //    Statistics.UncertainCurveDataCollection oldInflowOutflowCurve = editorVM.GetTheElementsCurve();
        //    //capture the current state
        //    editorVM.AssignValuesFromEditorToCurrentElement();
        //    //update parent table row
        //    UpdateTableRowIfModified(this, originalName, editorVM.CurrentElement);
        //    //update its own table
        //    if (editorVM.CurrentElement.SavesToTable())
        //    {
        //        editorVM.CurrentElement.UpdateTableIfModified(originalName, oldInflowOutflowCurve, editorVM.GetTheEditorsCurve());
        //    }

        //    //editorVM.UpdateUndoRedoButtons();
        //    DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(editorVM.CurrentElement.ChangeTableName());
        //    ((BaseViewModel)editorVM).UpdateUndoRedoVisibility(changeTableView, editorVM.CurrentElement.ChangeIndex);
        //}

        public void SaveExistingElement(Editors.SaveUndoRedoHelper saveHelper, OwnedElement element)
        {
            //only need to check for name conflict if the name has changed. We will always fail if
            //we check with the original name because the original name is already in the elements list.
            //if it is a new name, then we need to make sure that it is an available name.
            //string newName = editorVM.Name;
            //string originalName = editorVM.CurrentElement.Name;
            //bool nameHasChanged = !originalName.Equals(newName);

            //if (nameHasChanged)
            //{

            //    {
            //        bool isNameUnique = HandleNameConflict(ref newName);
            //        if (isNameUnique == false)
            //        {
            //            return;
            //        }
            //        else
            //        {
            //            editorVM.UpdateNameWithNewValue(newName);
            //        }
            //    }
            //}

            //Statistics.UncertainCurveDataCollection oldInflowOutflowCurve = editorVM.GetTheElementsCurve();
            ////capture the current state
            //editorVM.AssignValuesFromEditorToCurrentElement();
            //update parent table row
            UpdateTableRowIfModified(this, saveHelper.OldName, element);
            //update its own table
            if (element.SavesToTable())
            {
                element.UpdateTableIfModified(saveHelper.OldName, saveHelper.OldCurve, element.Curve);
            }

            //editorVM.UpdateUndoRedoButtons();
            //DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(saveHelper.SaveElement.ChangeTableName());
            //((BaseViewModel)editorVM).UpdateUndoRedoVisibility(changeTableView, saveHelper.SaveElement.ChangeIndex);
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

        private int GetElementIndexInTable(DataBase_Reader.DataTableView tableView, string name, int nameIndexInTheRow)
        {
            if (tableView != null)
            {
                List<object[]> rows = tableView.GetRows(0, Elements.Count - 1);

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
        public void UpdateTableRowIfModified(OwnerElement owner, string oldName, OwnedElement elem, int nameIndexInTheRow = 0)
        {
            if (!Storage.Connection.Instance.IsOpen)  {Storage.Connection.Instance.Open();  }
            if (elem.SavesToRow() == false) { return; }

            DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(TableName);

            int rowIndex = GetElementIndexInTable(tableView,oldName,nameIndexInTheRow);
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
                    if(nameHasChanged)
                    {                    
                        Storage.Connection.Instance.RenameTable(elem.GetTableConstant() + oldName + "-ChangeTable", elem.ChangeTableName());
                    }

                    tableView.EditRow(rowIndex,elem.RowData());
                    tableView.ApplyEdits();

                    RearangeElementChangeTable(elem.Name, elem);
                  
                    //Storage.Connection.Instance.Close();
                }
            }
        }

        private bool AreListsDifferent(object[] a, object[] b)
        {

            for(int i = 0;i<a.Length;i++)
            {
                //don't evaluate the last edit time which is the second one
                if(i == 1) { continue; }
                if(a[i] == null)
                {
                    a[i] = "";
                }
                if(!a[i].ToString().Equals(b[i].ToString()))
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
    
        public bool CheckForNameConflict(string newName)
        {

            foreach (OwnedElement o in _Elements)
            {
                if (o.Name.Equals(newName)) { return true; }
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
