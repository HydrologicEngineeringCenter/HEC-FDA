using Functions;
using Functions.CoordinatesFunctions;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
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

        internal List<IMetric> CreateMetrics(List<MetricEnum> types, List<double> values)
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

        internal IFrequencyFunction CreateFrequencyFunction(List<double> xs, List<double> ys, InterpolationEnum interpolator, ImpactAreaFunctionEnum type)
        {
            ICoordinatesFunction lpsCoordFunc = ICoordinatesFunctionsFactory.Factory(xs,ys, interpolator);
            return ImpactAreaFunctionFactory.FactoryFrequency(lpsCoordFunc, type);
        }

        internal ITransformFunction CreateTransformFunction(List<double> xs, List<double> ys, InterpolationEnum interpolator, ImpactAreaFunctionEnum type)
        {
            ICoordinatesFunction lpsCoordFunc = ICoordinatesFunctionsFactory.Factory(xs, ys, interpolator);
            return ImpactAreaFunctionFactory.FactoryTransform(lpsCoordFunc, type);
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
            return new InflowOutflow(function);

        }

        internal static InflowOutflow CreateInflowOutflowFunction(List<double> xs, List<double> ys)
        {

            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            return new InflowOutflow(function);

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
            return new InflowFrequency(function);

        }

        internal static InflowFrequency CreateInflowFrequencyFunctionDistributed()
        {
            List<double> xs = new List<double>()
            {
                1,2,3,4,5
            };
            List<IDistributedValue> ys = new List<IDistributedValue>();
            ys.Add(DistributedValueFactory.Factory(new Normal(1, 0)));
            ys.Add(DistributedValueFactory.Factory(new Normal(1, .1)));
            ys.Add(DistributedValueFactory.Factory(new Normal(1, .2)));
            ys.Add(DistributedValueFactory.Factory(new Normal(1, .3)));
            ys.Add(DistributedValueFactory.Factory(new Normal(1, .4)));


            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            return new InflowFrequency(function);

        }

        internal static InflowFrequency CreateInflowFrequencyFunction(List<double> xs, List<double> ys)
        {

            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            return new InflowFrequency(function);

        }
    }
}
