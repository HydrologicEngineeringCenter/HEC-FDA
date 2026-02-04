using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HEC.FDA.Model.paireddata
{
    public class PairedData : IPairedData
    {
        #region Fields 
        private readonly double[] _xVals;
        private readonly double[] _yVals;
        #endregion

        #region Properties 
        public CurveMetaData MetaData { get; }
        public IReadOnlyList<double> Xvals => _xVals;
        public IReadOnlyList<double> Yvals => _yVals;
        #endregion

        #region Constructors 
        /// <summary>
        /// WARNING: Paired data allows X vals in any order, but many methods require that they be sorted in ascending order
        /// X values must always be in increasing order.
        /// X values are the independent variable, and Y values are the dependent variable.
        /// Common paired data relationships in FDA follow these conventions:
        /// System Response :  stages, probability of fail
        /// Stage Freuqency : exceedance probabilities increasing, stages 
        /// Flow Frequency : exceedance probabilities increasing, flows
        /// Stage Damage : stages, damages
        /// Damage Frequency : damage, probabilities
        /// Unreg Regulated: flow, flow
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        public PairedData(IReadOnlyList<double> xs, IReadOnlyList<double> ys, CurveMetaData metadata = null)
        {
            _xVals = xs == null ? null : xs.ToArray();
            _yVals = ys == null ? null : ys.ToArray();
            MetaData = metadata;
        }
        #endregion

        #region Methods
        /// <summary>
        /// f implements ISample on PairedData, for a given input double x f produces an output double that represents the linearly interoplated value for y given x.
        /// Uses binary search to find the closest x value in the array, then interpolates between the two closest x values to find the y value.
        /// Requires x vals to be sorted in increasing order.
        /// </summary>
        public double f(double x)
        {
            if (_xVals[0] > _xVals[^1])
            {
                throw new ArgumentException("X values must be in increasing order.");
            }
            int index = Array.BinarySearch(_xVals, x);
            if (index >= 0)
            {
                //Matches a value exactly
                return Yvals[index];
            }
            else
            {
                //This is the next LARGER value.
                index = ~index;
                int len = Xvals.Count;
                if (index == len) return Yvals[len - 1];

                if (index == 0) return Yvals[0];

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
        /// Created to provide a method for searching paired data without using binary search. 
        /// </summary>
        public double f(double x, ref int indexOfPreviousTopOfSegment)
        {
            if (_xVals[0] > _xVals[^1])
            {
                throw new ArgumentException("X values must be in increasing order.");
            }
            //We're above the curve
            if (x > Xvals[^1])
            {
                return Yvals[^1];
            }
            //We're below the curve
            if (x < Xvals[0])
            {
                return 0;
            }
            //If we've got it right on
            if (Xvals[indexOfPreviousTopOfSegment] == x)
            {
                return Yvals[indexOfPreviousTopOfSegment];
            }
            //We're on the same segment
            if (x < Xvals[indexOfPreviousTopOfSegment])
            {
                return InterpolateYs(x, indexOfPreviousTopOfSegment);
            }
            else // x is greater than the last top of segment
            {
                while (x > Xvals[indexOfPreviousTopOfSegment])
                {
                    indexOfPreviousTopOfSegment++;
                }
                return InterpolateYs(x, indexOfPreviousTopOfSegment);
            }
        }
        private double InterpolateYs(double x, int index)
        {
            if (_xVals[0] > _xVals[^1])
            {
                throw new ArgumentException("X values must be in increasing order.");
            }
            double yAtIndexMinus1 = Yvals[index - 1];
            double xAtIndexMinus1 = Xvals[index - 1];
            double m = (Yvals[index] - yAtIndexMinus1) / (Xvals[index] - xAtIndexMinus1);
            double b = yAtIndexMinus1;
            double dx = x - xAtIndexMinus1;
            return m * dx + b;
        }

        /// <summary>
        /// f_inverse implements ISample on PairedData, for a given input double y f_inverse produces an output double that represents the linearly interoplated value for x given y.
        /// binary search assumes y to be in increasing order. Will give shitty answers if not. 
        /// </summary>
        public double f_inverse(double y)
        {
            if (_xVals[0] > _xVals[^1])
            {
                throw new ArgumentException("X values must be in increasing order.");
            }
            //binary search.
            double[] yvalsArray = _yVals;
            int index = Array.BinarySearch(yvalsArray, y);
            if (index >= 0)
            {
                //Matches a value exactly
                return Xvals[index];
            }
            else
            {
                //This is the next LARGER value.
                index = ~index;

                if (index == Yvals.Count) { return Xvals[^1]; }

                if (index == 0) { return Xvals[0]; }

                //Ok. Interpolate Y=mx+b
                double m = (Yvals[index] - Yvals[index - 1]) / (Xvals[index] - Xvals[index - 1]);
                double b = Xvals[index - 1];
                double dy = y - Yvals[index - 1];
                return dy / m + b;//not sure this is right. Need to develop tests.
            }
        }
        /// <summary>
        ///PairedData object to take the input y values as the x value (to determine the commensurate y value) from the subject function.
        ///Ultimately it creates a composed function with the Y from the subject, and the commensurate x from the input.
        ///Typical compose patterns:
        ///Stage Damage compose Frequency Stage = Damage Frequency
        ///System Response compose Frequency Stage = Failure Frequency
        ///FlowStageSample compose Frequency Flow = Frequency Stage
        /// </summary>
        public PairedData compose(IPairedData inputPairedData)
        {
            int count = inputPairedData.Xvals.Count;
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
            if (Xvals == null && Yvals == null)
            {
                return new PairedData(inputPairedData.Xvals.ToArray(), inputPairedData.Yvals.ToArray());
            }
            if (_xVals[0] > _xVals[^1])
            {
                throw new ArgumentException("X values must be in increasing order.");
            }
            double[] x = new double[inputPairedData.Xvals.Count];
            double[] ySum = new double[inputPairedData.Yvals.Count];
            for (int i = 0; i < inputPairedData.Xvals.Count; i++)
            {
                int index = Array.BinarySearch(_xVals, inputPairedData.Xvals[i]);
                double yValueFromSubject;
                if (index >= 0)
                {
                    //value matched exactly
                    yValueFromSubject = Yvals[index];

                }
                else
                {
                    index = ~index;

                    //if the x value from input is greater than all x values in subject
                    if (index == Xvals.Count)
                    {
                        yValueFromSubject = Yvals[^1];
                    }

                    //if the x value from the input is less than all x values in subject
                    else if (index == 0)
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


        public double Integrate(bool withPadding = true)
        {
            if (_xVals[0] > _xVals[^1])
            {
                throw new ArgumentException("X values must be in increasing order.");
            }
            if (withPadding)
            {
                return Mathematics.IntegrateCDF<double>(_xVals, _yVals);
            }
            return Mathematics.RealIntegrateTrapezoidal<double>(_xVals, _yVals);
            //This functionality was extracted to a static method to be shared with empirical.cs, which uses the same logic. 

        }

        /// <summary>
        /// Appropriate when subject is a stage damage curve, and the input is a fragility curve. 
        /// Assumes subject has stages as x values, and damage as y values. Assumes input has probability of failure as y values, and stages as x values.
        /// multiply multiplies a stage damage curve by a fragility curve. All damages below the curve are considered 0. 
        /// </summary>
        /// <param name="systemResponseFunction"></param>
        /// <returns> Returns a paired data where x values are stages from the subject, and y vals are damage*prob</returns>
        public PairedData multiply(IPairedData systemResponseFunction)
        {
            List<double> newXvals = new(); //xvals are stages in the stage-damage function
            List<double> newYvals = new(); //yvals are damage*prob(failure)

            // calculate damages for the range of the stage-damage function 
            for (int i = 0; i < Xvals.Count; i++)
            {
                double stageFromStageDamage = Xvals[i];
                double probabilityOfFailure = systemResponseFunction.f(stageFromStageDamage);
                double probabilityWeightedDamage = probabilityOfFailure * Yvals[i];

                newXvals.Add(stageFromStageDamage);
                newYvals.Add(probabilityWeightedDamage);
            }
            for (int i = 0; i < systemResponseFunction.Xvals.Count; i++)
            {
                double fragilityStage = systemResponseFunction.Xvals[i];
                bool fragilityStageIsInStages = newXvals.Contains(fragilityStage);
                if (!fragilityStageIsInStages)
                {
                    double probabilityOfFailure = systemResponseFunction.Yvals[i];
                    double unweightedDamage = f(fragilityStage);
                    double probabilityWeightedDamage = probabilityOfFailure * unweightedDamage;
                    newXvals.Add(fragilityStage);
                    newYvals.Add(probabilityWeightedDamage);
                }
            }
            double[] stages = newXvals.ToArray();
            double[] damages = newYvals.ToArray();
            //This sorts the stages and sorts the damage based on the sorting of the stages
            Array.Sort(stages, damages);
            return new PairedData(stages, damages);
        }

        /// <summary>
        /// weak monotonicity demands that the function must be either flat or increasing, and this is enforced by walking the function from the bottom -> up. Effectively capping the minimum value ot the minimum provided. 
        /// </summary>
        public void ForceWeakMonotonicityBottomUp()
        {
            double previousYval = Yvals[0];
            for (int i = 1; i < Yvals.Count; i++)
            {
                double currentY = Yvals[i];
                if (previousYval >= currentY)
                {
                    _yVals[i] = previousYval;
                }
                else
                {
                    previousYval = currentY;
                }
            }
        }

        /// <summary>
        /// strict monotonicity damands the function be increasing. flat sections are not permitted. this is enforced by walking the function from the top -> down. Effectively capping the maximum value to the max provided.
        /// </summary>
        public void ForceStrictMonotonicityTopDown()
        {
            double upperValue = Yvals[^1];

            for (int i = Yvals.Count - 2; i >= 0; i--)
            {
                if (Yvals[i] >= upperValue)
                {
                    upperValue -= double.Epsilon;
                    _yVals[i] = upperValue;
                }
                else
                {
                    upperValue = Yvals[i];
                }
            }
        }

        /// <summary>
        /// strict monotonicity damands the function be increasing. flat sections are not permitted. this is enforced by walking the function from the bottom -> up. Effectively capping the minimum value ot the minimum provided.
        /// </summary>
        public void ForceStrictMonotonicityBottomUp()
        {
            double previousYval = Yvals[0];
            for (int index = 1; index < Yvals.Count; index++)
            {
                double currentY = Yvals[index];
                if (previousYval >= currentY)
                {
                    previousYval += double.Epsilon;
                    _yVals[index] = previousYval;
                }
                else
                {
                    previousYval = currentY;
                }
            }
        }
        public void SortToIncreasingXVals()
        {
            Array.Sort(_xVals, _yVals);
        }
        #endregion
    }
}