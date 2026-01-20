using Geospatial.GDALAssist;
using HEC.FDA.Model.Spatial;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using NamedAction = HEC.MVVMFramework.ViewModel.Implementations.NamedAction;
using HEC.FDA.ViewModel.Hydraulics;

namespace HEC.FDA.ViewModel.FrequencyRelationships;

public class RetrieveGraphicalStageFrequencyVM : BaseViewModel
{
    private IndexPointsElement _selectedIndexPointSet;
    private HydraulicElement _selectedHydraulics;
    public ObservableCollection<HydraulicElement> AvailableHydraulics { get; set; }
    public ObservableCollection<IndexPointsElement> AvailableIndexPointSets { get; set; }
    public HydraulicElement SelectedHydraulics
    {
        get
        {
            return _selectedHydraulics;
        }
        set
        {
            _selectedHydraulics = value;
            NotifyPropertyChanged();
        }
    }
    public IndexPointsElement SelectedIndexPointSet
    {
        get
        {
            return _selectedIndexPointSet;
        }
        set
        {
            _selectedIndexPointSet = value;
            NotifyPropertyChanged();
        }
    }
    private NamedAction _generateFrequencyCurves;
    public NamedAction GenerateFrequencyCurves { get { return _generateFrequencyCurves; } set { _generateFrequencyCurves = value; NotifyPropertyChanged(); } }

    private int _equivalentRecordLength;

    public int EquivalentRecordLength
    {
        get { return _equivalentRecordLength; }
        set { _equivalentRecordLength = value; NotifyPropertyChanged(); }
    }

    #region Constructors
    public RetrieveGraphicalStageFrequencyVM()
    {
        Initialize();
        GenerateFrequencyCurves = new NamedAction();
        GenerateFrequencyCurves.Name = "Generate Curves";
        GenerateFrequencyCurves.Action = GenerateFrequencyCurvesAction;
    }
    #endregion
    #region Methods
    private void Initialize()
    {
        AvailableHydraulics = new ObservableCollection<HydraulicElement>();
        List<HydraulicElement> hydraulicElements = StudyCache.GetChildElementsOfType<HydraulicElement>();
        foreach (HydraulicElement hydraulic in hydraulicElements)
        {
            AvailableHydraulics.Add(hydraulic);
        }
        if(AvailableHydraulics.Count > 0){ SelectedHydraulics = AvailableHydraulics[0];}
        

        AvailableIndexPointSets = new ObservableCollection<IndexPointsElement>();
        List<IndexPointsElement> indexptsElements = StudyCache.GetChildElementsOfType<IndexPointsElement>();
        foreach (IndexPointsElement indexpt in indexptsElements)
        {
            AvailableIndexPointSets.Add(indexpt);
        }
        if (AvailableIndexPointSets.Count > 0) { SelectedIndexPointSet = AvailableIndexPointSets[0]; }
    }

    private void GenerateFrequencyCurvesAction(object arg1, EventArgs arg2)
    {

        string pointShapefileDirectory = Storage.Connection.Instance.IndexPointsDirectory + "\\" + SelectedIndexPointSet.Name + "\\";
        string pointShapefile = GetShapefileFromDirectory(pointShapefileDirectory);
        string hydraulicParentDirectory = Storage.Connection.Instance.HydraulicsDirectory + "\\" + SelectedHydraulics.Name;

        Projection studyProjection = GetStudyProjection();
        if(studyProjection == null)
        {
            MessageBox.Show("Failed to get projection from both study properties and terrain file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (EquivalentRecordLength < 1)
        {
            MessageBox.Show("Please enter an equivalent record length greater than 0.", "Warning" , MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        List<UncertainPairedData> freqCurves = SelectedHydraulics.DataSet.GetGraphicalStageFrequency(pointShapefile, hydraulicParentDirectory, studyProjection);
        if (freqCurves == null)
        {
            MessageBox.Show("Failed to create frequency curves", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        for (int i = 0; i < freqCurves.Count; i++)
        {
            AddFrequencyRelationship(freqCurves[i], SelectedIndexPointSet.IndexPoints[i] + "|" + SelectedHydraulics.Name, EquivalentRecordLength);
        }
        MessageBox.Show($"{freqCurves.Count} curves were added to the study tree.","Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private static Projection GetStudyProjection()
    {
        Projection studyProjection;
        string userSetProjection = Storage.Connection.Instance.ProjectionFile;
        if (userSetProjection.Equals(""))
        {
            studyProjection = RASHelper.GetProjectionFromTerrain(GetTerrainFile());
        }
        else
        {
            studyProjection = Projection.FromFile(userSetProjection);
        }
        return studyProjection;
    }

    public static string GetTerrainFile()
    {
        string filePath = "";
        List<TerrainElement> terrainElements = StudyCache.GetChildElementsOfType<TerrainElement>();
        if (terrainElements.Count > 0)
        {
            //there can only be one terrain in the study
            TerrainElement elem = terrainElements[0];
            filePath = Storage.Connection.Instance.TerrainDirectory + "\\" + elem.Name + "\\" + elem.FileName;
        }
        return filePath;
    }

    private static void AddFrequencyRelationship(UncertainPairedData upd, string name, int equivalentRecordLength)
    {
        string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
        int id = PersistenceFactory.GetElementManager<FrequencyElement>().GetNextAvailableId();

        FrequencyEditorVM vm = new();
        vm.IsGraphical = true;
        vm.MyGraphicalVM.UseFlow = false;
        vm.MyGraphicalVM.InputDataProvider.UpdateFromUncertainPairedData(upd);
        vm.MyGraphicalVM.EquivalentRecordLength = equivalentRecordLength;

        FrequencyElement element = new FrequencyElement(name, editDate, "Retrieved from Hydraulics", id,vm);
        IElementManager elementManager = PersistenceFactory.GetElementManager(element);

        List<FrequencyElement> existingElements = StudyCache.GetChildElementsOfType<FrequencyElement>();
        bool newElementMatchesExisting = false;
        foreach (FrequencyElement ele in existingElements)
        {
            if (ele.Name.Equals(name))
            {
                element.ID = ele.ID;
                elementManager.SaveExisting(element);
                newElementMatchesExisting = true;
                break;
            }
        }
        if (!newElementMatchesExisting)
        {
            elementManager.SaveNew(element);
        }
    }
    private string GetShapefileFromDirectory(string directoryPath)
    {

        string[] files = Directory.GetFiles(directoryPath, "*.shp");
        if (files.Length > 1)
        {
            MessageBox.Show("There should only be one shapefile in a given index points directory");
        }
        return files[0];



        #endregion
    }
}

