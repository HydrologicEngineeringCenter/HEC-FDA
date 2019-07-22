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
        public event EventHandler RemoveFromTabsDictionary;

        private ObservableCollection<Utilities.UndoRedoRowItem> _UndoRedoRows = new ObservableCollection<UndoRedoRowItem>();
        private ObservableCollection<Utilities.UndoRedoRowItem> _UndoRows;
        private ObservableCollection<Utilities.UndoRedoRowItem> _RedoRows;
        private bool _UndoEnabled = false;
        private bool _RedoEnabled = false;
        private bool _IsImporter = false;
        private bool _IsFirstSave = true;
        private int _ChangeTableIndex = 0;


        public Func<BaseEditorVM, ChildElement> CreateElementFromEditorAction { get; set; }
        public Action<BaseEditorVM, ChildElement> AssignValuesFromElementToEditorAction { get; set; }
        public Action<BaseEditorVM, ChildElement> AssignValuesFromEditorToElementAction { get; set; }
        public int ChangeIndex
        {
            get { return _ChangeTableIndex; }
            set { _ChangeTableIndex = value;  }
        }
        public ObservableCollection<Utilities.UndoRedoRowItem> UndoRedoRows
        {
            get { return _UndoRedoRows; }
            set { _UndoRedoRows = value;  }
        }

        public string ChangeTableName { get; set; }
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
        public Saving.IPersistableWithUndoRedo SavingManager { get; set; }

        #region Constructors

        //public SaveUndoRedoHelper(Saving.IPersistable savingManager)
        //{
        //    _IsImporter = true;

        //    //SaveAction = savingAction;
        //    SavingManager = savingManager;
        //    UndoRedoRows = new ObservableCollection<UndoRedoRowItem>();

        //    //LoadInitialStateOfUndoRedoValues();
        //}
        /// <summary>
        /// This one is for opening an existing element to edit
        /// </summary>
        /// <param name="savingAction"></param>
        /// <param name="changeTableName"></param>
        public SaveUndoRedoHelper(Saving.IPersistableWithUndoRedo savingManager, Func<BaseEditorVM, ChildElement> createElementFromEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromElementToEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromEditorToElementAction)
        {
            //SavingManager.ChangeTableIndexChanged += UpdateStuff;
            _IsImporter = true;
            ChangeIndex = 0;
            //ChangeTableName = changeTableName;
            SavingManager = savingManager;
            AssignValuesFromElementToEditorAction = assignValuesFromElementToEditorAction;
            AssignValuesFromEditorToElementAction = assignValuesFromEditorToElementAction;
            CreateElementFromEditorAction = createElementFromEditorAction;
            //if (_IsImporter == false)
            //{
            //    LoadInitialStateOfUndoRedoValues(changeTableName);
            //}
        }
        public SaveUndoRedoHelper(Saving.IPersistableWithUndoRedo savingManager, ChildElement element, Func<BaseEditorVM, ChildElement> createElementFromEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromElementToEditorAction, Action<BaseEditorVM, ChildElement> assignValuesFromEditorToElementAction)
        {
            _IsImporter = false;
            SavingManager = savingManager;
            ChangeIndex = 0;
            //ChangeTableName = savingManager.ChangeTableConstant + element.Name + "-ChangeTable";        
            LoadInitialStateOfUndoRedoValues(element);
            AssignValuesFromElementToEditorAction = assignValuesFromElementToEditorAction;
            AssignValuesFromEditorToElementAction = assignValuesFromEditorToElementAction;
            CreateElementFromEditorAction = createElementFromEditorAction;
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

        private void RemoveAndReplaceFromTabsDictionary(ChildElement element)
        {
            RemoveFromTabsDictionary?.Invoke(element, new EventArgs());
        }

        private void UpdateUndoRedo()
        {
            UpdateIndividualUndoRedoRows();
            UpdateUndoRedoVisibility(ChangeIndex);
        }

        //there should just be one save here. The two save options in the owner element should just take an OwnedElement and save that. Name pre processing should happen here
        public void Save(string oldName, ChildElement oldElement, ChildElement elementToSave)
        {

            //if (SaveElement == null)//then this is a new element and first time saving
            //{
            //    SaveElement = element;
            //}
            //elementToSave.LastEditDate = DateTime.Now.ToString("G");

            //should i check if the row and the curve has changed here?
            OldName = oldName;
            string newName = elementToSave.Name;
            OldCurve = oldElement.Curve;// oldCurve;

            elementToSave.GUID = Guid.NewGuid();
            //elementToSave.ChangeIndex = ChangeIndex;
            //SaveElement = elementToSave;
            // SaveAction.Invoke( (T)elementToSave);
            if (_IsImporter && _IsFirstSave)
            {
                RemoveAndReplaceFromTabsDictionary(elementToSave);
                SavingManager.SaveNew(elementToSave);
                _IsFirstSave = false;
                if (Study.FdaStudyVM._TabsDictionary.ContainsKey(ParentGUID))
                {
                    //Study.FdaStudyVM._TabsDictionary.Remove
                }
            }
            else
            {
                SavingManager.SaveExisting(oldElement, elementToSave,ChangeIndex);
            }

            ChangeIndex = 0;
            //add a row to the undoredorows
            UndoRedoRows = SavingManager.CreateUndoRedoRows(elementToSave);
            UpdateUndoRedo();
        }

        #region Undo/Redo
        /// <summary>
        /// Gets all the change table rows from the database
        /// </summary>
        /// <param name="element">The element that will be used to get the change table</param>
        public void LoadInitialStateOfUndoRedoValues(ChildElement element)
        {
            //if (SaveElement == null)
            //{
            //    UndoRedoRows = new ObservableCollection<UndoRedoRowItem>();
            //}
            //else
            //{
                 UndoRedoRows = SavingManager.CreateUndoRedoRows(element);// CreateUndoRedoRows();
            UpdateUndoRedo();

            // }
        }

        //public ObservableCollection<UndoRedoRowItem> CreateUndoRedoRows()
        //{
        //    return SavingManager.CreateUndoRedoRows();
        //}

        private void UpdateIndividualUndoRedoRows()
        {

            UndoRows = new ObservableCollection<UndoRedoRowItem>();
           RedoRows = new ObservableCollection<UndoRedoRowItem>();

            //int currentIndex = SavingManager.ChangeTableIndex;
            //load the undo rows
            for (int i = ChangeIndex + 1; i < UndoRedoRows.Count; i++)
            {
                UndoRows.Add(UndoRedoRows[i]);
            }

            //load the redo rows
            for(int j= ChangeIndex - 1;j>=0;j--)
            {
                RedoRows.Add(UndoRedoRows[j]);
            }
        }

        public ChildElement UndoElement(ChildElement element)
        {
            //ChildElement prevElement = null;
            //if (Storage.Connection.Instance.IsOpen != true)
            //{
            //    Storage.Connection.Instance.Open();
            //}
            //DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(ChangeTableName);
            //if (ChangeIndex < changeTableView.NumberOfRows - 1)
            //{
            //    prevElement = element.GetPreviousElementFromChangeTable(ChangeIndex + 1);
            //    if (prevElement != null)// null if out of range index
            //    {
            //        ChangeIndex += 1;
            //        UpdateUndoRedoVisibility(ChangeIndex, ChangeTableName);
            //        UpdateIndividualUndoRedoRows();
            //    }
            //}
            //return prevElement;
            ChildElement prevElement = SavingManager.Undo(element, ChangeIndex);
            ChangeIndex += 1;
            UpdateUndoRedo();
            return prevElement;
        }

        public ChildElement RedoElement(ChildElement element)
        {
            //ChildElement nextElement = null;
            //if (Storage.Connection.Instance.IsOpen != true)
            //{
            //    Storage.Connection.Instance.Open();
            //}
            //DataBase_Reader.DataTableView changeTableView = Storage.Connection.Instance.GetTable(ChangeTableName);
            //if (changeTableView != null)
            //{
            //    //get the previous state
            //    if (ChangeIndex > 0)
            //    {
            //        nextElement = (ChildElement)element.GetNextElementFromChangeTable(ChangeIndex - 1);
            //        if (nextElement != null)// null if out of range index
            //        {
            //            ChangeIndex -= 1;
            //            UpdateUndoRedoVisibility(ChangeIndex, ChangeTableName);
            //            UpdateIndividualUndoRedoRows();
            //        }
            //    }
            //}
            //return nextElement;
            ChildElement nextElement = SavingManager.Redo(element, ChangeIndex);
            ChangeIndex -= 1;
            UpdateUndoRedo();
            return nextElement;
        }


        public void UpdateUndoRedoVisibility( int currentIndex)
        {
            if (Storage.Connection.Instance.IsOpen != true)
            {
                Storage.Connection.Instance.Open();
            }
            //DataBase_Reader.DataTableView tableView = Storage.Connection.Instance.GetTable(changeTableName);
            //if (tableView == null) { return; }

            int numRows = UndoRedoRows.Count;
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

      

        #endregion



    }
}
