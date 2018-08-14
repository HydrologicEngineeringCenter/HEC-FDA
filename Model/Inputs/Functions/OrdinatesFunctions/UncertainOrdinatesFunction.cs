using System;
using System.Collections.Generic;
using FdaModel.Utilities.Attributes;

namespace FdaModel.Inputs.Functions.OrdinatesFunctions
{
    [Author("John Kucharski", "10/11/2016")]
    public class UncertainOrdinatesFunction : OrdinatesFunction
    {

        #region Fields
        private Statistics.MonotonicCurveUSingle _UncertainFunction;
        #endregion
        

        #region Properties
        public Statistics.MonotonicCurveUSingle UncertainFunction
        {
            get
            {
                return _UncertainFunction;
            }
            set
            {
                _UncertainFunction = value;
            }
        }
        #endregion

        #region Constructors
        public UncertainOrdinatesFunction(ComputationPoint indexPoint, float[] xValues, double[]yMeans, double[] yStandardDeviations, FunctionTypes functionType): base(indexPoint, functionType)
        {
            List<Statistics.Normal> yDistributions = new List<Statistics.Normal>();
            for (int i = 0; i < xValues.Length; i++)
            {
                yDistributions.Add(new Statistics.Normal(yMeans[i], yStandardDeviations[i]));
            }
            UncertainFunction = new Statistics.MonotonicCurveUSingle(xValues, yDistributions.ToArray());
        }

        public UncertainOrdinatesFunction(ComputationPoint indexPoint, float[] xValues, double[]yMin, double[]yMax, double[] yMostLikely, FunctionTypes functionType): base(indexPoint, functionType)
        {
            List<Statistics.Triangular> yDistributions = new List<Statistics.Triangular>();
            for (int i = 0; i < xValues.Length; i++)
            {
                yDistributions.Add(new Statistics.Triangular(yMin[i], yMax[i], yMostLikely[i]));
            }
            UncertainFunction = new Statistics.MonotonicCurveUSingle(xValues, yDistributions.ToArray());
        }
        #endregion

        #region Functions
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        public override BaseFunction SampleFunction(Random randomNumberGenerator)
        {
            Statistics.MonotonicCurveIncreasing sampledFunction = UncertainFunction.CurveSample(randomNumberGenerator.NextDouble());
            return OrdinatesFunctionFactory(IndexPoint, sampledFunction.X.ToArray(), sampledFunction.Y.ToArray(), FunctionType);
        }
        #endregion

    }
}