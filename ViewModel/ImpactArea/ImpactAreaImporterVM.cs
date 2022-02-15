using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using HEC.FDA.ViewModel.Editors;

namespace HEC.FDA.ViewModel.ImpactArea
{
    //[Author("q0heccdm", "10 / 13 / 2016 11:38:36 AM")]
    public class ImpactAreaImporterVM : Editors.BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 10/13/2016 11:38:36 AM
        #endregion
        #region Fields
        private string _Name;
        private string _Path;
        private string _Description;
        private ObservableCollection<string> _Paths;
        private List<string> _UniqueFields;
        //private ObservableCollection<ImpactAreaUniqueNameSet> _UniqueFields;
        //private ImpactAreaUniqueNameSet _SelectedImpactAreaUniqueNameSet;
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
        //public ObservableCollection<ImpactAreaUniqueNameSet> UniqueFields { get { return _UniqueFields; } set { _UniqueFields = value; NotifyPropertyChanged(); } }
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
  
        //public string Description
        //{
        //    get { return _Description; }
        //    set { _Description = value; NotifyPropertyChanged(); }
        //}
        //public ImpactAreaUniqueNameSet SelectedImpactAreaUniqueNameSet
        //{
        //    get { return _SelectedImpactAreaUniqueNameSet; }
        //    set { _SelectedImpactAreaUniqueNameSet = value; NotifyPropertyChanged(); }
        //}
        public string SelectedUniqueName
        {
            get { return _SelectedUniqueName; }
            set { _SelectedUniqueName = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImpactAreaImporterVM(ObservableCollection<string> PolygonPaths, EditorActionManager actionManager):base(actionManager)
        {
            SetDimensions(800, 500, 400, 400);
            AvailablePaths = PolygonPaths;
            IsInEditMode = false;
        }

        public ImpactAreaImporterVM(ImpactAreaElement element, ObservableCollection<ImpactAreaRowItem> impactAreaRows, EditorActionManager actionManager) :base(element, actionManager)
        {
            SetDimensions(800, 500, 400, 400);
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
        public void loadUniqueNames(string path)

        {
            if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(path, "dbf")))
            {
                //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                return;
            }
            SelectedPath = path; //isnt this bound??
            DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(System.IO.Path.ChangeExtension(SelectedPath, ".dbf"));
            DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

            List<string> uniqueNameList = new List<string>();

           // ObservableCollection<ImpactAreaUniqueNameSet>  uniques = new ObservableCollection<ImpactAreaUniqueNameSet>();
            for(int i = 0;i< dtv.ColumnNames.Count(); i++)
            {
                if(dtv.ColumnTypes[i]== typeof(string))
                {
                    // object[] col = dtv.GetColumn(i);
                    //ImpactAreaUniqueNameSet dt = new ImpactAreaUniqueNameSet(dtv.ColumnNames[i], col);
                    //uniques.Add(dt);
                    uniqueNameList.Add(dtv.ColumnNames[i]);
                }
            }
            if (!(uniqueNameList.Count > 0))
            {
                //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("The selected path: " + SelectedPath + "/ndoes not contain any string fields.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                return;
            }
            UniqueFields = uniqueNameList;
        }

        public void LoadTheRows()
        {
            if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(SelectedPath, "dbf")))
            {
                //ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                return;
            }

            DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(System.IO.Path.ChangeExtension(SelectedPath, ".dbf"));
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
            AddRule(nameof(Name), () => Name != null, "Name cannot be null.");
            AddRule(nameof(Name), () => Name != "", "Name cannot be null.");
            AddRule(nameof(SelectedUniqueName), () => Name != null, "A unique name has not been selected.");
            AddRule(nameof(ListOfRows), () => ListOfRows != null, "There are no impact area rows.");

            //AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank.");
            //if (IsNameReadOnly == false)
            // {
            //  AddRule(nameof(SelectedPath), () => SelectedPath != null, "You must select a shapefile");
            // }
            //AddRule(nameof(SelectedImpactAreaUniqueNameSet), () => SelectedImpactAreaUniqueNameSet != null, "You must select a unique name.");


            //AddRule(nameof(Mean), () => Mean > 1, "Mean must be greater than 1");

        }

        public bool IsValid()
        {
            //do some basic validation.
            //if()
            return true;
        }

        public override void Save()
        {
            if(Description == null) { Description = ""; }
                ImpactAreaElement elementToSave = new ImpactAreaElement(Name, Description, ListOfRows,SelectedPath);
            Saving.PersistenceManagers.ImpactAreaPersistenceManager manager = Saving.PersistenceFactory.GetImpactAreaManager();
            if (IsImporter && HasSaved == false)
            {
                manager.SaveNew(elementToSave);
                HasSaved = true;
                OriginalElement = elementToSave;
            }
            else
            {
                manager.SaveExisting((ImpactAreaElement)OriginalElement, elementToSave, 0);
            }
        }
    }
}
