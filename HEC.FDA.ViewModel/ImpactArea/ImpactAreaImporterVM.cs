using HEC.CS.Collections;
using HEC.FDA.Model.Spatial;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaImporterVM : BaseEditorVM
    {
        #region Fields
        private string _selectedPath;
        private List<string> _UniqueNames = new();
        private string _SelectedUniqueNameColumnHeader;
        #endregion
        #region Properties
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value; UniqueNames = new(); SelectedUniqueNameColumnHeader = null; LoadUniqueNames(); NotifyPropertyChanged(); } // using new because Clear() doesn't hit the setter. 
        }

        public CustomObservableCollection<ImpactAreaRowItem> ListOfRows { get; } = new CustomObservableCollection<ImpactAreaRowItem>();

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
        public ImpactAreaImporterVM(EditorActionManager actionManager) : base(actionManager)
        {
            AddValidationRules();
        }

        public ImpactAreaImporterVM(ImpactAreaElement element, List<ImpactAreaRowItem> impactAreaRows, EditorActionManager actionManager) : base(element, actionManager)
        {
            Name = element.Name;
            ListOfRows.AddRange(impactAreaRows);
            Description = element.Description;
            SelectedPath = Path.Combine(Storage.Connection.Instance.ImpactAreaDirectory, Name);
            AddValidationRules();
        }
        #endregion
        #region Voids
        public override void AddValidationRules()
        {
            base.AddValidationRules();
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
                string error = "";
                bool validShapefile = RASHelper.ShapefileIsValid(SelectedPath, ref error);
                bool isPolygon = RASHelper.IsPolygonShapefile(SelectedPath, ref error);
                if (!validShapefile || !isPolygon)
                {
                    System.Windows.MessageBox.Show(error, "Invalid Shapefile", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {
                    ShapefileHelper shp = new(SelectedPath);
                    UniqueNames = shp.GetColumns();
                }
            }
        }

        public void LoadTheRows()
        {
            ShapefileHelper shp = new(SelectedPath);
            List<string> names = shp.GetColumnValues(SelectedUniqueNameColumnHeader);
            if (names.Count == names.Distinct().Count()) // if the names are unique
            {
                ListOfRows.Clear();
                for (int i = 0; i < names.Count; i++)
                {
                    ListOfRows.Add(new ImpactAreaRowItem(i, names[i]));
                }
            }
            else
            {
                System.Windows.MessageBox.Show("The names in the column identified were not unique", "Names not unique", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion

        public override FdaValidationResult IsValid()
        {
            FdaValidationResult result = new();
            //Previous validation dictates that the shapefiles is valid, the unique name column is selected, and the names are unique.
            //if these rows are here. Then all that stuff is true too. 
            if (ListOfRows.Count < 1)
            {
                result.AddErrorMessage("There are no rows in the table. Check your shapefile and import again.");
            }
            //also need to check that the names are not empty
            foreach (ImpactAreaRowItem row in ListOfRows)
            {
                if (string.IsNullOrEmpty(row.Name))
                {
                    result.AddErrorMessage("The unique name cannot be blank. Check your shapefile and import again.");
                }
            }
            return result;
        }
        public override void Save()
        {
            int id = GetElementID<ImpactAreaElement>();

            ImpactAreaElement elementToSave = new(Name, Description, ListOfRows.ToList(), id, SelectedUniqueNameColumnHeader);

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
