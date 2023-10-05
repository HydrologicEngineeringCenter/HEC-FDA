using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData
{
    //[Author(q0heccdm, 9 / 1 / 2017 8:31:13 AM)]
    public partial class GriddedImporterVM : BaseEditorVM
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 9/1/2017 8:31:13 AM
        #endregion
        #region Fields
        private List<string> _OriginalFolderNames = new();
        private string _SelectedPath;

        #endregion
        #region Properties
        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; FileSelected(value); NotifyPropertyChanged(); }
        }

        public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = new ObservableCollection<WaterSurfaceElevationRowItemVM>();
        #endregion
        #region Constructors
        public GriddedImporterVM(EditorActionManager actionManager) : base(actionManager)
        {
            AddRule(nameof(ListOfRows), () => ListOfRows.Count > 0, "Invalid directory selected.");
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public GriddedImporterVM(HydraulicElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            SelectedPath = Connection.Instance.HydraulicsDirectory + "\\" + elem.Name;
            foreach (HydraulicProfile pp in elem.DataSet.HydraulicProfiles)
            {
                string path = Connection.Instance.HydraulicsDirectory + "\\" + pp.FileName;
                string folderName = Path.GetFileName(pp.FileName);
                _OriginalFolderNames.Add(folderName);
                AddRow(folderName, path, pp.Probability, false);
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

        private FdaValidationResult ContainsTif(string directoryPath)
        {
            FdaValidationResult vr = new();
            List<string> tifFiles = new();

            string[] fileList = Directory.GetFiles(directoryPath);
            foreach (string file in fileList)
            {
                if (Path.GetExtension(file) == ".tif")
                {
                    tifFiles.Add(file);
                }
            }
            string dirName = Path.GetFileName(directoryPath);
            vr.AddErrorMessage(ValidateTIFFiles(tifFiles, dirName).ErrorMessage);
            return vr;
        }

        private FdaValidationResult ValidateTIFFiles(List<string> tifFiles, string directoryName)
        {
            FdaValidationResult vr = new();
            if (tifFiles.Count == 0)
            {
                vr.AddErrorMessage("Directory " + directoryName + ": No .tif files found.");
            }
            return vr;
        }

        #endregion

        public void FileSelected(string fullpath)
        {
            if (fullpath != null && IsCreatingNewElement)
            {
                FdaValidationResult importResult = new();
                ListOfRows.Clear();
                //clear out any already existing rows
                if (!Directory.Exists(fullpath))
                {
                    return;
                }

                List<string> validDirectories = new();
                string[] directories = Directory.GetDirectories(fullpath);
                foreach (string directory in directories)
                {
                    FdaValidationResult result = ContainsTif(directory);
                    if (result.IsValid)
                    {
                        validDirectories.Add(directory);
                    }
                    else
                    {
                        importResult.AddErrorMessage(result.ErrorMessage);
                    }
                }
                    double prob = 0;
                    foreach (string dir in validDirectories)
                    {
                        prob += .1;
                        AddRow(Path.GetFileName(dir), Path.GetFullPath(dir), prob);
                    }
                    //we might have some message for the user?
                    if (!importResult.IsValid)
                    {
                        importResult.InsertMessage(0, "The selected directory contains at least 1 valid subdirectory and will ignore the following:\n");
                        MessageBox.Show(importResult.ErrorMessage, "Valid Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
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

        private void RenameDirectoryInTheStudy()
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
            //the user can not change files when editing, so the only changes would be new names and probs.    
            //if name is different then we need to update the directory name in the study hydraulics folder.
            RenameDirectoryInTheStudy();
            //might have to rename the sub folders.
            List<HydraulicProfile> newPathProbs = new();
            for (int i = 0; i < ListOfRows.Count; i++)
            {
                string newName = ListOfRows[i].Name;
                string originalName = _OriginalFolderNames[i];
                if (!newName.Equals(originalName))
                {
                    string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + originalName;
                    string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name + "\\" + newName;
                    Directory.Move(sourceFilePath, destinationFilePath);
                    _OriginalFolderNames[i] = newName;
                }
                string fileNameFromChildElementDir = getFilePathFromChildElement(ListOfRows[i]);
                newPathProbs.Add(new HydraulicProfile(ListOfRows[i].Probability, fileNameFromChildElementDir));
            }

            HydraulicElement elementToSave = new(Name, Description, newPathProbs, HydraulicDataSource.WSEGrid, OriginalElement.ID);
            base.Save(elementToSave);
        }

        private void SaveNew()
        {
            string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
            Directory.CreateDirectory(destinationDirectory);
            List<HydraulicProfile> pathProbs = new();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                _OriginalFolderNames.Add(row.Name);
                string fileNameFromChildElementDir = getFilePathFromChildElement(row);
                pathProbs.Add(new HydraulicProfile(row.Probability, fileNameFromChildElementDir));
                StudyFilesManager.CopyDirectory(row.Path, row.Name, destinationDirectory);
            }

            int id = GetElementID<HydraulicElement>();
            HydraulicElement elementToSave = new(Name, Description, pathProbs, HydraulicDataSource.WSEGrid, id);
            base.Save(elementToSave);
        }

        private string getFilePathFromChildElement(WaterSurfaceElevationRowItemVM row)
        {
            string directoryNameForSpecificGrid = Path.GetFileName(row.Name);
            string vrtFileWithPath = Directory.GetFiles(row.Path, "*.tif")[0];
            string vrtFileOnly = Path.GetFileName(vrtFileWithPath);
            return directoryNameForSpecificGrid + "\\" + vrtFileOnly;
        }
        #endregion
    }
}
