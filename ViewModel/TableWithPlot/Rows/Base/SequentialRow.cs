using Statistics;
using Base.Enumerations;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using Implementations = ViewModel.Implementations ;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows.Base
{
    public abstract class SequentialRow : Implementations.ValidatingBaseViewModel
    {
        private double _x;
        [DisplayAsColumn("X Value",0)]
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyPropertyChanged();
            }

        }
        protected abstract List<string> YMinProperties { get; }
        protected abstract List<string> YMaxProperties { get; }

        public IDistribution Y { get; set; }
        public SequentialRow PreviousRow { get; set; }
        public SequentialRow NextRow { get; set; }
        public SequentialRow(double x, IDistribution y, bool isMonotonicallyIncreasing = true)
        {
            X = x;
            Y = y;
            if (isMonotonicallyIncreasing)
            {
                AddSinglePropertyRule(nameof(X), new Rule(() => { if (PreviousRow == null) return true; return X > PreviousRow.X; }, "X values are not increasing.", ErrorLevel.Severe));
                AddSinglePropertyRule(nameof(X), new Rule(() => { if (NextRow == null) return true; return X < NextRow.X; }, "X values are not increasing.", ErrorLevel.Severe));
            }
            else
            {
                AddSinglePropertyRule(nameof(X), new Rule(() => { if (NextRow == null) return true; return X > NextRow.X; }, "X values are not decreasing.", ErrorLevel.Severe));
                AddSinglePropertyRule(nameof(X), new Rule(() => { if (PreviousRow == null) return true; return X < PreviousRow.X; }, "X values are not decreasing.", ErrorLevel.Severe));
            }

        }

        public void SetGlobalMaxRules(double xMax , double yMax , double xMin, double yMin)
        {
            AddSinglePropertyRule(nameof(X), new Rule(() => { return X <= xMax; }, "X was greater than the maximum allowed value", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(X), new Rule(() => { return X >= xMin; }, "X was smaller than the minimum allowed value", ErrorLevel.Severe));
            foreach(string propName in YMinProperties)
            {
                AddSinglePropertyRule(propName, new Rule(() => { return Y.InverseCDF(.999999) >= yMin; }, "y was smaller than the minimum allowed value", ErrorLevel.Severe));
            }
            foreach (string propName in YMaxProperties)
            {
                AddSinglePropertyRule(propName, new Rule(() => { return Y.InverseCDF(.000001) <= yMax; }, "Y was greater than the maximum allowed value", ErrorLevel.Severe));
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
                return (cExtreme < nExtreme);
            }

            else if (NextRow == null && PreviousRow != null)
            {
                pExtreme = PreviousRow.Y.InverseCDF(pval);
                cExtreme = Y.InverseCDF(pval);
                return (pExtreme < cExtreme);
            }

            else if (NextRow == null && PreviousRow == null)
            {
                return true;
            }

            pExtreme = PreviousRow.Y.InverseCDF(pval);
            cExtreme = Y.InverseCDF(pval);
            nExtreme = NextRow.Y.InverseCDF(pval);
            return ((pExtreme < cExtreme) && (cExtreme < nExtreme));
        }
    }
}
