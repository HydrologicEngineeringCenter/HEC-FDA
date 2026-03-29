using System.CommandLine;
using Geospatial.GDALAssist;
using HEC.FDA.TestingUtility.Configuration;

namespace HEC.FDA.TestingUtility;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Initialize GDAL early
        GDALSetup.InitializeMultiplatform();

        // Create root command
        RootCommand rootCommand = new("FDA Testing Utility - Regression Testing Tool for FDA Studies");

        // ============ COMPUTE COMMAND ============
        Command computeCommand = new("compute", "Run computations on FDA studies and generate CSV result reports");

        Option<FileInfo?> computeConfigOption = new(
            name: "--config",
            description: "Path to JSON configuration file");
        computeConfigOption.AddAlias("-c");

        Option<FileInfo?> studyPathOption = new(
            name: "--study-path",
            description: "Path to a .sqlite study file (runs all computations)");
        studyPathOption.AddAlias("-sp");

        Option<DirectoryInfo?> computeOutputOption = new(
            name: "--output",
            description: "Output directory for generated files");
        computeOutputOption.AddAlias("-o");

        Option<string[]> computeStudyOption = new(
            name: "--study",
            description: "Filter to specific study IDs (can specify multiple)")
        { AllowMultipleArgumentsPerToken = true };
        computeStudyOption.AddAlias("-s");

        computeCommand.AddOption(computeConfigOption);
        computeCommand.AddOption(studyPathOption);
        computeCommand.AddOption(computeOutputOption);
        computeCommand.AddOption(computeStudyOption);

        computeCommand.SetHandler(async (configFile, studyPath, outputDir, studyFilter) =>
        {
            try
            {
                Console.WriteLine("FDA Testing Utility - Compute");
                Console.WriteLine("=============================");
                Console.WriteLine();

                if (configFile != null && studyPath != null)
                {
                    Console.WriteLine("Error: Cannot specify both --config and --study-path. Use one or the other.");
                    return;
                }

                if (configFile == null && studyPath == null)
                {
                    Console.WriteLine("Error: Must specify either --config or --study-path.");
                    return;
                }

                TestConfiguration config;
                string outputPath;

                if (studyPath != null)
                {
                    if (!studyPath.Exists)
                    {
                        Console.WriteLine($"Error: Study file not found: {studyPath.FullName}");
                        return;
                    }

                    string studyName = Path.GetFileNameWithoutExtension(studyPath.Name);
                    string studyDir = studyPath.DirectoryName!;

                    Console.WriteLine($"Direct study mode: {studyPath.FullName}");
                    config = new TestConfiguration
                    {
                        TestSuiteId = $"direct-{studyName}",
                        Studies = new List<StudyConfiguration>
                        {
                            new()
                            {
                                StudyId = studyName,
                                StudyName = studyName,
                                NetworkSourcePath = studyDir,
                                RunAllStageDamage = true,
                                RunAllScenarios = true,
                                RunAllAlternatives = true,
                                RunAllAlternativeComparisons = true,
                            }
                        }
                    };

                    outputPath = outputDir?.FullName ?? studyDir;
                }
                else
                {
                    if (!configFile!.Exists)
                    {
                        Console.WriteLine($"Error: Configuration file not found: {configFile.FullName}");
                        return;
                    }

                    Console.WriteLine($"Loading configuration: {configFile.FullName}");
                    config = TestConfiguration.LoadFromFile(configFile.FullName);
                    outputPath = outputDir?.FullName ?? Environment.CurrentDirectory;
                }

                Directory.CreateDirectory(outputPath);

                ComputeRunner runner = new(
                    config,
                    outputPath,
                    studyFilter?.Length > 0 ? studyFilter : null);

                await runner.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }, computeConfigOption, studyPathOption, computeOutputOption, computeStudyOption);

        // Add subcommands to root
        rootCommand.AddCommand(computeCommand);

        // Run
        return await rootCommand.InvokeAsync(args);
    }
}
