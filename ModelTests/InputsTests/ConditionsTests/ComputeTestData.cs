using Functions;
using Functions.CoordinatesFunctions;
using Model;
using Model.Functions;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTests.InputsTests.ConditionsTests
{
    [ExcludeFromCodeCoverage]

    public abstract class ComputeTestData
    {
        internal void RegisterSamplers()
        {
            Sampler.RegisterSampler(new ConstantSampler());
            Sampler.RegisterSampler(new DistributionSampler());
            Sampler.RegisterSampler(new LinkedFunctionsSampler());
        }

        internal List<IMetric> CreateMetrics(List<IMetricEnum> types, List<double> values)
        {
            if(types.Count != values.Count)
            {
                throw new Exception("Metric types and values were different lengths.");
            }
            List<IMetric> metrics = new List<IMetric>();
            for(int i = 0;i<types.Count;i++)
            {
                metrics.Add( new Metric(types[i], values[i]));
            }
            return metrics;
        }

        internal IFrequencyFunction CreateFrequencyFunction(List<double> xs, List<double> ys, InterpolationEnum interpolator, IParameterEnum type)
        {
            ICoordinatesFunction lpsCoordFunc = ICoordinatesFunctionsFactory.Factory(xs,ys, interpolator);

            IFunction function = IFunctionFactory.Factory(lpsCoordFunc.Coordinates, lpsCoordFunc.Interpolator);
            return (IFrequencyFunction) IFdaFunctionFactory.Factory(function, type);

        }

        internal ITransformFunction CreateTransformFunction(List<double> xs, List<double> ys, InterpolationEnum interpolator, IParameterEnum type)
        {
            ICoordinatesFunction lpsCoordFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, interpolator);

            IFunction function = IFunctionFactory.Factory(lpsCoordFunc.Coordinates, lpsCoordFunc.Interpolator);
            return (ITransformFunction)IFdaFunctionFactory.Factory(function, type);
        }

        internal static InflowOutflow CreateInflowOutflowFunction()
        {
            List<double> xs = new List<double>()
            {
                1,2,3,4,5
            };
            List<double> ys = new List<double>()
            {
                3,4,5,6,7
            };

            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            IFunction func = IFunctionFactory.Factory(function.Coordinates, function.Interpolator);
            return new InflowOutflow(func, "Inflow Outflow");

        }

        internal static InflowOutflow CreateInflowOutflowFunction(List<double> xs, List<double> ys)
        {

            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            IFunction func = IFunctionFactory.Factory(function.Coordinates, function.Interpolator);
            return new InflowOutflow(func, "Inflow Outflow");

        }

        internal static InflowFrequency CreateInflowFrequencyFunction()
        {
            List<double> xs = new List<double>()
            {
                1,2,3,4,5
            };
            List<double> ys = new List<double>()
            {
                3,4,5,6,7
            };

            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            IFunction func = IFunctionFactory.Factory(function.Coordinates, function.Interpolator);
            return new InflowFrequency(func, "Inflow Frequency");

        }

        //internal static InflowFrequency CreateInflowFrequencyFunctionDistributed()
        //{
        //    List<double> xs = new List<double>()
        //    {
        //        1,2,3,4,5
        //    };
        //    List<IDistributedValue> ys = new List<IDistributedValue>();
        //    ys.Add(DistributedValueFactory.Factory(new Normal(1, 0)));
        //    ys.Add(DistributedValueFactory.Factory(new Normal(1, .1)));
        //    ys.Add(DistributedValueFactory.Factory(new Normal(1, .2)));
        //    ys.Add(DistributedValueFactory.Factory(new Normal(1, .3)));
        //    ys.Add(DistributedValueFactory.Factory(new Normal(1, .4)));


        //    ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
        //    return new InflowFrequency(function);

        //}

        internal static InflowFrequency CreateInflowFrequencyFunction(List<double> xs, List<double> ys)
        {

            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            IFunction func = IFunctionFactory.Factory(function.Coordinates, function.Interpolator);
            return new InflowFrequency(func, "Inflow Frequency");

        }
    }
}
