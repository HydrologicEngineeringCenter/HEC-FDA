using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace HEC.FDA.ViewModel.LifeLoss;
public class LifeSimImporterVM : BaseEditorVM
{
    private HydraulicElement _SelectedWaterSurfaceElevation;
    private IndexPointsElement _SelectedIndexPoints;

    

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

    public LifeSimImporterVM(EditorActionManager actionManager) : base(actionManager)
    {
        LoadHydraulics();
        LoadIndexPoints();
        AddLiveUpdateEvents();
    }

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
