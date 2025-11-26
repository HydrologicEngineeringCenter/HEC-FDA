using System;
using Utility.Progress;

namespace HEC.FDA.Model.utilities;
public static class ProgressReporterMessage
{
    private const int INDENT_SIZE = 6;

    public static void ReportTimestampedMessage(this ProgressReporter reporter,
        TimeSpan? elapsed, int indentLevel, string message, string timeFormat = @"hh\:mm\:ss\.fff")
    {
        var timestamp = elapsed?.ToString(timeFormat) ?? new string(' ', 12);
        var indent = new string(' ', indentLevel * INDENT_SIZE);
        reporter.ReportMessage($"{timestamp}{indent}{message}");
    }
}
