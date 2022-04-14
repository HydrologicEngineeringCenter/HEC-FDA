using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Editors;
using System.Windows;
using System.IO;
using HEC.MVVMFramework.ViewModel.Validation;

namespace HEC.FDA.ViewModel.ImpactArea
{
    //[Author("q0heccdm", "10 / 13 / 2016 11:38:36 AM")]
    public class ImpactAreaImporterVM : BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/13/2016 11:38:36 AM
        #endregion
        #region Fields
        private string _Path;
        private ObservableCollection<string> _Paths;
        private List<string> _UniqueFields;
        private ObservableCollection<ImpactAreaRowItem> _ListOfRows;
        private bool _IsInEditMode = false;
        private string _SelectedUniqueName;
        #endregion
        #region Properties
     
        public bool IsInEditMode
        {
            get { return _IsInEditMode; }
            set { _IsInEditMode = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<string> AvailablePaths
        {
            get { return _Paths; }
            set { _Paths = value; NotifyPropertyChanged(); }
        }
        public string SelectedPath
        {
            get { return _Path; }
            set { _Path = value;  NotifyPropertyChanged();}
        }
        public ObservableCollection<ImpactAreaRowItem> ListOfRows
        {
            get { return _ListOfRows; }
            set { _ListOfRows = value; NotifyPropertyChanged(); }
        }
        public List<string> UniqueFields
        {
            get { return _UniqueFields; }
            set { _UniqueFields = value; NotifyPropertyChanged(); }
        }
  
        public string SelectedUniqueName
        {
            get { return _SelectedUniqueName; }
            set { _SelectedUniqueName = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImpactAreaImporterVM(ObservableCollection<string> PolygonPaths, EditorActionManager actionManager):base(actionManager)
        {
            AvailablePaths = PolygonPaths;
            IsInEditMode = false;
        }

        public ImpactAreaImporterVM(ImpactAreaElement element, ObservableCollection<ImpactAreaRowItem> impactAreaRows, EditorActionManager actionManager) :base(element, actionManager)
        {
            Name = element.Name;
            ListOfRows = impactAreaRows;
            Description = element.Description;
            IsInEditMode = true;
        }
        #endregion
        #region Voids
        /// <summary>
        /// This method grabs all the column headers from the dbf and loads them into a unique name combobox.
        /// </summary>
        /// <param name="path"></param>
        public void LoadUniqueNames(string path)
        {
            if (!File.Exists(Path.ChangeExtension(path, "dbf")))
            {
                MessageBox.Show("This path has no associated *.dbf file.", "File Doesn't Exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            SelectedPath = path; //isnt this bound??
            DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(Path.ChangeExtension(SelectedPath, ".dbf"));
            DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

            List<string> uniqueNameList = new List<string>();

            for(int i = 0;i< dtv.ColumnNames.Count(); i++)
            {
                if(dtv.ColumnTypes[i]== typeof(string))
                {
                    uniqueNameList.Add(dtv.ColumnNames[i]);
                }
            }
            if (!(uniqueNameList.Count > 0))
            {
                MessageBox.Show("The selected path: " + SelectedPath + "/ndoes not contain any string fields.", "No String Fields", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            UniqueFields = uniqueNameList;
        }

        public void LoadTheRows()
        {
            if (!File.Exists(Path.ChangeExtension(SelectedPath, "dbf")))
            {
                MessageBox.Show("This path has no associated *.dbf file.", "No dbf File", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(Path.ChangeExtension(SelectedPath, ".dbf"));
            DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

            for (int i = 0; i < dtv.ColumnNames.Count(); i++)
            {
                if (dtv.ColumnNames[i] == SelectedUniqueName)
                {
                    object[] col = dtv.GetColumn(i);
                    ImpactAreaUniqueNameSet iauns = new ImpactAreaUniqueNameSet(dtv.ColumnNames[i], col);
                    ListOfRows = iauns.RowItems;
                }
            }
        }
        #endregion
        #region Functions
        #endregion
        public override void AddValidationRules()
        {
            base.AddValidationRules();

            AddSinglePropertyRule(nameof(SelectedUniqueName), new Rule(() =>
            {
                return SelectedUniqueName == null;
            }, "A unique name has not been selected.", MVVMFramework.Base.Enumerations.ErrorLevel.Severe));

            AddSinglePropertyRule(nameof(ListOfRows), new Rule(() =>
            {
                return ListOfRows == null;
            }, "There are no impact area rows.", MVVMFramework.Base.Enumerations.ErrorLevel.Severe));
        }

        public override void Save()
        {
            if (Description == null) 
            { 
                Description = ""; 
            }
            Saving.PersistenceManagers.ImpactAreaPersistenceManager manager = Saving.PersistenceFactory.GetImpactAreaManager();
            int id = GetElementID(Saving.PersistenceFactory.GetImpactAreaManager());
            ImpactAreaElement elementToSave = new ImpactAreaElement(Name, Description, ListOfRows, SelectedPath, id);
            if (IsCreatingNewElement && HasSaved == false)
            {
                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
            else
            {
                manager.SaveExisting((ImpactAreaElement)OriginalElement, elementToSave);
            }
        }
    }
}
