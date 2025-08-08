using HEC.FDA.Model.LifeLoss;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Utilities;
using SciChart.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HEC.FDA.ViewModel.LifeLoss;
public class LifeSimImporterVM : BaseEditorVM
{
    #region Properties
    private string _SelectedPath;
    private HydraulicElement _SelectedWaterSurfaceElevation;
    private IndexPointsElement _SelectedIndexPoints;
    private LifeSimSimulation _SelectedSimuation;

    public string SelectedPath
    {
        get { return _SelectedPath; }
        set { 
                _SelectedPath = value; 
                NotifyPropertyChanged(); 
                OnSelectedPathChanged();  
            }
    }
    private void OnSelectedPathChanged()
    {
        if (SelectedPath.IsNullOrWhiteSpace()) return;
        // reset the simulation options
        Simulations.Clear();
        LifeSimDatabase db = new(SelectedPath);
        // add new simulations from the newly selected database
        List<LifeSimSimulation> newSimulations = db.UpdateSimulations();
        foreach (LifeSimSimulation simulation in newSimulations)
            Simulations.Add(simulation);
    }

    public ObservableCollection<HydraulicElement> WaterSurfaceElevations { get; } = new();
    public HydraulicElement SelectedWaterSurfaceElevation
    {
        get { return _SelectedWaterSurfaceElevation; }
        set { _SelectedWaterSurfaceElevation = value; NotifyPropertyChanged(); }
    }

    public ObservableCollection<IndexPointsElement> IndexPoints { get; } = new();
    public IndexPointsElement SelectedIndexPoints
    {
        get { return _SelectedIndexPoints; }
        set { _SelectedIndexPoints = value; NotifyPropertyChanged(); }
    }

    public ObservableCollection<LifeSimSimulation> Simulations { get; set; } 
    public LifeSimSimulation SelectedSimulation
    {
        get { return _SelectedSimuation; }
        set { 
                _SelectedSimuation = value; 
                NotifyPropertyChanged(); 
                OnSelectedSimulationChanged(value);
            }
    }
    private void OnSelectedSimulationChanged(LifeSimSimulation simulation)
    {
        // reset the other options
        Alternatives.Clear();
        HazardTimes.Clear();

        // update options to match the currently selected simulation
        foreach (string alternative in simulation.Alternatives)
            Alternatives.Add(new CheckableItem { Name = alternative });

        foreach (string hazardTime in simulation.HazardTimes)
        {
            if (int.TryParse(hazardTime, out int time))
            {
                string formattedTime = time < 10 ? $"0{time}00" : $"{time}00";
                HazardTimes.Add(new CheckableItem { Name = formattedTime, Value = time.ToString() });
            }
        }
    }

    public ObservableCollection<CheckableItem> Alternatives { get; set; }
    public ObservableCollection<CheckableItem> HazardTimes { get; set; }
    #endregion

    public LifeSimImporterVM(EditorActionManager actionManager) : base(actionManager)
    {
        Simulations = [];
        Alternatives = [];
        HazardTimes = [];
        LoadHydraulics();
        LoadIndexPoints();
        AddLiveUpdateEvents();
    }

    #region Live Update Events
    private void AddLiveUpdateEvents()
    {
        StudyCache.WaterSurfaceElevationAdded += AddHydraulicElement;
        StudyCache.WaterSurfaceElevationRemoved += RemoveHydraulicElement;
    }

    private void AddHydraulicElement(object sender, ElementAddedEventArgs e)
    {
        WaterSurfaceElevations.Add((HydraulicElement)e.Element);
    }

    private void RemoveHydraulicElement(object sender, ElementAddedEventArgs e)
    {
        WaterSurfaceElevations.Remove(WaterSurfaceElevations.Single(s =>
        {
            if (s.ID == e.Element.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }));
    }

    private void AddIndexPointsElement(object sender, ElementAddedEventArgs e)
    {
        IndexPoints.Add((IndexPointsElement)e.Element);
    }

    private void RemoveIndexPointsElement(object sender, ElementAddedEventArgs e)
    {
        IndexPoints.Remove(IndexPoints.Single(s =>
        {
            if (s.ID == e.Element.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }));
    }
    #endregion

    private void LoadHydraulics()
    {
        List<HydraulicElement> hydraulicElements = StudyCache.GetChildElementsOfType<HydraulicElement>();
        foreach (HydraulicElement elem in hydraulicElements)
        {
            WaterSurfaceElevations.Add(elem);
        }
        if (WaterSurfaceElevations.Count > 0)
        {
            SelectedWaterSurfaceElevation = WaterSurfaceElevations[0];
        }
    }

    private void LoadIndexPoints()
    {
        List<IndexPointsElement> indexPoints = StudyCache.GetChildElementsOfType<IndexPointsElement>();
        foreach (IndexPointsElement elem in indexPoints)
        {
            IndexPoints.Add(elem);
        }
        if (indexPoints.Count > 0)
        {
            SelectedIndexPoints = IndexPoints[0];
        }
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }
}
