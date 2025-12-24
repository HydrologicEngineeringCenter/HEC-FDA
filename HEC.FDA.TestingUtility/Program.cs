using System.CommandLine;
using Geospatial.GDALAssist;
using HEC.FDA.TestingUtility;
using HEC.FDA.TestingUtility.Configuration;

// Initialize GDAL early
GDALSetup.InitializeMultiplatform();

// Create root command
RootCommand rootCommand = new("FDA Testing Utility - Regression Testing Tool for FDA Studies");

// ============ COMPUTE COMMAND ============
Command computeCommand = new("compute", "Run computations on FDA studies and generate result files");

Option<FileInfo> computeConfigOption = new(
    name: "--config",
    description: "Path to JSON configuration file")
{ IsRequired = true };
computeConfigOption.AddAlias("-c");

Option<DirectoryInfo> computeOutputOption = new(
    name: "--output",
    description: "Output directory for generated files",
    getDefaultValue: () => new DirectoryInfo(Environment.CurrentDirectory));
computeOutputOption.AddAlias("-o");

Option<string[]> computeStudyOption = new(
    name: "--study",
    description: "Filter to specific study IDs (can specify multiple)")
{ AllowMultipleArgumentsPerToken = true };
computeStudyOption.AddAlias("-s");

computeCommand.AddOption(computeConfigOption);
computeCommand.AddOption(computeOutputOption);
computeCommand.AddOption(computeStudyOption);

computeCommand.SetHandler(async (configFile, outputDir, studyFilter) =>
{
    try
    {
        Console.WriteLine("FDA Testing Utility - Compute");
        Console.WriteLine("=============================");
        Console.WriteLine();

        if (!configFile.Exists)
        {
            Console.WriteLine($"Error: Configuration file not found: {configFile.FullName}");
            Environment.Exit(1);
        }

        Console.WriteLine($"Loading configuration: {configFile.FullName}");
        TestConfiguration config = TestConfiguration.LoadFromFile(configFile.FullName);

        if (!outputDir.Exists)
        {
            outputDir.Create();
        }

        ComputeRunner runner = new(
            config,
            outputDir.FullName,
            studyFilter?.Length > 0 ? studyFilter : null);

        int exitCode = await runner.RunAsync();
        Environment.Exit(exitCode);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fatal error: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        Environment.Exit(1);
    }
}, computeConfigOption, computeOutputOption, computeStudyOption);

// ============ COMPARE COMMAND ============
Command compareCommand = new("compare", "Compare two sets of computed results and generate a comparison report");

Option<DirectoryInfo> baselineOption = new(
    name: "--baseline",
    description: "Directory containing baseline result files")
{ IsRequired = true };
baselineOption.AddAlias("-b");

Option<DirectoryInfo> newResultsOption = new(
    name: "--new",
    description: "Directory containing new result files to compare against baseline")
{ IsRequired = true };
newResultsOption.AddAlias("-n");

Option<FileInfo> compareOutputOption = new(
    name: "--output",
    description: "Output file for comparison report",
    getDefaultValue: () => new FileInfo(Path.Combine(Environment.CurrentDirectory, "comparison_report.txt")));
compareOutputOption.AddAlias("-o");

Option<double> toleranceOption = new(
    name: "--tolerance",
    description: "Relative tolerance for numeric comparisons (default: 0.01 = 1%)",
    getDefaultValue: () => 0.01);
toleranceOption.AddAlias("-t");

compareCommand.AddOption(baselineOption);
compareCommand.AddOption(newResultsOption);
compareCommand.AddOption(compareOutputOption);
compareCommand.AddOption(toleranceOption);

compareCommand.SetHandler((baselineDir, newDir, outputFile, tolerance) =>
{
    try
    {
        Console.WriteLine("FDA Testing Utility - Compare");
        Console.WriteLine("=============================");
        Console.WriteLine();

        if (!baselineDir.Exists)
        {
            Console.WriteLine($"Error: Baseline directory not found: {baselineDir.FullName}");
            Environment.Exit(1);
        }

        if (!newDir.Exists)
        {
            Console.WriteLine($"Error: New results directory not found: {newDir.FullName}");
            Environment.Exit(1);
        }

        CompareRunner runner = new(
            baselineDir.FullName,
            newDir.FullName,
            outputFile.FullName,
            tolerance);

        int exitCode = runner.Run();
        Environment.Exit(exitCode);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fatal error: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        Environment.Exit(1);
    }
}, baselineOption, newResultsOption, compareOutputOption, toleranceOption);

// Add subcommands to root
rootCommand.AddCommand(computeCommand);
rootCommand.AddCommand(compareCommand);

// Run
return await rootCommand.InvokeAsync(args);
