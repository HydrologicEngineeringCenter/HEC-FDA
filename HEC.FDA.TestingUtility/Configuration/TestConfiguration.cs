using System.Text.Json;
using System.Text.Json.Serialization;

namespace HEC.FDA.TestingUtility.Configuration;

public class TestConfiguration
{
    [JsonPropertyName("testSuiteId")]
    public string TestSuiteId { get; set; } = string.Empty;

    [JsonPropertyName("globalSettings")]
    public GlobalSettings GlobalSettings { get; set; } = new();

    [JsonPropertyName("studies")]
    public List<StudyConfiguration> Studies { get; set; } = new();

    public static TestConfiguration LoadFromFile(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TestConfiguration>(json)
            ?? throw new InvalidOperationException($"Failed to deserialize configuration from {path}");
    }
}

public class GlobalSettings
{
    [JsonPropertyName("localTempDirectory")]
    public string LocalTempDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "FDATests");

    [JsonPropertyName("timeoutMinutes")]
    public int TimeoutMinutes { get; set; } = 30;
}

public class StudyConfiguration
{
    [JsonPropertyName("studyId")]
    public string StudyId { get; set; } = string.Empty;

    [JsonPropertyName("studyName")]
    public string StudyName { get; set; } = string.Empty;

    [JsonPropertyName("networkSourcePath")]
    public string NetworkSourcePath { get; set; } = string.Empty;

    [JsonPropertyName("baselineDirectory")]
    public string BaselineDirectory { get; set; } = string.Empty;

    [JsonPropertyName("runAllScenarios")]
    public bool RunAllScenarios { get; set; } = false;

    [JsonPropertyName("runAllAlternatives")]
    public bool RunAllAlternatives { get; set; } = false;

    [JsonPropertyName("runAllStageDamage")]
    public bool RunAllStageDamage { get; set; } = false;

    [JsonPropertyName("computations")]
    public List<ComputeConfiguration> Computations { get; set; } = new();
}

public class ComputeConfiguration
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("elementName")]
    public string ElementName { get; set; } = string.Empty;

    [JsonPropertyName("elementId")]
    public int? ElementId { get; set; }
}
