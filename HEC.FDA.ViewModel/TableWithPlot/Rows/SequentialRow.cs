using Statistics;
using System.Collections.Generic;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Implementations;
using Statistics.Distributions;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    public abstract class SequentialRow : MVVMFramework.ViewModel.Implementations.ValidatingBaseViewModel
    {
        private const string XDecreasingErrorMessage = "X values are not decreasing.";
        private const string XIncreasingErrorMessage = "X values are not increasing.";
        private const string XMaxErrorMessage = "X was greater than the maximum allowed value ";
        private const string XMinErrorMessage = "X was smaller than the minimum allowed value ";
        private const string YMinErrorMessage = "The .001 percentile of the Y distribution is less than the minimum allowable value of ";
        private const string YMaxErrorMessage = "The 99.999 percentile of the Y distribution is greater than the maximum allowable value of ";

        public abstract void UpdateRow(int col, double x);
        protected abstract List<string> YMinProperties { get; }
        protected abstract List<string> YMaxProperties { get; }
        public abstract double X { get; set; }
        public IDistribution Y { get; set; }
        public double ZScore { get { return Normal.StandardNormalInverseCDF(X); } }
        public SequentialRow PreviousRow { get; set; }
        public SequentialRow NextRow { get; set; }
        public bool IsStrictMonotonic { get; set; }
        public SequentialRow(double x, IDistribution y, bool isStrictMonotonic = false, bool xIsDecreasing = false)
        {
            X = x;
            Y = y;
            IsStrictMonotonic = isStrictMonotonic;
            if (xIsDecreasing)
            {
                if (isStrictMonotonic)
                {
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (PreviousRow == null) return true; return X > PreviousRow.X; }, XDecreasingErrorMessage, ErrorLevel.Severe));
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (NextRow == null) return true; return X < NextRow.X; }, XDecreasingErrorMessage, ErrorLevel.Severe));
                }
                else
                {
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (NextRow == null) return true; return X >= NextRow.X; }, XDecreasingErrorMessage, ErrorLevel.Severe));
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (PreviousRow == null) return true; return X <= PreviousRow.X; }, XDecreasingErrorMessage, ErrorLevel.Severe));
                }
            }
            else
            {
                if (isStrictMonotonic)
                {
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (PreviousRow == null) return true; return X < PreviousRow.X; }, XIncreasingErrorMessage, ErrorLevel.Severe));
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (NextRow == null) return true; return X > NextRow.X; }, XIncreasingErrorMessage, ErrorLevel.Severe));
                }
                else
                {
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (NextRow == null) return true; return X <= NextRow.X; }, XIncreasingErrorMessage, ErrorLevel.Severe));
                    AddSinglePropertyRule(nameof(X), new Rule(() => { if (PreviousRow == null) return true; return X >= PreviousRow.X; }, XIncreasingErrorMessage, ErrorLevel.Severe));
                }
            }
        }

        public void SetGlobalMaxRules(double xMax, double yMax, double xMin, double yMin)
        {
            double maxProbability = .999999;
            double minProbability = .000001;
            AddSinglePropertyRule(nameof(X), new Rule(() => { return X <= xMax; }, XMaxErrorMessage + xMax, ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(X), new Rule(() => { return X >= xMin; }, XMinErrorMessage + xMin, ErrorLevel.Severe));
            foreach (string propName in YMinProperties)
            {
                AddSinglePropertyRule(propName, new Rule(() => { return Y.InverseCDF(minProbability) >= yMin; }, YMinErrorMessage + yMin, ErrorLevel.Severe));
            }
            foreach (string propName in YMaxProperties)
            {
                AddSinglePropertyRule(propName, new Rule(() => { return Y.InverseCDF(maxProbability) <= yMax; }, YMaxErrorMessage + yMax, ErrorLevel.Severe));
            }

        }
        protected bool CheckNormalDistExtremes(double pval)
        {
            double pExtreme;
            double cExtreme;
            double nExtreme;

            if (PreviousRow == null && NextRow != null)
            {
                cExtreme = Y.InverseCDF(pval);
                nExtreme = NextRow.Y.InverseCDF(pval);
                if (IsStrictMonotonic)
                {
                    return (cExtreme < nExtreme);
                }
                else
                {
                    return (cExtreme <= nExtreme);
                }
            }

            else if (NextRow == null && PreviousRow != null)
            {
                pExtreme = PreviousRow.Y.InverseCDF(pval);
                cExtreme = Y.InverseCDF(pval);
                if (IsStrictMonotonic)
                {
                    return (pExtreme < cExtreme);
                }
                else
                {
                    return (pExtreme <= cExtreme);
                }

            }

            else if (NextRow == null && PreviousRow == null)
            {
                return true;
            }

            pExtreme = PreviousRow.Y.InverseCDF(pval);
            cExtreme = Y.InverseCDF(pval);
            nExtreme = NextRow.Y.InverseCDF(pval);
            if (IsStrictMonotonic)
            {
                return ((pExtreme < cExtreme) && (cExtreme < nExtreme));
            }
            else
            {
                return ((pExtreme <= cExtreme) && (cExtreme <= nExtreme));
            }
        }
    }
}
