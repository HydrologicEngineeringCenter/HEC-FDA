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
    private string _selectedPath;
    private HydraulicElement _selectedHydraulics;
    private IndexPointsElement _selectedIndexPoints;
    private LifeSimSimulation _selectedSimuation;
    private ObservableCollection<LifeSimSimulation> _simulations = [];
    private ObservableCollection<CheckableItem> _lifesimAlternatives = [];
    private ObservableCollection<CheckableItem> _hazardTimes = [];

    public string SelectedPath
    {
        get { return _selectedPath; }
        set
        {
            _selectedPath = value;
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
        if (Simulations.Count > 0)
            SelectedSimulation = Simulations[0];
    }

    public ObservableCollection<HydraulicElement> Hydraulics { get; } = new();
    public HydraulicElement SelectedHydraulics
    {
        get { return _selectedHydraulics; }
        set { _selectedHydraulics = value; NotifyPropertyChanged(); }
    }

    public ObservableCollection<IndexPointsElement> IndexPoints { get; } = new();
    public IndexPointsElement SelectedIndexPoints
    {
        get { return _selectedIndexPoints; }
        set { _selectedIndexPoints = value; NotifyPropertyChanged(); }
    }

    public ObservableCollection<LifeSimSimulation> Simulations
    {
        get { return _simulations; }
        set { _simulations = value; }
    }
    public LifeSimSimulation SelectedSimulation
    {
        get { return _selectedSimuation; }
        set
        {
            _selectedSimuation = value;
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

    public ObservableCollection<CheckableItem> LifeSimAlternatives
    {
        get { return _lifesimAlternatives; }
        set
        {
            _lifesimAlternatives = value;
            NotifyPropertyChanged();
        }
    }
    public ObservableCollection<CheckableItem> HazardTimes
    {
        get { return _hazardTimes; }
        set
        {
            _hazardTimes = value;
            NotifyPropertyChanged();
        }
    }

    public PlotModel MyModel { get; set; } = new();
    public List<LifeLossFunction> LifeLossFunctions { get; private set; } = [];
    public bool WasRecomputed { get; private set; } = false;
    private int _plotIndex = 0;

    #endregion

    // called when creating new element
    public IndexPointsLifeLossVM()
    {
        LoadHydraulics();
        LoadIndexPoints();
        SubscribeToLiveUpdateEvents();
    }

    // called when opening existing element editor
    public IndexPointsLifeLossVM(
        string LifeSimDataBasePath,
        int hydraulicsID,
        int indexPointsID,
        string selectedSimulation,
        List<string> selectedAlternatives,
        List<string> selectedHazardTimes)
    {
        SelectedPath = LifeSimDataBasePath;
        LoadHydraulics();
        SelectHydraulics(hydraulicsID);
        LoadIndexPoints();
        SelectIndexPoints(indexPointsID);
        SelectSimulation(selectedSimulation);
        SelectAlternatives(selectedAlternatives);
        SelectHazardTimes(selectedHazardTimes);
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

    private void SelectHydraulics(int id)
    {
        foreach (HydraulicElement hyd in Hydraulics)
        {
            if (hyd.ID == id)
            {
                SelectedHydraulics = hyd;
                return;
            }
        }
    }

    private void SelectIndexPoints(int id)
    {
        foreach (IndexPointsElement idx in IndexPoints)
        {
            if (idx.ID == id)
            {
                SelectedIndexPoints = idx;
                return;
            }
        }
    }

    private void SelectSimulation(string selectedSimulation)
    {
        foreach (LifeSimSimulation simulation in Simulations)
        {
            string name = simulation.Name;
            if (name == selectedSimulation)
            {
                SelectedSimulation = simulation;
            }
        }
    }

    private void SelectAlternatives(List<string> selectedAlternativers)
    {
        foreach (CheckableItem alternative in LifeSimAlternatives)
        {
            alternative.IsChecked = false;
            string name = alternative.Name;
            if (selectedAlternativers.Contains(name))
                alternative.IsChecked = true;
        }
    }

    private void SelectHazardTimes(List<string> selectedHazardTimes)
    {
        foreach (CheckableItem ht in HazardTimes)
        {
            ht.IsChecked = false;
            string name = ht.Name;
            if (selectedHazardTimes.Contains(name))
                ht.IsChecked = true;
        }
    }

    [RelayCommand]
    public async Task ComputeCurves()
    {
        if (LifeSimAlternatives.IsEmpty() || HazardTimes.IsEmpty()) return;

        // CURRENTLY USING THIS FOLDER, WILL CHANGE TO ACTUAL PROJECT FOLDER ONCE I REFACTOR
        // HOW THE GENERATOR READS IN HYDRAULICS
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
        LifeLossFunctions.Clear();
        List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        string uniqueImpactAreaHeader = impactAreaElements[0].UniqueNameColumnHeader;
        LifeLossFunctions = await generator.CreateLifeLossFunctionsAsync(impactAreasFile, indexPointsFile, uniqueImpactAreaHeader);
        _plotIndex = 0;
        ChangePlot(_plotIndex);
        WasRecomputed = true;
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

    [RelayCommand]
    public void NextPlot()
    {
        if (LifeLossFunctions.IsEmpty()) return;
        _plotIndex = (_plotIndex + 1) % LifeLossFunctions.Count;
        ChangePlot(_plotIndex);
    }

    /// <summary>
    /// Update the plot displayed in the UI
    /// </summary>
    /// <param name="next">The index of the plot to be switched to</param>
    private void ChangePlot(int next)
    {
        if (LifeLossFunctions.IsNullOrEmpty()) return;

        LifeLossFunction relationship = LifeLossFunctions[next];
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
