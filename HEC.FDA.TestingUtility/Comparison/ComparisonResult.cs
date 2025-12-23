namespace HEC.FDA.TestingUtility.Comparison;

public class ComparisonResult
{
    public string ElementName { get; set; } = string.Empty;
    public string ElementType { get; set; } = string.Empty;
    public bool Passed { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public List<Difference> Differences { get; } = new();

    public string Summary => Passed
        ? "All values match"
        : ErrorMessage ?? $"{Differences.Count} difference(s) found";
}

public class Difference
{
    public string Metric { get; set; } = string.Empty;
    public double? Expected { get; set; }
    public double? Actual { get; set; }
    public double? AbsoluteDifference => Expected.HasValue && Actual.HasValue
        ? Math.Abs(Expected.Value - Actual.Value)
        : null;
    public double? PercentDifference => Expected.HasValue && Actual.HasValue && Expected.Value != 0
        ? Math.Abs((Expected.Value - Actual.Value) / Expected.Value) * 100
        : null;

    public override string ToString()
    {
        if (Expected.HasValue && Actual.HasValue)
        {
            return $"{Metric}: Expected={Expected:F4}, Actual={Actual:F4}, Diff={AbsoluteDifference:F4} ({PercentDifference:F2}%)";
        }
        return $"{Metric}: Expected={Expected}, Actual={Actual}";
    }
}
