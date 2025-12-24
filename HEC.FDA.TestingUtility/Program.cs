using System.CommandLine;
using Geospatial.GDALAssist;
using HEC.FDA.TestingUtility;
using HEC.FDA.TestingUtility.Configuration;

// Initialize GDAL early
GDALSetup.InitializeMultiplatform();

// Create root command
RootCommand rootCommand = new("FDA Testing Utility - Regression Testing Tool for FDA Studies");

// ============ COMPUTE COMMAND ============
Command computeCommand = new("compute", "Run computations on FDA studies and generate CSV result reports");

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

// Add subcommands to root
rootCommand.AddCommand(computeCommand);

// Run
return await rootCommand.InvokeAsync(args);
