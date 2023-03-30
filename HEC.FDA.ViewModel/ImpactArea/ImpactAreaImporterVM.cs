using HEC.CS.Collections;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

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
        private List<string> _UniqueNames;
        private string _SelectedUniqueNameColumnHeader;
        #endregion
        #region Properties
        public string SelectedPath
        {
            get { return _Path; }
            set { _Path = value; LoadUniqueNames();  NotifyPropertyChanged();}
        }
        public CustomObservableCollection <ImpactAreaRowItem> ListOfRows { get; } = new CustomObservableCollection<ImpactAreaRowItem>();

        public List<string> UniqueNames
        {
            get { return _UniqueNames; }
            set { _UniqueNames = value; NotifyPropertyChanged(); }
        }
        public string SelectedUniqueNameColumnHeader
        {
            get { return _SelectedUniqueNameColumnHeader; }
            set { _SelectedUniqueNameColumnHeader = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImpactAreaImporterVM(EditorActionManager actionManager):base(actionManager)
        {
            AddValidationRules();
        }

        public ImpactAreaImporterVM(ImpactAreaElement element, List<ImpactAreaRowItem> impactAreaRows, EditorActionManager actionManager) :base(element, actionManager)
        {
            Name = element.Name;
            ListOfRows.AddRange( impactAreaRows);
            Description = element.Description;
            SelectedPath = Storage.Connection.Instance.ImpactAreaDirectory + "\\" + Name;
            AddValidationRules();
        }
        #endregion
        #region Voids
        private void AddValidationRules()
        {
            AddRule(nameof(SelectedUniqueNameColumnHeader), () =>
            {
                return !string.IsNullOrEmpty(SelectedUniqueNameColumnHeader);
            }, "No unique name column header selected");
        }

        /// <summary>
        /// This method grabs all the column headers from the dbf and loads them into a unique name combobox.
        /// </summary>
        /// <param name="path"></param>
        public void LoadUniqueNames()
        {
            if (IsCreatingNewElement)
            {
                if (!File.Exists(Path.ChangeExtension(_Path, "dbf")))
                {
                    MessageBox.Show("This path has no associated *.dbf file.", "File Doesn't Exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DatabaseManager.DbfReader dbf = new DatabaseManager.DbfReader(Path.ChangeExtension(_Path, ".dbf"));
                    DatabaseManager.DataTableView dtv = dbf.GetTableManager(dbf.GetTableNames()[0]);

                    List<string> uniqueNameList = dtv.ColumnNames.ToList();

                    UniqueNames = uniqueNameList;
                }
            }
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
                if (dtv.ColumnNames[i] == SelectedUniqueNameColumnHeader)
                {
                    object[] col = dtv.GetColumn(i);
                    ImpactAreaUniqueNameSet iauns = new ImpactAreaUniqueNameSet(dtv.ColumnNames[i], col);
                    ListOfRows.Clear();
                    ListOfRows.AddRange(iauns.RowItems);
                }
            }
        }
        #endregion

        public override void Save()
        {
            int id = GetElementID<ImpactAreaElement>();

            ImpactAreaElement elementToSave = new ImpactAreaElement(Name, Description, ListOfRows.ToList(), id, SelectedUniqueNameColumnHeader);

            if (IsCreatingNewElement)
            {
                StudyFilesManager.CopyFilesWithSameName(SelectedPath, Name, elementToSave.GetType());
            }
            else
            {
                StudyFilesManager.RenameDirectory(OriginalElement.Name, Name, elementToSave.GetType());
            }
            //this call handles the sqlite data
            Save(elementToSave);
        }
    }
}
