using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    internal class ConvergeCriteria : IConvergenceCriteria
    {
        private readonly Utilities.IRange<int> _TestRange;

        public double Quantile { get; }
        public double Tolerance { get; }
        public string TestRange => PrintTestRange();
        public int MinimumSampleSize { get; }

        internal ConvergeCriteria(double quantile, double tolerance, int minNewObservations, IRange<int> testRange)
        {
            Quantile = quantile;
            Tolerance = tolerance;
            _TestRange = testRange;
            MinimumSampleSize = minNewObservations;
        }
            
        public IConvergenceResult Test(double qValueBefore, double qValueAfter, int nSampleObservations, int nTotalObservations)
        {
            double deviation = (qValueAfter - qValueBefore) / qValueBefore;
            bool isEnoughSampleObs = !(nSampleObservations < MinimumSampleSize),
                 isBelowRange = IsBelowRange(nTotalObservations), 
                 isAboveRange = IsAboveRange(nTotalObservations),  
                 isInTolerance = deviation < Tolerance,
                 passed = isAboveRange || (isEnoughSampleObs && !isBelowRange && !isAboveRange && isInTolerance) ? true : false;
            return IConvergenceResultFactory.Factory(passed, IMessageFactory.Factory(IMessageLevels.Message, PrintTestResult(passed, nSampleObservations, isBelowRange, isAboveRange, nTotalObservations, isInTolerance, deviation)));
        }
        private bool IsBelowRange(int observations) => _TestRange == null || (_TestRange != null && _TestRange.Min <= observations) ? false : true;
        private bool IsAboveRange(int observations) => _TestRange == null || (_TestRange != null && _TestRange.Max >= observations) ? false : true;
        private bool IsOnRange(int observations) => _TestRange == null || Utilities.Validate.IsOnRange(_TestRange.Min, _TestRange.Max, observations) ? true : false;
        private string PrintTestResult(bool passed, int sampleObs, bool isBelowRange, bool isAboveRange, int totalObs, bool isInTolerance, double deviation)
        {
            string result = passed ? "CONVERGED: " : "FAILED to converge: ";
            StringBuilder explaination = new StringBuilder();
            if (sampleObs < MinimumSampleSize) explaination.Append($"{sampleObs} new observations were fit to the distribution, less than the {MinimumSampleSize} required for convergence to be evaluated. ");
            if (isBelowRange) explaination.Append($"{_TestRange.Min - totalObs} fewer than the required minimum {_TestRange.Min} observations for convergence have been amassed. ");
            if (isAboveRange) explaination.Append($"{totalObs} observations, more than the maximum {_TestRange.Max} number of observations before conveyance is assumed have been amassed. ");
            if (isInTolerance)
            {
                if (explaination.Length > 0)
                    explaination.Append($"The distribution fit with the new sample deviated from the original distribution by {deviation * 100} percent at the {Quantile} value, within the tolerance for convergence of less than {Tolerance * 100} percent.");
                else
                    explaination.Append($"the distribution fit with the new sample deviated from the original distribution by {deviation * 100} percent at the {Quantile} value, within the tolerance for convergence of less than {Tolerance * 100} percent.");
            }                
            else
            {
                if (explaination.Length > 0)
                    explaination.Append($"The distribution fit with the new sample deviated from the original distribution by {deviation * 100} percent at the {Quantile} value, outside the tolerance for convergence of less than {Tolerance * 100} percent.");
                else
                    explaination.Append($"the distribution fit with the new sample deviated from the original distribution by {deviation * 100} percent at the {Quantile} value, outside the tolerance for convergence of less than {Tolerance * 100} percent.");
            }
            return result + explaination.ToString();
        }
        
        private string PrintTestRange() => _TestRange == null ? "No test range criteria." : "Test range: " + _TestRange.Print() + ".";
        public string Print() => $"Convergence Criteria(quantile: {Quantile}, tolerance: {Tolerance}, test range: {TestRange})";    
    }
}
