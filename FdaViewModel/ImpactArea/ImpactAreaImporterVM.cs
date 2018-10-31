using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace FdaViewModel.ImpactArea
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
        private bool _IsNameReadOnly = false;
        #endregion
        #region Properties
            public bool IsNameReadOnly
        {
            get { return _IsNameReadOnly; }
            set { _IsNameReadOnly = value; }
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
        public string SelectedUniqueName { get; set; }
        #endregion
        #region Constructors
        public ImpactAreaImporterVM(ObservableCollection<string> PolygonPaths):base(null)
        {
            AvailablePaths = PolygonPaths;   
        }

        public ImpactAreaImporterVM(ImpactAreaElement element, ObservableCollection<ImpactAreaRowItem> impactAreaRows):base(element,null)
        {
            Name = element.Name;
            ListOfRows = impactAreaRows;
            Description = element.Description;
            IsNameReadOnly = true;

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
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                return;
            }
            SelectedPath = path; //isnt this bound??
            DataBase_Reader.DbfReader dbf = new DataBase_Reader.DbfReader(System.IO.Path.ChangeExtension(SelectedPath, ".dbf"));
            DataBase_Reader.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

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
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("The selected path: " + SelectedPath + "/ndoes not contain any string fields.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                return;
            }
            UniqueFields = uniqueNameList;
        }

        public void LoadTheRows()
        {
            if (!System.IO.File.Exists(System.IO.Path.ChangeExtension(SelectedPath, "dbf")))
            {
                ReportMessage(new FdaModel.Utilities.Messager.ErrorMessage("This path has no associated *.dbf file.", FdaModel.Utilities.Messager.ErrorMessageEnum.ViewModel | FdaModel.Utilities.Messager.ErrorMessageEnum.Report));
                return;
            }
            
            DataBase_Reader.DbfReader dbf = new DataBase_Reader.DbfReader(System.IO.Path.ChangeExtension(SelectedPath, ".dbf"));
            DataBase_Reader.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

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
            //AddRule(nameof(Name), () => { if (Name == null) { return false; } else { return !Name.Equals(""); } }, "Name cannot be blank.");
            //if (IsNameReadOnly == false)
           // {
              //  AddRule(nameof(SelectedPath), () => SelectedPath != null, "You must select a shapefile");
           // }
            //AddRule(nameof(SelectedImpactAreaUniqueNameSet), () => SelectedImpactAreaUniqueNameSet != null, "You must select a unique name.");


            //AddRule(nameof(Mean), () => Mean > 1, "Mean must be greater than 1");

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
