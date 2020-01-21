using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Functions;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using Statistics.Distributions;

namespace ModelTests.Condition.ComputePoint
{
    class ComputeTestData
    {


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

            ICoordinatesFunction function =  ICoordinatesFunctionsFactory.Factory(xs, ys);
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
