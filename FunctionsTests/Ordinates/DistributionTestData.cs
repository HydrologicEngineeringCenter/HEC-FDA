using Functions;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionsTests.Ordinates
{
    public class DistributionTestData
    {
        public static TheoryData<IDistributedOrdinate> GoodData_Distributed =>
            new TheoryData<IDistributedOrdinate>
            {
                { new Distribution( new Normal(1,1))},
                { new Distribution( new Normal(0,0))},
                { new Distribution( new Normal(2,2))},
                { new Distribution( new Normal(3,3))},
                { new Distribution( new Normal(4,4))},

            };
    }
}
