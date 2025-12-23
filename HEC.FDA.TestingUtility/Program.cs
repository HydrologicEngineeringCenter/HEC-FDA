using System.CommandLine;
using Geospatial.GDALAssist;
using HEC.FDA.TestingUtility;
using HEC.FDA.TestingUtility.Configuration;

// Define command line options
var configOption = new Option<FileInfo>(
    name: "--config",
    description: "Path to JSON configuration file")
{ IsRequired = true };
configOption.AddAlias("-c");

var outputOption = new Option<DirectoryInfo>(
    name: "--output",
    description: "Output directory for results",
    getDefaultValue: () => new DirectoryInfo(Environment.CurrentDirectory));
outputOption.AddAlias("-o");

var verboseOption = new Option<bool>(
    name: "--verbose",
    description: "Enable verbose output",
    getDefaultValue: () => false);
verboseOption.AddAlias("-v");

var studyOption = new Option<string[]>(
    name: "--study",
    description: "Filter to specific study IDs (can specify multiple)")
{ AllowMultipleArgumentsPerToken = true };
studyOption.AddAlias("-s");

// Create root command
var rootCommand = new RootCommand("FDA Testing Utility - Regression Testing Tool for FDA Studies");
rootCommand.AddOption(configOption);
rootCommand.AddOption(outputOption);
rootCommand.AddOption(verboseOption);
rootCommand.AddOption(studyOption);

// Set handler
rootCommand.SetHandler(async (configFile, outputDir, verbose, studyFilter) =>
{
    try
    {
        Console.WriteLine("FDA Testing Utility v1.0");
        Console.WriteLine("========================");
        Console.WriteLine();

        // Initialize GDAL for spatial operations
        GDALSetup.InitializeMultiplatform();

        // Load configuration
        if (!configFile.Exists)
        {
            Console.WriteLine($"Error: Configuration file not found: {configFile.FullName}");
            Environment.Exit(1);
        }

        Console.WriteLine($"Loading configuration: {configFile.FullName}");
        var config = TestConfiguration.LoadFromFile(configFile.FullName);

        // Ensure output directory exists
        if (!outputDir.Exists)
        {
            outputDir.Create();
        }

        // Create and run test runner
        var runner = new TestRunner(
            config,
            outputDir.FullName,
            verbose,
            studyFilter?.Length > 0 ? studyFilter : null);

        int exitCode = await runner.RunAsync();
        Environment.Exit(exitCode);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fatal error: {ex.Message}");
        if (verbose)
        {
            Console.WriteLine(ex.StackTrace);
        }
        Environment.Exit(1);
    }
}, configOption, outputOption, verboseOption, studyOption);

// Run
return await rootCommand.InvokeAsync(args);
