using Geospatial.Features;
using Geospatial.GDALAssist;
using Geospatial.GDALAssist.Vectors;
using Geospatial.IO;
using HEC.FDA.Model.compute;
using HEC.FDA.Model.metrics;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.scenarios;
using HEC.FDA.Model.Spatial;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.AlternativeComparisonReport;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.IndexPoints;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.LifeLoss;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;
using RasMapperLib;
using Statistics;
using Statistics.Distributions;
using Statistics.Histograms;
using ScottPlot;
using HEC.FDA.Model.utilities;

namespace ScratchSpace.EntryPoints;

public static class Beam
{
    // ============================================================================
    // CONFIGURATION - Set your study path and scenario name here
    // ============================================================================
    private static readonly string STUDY_PATH = @"C:\Users\HEC\Projects\AlaiWai2\AlaiWai2\AlaiWai2.sqlite";
    private static readonly string SCENARIO_NAME = "FWOP"; // Leave empty to compute all scenarios, or set a specific name

    public static void EntryPoint()
    {
        RunScenarioCompute();
    }

    /// <summary>
    /// Loads an FDA study from a hardcoded path and fires off a Scenario compute.
    /// </summary>
    public static void RunScenarioCompute()
    {
        Console.WriteLine("===========================================");
        Console.WriteLine("FDA Scenario Compute Script");
        Console.WriteLine("===========================================\n");

        // Step 1: Load the study
        Console.WriteLine($"Loading study from: {STUDY_PATH}");
        if (!System.IO.File.Exists(STUDY_PATH))
        {
            Console.WriteLine($"ERROR: Study file not found at {STUDY_PATH}");
            Console.WriteLine("Please update the STUDY_PATH constant in Beam.cs");
            return;
        }

        LoadStudy(STUDY_PATH);
        Console.WriteLine("Study loaded successfully!\n");

        // Step 2: List available scenarios
        List<IASElement> scenarios = GetScenarios();
        Console.WriteLine($"Found {scenarios.Count} scenario(s):");
        foreach (var scenario in scenarios)
        {
            Console.WriteLine($"  - [{scenario.ID}] {scenario.Name} (Year: {scenario.AnalysisYear})");
        }
        Console.WriteLine();

        if (scenarios.Count == 0)
        {
            Console.WriteLine("No scenarios found in this study.");
            return;
        }

        // Step 3: Select scenario(s) to compute
        List<IASElement> scenariosToCompute;
        if (string.IsNullOrEmpty(SCENARIO_NAME))
        {
            scenariosToCompute = scenarios;
            Console.WriteLine("Computing ALL scenarios...\n");
        }
        else
        {
            scenariosToCompute = scenarios.Where(s => s.Name.Equals(SCENARIO_NAME, StringComparison.OrdinalIgnoreCase)).ToList();
            if (scenariosToCompute.Count == 0)
            {
                Console.WriteLine($"ERROR: No scenario found with name '{SCENARIO_NAME}'");
                return;
            }
            Console.WriteLine($"Computing scenario: {SCENARIO_NAME}\n");
        }

        // Step 4: Compute each scenario
        foreach (var iasElement in scenariosToCompute)
        {
            ComputeScenario(iasElement);
        }

        Console.WriteLine("\n===========================================");
        Console.WriteLine("All computations complete!");
        Console.WriteLine("===========================================");
    }

    /// <summary>
    /// Loads an FDA study by setting up the Connection and loading elements into the cache.
    /// </summary>
    private static void LoadStudy(string studyPath)
    {
        // Set up the connection to the SQLite database
        Connection.Instance.ProjectFile = studyPath;

        // Create the cache and set it on the static properties
        FDACache cache = new();
        BaseViewModel.StudyCache = cache;
        PersistenceFactory.StudyCacheForSaving = cache;

        // Load all elements from the database
        LoadElementsFromDB();
    }

    /// <summary>
    /// Loads all element types from the database into the cache.
    /// </summary>
    private static void LoadElementsFromDB()
    {
        PersistenceFactory.GetElementManager<StageDischargeElement>().Load();
        PersistenceFactory.GetElementManager<TerrainElement>().Load();
        PersistenceFactory.GetElementManager<ImpactAreaElement>().Load();
        PersistenceFactory.GetElementManager<IndexPointsElement>().Load();
        PersistenceFactory.GetElementManager<HydraulicElement>().Load();
        PersistenceFactory.GetElementManager<FrequencyElement>().Load();
        PersistenceFactory.GetElementManager<InflowOutflowElement>().Load();
        PersistenceFactory.GetElementManager<ExteriorInteriorElement>().Load();
        PersistenceFactory.GetElementManager<LateralStructureElement>().Load();
        PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().Load();
        PersistenceFactory.GetElementManager<StageLifeLossElement>().Load();
        PersistenceFactory.GetElementManager<InventoryElement>().Load();
        PersistenceFactory.GetElementManager<IASElement>().Load();
        PersistenceFactory.GetElementManager<AlternativeElement>().Load();
        PersistenceFactory.GetElementManager<AlternativeComparisonReportElement>().Load();
        PersistenceFactory.GetElementManager<OccupancyTypesElement>().Load();
        PersistenceFactory.GetElementManager<StudyPropertiesElement>().Load();
    }

    /// <summary>
    /// Gets all scenarios (IASElements) from the loaded study.
    /// </summary>
    private static List<IASElement> GetScenarios()
    {
        return BaseViewModel.StudyCache.GetChildElementsOfType<IASElement>();
    }

    /// <summary>
    /// Computes a single scenario and reports results.
    /// </summary>
    private static void ComputeScenario(IASElement iasElement)
    {
        Console.WriteLine($"-------------------------------------------");
        Console.WriteLine($"Computing Scenario: {iasElement.Name}");
        Console.WriteLine($"-------------------------------------------");

        // Validate the scenario can be computed
        FdaValidationResult canComputeResult = iasElement.CanCompute();
        if (!canComputeResult.IsValid)
        {
            Console.WriteLine($"ERROR: Cannot compute scenario '{iasElement.Name}'");
            Console.WriteLine($"  {canComputeResult.ErrorMessage}");
            return;
        }

        // Create simulations from the scenario's SpecificIAS elements
        List<SpecificIAS> specificIASList = iasElement.SpecificIASElements;
        Console.WriteLine($"  Impact Areas: {specificIASList.Count}");

        List<ImpactAreaScenarioSimulation> simulations = new();
        foreach (SpecificIAS specificIAS in specificIASList)
        {
            ImpactAreaScenarioSimulation sim = specificIAS.CreateSimulation();
            if (sim != null)
            {
                simulations.Add(sim);
            }
        }

        if (simulations.Count == 0)
        {
            Console.WriteLine("  ERROR: No valid simulations could be created.");
            return;
        }

        // Create the Scenario object
        Scenario scenario = new(simulations);

        // Get convergence criteria from study properties
        ConvergenceCriteria convergenceCriteria = BaseViewModel.StudyCache.GetStudyPropertiesElement().GetStudyConvergenceCriteria();
        Console.WriteLine($"  Convergence: min={convergenceCriteria.MinIterations}, max={convergenceCriteria.MaxIterations}");

        // Run the compute
        Console.WriteLine("  Starting computation...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        CancellationTokenSource cts = new();
        ScenarioResults results = scenario.Compute(convergenceCriteria, cts.Token, computeIsDeterministic: false);

        stopwatch.Stop();
        Console.WriteLine($"  Computation completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");

        // Report results
        ReportResults(iasElement.Name, results);

        // Optionally save results back to the element
        iasElement.Results = results;
        // Uncomment the following to persist results to the database:
        // iasElement.UpdateComputeDate = true;
        // PersistenceFactory.GetIASManager().SaveExisting(iasElement);
        // iasElement.UpdateComputeDate = false;
    }

    /// <summary>
    /// Reports summary of scenario results to the console.
    /// </summary>
    private static void ReportResults(string scenarioName, ScenarioResults results)
    {
        Console.WriteLine($"\n  Results for '{scenarioName}':");

        foreach (var iaResult in results.ResultsList)
        {
            Console.WriteLine($"\n  Impact Area ID: {iaResult.ImpactAreaID}");

            // Get EAD (Expected Annual Damage) if available
            try
            {
                double meanEAD = iaResult.MeanExpectedAnnualConsequences(damageCategory: null, assetCategory: null, impactAreaID: iaResult.ImpactAreaID);
                Console.WriteLine($"    Mean EAD: ${meanEAD:N0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    (Could not retrieve EAD: {ex.Message})");
            }
        }
    }
}
