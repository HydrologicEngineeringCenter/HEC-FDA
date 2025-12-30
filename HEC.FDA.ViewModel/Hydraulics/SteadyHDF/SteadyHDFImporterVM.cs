using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace HEC.FDA.ViewModel.Hydraulics.SteadyHDF
{
    public partial class SteadyHDFImporterVM : BaseEditorVM, IHaveListOfWSERows
    {
        private string _SelectedPath;
        public string SelectedPath
        {
            get { return _SelectedPath; }
            set { _SelectedPath = value; PopulateRows(value); NotifyPropertyChanged(); }
        }

        public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = [];

        /// <summary>
        /// creating a new new node. 
        /// </summary>
        /// <param name="actionManager"></param>
        public SteadyHDFImporterVM(EditorActionManager actionManager) : base(actionManager)
        {
            AddRule(nameof(ListOfRows), () => ListOfRows.Count > 0, "Invalid file selected.");
        }
        /// <summary>
        /// Constructor used when editing an existing child node.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public SteadyHDFImporterVM(HydraulicElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            SelectedPath = Connection.Instance.HydraulicsDirectory + "\\" + elem.Name;
            Name = elem.Name;
            Description = elem.Description;
            foreach (IHydraulicProfile pp in elem.DataSet.HydraulicProfiles)
            {
                string path = Connection.Instance.HydraulicsDirectory + "\\" + pp.FileName;
                AddRow(pp.ProfileName, path, pp.Probability, false);
            }
        }
        [RelayCommand]
        private void OpenStudyProperties()
        {
            StudyPropertiesElement propertiesElement = StudyCache.GetStudyPropertiesElement();
            PropertiesVM vm = new(propertiesElement);
            DynamicTabVM tab = new(StringConstants.STUDY_PROPERTIES, vm, StringConstants.PROPERTIES);
            Navigate(tab, false, false);
        }
        public void AddRow(string name, string path, double probability, bool isEnabled = true)
        {
            WaterSurfaceElevationRowItemVM newRow = new(name, path, probability, isEnabled);
            ListOfRows.Add(newRow);
        }
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
        public void PopulateRows(string fullpath)
        {
            FdaValidationResult vr = new();
            if (fullpath != null && IsCreatingNewElement)
            {
                ListOfRows.Clear();

                FdaValidationResult fileValidResult = IsFileValid(fullpath);
                if (fileValidResult.IsValid)
                {

                    string[] profileNames = GetProfileNamesFromFilePath(fullpath);
                    foreach (string name in profileNames)
                    {
                        AddRow(name, Path.GetFullPath(name), 0);
                    }
                    if (!vr.IsValid)
                    {
                        vr.InsertMessage(0, "Some files or subdirectories are being ignored:\n");
                        MessageBox.Show(vr.ErrorMessage, "Invalid Files", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    vr.AddErrorMessage(fileValidResult.ErrorMessage);
                    MessageBox.Show(vr.ErrorMessage, "Invalid Files", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private static string[] GetProfileNamesFromFilePath(string fullpath)
        {
            string[] profileNames = null;
            try
            {
                RasMapperLib.RASResults result = new(fullpath);
                if (!result.SourceFileExists) { throw new Exception(Path.GetFullPath(fullpath) + " does not exist."); }
                profileNames = result.ProfileNames;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error getting profile names from selected file: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return profileNames;
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

        private void CopyFileToStudyDirectory()
        {
            string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
            Directory.CreateDirectory(destinationDirectory);
            string originalFileName = Path.GetFileName(SelectedPath);
            File.Copy(SelectedPath, destinationDirectory + "\\" + originalFileName);
        }

        private void SaveNew()
        {
            CopyFileToStudyDirectory();

            List<HydraulicProfile> pathProbs = new();
            foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
            {
                pathProbs.Add(new HydraulicProfile( row.Probability, Path.GetFileName(SelectedPath), row.Name));
            }

            int id = GetElementID<HydraulicElement>();
            HydraulicElement elementToSave = new(Name, Description, pathProbs, HydraulicDataSource.SteadyHDF, id);
            base.Save(elementToSave);            
        }

        private void SaveExisting()
        {
            //the user can not change files when editing, so the only changes would be new names and probs.    
            //if name is different then we need to update the directory name in the study hydraulics folder.
            RenameDirectoryInStudy();

            List<HydraulicProfile> newPathProbs = new();
            for (int i = 0; i < ListOfRows.Count; i++)
            {
                newPathProbs.Add(new HydraulicProfile( ListOfRows[i].Probability, Path.GetFileName(SelectedPath), ListOfRows[i].Name));
            }
            HydraulicElement elemToSave = new(Name, Description, newPathProbs, HydraulicDataSource.SteadyHDF, OriginalElement.ID);
            base.Save(elemToSave);
        }
    }
}
