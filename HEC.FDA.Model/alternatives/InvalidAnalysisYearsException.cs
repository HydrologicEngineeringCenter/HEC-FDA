using System;

namespace HEC.FDA.Model.alternatives
{
    /// <summary>
    /// Thrown when an alternative's analysis years cannot be discounted over the study's period of analysis.
    /// Distinct from ArgumentException so callers can surface this to the user as a validation problem without
    /// also swallowing genuine argument defects thrown from inside the compute.
    /// </summary>
    public class InvalidAnalysisYearsException : Exception
    {
        public InvalidAnalysisYearsException(string message) : base(message)
        {
        }
    }
}
