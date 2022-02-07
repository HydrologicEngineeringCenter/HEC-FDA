using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paireddata
{
    public class PairedData : Base.Implementations.Validation, IPairedData
    {
        private CurveMetaData _metadata;
        public double[] Xvals { get; }
        public double[] Yvals { get; }
        public string Category
        {
            get { return _metadata.Category; }
        }
        //sorting Array.Sort(yarr,System.Collections.Comparer.);//make sure that this is ascending 
        //or array.reverse
        //we need the right comparer to sort the right way the first way 
        public PairedData(double[] xs, double[] ys)
        {
            Xvals = xs;
            Yvals = ys;
            //Category = "Default";
            _metadata = new CurveMetaData("default");
            AddRules();
        }
        public PairedData(double[] xs, double[] ys, CurveMetaData metadata)
        {
            _metadata = metadata;
            Xvals = xs;
            Yvals = ys;
            //Category = Category;
            AddRules();
        }
        private void AddRules()
        {
            switch (_metadata.CurveType)
            {
                case CurveTypesEnum.StrictlyMonotonicallyIncreasing:
                    AddSinglePropertyRule(nameof(Xvals), new Base.Implementations.Rule(() => IsArrayValid(Xvals, (a, b) => (a >= b)), "X must be strictly monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Base.Implementations.Rule(() => IsArrayValid(Yvals, (a, b) => (a >= b)), "Y must be strictly monotonically increasing"));
                    break;
                case CurveTypesEnum.MonotonicallyIncreasing:
                    AddSinglePropertyRule(nameof(Xvals), new Base.Implementations.Rule(() => IsArrayValid(Xvals, (a, b) => (a > b)), "X must be monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Base.Implementations.Rule(() => IsArrayValid(Yvals, (a, b) => (a > b)), "Y must be monotonically increasing"));
                    break;
                //case CurveTypesEnum.StrictlyMonotonicallyDecreasing:
                //    AddSinglePropertyRule(nameof(Xvals), new Base.Implementations.Rule(() => IsArrayValid(Xvals, (a, b) => (a >= b)), "X must be strictly monotonically decreasing"));
                //    AddSinglePropertyRule(nameof(Yvals), new Base.Implementations.Rule(() => IsArrayValid(Yvals, (a, b) => (a <= b)), "Y must be strictly monotonically decreasing"));
                //    break;
                //case CurveTypesEnum.MonotonicallyDecreasing:
                //    AddSinglePropertyRule(nameof(Xvals), new Base.Implementations.Rule(() => IsArrayValid(Xvals, (a, b) => (a > b)), "X must be monotonically decreasing"));
                //    AddSinglePropertyRule(nameof(Yvals), new Base.Implementations.Rule(() => IsArrayValid(Yvals, (a, b) => (a < b)), "Y must be monotonically decreasing"));
                //    break;
                default:
                    break;
            }

        }
        private bool IsArrayValid(double[] arrayOfData, Func<double, double, bool> comparison)
        {
            if (arrayOfData == null) return false;
            for (int i = 0; i < arrayOfData.Length - 1; i++)
            {
                if (comparison(arrayOfData[i], arrayOfData[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// f implements ISample on PairedData, for a given input double x f produces an output double that represents the linearly interoplated value for y given x.
        /// </summary>
        public double f(double x)
        {
            //binary search.
            //double[] xarr = Xvals; //probably not necessary anymore.
            Int32 idx = Array.BinarySearch(Xvals, x);
            if (idx >= 0)
            {
                //Matches a value exactly
                return Yvals[idx];
            }
            else
            {
                //This is the next LARGER value.
                idx = ~idx;

                if (idx == Xvals.Count()) { return Yvals[Xvals.Length - 1]; }

                if (idx == 0) { return Yvals[0]; }

                //Ok. Interpolate Y=mx+b
                double yidxminus1 = Yvals[idx - 1];
                double xidxminus1 = Xvals[idx-1];
                double m = (Yvals[idx] - yidxminus1) / (Xvals[idx] - xidxminus1);
                double b = yidxminus1;
                double dx = x - xidxminus1;
                return m * dx + b;
            }
        }
        /// <summary>
        /// f_inverse implements ISample on PairedData, for a given input double y f_inverse produces an output double that represents the linearly interoplated value for x given y.
        /// </summary>
        public double f_inverse(double y)
        {
            //binary search.
            double[] yarr = Yvals;
            Int32 idx = Array.BinarySearch(yarr, y);
            if (idx >= 0)
            {
                //Matches a value exactly
                return Xvals[idx];
            }
            else
            {
                //This is the next LARGER value.
                idx = ~idx;

                if (idx == Yvals.Count()) { return Xvals[Xvals.Length - 1]; }

                if (idx == 0) { return Xvals[0]; }

                //Ok. Interpolate Y=mx+b
                double m = (Yvals[idx] - Yvals[idx - 1]) / (Xvals[idx] - Xvals[idx - 1]);
                double b = Xvals[idx - 1];
                double dy = y - Yvals[idx - 1];
                return (dy / m) + b;//not sure this is right. Need to develop tests.
            }
        }
        /// <summary>
        /// compose implements the IComposable interface on PairedData, which allows a PairedData object to take the input y values as the x value (to determine the commensurate y value) from the subject function. Ultimately it creates a composed function with the Y from the subject, and the commensurate x from the input.
        /// </summary>
        public IPairedData compose(IPairedData input)
        {
            int count = input.Xvals.Length;
            double[] x = new double[count];
            double[] y = new double[count];

            for (int i = 0; i < count; i++)
            {
                y[i] = f(input.Yvals[i]);
                x[i] = input.Xvals[i];
            }
            return new PairedData(x, y);
        }

        public PairedData SumYsForGivenX(IPairedData input)
        {
            if (Xvals != null && Yvals != null)
            {

                double[] x = new double[input.Xvals.Length];
                double[] ySum = new double[input.Yvals.Length];
                for (int i = 0; i < input.Xvals.Length; i++)
                {
                    int index = Array.BinarySearch(Xvals, input.Xvals[i]);
                    double yValueFromSubject = 0;
                    if (index >= 0)
                    {
                        //value matched exactly
                        yValueFromSubject = Yvals[index];

                    }
                    else
                    {
                        index = ~index;

                        //if the x value from input is greater than all x values in subject
                        if (index == Xvals.Length)
                        {
                            yValueFromSubject = Yvals[Yvals.Length - 1];
                        }

                        //if the x value from the input is less than all x values in subject
                        if (index == 0)
                        {
                            yValueFromSubject = Yvals[0];
                        }
                        else //x value from the input is within range of x values in subject, but does not match exactly
                        { //Interpolate Y=mx+b
                            double slope = (Yvals[index] - Yvals[index - 1]) / (Xvals[index] - Xvals[index - 1]);
                            double intercept = Yvals[index] - slope * Xvals[index];
                            yValueFromSubject = intercept + slope * input.Xvals[i];
                        }
                    }
                    ySum[i] = input.Yvals[i] + yValueFromSubject;
                    x[i] = input.Xvals[i];
                }
                return new PairedData(x, ySum);
            }
            else
            {
                return new PairedData(input.Xvals, input.Yvals);
            }
        }

        /// <summary>
        ///Calcualtes the area under the paired data curve across the range of x values using trapizoidal integration. 
        ///Assumes X vals are non-exceedance probabilities increasing from 0. Assumes an additional x ord of 1, and y ordinate equal to the last one in the array.
        /// </summary>
        public double integrate()
        {

            double triangle;
            double square;
            double x1 = 1.0;
            double y1 = 0.0;
            double ead = 0.0;
            for (int i = 0; i < Xvals.Length; i++)
            {
                double xdelta = Xvals[i] - x1;
                square = xdelta * y1;
                triangle = ((xdelta) * (Yvals[i] - y1)) / 2.0;
                ead += square + triangle;
                x1 = Xvals[i];
                y1 = Yvals[i];
            }
            if (x1 != 1)
            {
                double xdelta = 1 - x1;
                ead += xdelta * y1;
            }
            return ead;
        }

        /// <summary>
        /// Appropriate when subject is a stage damage curve, and the input is a fragility curve. 
        /// multiply multiplies a stage damage curve by a fragility curve. All damages below the curve are considered 0.
        /// A point is added just above and just below the curve. 
        /// </summary>
        public IPairedData multiply(IPairedData g)
        {
            double belowFragilityCurveValue = 0.0;
            double aboveFragilityCurveValue = 1.0;
            List<double> newXvals = new List<double>();
            List<double> newYvals = new List<double>();
            double buffer = .001; //buffer to define point just above and just below the multiplying curve.
            if (Xvals[0] < g.Xvals[0])
            {
                //cacluate no damage until the bottom of the fragility curve
                double bottom = g.Xvals[0];
                foreach (double dcx in Xvals)
                {
                    if (dcx < bottom)
                    {
                        //set to zero
                        newXvals.Add(dcx);
                        newYvals.Add(belowFragilityCurveValue);
                    }
                    else
                    {
                        //create a point on the curve just below the bottom of the levee at damage zero.
                        newXvals.Add(bottom - buffer);
                        newYvals.Add(belowFragilityCurveValue);
                        break;
                    }
                }
            }
            // calculate damages for the range of the fragility curve
            for (int idx = 0; idx < g.Xvals.Count(); idx++)
            {
                //modify
                double lcx = g.Xvals[idx];
                double damage = f(lcx) * g.Yvals[idx];
                newXvals.Add(lcx);
                newYvals.Add(damage);
            }
            // calculate damages above the fragility curve
            if (g.Xvals.Last() < Xvals.Last())
            {
                double top = g.Xvals.Last();
                //create a point at the top of the fragility curve
                newXvals.Add(top + buffer);
                double damageabove = f(top + buffer) * aboveFragilityCurveValue;
                newYvals.Add(damageabove);
                for (int idx = 0; idx < Xvals.Count(); idx++)
                {
                    double dcx = Xvals[idx];
                    if (dcx > top)
                    {
                        //set to max val
                        newXvals.Add(dcx);
                        double d = Yvals[idx] * aboveFragilityCurveValue;
                        newYvals.Add(d);
                    }
                }
            }
            return new PairedData(newXvals.ToArray(), newYvals.ToArray());
        }
        public UncertainPairedData toUncertainPairedData()
        {
            IDistribution[] ydists = new IDistribution[Yvals.Length];
            int idx = 0;
            foreach(double val in Yvals)
            {
                ydists[idx] = new Deterministic(val);
                idx++;
            }
            return new UncertainPairedData(Xvals, ydists, _metadata);
        }
    }
}