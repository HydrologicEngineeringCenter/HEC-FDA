using System.Data.SQLite;
using HEC.FDA.TestingUtility.Configuration;
using HEC.FDA.ViewModel;
using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Alternatives;
using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.FrequencyRelationships;
using HEC.FDA.ViewModel.GeoTech;
using HEC.FDA.ViewModel.Hydraulics;
using HEC.FDA.ViewModel.Hydraulics.GriddedData;
using HEC.FDA.ViewModel.ImpactArea;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Inventory;
using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Study;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Watershed;

namespace HEC.FDA.TestingUtility.Services;

public class StudyLoader : IDisposable
{
    private string? _localStudyPath;
    private bool _disposed;

    public void LoadStudy(string networkSourcePath, string localTempDirectory)
    {
        // 1. Copy study folder from network to local temp directory
        _localStudyPath = CopyStudyToLocal(networkSourcePath, localTempDirectory);

        // 2. Find the database file
        string dbPath = FindDatabaseFile(_localStudyPath);

        // 3. Set up connection (triggers folder structure creation)
        Connection.Instance.ProjectFile = dbPath;

        // 4. Create and assign cache
        FDACache cache = new();
        BaseViewModel.StudyCache = cache;
        PersistenceFactory.StudyCacheForSaving = cache;

        // 5. Load elements in dependency order
        LoadAllElements();
    }

    private static string CopyStudyToLocal(string networkPath, string localTempDir)
    {
        string studyFolderName = Path.GetFileName(networkPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        string localPath = Path.Combine(localTempDir, studyFolderName + "_" + Guid.NewGuid().ToString("N")[..8]);

        Console.WriteLine($"  Copying study from {networkPath} to {localPath}...");

        CopyDirectory(networkPath, localPath);

        Console.WriteLine($"  Study copied successfully.");
        return localPath;
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
        }

        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
            CopyDirectory(subDir, destSubDir);
        }
    }

    private static string FindDatabaseFile(string studyPath)
    {
        // Look for .sqlite or .db files recursively
        string[] dbExtensions = { "*.sqlite", "*.db" };

        foreach (string pattern in dbExtensions)
        {
            string[] files = Directory.GetFiles(studyPath, pattern, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                return files[0];
            }
        }

        throw new FileNotFoundException($"No database file (.sqlite or .db) found in study folder: {studyPath}");
    }

    private static void LoadAllElements()
    {
        // Order matters - dependencies first
        Console.WriteLine("  Loading study elements...");

        LoadElementType<TerrainElement>("Terrains");
        LoadElementType<ImpactAreaElement>("Impact Areas");
        LoadElementType<HydraulicElement>("Hydraulics");
        LoadElementType<FrequencyElement>("Frequency Relationships");
        LoadElementType<InflowOutflowElement>("Inflow-Outflow");
        LoadElementType<StageDischargeElement>("Stage-Discharge");
        LoadElementType<ExteriorInteriorElement>("Exterior-Interior");
        LoadElementType<LateralStructureElement>("Lateral Structures");
        LoadElementType<OccupancyTypesElement>("Occupancy Types");
        LoadElementType<InventoryElement>("Inventories");
        LoadElementType<AggregatedStageDamageElement>("Stage Damage");
        LoadElementType<IASElement>("Scenarios");
        LoadElementType<AlternativeElement>("Alternatives");
        LoadElementType<StudyPropertiesElement>("Study Properties");

        Console.WriteLine("  All elements loaded.");
    }

    private static void LoadElementType<T>(string displayName) where T : ChildElement
    {
        try
        {
            PersistenceFactory.GetElementManager<T>().Load();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    Warning: Failed to load {displayName}: {ex.Message}");
        }
    }

    public void Cleanup()
    {
        try
        {
            // Close the connection first
            if (!Connection.Instance.IsConnectionNull && Connection.Instance.IsOpen)
            {
                Connection.Instance.Close();
            }

            // Clear SQLite connection pool to release file handles
            SQLiteConnection.ClearAllPools();

            // Force garbage collection to release any remaining handles
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Warning: Failed to close connections: {ex.Message}");
        }

        if (_localStudyPath != null)
        {
            Console.WriteLine($"  Temp study folder retained at: {_localStudyPath}");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Cleanup();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
