using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF
{
    public partial class UnsteadyHDFImporterVM : BaseEditorVM, IHaveListOfWSERows
    {
        private string _SelectedPath;
        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; FileSelected(value); NotifyPropertyChanged(); }
        }
        public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = [];
        #region Constructors
        public UnsteadyHDFImporterVM(EditorActionManager actionManager) : base(actionManager)
        {
            AddRule(nameof(ListOfRows), () => ListOfRows.Count > 0, "Invalid directory selected.");
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public UnsteadyHDFImporterVM(HydraulicElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            SelectedPath = Connection.Instance.HydraulicsDirectory + "\\" + elem.Name;
            foreach (HydraulicProfile pp in elem.DataSet.HydraulicProfiles.Cast<HydraulicProfile>())
            {
                string path = SelectedPath + "\\" + pp.FileName;
                string name = GetUnsteadyRASResultName(path);
                AddRow(name, path, pp.Probability, false);
            }
        }
        #endregion
        #region Commands
        [RelayCommand]
        private void OpenStudyProperties()
        {
            StudyPropertiesElement propertiesElement = StudyCache.GetStudyPropertiesElement();
            PropertiesVM vm = new(propertiesElement);
            DynamicTabVM tab = new(StringConstants.STUDY_PROPERTIES, vm, StringConstants.PROPERTIES);
            Navigate(tab, false, false);
        }
        #endregion


        #region Voids
        public void AddRow(string name, string path, double probability, bool isEnabled = true)
        {
            WaterSurfaceElevationRowItemVM newRow = new(name, path, probability, isEnabled);
            ListOfRows.Add(newRow);
        }
        public void RemoveRows(List<int> rowIndices)
        {
            for (int i = rowIndices.Count() - 1; i >= 0; i--)
            {
                ListOfRows.RemoveAt(rowIndices[i]);
            }
        }


        #region validation
        private FdaValidationResult ValidateImporter()
        {
            FdaValidationResult vr = new();

            List<double> probs = new();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                vr.AddErrorMessage(row.IsValid().ErrorMessage);
                probs.Add(row.Probability);
            }
            if (probs.Count != probs.Distinct().Count())
            {
                vr.AddErrorMessage("Duplicate probabilities found. Probabilities must be unique.");
            }
            return vr;
        }

        #endregion

        private static FdaValidationResult IsFileValid(string file)
        {
            FdaValidationResult vr = new();

            // Check for .hdf extension
            if (!Path.GetExtension(file).Equals(".hdf", System.StringComparison.OrdinalIgnoreCase))
            {
                vr.AddErrorMessage("Ignoring file without .hdf extension: " + file);
                return vr;
            }

            // Check that it's a valid RAS result
            try
            {
                RasMapperLib.RASResults result = new(file);
                if (result == null || string.IsNullOrEmpty(result.PlanAttributes?.PlanTitle))
                {
                    vr.AddErrorMessage("File is not a valid RAS result: " + file);
                }
            }
            catch
            {
                vr.AddErrorMessage("File is not a valid RAS result: " + file);
            }

            return vr;
        }

        public void FileSelected(string fullpath)
        {
            if (fullpath == null || !IsCreatingNewElement)
            {
                return;
            }
            ListOfRows.Clear();

            string[] files = Directory.GetFiles(fullpath, "*", SearchOption.AllDirectories);
            List<string> validFiles = new();
            foreach (string file in files)
            {
                FdaValidationResult fileValidResult = IsFileValid(file);
                if (fileValidResult.IsValid)
                {
                    validFiles.Add(file);
                }
            }

            if (validFiles.Count == 0)
            {
                MessageBox.Show("No valid RAS result files were detected. You must select a directory that contains valid .hdf RAS result files.", "Errors", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                foreach (string file in validFiles)
                {
                    AddRow(GetUnsteadyRASResultName(file), Path.GetFullPath(file), 0);
                }
            }

        }

        private static string GetUnsteadyRASResultName(string file)
        {
            RasMapperLib.RASResults result = new(file);
            if (result == null)
            {
                return "INVALID";
            }
            return result.PlanAttributes.PlanTitle;
        }

        public override void Save()
        {
            FdaValidationResult validResult = ValidateImporter();
            if (validResult.IsValid)
            {
                if (IsCreatingNewElement)
                {
                    SaveNew();
                }
                else
                {
                    SaveExisting();
                }
            }
            else
            {
                MessageBox.Show(validResult.ErrorMessage, "Invalid Values", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RenameDirectoryInStudy()
        {
            if (!Name.Equals(OriginalElement.Name))
            {
                string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + OriginalElement.Name;
                string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name;
                Directory.Move(sourceFilePath, destinationFilePath);
            }
        }

        private void SaveExisting()
        {
            RenameDirectoryInStudy();
            List<HydraulicProfile> newPathProbs = new();
            for (int i = 0; i < ListOfRows.Count; i++)
            {
                string filename = Path.GetFileName(ListOfRows[i].Path);
                newPathProbs.Add(new HydraulicProfile(ListOfRows[i].Probability, filename, "MAX"));
            }

            HydraulicElement elementToSave = new(Name, Description, newPathProbs, HydraulicDataSource.UnsteadyHDF, OriginalElement.ID);
            base.Save(elementToSave);
        }

        private void SaveNew()
        {
            string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
            Directory.CreateDirectory(destinationDirectory);

            List<HydraulicProfile> hydroProfiles = new();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                string filename = Path.GetFileName(row.Path);
                hydroProfiles.Add(new HydraulicProfile(row.Probability, filename, "MAX"));

                File.Copy(row.Path, destinationDirectory + "\\" + filename);
            }

            int id = GetElementID<HydraulicElement>();
            HydraulicElement elementToSave = new(Name, Description, hydroProfiles, HydraulicDataSource.UnsteadyHDF, id);
            base.Save(elementToSave);
        }
        #endregion
    }
}
