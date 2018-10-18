using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;
using System.Collections.ObjectModel;

namespace FdaViewModel.Editors
{
    public class SaveUndoRedoHelper  : BaseViewModel 
    {

        private ObservableCollection<Utilities.UndoRedoRowItem> _UndoRedoRows = new ObservableCollection<UndoRedoRowItem>();
        private ObservableCollection<Utilities.UndoRedoRowItem> _UndoRows;
        private ObservableCollection<Utilities.UndoRedoRowItem> _RedoRows;
        private bool _UndoEnabled = false;
        private bool _RedoEnabled = false;
        private bool _IsImporter = false;
        private bool _IsFirstSave = true;

        public int ChangeIndex { get; set; }
        public ObservableCollection<Utilities.UndoRedoRowItem> UndoRedoRows
        {
            get { return _UndoRedoRows; }
            set { _UndoRedoRows = value;  }
        }

        public bool UndoEnabled
        {
            get { return _UndoEnabled; }
            set { _UndoEnabled = value; NotifyPropertyChanged(); }
        }
        public bool RedoEnabled
        {
            get { return _RedoEnabled; }
            set { _RedoEnabled = value; NotifyPropertyChanged(); }
        }


        public ObservableCollection<Utilities.UndoRedoRowItem> UndoRows
        {
            get { return _UndoRows; }
            set { _UndoRows = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<Utilities.UndoRedoRowItem> RedoRows
        {
            get { return _RedoRows; }
            set { _RedoRows = value; NotifyPropertyChanged(); }
        }
        public ChildElement SelectedIndexInRedoList(int index, ChildElement element)
        {
            ChangeIndex -= index;
            return RedoElement(element);
        }

        public ChildElement SelectedIndexInUndoList(int index, ChildElement element)
        {
                ChangeIndex += index;
                return UndoElement(element);            
        }

        public string OldName { get; set; }
        public Statistics.UncertainCurveDataCollection OldCurve { get; set; }
        //public OwnedElement SaveElement { get; set; } 
        //public Action<ChildElement> SaveAction { get; set; }
        public Saving.IPersistable SavingManager { get; set; }

        #region Constructors

        public SaveUndoRedoHelper(Saving.IPersistable savingManager)
        {
            _IsImporter = true;

            //SaveAction = savingAction;
            SavingManager = savingManager;
            UndoRedoRows = new ObservableCollection<UndoRedoRowItem>();

            //LoadInitialStateOfUndoRedoValues();
        }
        /// <summary>
        /// This one is for opening and existing element to edit
        /// </summary>
        /// <param name="savingAction"></param>
        /// <param name="changeTableName"></param>
        public SaveUndoRedoHelper(Saving.IPersistable savingManager, string changeTableName)
        {
            SavingManager = savingManager;
            LoadInitialStateOfUndoRedoValues(changeTableName);
        }

        //public SaveUndoRedoHelper( Action<SaveUndoRedoHelper> savingAction)
        //{
        //    //element.Name = "the great one";
        //    SaveAction = savingAction;
        //    //SaveElement = element;
        //    //SaveElement.Name = "testing blah";
        //    LoadInitialStateOfUndoRedoValues();
        //}

        #endregion

        //there should just be one save here. The two save options in the owner element should just take an OwnedElement and save that. Name pre processing should happen here
        public void Save(string oldName, Statistics.UncertainCurveDataCollection oldCurve, ChildElement elementToSave)
        {

            //if (SaveElement == null)//then this is a new element and first time saving
            //{
            //    SaveElement = element;
            //}
            //elementToSave.LastEditDate = DateTime.Now.ToString("G");

            //should i check if the row and the curve has changed here?
            OldName = oldName;
            string newName = elementToSave.Name;
            OldCurve = oldCurve;

            elementToSave.ChangeIndex = ChangeIndex;
            //SaveElement = elementToSave;
            // SaveAction.Invoke( (T)elementToSave);
            if (_IsImporter && _IsFirstSave)
            {
                SavingManager.SaveNew(elementToSave);
                _IsFirstSave = false;
            }
            else
            {
                SavingManager.SaveExisting(elementToSave, OldName, OldCurve);
            }

            ChangeIndex = 0;
            //add a row to the undoredorows
            UndoRedoRows = CreateUndoRedoRows(elementToSave.ChangeTableName());
            UpdateIndividualUndoRedoRows();
            UpdateUndoRedoVisibility(0, elementToSave.ChangeTableName());
        }

        #region Undo/Redo

        public void LoadInitialStateOfUndoRedoValues(string changeTableName)
        {
            //if (SaveElement == null)
            //{
            //    UndoRedoRows = new ObservableCollection<UndoRedoRowItem>();
            //}
            //else
            //{
                UndoRedoRows = CreateUndoRedoRows(changeTableName);
                UpdateIndividualUndoRedoRows();
                UpdateUndoRedoVisibility(0, changeTableName);
           // }
        }

        public ObservableCollection<UndoRedoRowItem> CreateUndoRedoRows(string changeTableName)
        {
            if(Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }

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

        private void UpdateIndividualUndoRedoRows()
        {

            UndoRows = new ObservableCollection<UndoRedoRowItem>();
           RedoRows = new ObservableCollection<UndoRedoRowItem>();

            int currentIndex = ChangeIndex;
            //load the undo rows
            for (int i = currentIndex + 1; i < UndoRedoRows.Count; i++)
            {
                UndoRows.Add(UndoRedoRows[i]);
            }

            //load the redo rows
            for(int j=currentIndex -1;j>=0;j--)
            {
                RedoRows.Add(UndoRedoRows[j]);
            }
        }

        public ChildElement UndoElement(ChildElement element)
        {
            ChildElement prevElement = null;
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(element.ChangeTableName());
            if (ChangeIndex < changeTableView.NumberOfRows - 1)
            {
                prevElement = element.GetPreviousElementFromChangeTable(ChangeIndex + 1);
                if (prevElement != null)// null if out of range index
                {
                    ChangeIndex += 1;
                    UpdateUndoRedoVisibility(ChangeIndex, element.ChangeTableName());
                    UpdateIndividualUndoRedoRows();
                }
            }
            return prevElement;
        }

        public ChildElement RedoElement(ChildElement element)
        {
            ChildElement nextElement = null;
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(element.ChangeTableName());
            if (changeTableView != null)
            {
                //get the previous state
                if (ChangeIndex > 0)
                {
                    nextElement = (ChildElement)element.GetNextElementFromChangeTable(ChangeIndex - 1);
                    if (nextElement != null)// null if out of range index
                    {
                        ChangeIndex -= 1;
                        UpdateUndoRedoVisibility(ChangeIndex, element.ChangeTableName());
                        UpdateIndividualUndoRedoRows();
                    }
                }
            }
            return nextElement;
        }


        public void UpdateUndoRedoVisibility( int currentIndex, string changeTableName)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(changeTableName);
            if (tableView == null) { return; }

            int numRows = tableView.NumberOfRows;
            //if only 0 or 1 rows, there is no back and forth
            if (numRows < 2)
            {
                UndoEnabled = false;
                RedoEnabled = false;
            }
            else if (currentIndex == numRows - 1)//we are at the oldest record
            {
                UndoEnabled = false;
                RedoEnabled = true;
            }
            else if (currentIndex == 0)//at the newest record
            {
                UndoEnabled = true;
                RedoEnabled = false;
            }
            else //we are in the middle somewhere
            {
                UndoEnabled = true;
                RedoEnabled = true;
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        #endregion



    }
}
