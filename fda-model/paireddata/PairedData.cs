using HEC.MVVMFramework.Base.Implementations;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paireddata
{
    public class PairedData : HEC.MVVMFramework.Base.Implementations.Validation, IPairedData
    {
        private CurveMetaData _metadata;
        public double[] Xvals { get; }
        public double[] Yvals { get; private set; }
        [Obsolete("Lets deprecate this and just access info through the metadata object")]
        public string DamageCategory
        {
            get { return _metadata.DamageCategory; }
        }
        public CurveMetaData CurveMetaData
        {
            get
            {
                return _metadata;
            }
        }
        public PairedData(double[] xs, double[] ys)
        {
            Xvals = xs;
            Yvals = ys;
            _metadata = new CurveMetaData("default");
            AddRules();
        }
        public PairedData(double[] xs, double[] ys, CurveMetaData metadata)
        {
            _metadata = metadata;
            Xvals = xs;
            Yvals = ys;
            AddRules();
        }
        private void AddRules()
        {
            switch (_metadata.CurveType)
            {
                case CurveTypesEnum.StrictlyMonotonicallyIncreasing:
                    AddSinglePropertyRule(nameof(Xvals), new Rule(() => IsArrayValid(Xvals, (a, b) => (a >= b)), "X must be strictly monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsArrayValid(Yvals, (a, b) => (a >= b)), "Y must be strictly monotonically increasing"));
                    break;
                case CurveTypesEnum.MonotonicallyIncreasing:
                    AddSinglePropertyRule(nameof(Xvals), new Rule(() => IsArrayValid(Xvals, (a, b) => (a > b)), "X must be monotonically increasing"));
                    AddSinglePropertyRule(nameof(Yvals), new Rule(() => IsArrayValid(Yvals, (a, b) => (a > b)), "Y must be monotonically increasing"));
                    break;
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
            Int32 index = Array.BinarySearch(Xvals, x);
            if (index >= 0)
            {
                //Matches a value exactly
                return Yvals[index];
            }
            else
            {
                //This is the next LARGER value.
                index = ~index;

                if (index == Xvals.Count()) { return Yvals[Xvals.Length - 1]; }

                if (index == 0) { return Yvals[0]; }

                //Ok. Interpolate Y=mx+b
                double yIndexMinus1 = Yvals[index - 1];
                double xIndexMinus1 = Xvals[index - 1];
                double m = (Yvals[index] - yIndexMinus1) / (Xvals[index] - xIndexMinus1);
                double b = yIndexMinus1;
                double dx = x - xIndexMinus1;
                return m * dx + b;
            }
        }
        /// <summary>
        /// f_inverse implements ISample on PairedData, for a given input double y f_inverse produces an output double that represents the linearly interoplated value for x given y.
        /// </summary>
        public double f_inverse(double y)
        {
            //binary search.
            double[] yvalsArray = Yvals;
            Int32 index = Array.BinarySearch(yvalsArray, y);
            if (index >= 0)
            {
                //Matches a value exactly
                return Xvals[index];
            }
            else
            {
                //This is the next LARGER value.
                index = ~index;

                if (index == Yvals.Count()) { return Xvals[Xvals.Length - 1]; }

                if (index == 0) { return Xvals[0]; }

                //Ok. Interpolate Y=mx+b
                double m = (Yvals[index] - Yvals[index - 1]) / (Xvals[index] - Xvals[index - 1]);
                double b = Xvals[index - 1];
                double dy = y - Yvals[index - 1];
                return (dy / m) + b;//not sure this is right. Need to develop tests.
            }
        }
        /// <summary>
        /// compose implements the IComposable interface on PairedData, which allows a PairedData object to take the input y values as the x value (to determine the commensurate y value) from the subject function. Ultimately it creates a composed function with the Y from the subject, and the commensurate x from the input.
        /// </summary>
        public IPairedData compose(IPairedData inputPairedData)
        {
            int count = inputPairedData.Xvals.Length;
            double[] x = new double[count];
            double[] y = new double[count];

            for (int i = 0; i < count; i++)
            {
                y[i] = f(inputPairedData.Yvals[i]);
                x[i] = inputPairedData.Xvals[i];
            }
            return new PairedData(x, y);
        }

        public PairedData SumYsForGivenX(IPairedData inputPairedData)
        {
            if (Xvals != null && Yvals != null)
            {

                double[] x = new double[inputPairedData.Xvals.Length];
                double[] ySum = new double[inputPairedData.Yvals.Length];
                for (int i = 0; i < inputPairedData.Xvals.Length; i++)
                {
                    int index = Array.BinarySearch(Xvals, inputPairedData.Xvals[i]);
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
                            yValueFromSubject = intercept + slope * inputPairedData.Xvals[i];
                        }
                    }
                    ySum[i] = inputPairedData.Yvals[i] + yValueFromSubject;
                    x[i] = inputPairedData.Xvals[i];
                }
                return new PairedData(x, ySum);
            }
            else
            {
                return new PairedData(inputPairedData.Xvals, inputPairedData.Yvals);
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
        public IPairedData multiply(IPairedData systemResponseFunction)
        {
            double belowFragilityCurveValue = 0.0;
            double aboveFragilityCurveValue = 1.0;
            List<double> newXvals = new List<double>();
            List<double> newYvals = new List<double>();
            double buffer = .001; //buffer to define point just above and just below the multiplying curve.
            if (Xvals[0] < systemResponseFunction.Xvals[0])
            {
                //cacluate no damage until the bottom of the fragility curve
                double bottomStageOfSystemResponse = systemResponseFunction.Xvals[0];
                foreach (double damageValue in Xvals)
                {
                    if (damageValue < bottomStageOfSystemResponse)
                    {
                        //set to zero
                        newXvals.Add(damageValue);
                        newYvals.Add(belowFragilityCurveValue);
                    }
                    else
                    {
                        //create a point on the curve just below the bottom of the levee at damage zero.
                        newXvals.Add(bottomStageOfSystemResponse - buffer);
                        newYvals.Add(belowFragilityCurveValue);
                        break;
                    }
                }
            }
            // calculate damages for the range of the fragility curve
            for (int index = 0; index < systemResponseFunction.Xvals.Count(); index++)
            {
                //modify
                double stageOnSystemResponse = systemResponseFunction.Xvals[index];
                double probabilityWeightedDamage = f(stageOnSystemResponse) * systemResponseFunction.Yvals[index];
                newXvals.Add(stageOnSystemResponse);
                newYvals.Add(probabilityWeightedDamage);
            }
            // calculate damages above the fragility curve
            if (systemResponseFunction.Xvals.Last() < Xvals.Last())
            {
                double topStageOfSystemResponseFunction = systemResponseFunction.Xvals.Last();
                //create a point at the top of the fragility curve
                newXvals.Add(topStageOfSystemResponseFunction + buffer);
                double damageabove = f(topStageOfSystemResponseFunction + buffer) * aboveFragilityCurveValue;
                newYvals.Add(damageabove);
                for (int idx = 0; idx < Xvals.Count(); idx++)
                {
                    double dcx = Xvals[idx];
                    if (dcx > topStageOfSystemResponseFunction)
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
            int index = 0;
            foreach (double yVal in Yvals)
            {
                ydists[index] = new Deterministic(yVal);
                index++;
            }
            return new UncertainPairedData(Xvals, ydists, _metadata);
        }

        public void ForceMonotonic(double max = double.MaxValue, double min = double.MinValue)
        {
            double previousYval = min;

            double[] update = new double[Yvals.Length];
            int index = 0;
            foreach (double currentY in Yvals)
            {
                if (previousYval > currentY)
                {
                    update[index] = previousYval;
                }
                else
                {
                    if (currentY > max)
                    {
                        update[index] = max;
                        previousYval = max;
                    }
                    else
                    {
                        update[index] = currentY;
                        previousYval = currentY;
                    }
                }
                index++;
            }
            Yvals = update;
        }
    }
}