using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Statistics
{
    internal class ConvergeCriteria : IConvergenceCriteria
    {
        public IRange<int> TestRange { get; }
        public double Quantile { get; }
        public double Tolerance { get; }
        //public string TestRange => PrintTestRange();
        public int MinSampleSize { get; }

        internal ConvergeCriteria(double quantile, double tolerance, int minNewObservations, IRange<int> testRange)
        {
            if (testRange.IsNull())
            {
                testRange = IRangeFactory.Factory(1000, 100000);
            }
            Quantile = quantile;
            Tolerance = tolerance;
            TestRange = testRange;
            MinSampleSize = minNewObservations;
        }   
        public IConvergenceResult Test(double qValueBefore, double qValueAfter, int nSample, int nTotal)
        {
            double deviation = (qValueAfter - qValueBefore) / qValueBefore;
            bool isBigEnoughSample = nSample >= MinSampleSize;
            bool isEnoughObservations = nTotal >= TestRange.Min;
            bool isConvergedBySampleSize = nTotal > TestRange.Max;
            bool isConvergedByCalculation = deviation < Tolerance;
            bool isConverged =
                isConvergedBySampleSize ||
                isEnoughObservations && isBigEnoughSample && isConvergedByCalculation;
            return IConvergenceResultFactory.Factory(isConverged, IMessageFactory.Factory(IMessageLevels.Message, PrintIsConverged(isConverged, nTotal) + ".", PrintTestResult(isConverged, nSample, isBigEnoughSample, nTotal, isEnoughObservations, isConvergedBySampleSize, deviation, isConvergedByCalculation)));
        }        
        private string PrintTestResult(bool isConverged, int nSample, bool isBigEnoughSample, int nTotal, bool isEnoughObservations, bool isConvergedBySampleSize, double deviation, bool isConvergedByCalculation)
        {
            return $"{PrintIsConverged(isConverged, nTotal)} with {nSample} new sample observations. {PrintTestResultReason(nSample, isBigEnoughSample, nTotal, isEnoughObservations, isConvergedBySampleSize, deviation, isConvergedByCalculation)}";
        }
        private string PrintIsConverged(bool isConverged, int nTotal) => isConverged ? $"Converged at {nTotal} observations" : $"Convergence not reached at {nTotal} observations";
        private string PrintTestResultReason(int nSample, bool isBigEnoughSample, int nTotal, bool isEnoughObservations, bool isConvergedBySampleSize, double deviation, bool isConvergedByCalculation)
        {
            if (!isBigEnoughSample) return $"The number of new observations being tested: {nSample} is below the minimum required number new observations: {MinSampleSize} needed to retest convergence.";
            if (!isEnoughObservations) return $"The total number of observations: {nTotal} is below the minimum required: {TestRange.Max} observations needed to test for convergence.";
            if (isConvergedBySampleSize) return $"The total number of observations: {nTotal} surpassed the minimum at which convergence is assumed: {TestRange.Max}.";
            if (isConvergedByCalculation) return $"The new observations caused the {Quantile.Print()} value to deviate by: {(deviation * 100).Print()} percent, within the specified tolerance of: {(Tolerance * 100).Print()} percent.";
            else return $"The new observations caused the {Quantile.Print()} value to deviate by: {(deviation * 100).Print()} percent, outside the specified tolerance of: {(Tolerance * 100).Print()} percent.";
        }
        
        public string Print() => $"Convergence Criteria(quantile: {Quantile}, tolerance: {Tolerance}, test range: {TestRange})";    
    }
}
