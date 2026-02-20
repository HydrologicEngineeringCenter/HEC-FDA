using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.hydraulics;
using HEC.FDA.Model.hydraulics.enums;
using HEC.FDA.Model.hydraulics.Interfaces;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.Hydraulics.GriddedData;

public partial class GriddedImporterVM : BaseEditorVM,IHaveListOfWSERows
{
    private string _SelectedPath;
    public string SelectedPath
    {
        get { return _SelectedPath; }
        set { _SelectedPath = value; FolderSelected(value); NotifyPropertyChanged(); }
    }
    public ObservableCollection<WaterSurfaceElevationRowItemVM> ListOfRows { get; } = [];
    public GriddedImporterVM(EditorActionManager actionManager) : base(actionManager)
    {
        AddRule(nameof(ListOfRows), () => ListOfRows.Count > 0, "Invalid directory selected.");
    }
    /// <summary>
    /// Constructor used when editing an existing child node.
    /// </summary>
    public GriddedImporterVM(HydraulicElement elem, EditorActionManager actionManager) : base(elem, actionManager)
    {
        IEnumerable<IHydraulicProfile> profiles = elem.DataSet.HydraulicProfiles.OrderBy(x => x.Probability);
        ListOfRows.Clear();
        foreach (IHydraulicProfile profile in profiles)
        {
            WaterSurfaceElevationRowItemVM newRow = CreateRowFromHydraulicProfile(profile, elem.Name);
            ListOfRows.Add(newRow);
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
    #region Voids
    /// <param name="name"> The name visible to the UI</param>
    /// <param name="path"> The absolute path to the file </param>
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

    private static FdaValidationResult ValidateTIFFiles(List<string> tifFiles, string directoryName)
    {
        FdaValidationResult vr = new();
        if (tifFiles.Count == 0)
        {
            vr.AddErrorMessage("Directory " + directoryName + ": No .tif files found.");
        }
        return vr;
    }

    #endregion

    /// <summary>
    /// Creates a VM row from the HydraulicProfile. This is used when editing an existing element.
    /// </summary>
    private static WaterSurfaceElevationRowItemVM CreateRowFromHydraulicProfile(IHydraulicProfile profile, string elementName)
    {
        string name = Path.GetDirectoryName(profile.FileName); //name is what's shown to the UI. For Gridded data, we want the directory name, not the file name. 
        string path = Path.Combine(Connection.Instance.HydraulicsDirectory, elementName, profile.FileName); //path is the full path to the file from the element root.
        double prob = profile.Probability;
        return new WaterSurfaceElevationRowItemVM(name, path, prob, false); //setting this to false means the user can't change anything meaningful about this dataset. 
    }

    public void RemoveRows(List<int> rowIndices)
    {
        for (int i = rowIndices.Count() - 1; i >= 0; i--)
        {
            ListOfRows.RemoveAt(rowIndices[i]);
        }
    }

    public void FolderSelected(string fullpath)
    {
        if (fullpath != null)
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
            foreach (string dir in validDirectories)
            {
                string name = Path.GetFileName(dir);
                string[] tifFiles = Directory.GetFiles(dir, "*.tif");
                string tif = tifFiles[0]; //there should only be one, but if there are more, we'll just take the first one
                AddRow(name, tif, 0); //initialize to 0, user will have to change to valid probability. 
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

    /// <summary>
    /// Checks whether the Name has changed in the UI and if so, renames the directory in the study.
    /// </summary>
    private void RenameDirectoryInTheStudy()
    {
        if (Name.Equals(OriginalElement.Name)) { return; }
        string sourceFilePath = Connection.Instance.HydraulicsDirectory + "\\" + OriginalElement.Name;
        string destinationFilePath = Connection.Instance.HydraulicsDirectory + "\\" + Name;
        Directory.Move(sourceFilePath, destinationFilePath);
    }

    private void SaveExisting()
    {
        //Currently all profiles are immutable, so the only thing that can change is the name of the element.
        RenameDirectoryInTheStudy();
        List<HydraulicProfile> pathProbs = new();
        foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
        {
            string fileNameFromChildElementDir = GetFilePathFromChildElementRoot(row);
            pathProbs.Add(new HydraulicProfile(row.Probability, fileNameFromChildElementDir, null));
        }
        HydraulicElement elementToSave = new(Name, Description, pathProbs, HydraulicDataSource.WSEGrid, OriginalElement.ID);
        base.Save(elementToSave);
    }

    private void SaveNew()
    {
        string destinationDirectory = Connection.Instance.HydraulicsDirectory + "\\" + Name;
        Directory.CreateDirectory(destinationDirectory);
        List<HydraulicProfile> hydraulicProfiles = new();
        foreach (WaterSurfaceElevationRowItemVM row in ListOfRows)
        {
            string fileNameFromChildElementDir = GetFilePathFromChildElementRoot(row);
            HydraulicProfile prof = new(row.Probability, fileNameFromChildElementDir, null);//profile name is not used for gridded data.
            hydraulicProfiles.Add(prof);
            string sourceDirectory = Directory.GetParent(row.Path).FullName;
            StudyFilesManager.CopyDirectory(sourceDirectory, row.Name, destinationDirectory);
        }

        int id = GetElementID<HydraulicElement>();
        HydraulicElement elementToSave = new(Name, Description, hydraulicProfiles, HydraulicDataSource.WSEGrid, id);
        base.Save(elementToSave);
    }

    /// <summary>
    /// Gets the file path which corresponds to the FileName property of a Hydraulics Profile.
    /// </summary>
    private static string GetFilePathFromChildElementRoot(WaterSurfaceElevationRowItemVM row)
    {
        string[] splitPathOnDirectories = row.Path.Split(Path.DirectorySeparatorChar);
        string tifName = splitPathOnDirectories[^1];
        string dirName = splitPathOnDirectories[^2];
        return Path.Combine(dirName, tifName);
    }
}
