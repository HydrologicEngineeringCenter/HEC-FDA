using System;
using System.Collections.Generic;

namespace HEC.FDA.ViewModel.Reporting;

public record ReportInputState(string Name, string LastEditDate, bool IsDirty = false, bool WasComputed = false);

/// <summary>Uses FDA's existing edit/compute dates and dirty node signal. Null means reuse.</summary>
public static class ReportFreshness
{
    public static string GetComputeReason(bool hasResults, string lastEditDate, string computeDate,
        bool isDirty, IEnumerable<ReportInputState> inputs)
    {
        if (!hasResults) return "no complete saved results";
        if (isDirty) return "the component node is dirty";
        if (!DateTime.TryParse(computeDate, out DateTime computed))
            return "the last compute date is unavailable";
        if (!DateTime.TryParse(lastEditDate, out DateTime edited))
            return "the last edit date is unavailable";
        if (edited > computed) return "the component was edited since its last compute";
        foreach (var input in inputs)
        {
            if (input.WasComputed) return $"'{input.Name}' was computed during this report run";
            if (input.IsDirty) return $"input '{input.Name}' is dirty";
            if (!DateTime.TryParse(input.LastEditDate, out DateTime inputEdited))
                return $"the edit date for input '{input.Name}' is unavailable";
            if (inputEdited > computed) return $"input '{input.Name}' changed since the last compute";
        }
        return null;
    }
}
