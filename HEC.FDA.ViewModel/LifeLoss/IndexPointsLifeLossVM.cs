using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using OxyPlot.Series;
using SciChart.Core.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.LifeLoss;
public partial class IndexPointsLifeLossVM : BaseViewModel
{
    #region Properties
    private string _SelectedPath;
    private HydraulicElement _SelectedHydraulics;
    private IndexPointsElement _SelectedIndexPoints;
    private LifeSimSimulation _SelectedSimuation;

    public string SelectedPath
    {
        get { return _SelectedPath; }
        set
        {
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

    public ObservableCollection<HydraulicElement> Hydraulics { get; } = new();
    public HydraulicElement SelectedHydraulics
    {
        get { return _SelectedHydraulics; }
        set { _SelectedHydraulics = value; NotifyPropertyChanged(); }
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
        set
        {
            _SelectedSimuation = value;
            NotifyPropertyChanged();
            OnSelectedSimulationChanged(value);
        }
    }
    private void OnSelectedSimulationChanged(LifeSimSimulation simulation)
    {
        // reset the other options
        LifeSimAlternatives.Clear();
        HazardTimes.Clear();

        // update options to match the currently selected simulation
        foreach (string alternative in simulation.Alternatives)
            LifeSimAlternatives.Add(new CheckableItem { Name = alternative });

        foreach (string hazardTime in simulation.HazardTimes)
        {
            if (int.TryParse(hazardTime, out int time))
            {
                string formattedTime = time < 10 ? $"0{time}00" : $"{time}00";
                HazardTimes.Add(new CheckableItem { Name = formattedTime, Value = time.ToString() });
            }
        }
    }

    public ObservableCollection<CheckableItem> LifeSimAlternatives { get; set; }
    public ObservableCollection<CheckableItem> HazardTimes { get; set; }
    public PlotModel MyModel { get; set; }
    private int _plotIndex = 0;
    private List<LifeLossFunction> _lifeLossFunctions = [];
    #endregion

    public IndexPointsLifeLossVM()
    {
        Simulations = [];
        LifeSimAlternatives = [];
        HazardTimes = [];
        MyModel = new PlotModel();
        LoadHydraulics();
        LoadIndexPoints();
        SubscribeToLiveUpdateEvents();
    }

    #region Live Update Events
    private void SubscribeToLiveUpdateEvents()
    {
        StudyCache.WaterSurfaceElevationAdded += AddHydraulicElement;
        StudyCache.WaterSurfaceElevationRemoved += RemoveHydraulicElement;
        StudyCache.IndexPointsAdded += AddIndexPointsElement;
        StudyCache.IndexPointsRemoved += RemoveIndexPointsElement;
    }

    private void AddHydraulicElement(object sender, ElementAddedEventArgs e)
    {
        Hydraulics.Add((HydraulicElement)e.Element);
    }

    private void RemoveHydraulicElement(object sender, ElementAddedEventArgs e)
    {
        Hydraulics.Remove(Hydraulics.Single(s =>
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
            Hydraulics.Add(elem);
        }
        if (Hydraulics.Count > 0)
        {
            SelectedHydraulics = Hydraulics[0];
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

    [RelayCommand]
    public async Task ComputeCurves()
    {
        if (LifeSimAlternatives.IsEmpty() || HazardTimes.IsEmpty()) return;

        string hydraulicsFolder = @"C:\FDA_Test_Data\WKS20230525\WKS20230525\Hydraulic_Data";
        //string hydraulicsFolder = Connection.Instance.HydraulicsDirectory;
        LifeSimSimulation currentSimulation = new(SelectedSimulation.Name, hydraulicsFolder);
        foreach (CheckableItem a in LifeSimAlternatives)
            if (a.IsChecked) currentSimulation.Alternatives.Add(a.Name);
        foreach (CheckableItem h in HazardTimes)
            if (h.IsChecked) currentSimulation.HazardTimes.Add(h.Value);
        LifeLossFunctionGenerator generator = new(SelectedPath, currentSimulation);
        string indexPointsFile = GetIndexPointsFile();
        string impactAreasFile = GetImpactAreaFile();
        _lifeLossFunctions.Clear();
        List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        string uniqueImpactAreaHeader = impactAreaElements[0].UniqueNameColumnHeader;
        _lifeLossFunctions = await generator.CreateLifeLossFunctionsAsync(impactAreasFile, indexPointsFile, uniqueImpactAreaHeader);
        _plotIndex = 0;
        ChangePlot(_plotIndex);
    }

    public FdaValidationResult ValidateForm()
    {
        FdaValidationResult vr = new();
        if (SelectedHydraulics == null)
        {
            const string Msg = "A hydraulics data set must be selected.";
            vr.AddErrorMessage(Msg);
        }
        if (SelectedIndexPoints == null)
        {
            const string Msg = "Index Points must be selected.";
            vr.AddErrorMessage(Msg);
        }
        return vr;
    }

    private string GetImpactAreaFile()
    {
        List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        ImpactAreaElement impactAreaElement = impactAreaElements[0];
        string directory = Path.Combine(Connection.Instance.ImpactAreaDirectory, impactAreaElement.Name);
        string file = Directory.GetFiles(directory, "*.shp")[0];
        return file;
    }

    private string GetIndexPointsFile()
    {
        string directory = Path.Combine(Connection.Instance.IndexPointsDirectory, SelectedIndexPoints.Name);
        string file = Directory.GetFiles(directory, "*.shp")[0];
        return file;
    }

    /// <summary>
    /// Update the plot displayed in the UI
    /// </summary>
    /// <param name="next">The index of the plot to be switched to</param>
    private void ChangePlot(int next)
    {
        if (_lifeLossFunctions.IsNullOrEmpty()) return;

        LifeLossFunction relationship = _lifeLossFunctions[next];
        UncertainPairedData uncertainData = relationship.Data;
        var upper = uncertainData.SamplePairedData(0.975);
        var lower = uncertainData.SamplePairedData(0.025);
        var mid = uncertainData.SamplePairedData(0.5);

        MyModel.Series.Clear();
        AddLineSeriesToPlot(upper, true);
        AddLineSeriesToPlot(lower, true);
        AddLineSeriesToPlot(mid);

        MyModel.ResetAllAxes(); // recenter on the newly plotted lines
        int time = int.Parse(relationship.HazardTime);
        string formattedTime = time < 10 ? $"0{time}00" : $"{time}00";
        MyModel.Title = $"{relationship.SimulationName}: {relationship.SummaryZone} {formattedTime}";
        MyModel.InvalidatePlot(true);
    }

    /// <summary>
    /// Add a single line series to a plot
    /// </summary>
    /// <param name="function"></param>
    /// <param name="isConfidenceLimit"></param>
    private void AddLineSeriesToPlot(PairedData function, bool isConfidenceLimit = false)
    {
        LineSeries lineSeries = new();

        DataPoint[] points = new DataPoint[function.Xvals.Length];
        for (int i = 0; i < function.Xvals.Length; i++)
        {
            double stage = function.Xvals[i];
            double lifeLoss = function.Yvals[i];
            points[i] = new DataPoint(stage, lifeLoss);
        }

        if (isConfidenceLimit) { lineSeries.Color = OxyColors.Blue; lineSeries.LineStyle = LineStyle.Dash; }
        else { lineSeries.Color = OxyColors.Black; }

        lineSeries.ItemsSource = points;
        //lineSeries.DataFieldX = "Stage";
        //lineSeries.DataFieldY = "Life Loss";
        MyModel.Series.Add(lineSeries);
    }
}
