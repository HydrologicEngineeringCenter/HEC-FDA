using System.Text;
using System.Xml.Linq;

namespace HEC.FDA.TestingUtility;

/// <summary>
/// Compares two sets of FDA computation results and generates a comparison report.
/// </summary>
public class CompareRunner
{
    private readonly string _baselineDir;
    private readonly string _newDir;
    private readonly string _outputPath;
    private readonly double _tolerance;

    public CompareRunner(string baselineDir, string newDir, string outputPath, double tolerance)
    {
        _baselineDir = baselineDir;
        _newDir = newDir;
        _outputPath = outputPath;
        _tolerance = tolerance;
    }

    public int Run()
    {
        Console.WriteLine($"Baseline directory: {_baselineDir}");
        Console.WriteLine($"New results directory: {_newDir}");
        Console.WriteLine($"Tolerance: {_tolerance:P1}");
        Console.WriteLine();

        // Find all XML result files in both directories
        string[] baselineFiles = Directory.GetFiles(_baselineDir, "*_results.xml");
        string[] newFiles = Directory.GetFiles(_newDir, "*_results.xml");

        if (baselineFiles.Length == 0)
        {
            Console.WriteLine("No baseline result files (*_results.xml) found.");
            return 1;
        }

        if (newFiles.Length == 0)
        {
            Console.WriteLine("No new result files (*_results.xml) found.");
            return 1;
        }

        StringBuilder report = new();
        report.AppendLine("FDA Results Comparison Report");
        report.AppendLine("=============================");
        report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Baseline: {_baselineDir}");
        report.AppendLine($"New: {_newDir}");
        report.AppendLine($"Tolerance: {_tolerance:P1}");
        report.AppendLine();

        int totalDifferences = 0;
        int filesCompared = 0;

        // Match files by study ID
        foreach (string baselineFile in baselineFiles)
        {
            string fileName = Path.GetFileName(baselineFile);
            string newFile = Path.Combine(_newDir, fileName);

            if (!File.Exists(newFile))
            {
                report.AppendLine($"MISSING: {fileName} not found in new results");
                Console.WriteLine($"MISSING: {fileName}");
                totalDifferences++;
                continue;
            }

            Console.WriteLine($"Comparing: {fileName}");
            int differences = CompareFiles(baselineFile, newFile, report);
            totalDifferences += differences;
            filesCompared++;

            if (differences == 0)
            {
                Console.WriteLine($"  MATCH: No differences found");
            }
            else
            {
                Console.WriteLine($"  DIFF: {differences} difference(s) found");
            }
        }

        // Check for files in new that aren't in baseline
        foreach (string newFile in newFiles)
        {
            string fileName = Path.GetFileName(newFile);
            string baselineFile = Path.Combine(_baselineDir, fileName);

            if (!File.Exists(baselineFile))
            {
                report.AppendLine($"NEW: {fileName} exists only in new results");
                Console.WriteLine($"NEW: {fileName}");
            }
        }

        // Summary
        report.AppendLine();
        report.AppendLine("=== Summary ===");
        report.AppendLine($"Files compared: {filesCompared}");
        report.AppendLine($"Total differences: {totalDifferences}");
        report.AppendLine($"Result: {(totalDifferences == 0 ? "PASS" : "FAIL")}");

        // Save report
        File.WriteAllText(_outputPath, report.ToString());
        Console.WriteLine();
        Console.WriteLine($"Report saved to: {_outputPath}");
        Console.WriteLine();
        Console.WriteLine($"=== Result: {(totalDifferences == 0 ? "PASS" : "FAIL")} ===");
        Console.WriteLine($"Files compared: {filesCompared}");
        Console.WriteLine($"Total differences: {totalDifferences}");

        return totalDifferences > 0 ? 1 : 0;
    }

    private int CompareFiles(string baselinePath, string newPath, StringBuilder report)
    {
        int differences = 0;

        try
        {
            XElement baseline = XElement.Load(baselinePath);
            XElement newResults = XElement.Load(newPath);

            string studyId = baseline.Attribute("studyId")?.Value ?? Path.GetFileNameWithoutExtension(baselinePath);
            report.AppendLine($"=== {studyId} ===");

            // Compare scenarios
            differences += CompareElements(baseline, newResults, "ScenarioResults", "name", report);

            // Compare alternatives
            differences += CompareElements(baseline, newResults, "AlternativeResults", "name", report);

            // Compare stage damage
            differences += CompareElements(baseline, newResults, "StageDamage", "name", report);

            // Compare alternative comparison reports
            differences += CompareElements(baseline, newResults, "AlternativeComparisonReport", "name", report);

            if (differences == 0)
            {
                report.AppendLine("  All values match within tolerance.");
            }
            report.AppendLine();
        }
        catch (Exception ex)
        {
            report.AppendLine($"  ERROR: {ex.Message}");
            differences++;
        }

        return differences;
    }

    private int CompareElements(XElement baseline, XElement newResults, string elementType, string nameAttribute, StringBuilder report)
    {
        int differences = 0;

        IEnumerable<XElement> baselineElements = baseline.Elements(elementType);
        IEnumerable<XElement> newElements = newResults.Elements(elementType);

        foreach (XElement baselineElem in baselineElements)
        {
            string name = baselineElem.Attribute(nameAttribute)?.Value ?? "unknown";
            XElement? newElem = newElements.FirstOrDefault(e => e.Attribute(nameAttribute)?.Value == name);

            if (newElem == null)
            {
                report.AppendLine($"  MISSING: {elementType} '{name}' not in new results");
                differences++;
                continue;
            }

            // Compare numeric attributes
            List<(string path, double baseline, double newVal)> numericDiffs = new();
            CompareNumericValues(baselineElem, newElem, "", numericDiffs);

            foreach ((string path, double baselineVal, double newVal) in numericDiffs)
            {
                double absDiff = Math.Abs(baselineVal - newVal);
                double relDiff = baselineVal != 0 ? absDiff / Math.Abs(baselineVal) : absDiff;

                if (relDiff > _tolerance && absDiff > 1.0) // At least $1 difference
                {
                    differences++;
                    report.AppendLine($"  DIFF: {elementType} '{name}' {path}");
                    report.AppendLine($"        Baseline: {baselineVal:F4}");
                    report.AppendLine($"        New:      {newVal:F4}");
                    report.AppendLine($"        Diff:     {absDiff:F4} ({relDiff:P2})");
                }
            }
        }

        // Check for elements in new that aren't in baseline
        foreach (XElement newElem in newElements)
        {
            string name = newElem.Attribute(nameAttribute)?.Value ?? "unknown";
            XElement? baselineElem = baselineElements.FirstOrDefault(e => e.Attribute(nameAttribute)?.Value == name);

            if (baselineElem == null)
            {
                report.AppendLine($"  NEW: {elementType} '{name}' only in new results");
            }
        }

        return differences;
    }

    private void CompareNumericValues(XElement baseline, XElement newElem, string path, List<(string, double, double)> diffs)
    {
        // Compare attributes
        foreach (XAttribute attr in baseline.Attributes())
        {
            if (double.TryParse(attr.Value, out double baselineVal))
            {
                XAttribute? newAttr = newElem.Attribute(attr.Name);
                if (newAttr != null && double.TryParse(newAttr.Value, out double newVal))
                {
                    string attrPath = string.IsNullOrEmpty(path) ? $"@{attr.Name}" : $"{path}/@{attr.Name}";
                    diffs.Add((attrPath, baselineVal, newVal));
                }
            }
        }

        // Compare child elements recursively
        foreach (XElement baselineChild in baseline.Elements())
        {
            string childName = baselineChild.Name.LocalName;

            // Try to find matching child by element name and any identifying attributes
            XElement? matchingChild = FindMatchingChild(newElem, baselineChild);

            if (matchingChild != null)
            {
                string childPath = string.IsNullOrEmpty(path) ? childName : $"{path}/{childName}";

                // Add identifying attributes to path if present
                string? id = baselineChild.Attribute("id")?.Value ?? baselineChild.Attribute("name")?.Value;
                if (id != null)
                {
                    childPath += $"[{id}]";
                }

                CompareNumericValues(baselineChild, matchingChild, childPath, diffs);
            }
        }
    }

    private static XElement? FindMatchingChild(XElement parent, XElement target)
    {
        string targetName = target.Name.LocalName;
        IEnumerable<XElement> candidates = parent.Elements(targetName);

        // Try to match by id or name attribute
        string? targetId = target.Attribute("id")?.Value;
        string? targetNameAttr = target.Attribute("name")?.Value;

        if (targetId != null)
        {
            return candidates.FirstOrDefault(c => c.Attribute("id")?.Value == targetId);
        }

        if (targetNameAttr != null)
        {
            return candidates.FirstOrDefault(c => c.Attribute("name")?.Value == targetNameAttr);
        }

        // If no identifying attributes, just take the first one with the same name
        return candidates.FirstOrDefault();
    }
}
