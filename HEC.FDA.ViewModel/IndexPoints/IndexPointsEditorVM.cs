using Geospatial.GDALAssist;
using Geospatial.IO;
using HEC.CS.Collections;
using HEC.FDA.Model.Spatial;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using RasMapperLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Utilities;
using Utility.Logging;

namespace HEC.FDA.ViewModel.IndexPoints
{
    public class IndexPointsEditorVM : BaseEditorVM
    {
        #region Fields
        private string _Path;
        private List<string> _UniqueFields = new();
        private string _SelectedUniqueName;
        #endregion
        #region Properties

        public string SelectedPath
        {
            get { return _Path; }
            set { _Path = value; UniqueFields = new() ; SelectedUniqueName = null; LoadUniqueNames(); NotifyPropertyChanged(); } // using new because Clear() doesn't hit the setter. 
        }
        public CustomObservableCollection<string> ListOfRows { get; } = new CustomObservableCollection<string>();

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
        public IndexPointsEditorVM(EditorActionManager actionManager) : base(actionManager)
        {
            AddValidationRules();
        }

        public IndexPointsEditorVM(IndexPointsElement element, List<string> indexPoints, EditorActionManager actionManager) : base(element, actionManager)
        {
            Name = element.Name;
            ListOfRows.AddRange(indexPoints);
            Description = element.Description;
            SelectedPath = Storage.Connection.Instance.IndexPointsDirectory + "\\" + Name;
            AddValidationRules();
        }
        #endregion
        #region Voids
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
                bool isPoint = RASHelper.IsPointShapefile(SelectedPath, ref error);
                if (!validShapefile || !isPoint)
                {
                    System.Windows.MessageBox.Show(error, "Invalid Shapefile", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else
                {
                    PointFeatureLayer pfl = new("unused", SelectedPath);
                    UniqueFields = pfl.ColumnNames();
                }
            }
        }

        public void LoadTheRows()
        {
            PointFeatureLayer pfl = new("ThisNameIsnotUsed", SelectedPath);
            List<string> columnNames = pfl.ColumnNames();
            List<object> columnVals = pfl.GetValuesFromColumn(SelectedUniqueName);

            List<string> names = columnVals.Select(x => x.ToString()).ToList();
            if (names.Count == names.Distinct().Count()) // if the names are unique
            {
                ListOfRows.Clear();
                ListOfRows.AddRange(names);
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
            foreach ( string name in ListOfRows)
            {
                if (name.IsNullOrEmpty())
                {
                    result.AddErrorMessage("The unique name cannot be blank. Modify your shapefile and import again.");
                }
            }
            return result;
        }

        public override void Save()
        {
            int id = GetElementID<IndexPointsElement>();
            IndexPointsElement elementToSave = new(Name, Description, ListOfRows.ToList(), id);

            if (IsCreatingNewElement)
            {
                FdaValidationResult reprojectResult = SaveWithReprojection(Name);
                if (!reprojectResult.IsValid)
                {
                    System.Windows.MessageBox.Show(reprojectResult.ErrorMessage, "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                StudyFilesManager.RenameDirectory(OriginalElement.Name, Name, elementToSave.GetType());
            }
            //this call handles the sqlite data
            Save(elementToSave);
        }

        private FdaValidationResult SaveWithReprojection(string directoryName)
        {
            FdaValidationResult result = new();

            // Check if study projection is set
            string projectionFile = Connection.Instance.ProjectionFile;
            if (string.IsNullOrEmpty(projectionFile) || !File.Exists(projectionFile))
            {
                result.AddErrorMessage("Study projection is not set. Please set the study projection in Study Properties before importing index points.");
                return result;
            }

            // Load the study projection
            Projection studyProjection = Projection.FromFile(projectionFile);
            if (studyProjection == null)
            {
                result.AddErrorMessage("Failed to load study projection. Please verify the projection file is valid.");
                return result;
            }

            // Read the shapefile with reprojection to study projection
            OperationResult readResult = ShapefileIO.TryRead(SelectedPath, out Geospatial.Features.PointFeatureCollection collection, studyProjection);
            if (!readResult.Result)
            {
                result.AddErrorMessage($"Failed to read shapefile: {readResult.GetConcatenatedMessages()}");
                return result;
            }

            // Create destination directory
            string destinationDirectory = Path.Combine(Connection.Instance.IndexPointsDirectory, directoryName);
            Directory.CreateDirectory(destinationDirectory);

            // Write the reprojected shapefile
            string destinationShpPath = Path.Combine(destinationDirectory, Path.GetFileName(SelectedPath));
            try
            {
                ShapefileIO.Write(destinationShpPath, collection.Features.ToList(), collection.AttributeTable);

                // Write the projection file
                string destinationPrjPath = Path.ChangeExtension(destinationShpPath, ".prj");
                studyProjection.ExportEsri(destinationPrjPath);
            }
            catch (Exception ex)
            {
                result.AddErrorMessage($"Failed to write reprojected shapefile: {ex.Message}");
                return result;
            }

            return result;
        }

    }
}
