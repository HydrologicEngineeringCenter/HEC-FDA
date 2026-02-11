using CommunityToolkit.Mvvm.Input;
using HEC.FDA.Model.LifeLoss;
using HEC.FDA.Model.LifeLoss.Saving;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using SciChart.Core.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utility;
using Utility.Extensions;
using Visual.Observables;

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
    private ObservableCollection<WeightedCheckableItem> _hazardTimes = [];
    private LifeLossFunction _selectedFunction;

    // Store saved configuration for restoring when swapping simulations
    private string _savedSimulationName;
    private Dictionary<string, double> _savedHazardTimeWeights;

    // Track weights used for computation to detect mismatches
    private Dictionary<string, double> _computedHazardTimeWeights = [];

    public Dictionary<string, double> ComputedHazardTimeWeights => _computedHazardTimeWeights;

    /// <summary>
    /// Returns true if the currently selected weights differ from the weights used to compute the curves.
    /// </summary>
    public bool HasWeightMismatch
    {
        get
        {
            if (_computedHazardTimeWeights == null || _computedHazardTimeWeights.Count == 0)
                return false;

            var currentWeights = GetCurrentSelectedWeights();
            if (currentWeights.Count != _computedHazardTimeWeights.Count)
                return true;

            foreach (var kvp in currentWeights)
            {
                if (!_computedHazardTimeWeights.TryGetValue(kvp.Key, out double computedWeight))
                    return true;
                if (System.Math.Abs(kvp.Value - computedWeight) > 0.001)
                    return true;
            }
            return false;
        }
    }

    private Dictionary<string, double> GetCurrentSelectedWeights()
    {
        Dictionary<string, double> weights = [];
        foreach (WeightedCheckableItem ht in HazardTimes)
        {
            if (ht.IsChecked)
                weights[ht.Name] = ht.Weight;
        }
        return weights;
    }

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

    private BatchJob _job;
    public BatchJob Job
    {
        get => _job;
        private set
        {
            _job = value;
            NotifyPropertyChanged();
        }
    }

    private void OnSelectedSimulationChanged(LifeSimSimulation simulation)
    {
        // reset the other options
        LifeSimAlternatives.Clear();
        foreach (var ht in HazardTimes)
            ht.PropertyChanged -= On_Hazard_Time_Checkbox_Changed;
        HazardTimes.Clear();

        // update options to match the currently selected simulation
        foreach (string alternative in simulation.Alternatives)
            LifeSimAlternatives.Add(new CheckableItem { Name = alternative, IsChecked = true });

        int i = 0;
        foreach (string hazardTime in simulation.HazardTimes.Keys)
        {
            if (int.TryParse(hazardTime, out int time))
            {
                string formattedTime = time < 10 ? $"0{time}00" : $"{time}00";
                WeightedCheckableItem item = new() { Name = formattedTime, Value = time.ToString() };
                if (i == 0)
                    item.Weight = 0.55;
                else if (i == 1)
                    item.Weight = 0.45;
                if (i < 2)
                {
                    item.IsChecked = true;
                    item.IsEnabled = true;
                }
                item.PropertyChanged += On_Hazard_Time_Checkbox_Changed;
                HazardTimes.Add(item);
            }
            i++;
        }

        // Restore saved weights if switching back to the originally saved simulation
        if (simulation.Name == _savedSimulationName && _savedHazardTimeWeights != null)
        {
            foreach (WeightedCheckableItem ht in HazardTimes)
            {
                if (_savedHazardTimeWeights.TryGetValue(ht.Name, out double weight))
                {
                    ht.IsChecked = true;
                    ht.Weight = weight;
                }
                else
                {
                    ht.IsChecked = false;
                }
            }
        }
    }

    private void On_Hazard_Time_Checkbox_Changed(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WeightedCheckableItem.IsChecked))
        {
            int checkedCount = HazardTimes.Count(x => x.IsChecked);
            bool maxReached = checkedCount >= 2;

            foreach (var item in HazardTimes)
            {
                // Disable unchecked items when 2 are already checked
                item.IsEnabled = item.IsChecked || !maxReached;
            }
        }

        // Update mismatch warning when weights or selection changes
        if (e.PropertyName == nameof(WeightedCheckableItem.IsChecked) ||
            e.PropertyName == nameof(WeightedCheckableItem.Weight))
        {
            NotifyPropertyChanged(nameof(HasWeightMismatch));
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
    public ObservableCollection<WeightedCheckableItem> HazardTimes
    {
        get { return _hazardTimes; }
        set
        {
            _hazardTimes = value;
            NotifyPropertyChanged();
        }
    }

    public LifeLossFunction SelectedFunction
    {
        get { return _selectedFunction; }
        set
        {
            _selectedFunction = value;
            NotifyPropertyChanged();
            OnSelectedFunctionChanged(value);
        }
    }
    private void OnSelectedFunctionChanged(LifeLossFunction function)
    {
        if (function == null) return;
        int index = function.FunctionID - 1;
        ChangePlot(index);
    }

    public ViewResolvingPlotModel MyModel { get; set; } = new();
    public ObservableCollection<LifeLossFunction> LifeLossFunctions { get; private set; } = [];
    public bool WasRecomputed { get; private set; } = false;

    #endregion

    /// <summary>
    /// Updates the saved configuration after a successful save, so that swapping
    /// simulations and switching back will restore the newly saved weights.
    /// </summary>
    public void UpdateSavedConfiguration()
    {
        _savedSimulationName = SelectedSimulation?.Name;
        _savedHazardTimeWeights = [];
        foreach (WeightedCheckableItem ht in HazardTimes)
        {
            if (ht.IsChecked)
                _savedHazardTimeWeights[ht.Name] = ht.Weight;
        }
    }

    // called when creating new element
    public IndexPointsLifeLossVM()
    {
        _savedSimulationName = null;
        _savedHazardTimeWeights = null;
        LoadHydraulics();
        LoadIndexPoints();
        SubscribeToLiveUpdateEvents();
    }

    // called when opening existing element editor
    public IndexPointsLifeLossVM(
        int elementID,
        string LifeSimDataBaseFileName,
        int hydraulicsID,
        int indexPointsID,
        string selectedSimulation,
        List<string> selectedAlternatives,
        Dictionary<string, double> selectedHazardTimes,
        Dictionary<string, double> computedHazardTimeWeights)
    {
        // Store saved configuration for restoring when swapping simulations
        _savedSimulationName = selectedSimulation;
        _savedHazardTimeWeights = selectedHazardTimes ?? [];

        // Store computed weights for mismatch detection
        _computedHazardTimeWeights = computedHazardTimeWeights ?? [];

        SelectedPath = LifeSimDataBaseFileName.IsNullOrEmpty() ? null : Path.Combine(Connection.Instance.LifeSimDirectory, LifeSimDataBaseFileName);
        LoadHydraulics();
        SelectHydraulics(hydraulicsID);
        LoadIndexPoints();
        SelectIndexPoints(indexPointsID);
        SelectSimulation(selectedSimulation);
        SelectAlternatives(selectedAlternatives);
        SelectHazardTimes(selectedHazardTimes);
        LoadStageLifeLossRelationships(elementID);
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

    private void SelectHazardTimes(Dictionary<string, double> selectedHazardTimes)
    {
        foreach (WeightedCheckableItem ht in HazardTimes)
        {
            ht.IsChecked = false;
            string name = ht.Name;
            if (selectedHazardTimes.TryGetValue(name, out double weight))
            {
                ht.IsChecked = true;
                ht.Weight = weight;
            }
        }
    }

    private void LoadStageLifeLossRelationships(int elementID)
    {
        string projFile = Connection.Instance.ProjectFile;
        List<StageLifeLossElement> lifeLossElements = StudyCache.GetChildElementsOfType<StageLifeLossElement>();

        StageLifeLossElement matchingElement = lifeLossElements
            .FirstOrDefault(e => e.ID == elementID);

        if (matchingElement == null)
        {
            throw new System.Exception($"No StageLifeLossElement with ID matching {elementID}");
        }

        List<LifeLossFunction> functions = matchingElement.LoadStageLifeLossRelationships();
        LifeLossFunctions.Clear();
        LifeLossFunctions.AddRange(functions);

        if (LifeLossFunctions.Count > 0)
        {
            ChangePlot(0);
        }
    }

    [RelayCommand]
    public async Task ComputeCurves()
    {
        if (LifeSimAlternatives.IsEmpty() || HazardTimes.IsEmpty())
            return;

        string hydraulicsFolder = Path.Combine(Connection.Instance.HydraulicsDirectory, SelectedHydraulics.Name);

        LifeSimSimulation currentSimulation = new(SelectedSimulation.Name, hydraulicsFolder);
        foreach (CheckableItem a in LifeSimAlternatives)
            if (a.IsChecked) currentSimulation.Alternatives.Add(a.Name);
        foreach (WeightedCheckableItem h in HazardTimes)
            if (h.IsChecked) currentSimulation.HazardTimes[h.Value] = h.Weight;
        string indexPointsFile = GetIndexPointsFile();
        string impactAreasFile = GetImpactAreaFile();
        LifeLossFunctions.Clear();
        List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
        string uniqueImpactAreaHeader = impactAreaElements[0].UniqueNameColumnHeader;
        Dictionary<string, int> IANameToID = impactAreaElements[0].GetNameToIDPairs();
        LifeLossFunctionGenerator generator = new(SelectedPath, currentSimulation, IANameToID);
        var missings = generator.GetMissingHydraulics();
        if (missings.Count != 0)
        {
            string msg = "The following hydraulics files are missing and are required to compute life loss functions:\n";
            foreach (string missing in missings)
            {
                msg += $"- {missing}\n";
            }
            System.Windows.MessageBox.Show(msg, "Missing Hydraulics Files", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        ISynchronizationContext context = new SynchronizationContext(action => Application.Current.Dispatcher.BeginInvoke(action));
        Job = new(uiThreadSyncContext: context);
        List<LifeLossFunction> newFunctions = await generator.CreateLifeLossFunctionsAsync(impactAreasFile, indexPointsFile, uniqueImpactAreaHeader, Job.Reporter);
        if (newFunctions.Count == 0)
            return; // Computation was aborted (e.g., weight validation failed)

        LifeLossFunctions.Clear();
        LifeLossFunctions.AddRange(newFunctions);
        ChangePlot(0);
        WasRecomputed = true;

        // Store the weights used for this computation
        _computedHazardTimeWeights = GetCurrentSelectedWeights();
        NotifyPropertyChanged(nameof(HasWeightMismatch));
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
        if (LifeLossFunctions.IsNullOrEmpty()) return;

        LifeLossFunction llf = LifeLossFunctions[next];
        UncertainPairedData uncertainData = llf.Data;
        var upper = uncertainData.SamplePairedData(0.975);
        var lower = uncertainData.SamplePairedData(0.025);
        var mid = uncertainData.SamplePairedData(0.5);

        MyModel.Series.Clear();
        MyModel.Legends.Clear();
        MyModel.Legends.Add(new Legend
        {
            LegendPosition = LegendPosition.TopLeft,
            LegendPlacement = LegendPlacement.Inside
        });
        AddLineSeriesToPlot(upper, "97.5 Percentile", isConfidenceLimit: true);
        AddLineSeriesToPlot(mid, "Median");
        AddLineSeriesToPlot(lower, "2.5 Percentile", isConfidenceLimit: true);

        MyModel.ResetAllAxes(); // recenter on the newly plotted lines
        if (!int.TryParse(llf.HazardTime, out int time))
        {
            if (!llf.HazardTime.StartsWith(LifeLossStringConstants.COMBINED_MAGIC_STRING))
                throw new System.Exception($"Could not parse hazard time {llf.HazardTime}.");
            MyModel.Title = $"{llf.SimulationName}: {llf.SummaryZone} {llf.HazardTime}";
        }
        else
        {
            string formattedTime = time < 10 ? $"0{time}00" : $"{time}00";
            MyModel.Title = $"{llf.SimulationName}: {llf.SummaryZone} {formattedTime}";
        }
        MyModel.InvalidatePlot(true);
    }

    /// <summary>
    /// Add a single line series to a plot
    /// </summary>
    /// <param name="function"></param>
    /// <param name="title">Title for the legend</param>
    /// <param name="isConfidenceLimit"></param>
    private void AddLineSeriesToPlot(PairedData function, string title, bool isConfidenceLimit = false)
    {
        LineSeries lineSeries = new()
        {
            Title = title
        };

        DataPoint[] points = new DataPoint[function.Xvals.Count];
        for (int i = 0; i < function.Xvals.Count; i++)
        {
            double stage = function.Xvals[i];
            double lifeLoss = function.Yvals[i];
            points[i] = new DataPoint(stage, lifeLoss);
        }

        if (isConfidenceLimit) { lineSeries.Color = OxyColors.Blue; lineSeries.LineStyle = LineStyle.Dash; }
        else { lineSeries.Color = OxyColors.Black; }

        lineSeries.ItemsSource = points;
        MyModel.Series.Add(lineSeries);
    }
}
