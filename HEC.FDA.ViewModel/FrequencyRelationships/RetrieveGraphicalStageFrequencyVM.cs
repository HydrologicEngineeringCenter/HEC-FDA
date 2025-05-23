﻿using Geospatial.GDALAssist;
using HEC.FDA.Model.Spatial;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Watershed;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using NamedAction = HEC.MVVMFramework.ViewModel.Implementations.NamedAction;

namespace HEC.FDA.ViewModel.FrequencyRelationships;

public class RetrieveGraphicalStageFrequencyVM : BaseViewModel
{
    #region Fields
    private IndexPointsElement _selectedIndexPointSet;
    private HydraulicElement _selectedHydraulics;
    #endregion
    #region Properties
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
    #endregion
    #region Named Actions
    private NamedAction _generateFrequencyCurves;
    public NamedAction GenerateFrequencyCurves { get { return _generateFrequencyCurves; } set { _generateFrequencyCurves = value; NotifyPropertyChanged(); } }
    #endregion
    #region Constructors
    public RetrieveGraphicalStageFrequencyVM()
    {
        Initialize();
        GenerateFrequencyCurves = new NamedAction();
        GenerateFrequencyCurves.Name = "GenerateFrequencyCurves";
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
            MessageBox.Show("Failed to get projection from both study properties and terrain file");
            return;
        }

        List<UncertainPairedData> freqCurves = SelectedHydraulics.DataSet.GetGraphicalStageFrequency(pointShapefile, hydraulicParentDirectory, studyProjection);
        if (freqCurves == null)
        {
            MessageBox.Show("Failed to create frequency curves");
        }
        for (int i = 0; i < freqCurves.Count; i++)
        {
            AddFrequencyRelationship(freqCurves[i], SelectedIndexPointSet.IndexPoints[i] + "|" + SelectedHydraulics.Name);
        }
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

    private static void AddFrequencyRelationship(UncertainPairedData upd, string name)
    {
        string editDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM
        int id = PersistenceFactory.GetElementManager<FrequencyElement>().GetNextAvailableId();

        FrequencyEditorVM vm = new();
        vm.IsGraphical = true;
        vm.MyGraphicalVM.UseFlow = false;
        vm.MyGraphicalVM.InputDataProvider.UpdateFromUncertainPairedData(upd);

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

