using System;
using System.Collections.Generic;
using FdaModel.Utilities.Attributes;
using FdaModel.Utilities.Messager;

namespace FdaModel.Functions.OrdinatesFunctions
{
    [Author("John Kucharski", "10/11/2016")]
    public class UncertainOrdinatesFunction : BaseFunction
    {

        #region Fields
        private Statistics.UncertainCurveIncreasing _UncertainFunction;
        #endregion
        

        #region Properties
        public Statistics.UncertainCurveIncreasing UncertainFunction
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
        public UncertainOrdinatesFunction(Statistics.UncertainCurveIncreasing function, FunctionTypes functionType): base( )
        {
            Messaging.MessageHub.Register(this);
            ReportMessage(this, new Messaging.MessageEventArgs( new Messaging.Message("UncertainOrdinatesFunction being created")));
            Messages = new ModelErrors();
            FunctionType = functionType;
            UncertainFunction = function;
        }

        public UncertainOrdinatesFunction(float[] xValues, double[]yMeans, double[] yStandardDeviations, FunctionTypes functionType): base( )
        {
            Messages = new ModelErrors( );
            FunctionType = functionType;
            List<Statistics.Normal> yDistributions = new List<Statistics.Normal>();
            List<double> xVals = new List<double>();
            for (int i = 0; i < xValues.Length; i++)
            {
                yDistributions.Add(new Statistics.Normal(yMeans[i], yStandardDeviations[i]));
                xVals.Add(xValues[i]);
            }
            UncertainFunction = new Statistics.UncertainCurveIncreasing(xVals.ToArray(), yDistributions.ToArray(),true, true,Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal);
        }

        public UncertainOrdinatesFunction(float[] xValues, double[]yMin, double[]yMax, double[] yMostLikely, FunctionTypes functionType): base( )
        {
            Messages = new ModelErrors( );
            FunctionType = FunctionType;
            List<Statistics.Triangular> yDistributions = new List<Statistics.Triangular>();
            List<double> xVals = new List<double>();
            for (int i = 0; i < xValues.Length; i++)
            {
                yDistributions.Add(new Statistics.Triangular(yMin[i], yMax[i], yMostLikely[i]));
                xVals.Add(xValues[i]);
            }
            UncertainFunction = new Statistics.UncertainCurveIncreasing(xVals.ToArray(), yDistributions.ToArray(), true, true, Statistics.UncertainCurveDataCollection.DistributionsEnum.Triangular);
        }
        #endregion


        #region Functions
        public override void Validate()
        {
            throw new NotImplementedException();
        }

        public override BaseFunction SampleFunction(Random randomNumberGenerator)
        {
            Statistics.CurveDataCollection sampledFunction = UncertainFunction.CurveSample(randomNumberGenerator.NextDouble());

            double[] xs = new double[sampledFunction.Count];
            double[] ys = new double[sampledFunction.Count];
            for (int i = 0; i < sampledFunction.Count; i++)
            {
                xs[i] = sampledFunction.get_X(i);
                ys[i] = sampledFunction.get_Y(i);
            }
            return new OrdinatesFunction(xs, ys, FunctionType);
        }

        public override OrdinatesFunction GetOrdinatesFunction( )
        {
            double[ ] xs = new double[UncertainFunction.Count];
            double[ ] ys = new double[UncertainFunction.Count];

            for(int i = 0; i < UncertainFunction.Count; i++)
            {
                xs[i] = UncertainFunction.get_X(i);
                ys[i] = UncertainFunction.get_Y(i).GetCentralTendency;
            }
            return new OrdinatesFunction(xs, ys, this.FunctionType);
        }

        public override OrdinatesFunction Compose(OrdinatesFunction YsFunction, ref List<ErrorMessage> errors)
        {
            throw new NotImplementedException();
        }

        public override double GetXfromY(double Y, ref List<ErrorMessage> errors)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}