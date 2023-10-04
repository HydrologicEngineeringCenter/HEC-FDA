using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;
using System.Windows;
using Utilities;

namespace HEC.FDA.ViewModel.Hydraulics.UnsteadyHDF
{
    public partial class UnsteadyHDFImporterVM : BaseEditorVM
    {
        #region Fields
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
                string path = Connection.Instance.HydraulicsDirectory + "\\" + pp.FileName;
                string filename = Path.GetFileName(pp.FileName);
                AddRow(filename, path, pp.Probability, false);
            }
        }
        #endregion
        #region Actions
        [RelayCommand]
        private void OpenStudyPropertiesAction()
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

        #region copy files

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                string newPath = Path.Combine(target.FullName, fi.Name);
                fi.CopyTo(newPath, true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        #endregion

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
            int firstPeriodIndex = file.IndexOf(".");
            if (firstPeriodIndex != -1)
            {
                string substring = file.Substring(firstPeriodIndex + 1);
                Regex r = new("p??.hdf");
                if (!r.Match(substring).Success)
                {
                    //failed
                    vr.AddErrorMessage("Ignoring file that did not match the pattern of '*.p##.hdf'. " + file);
                }
            }
            else
            {
                //wrong format no period found in file path
                vr.AddErrorMessage("Ignoring file that did not match the pattern of '*.p##.hdf'. " + file);
            }

            return vr;
        }

        public void FileSelected(string fullpath)
        {
            FdaValidationResult vrErrors = new();
            FdaValidationResult vrWarnings = new();

            if (fullpath != null && IsCreatingNewElement)
            {
                ListOfRows.Clear();

                string[] files = Directory.GetFiles(fullpath);
                List<string> validFiles = new();
                foreach (string file in files)
                {
                    FdaValidationResult fileValidResult = IsFileValid(file);
                    if (fileValidResult.IsValid)
                    {
                        validFiles.Add(file);
                    }
                    else
                    {
                        vrWarnings.AddErrorMessage(fileValidResult.ErrorMessage);
                    }
                }

                //warn users that these directories were ignored.
                string[] directories = Directory.GetDirectories(fullpath);
                if (directories.Length > 0)
                {
                    vrWarnings.AddErrorMessage("Ignoring subdirectories.");
                }

                if (validFiles.Count == 0)
                {
                    vrErrors.AddErrorMessage("No valid hdf files were detected. You must select a directory that contains files that match pattern '*.p##.hdf'.");
                }
                else
                {
                    foreach (string file in validFiles)
                    {
                        AddRow(GetUnsteadyRASResultName(file), Path.GetFullPath(file), 0);
                    }
                }
                if (!vrWarnings.IsValid)
                {
                    MessageBox.Show(vrWarnings.ErrorMessage, "Warnings", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (!vrErrors.IsValid)
                {
                    MessageBox.Show(vrErrors.ErrorMessage, "Errors", MessageBoxButton.OK, MessageBoxImage.Error);
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
                newPathProbs.Add(new HydraulicProfile(ListOfRows[i].Probability, ListOfRows[i].Name));
            }

            HydraulicElement elementToSave = new(Name, Description, newPathProbs, HydraulicDataSource.UnsteadyHDF, OriginalElement.ID);
            base.Save(elementToSave);
        }

        private void SaveNew()
        {
            string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
            Directory.CreateDirectory(destinationDirectory);

            List<HydraulicProfile> pathProbs = new();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                string filename = Path.GetFileName(row.Path);
                pathProbs.Add(new HydraulicProfile(row.Probability, filename));

                File.Copy(row.Path, destinationDirectory + "\\" + filename);
            }

            int id = GetElementID<HydraulicElement>();
            HydraulicElement elementToSave = new(Name, Description, pathProbs, HydraulicDataSource.UnsteadyHDF, id);
            base.Save(elementToSave);
        }
        #endregion
    }
}
