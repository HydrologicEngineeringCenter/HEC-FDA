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
        public static TheoryData<IDistributedValue> GoodData_Distributed =>
            new TheoryData<IDistributedValue>
            {
                { new DistributedValue( new Normal(1,1))},
                { new DistributedValue( new Normal(0,0))},
                { new DistributedValue( new Normal(2,2))},
                { new DistributedValue( new Normal(3,3))},
                { new DistributedValue( new Normal(4,4))},

            };
    }
}
