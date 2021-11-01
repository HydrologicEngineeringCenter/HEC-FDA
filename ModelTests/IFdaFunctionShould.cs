using Functions;
using MathNet.Numerics.Distributions;
using Model;
using Statistics;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using paireddata;

namespace ModelTests
{
    public class IFdaFunctionShould
    {
        [Fact]
        void ReturnEquivalentXandYasUncertainPairedData()
        {

            List<double> listx = new List<double> { 0, 1, 2, 5, 10 };
            List<Statistics.IDistribution> listy = new List<Statistics.IDistribution>();
            for (int i = 0; i < listx.Count; i++ )
            {
                listy.Add(new Statistics.Distributions.Uniform(0, 1));
            }
            ICoordinatesFunction fun = ICoordinatesFunctionsFactory.Factory(listx, listy);
            IFdaFunction func = IFdaFunctionFactory.Factory(IParameterEnum.Rating, fun);
            UncertainPairedData upd = func.ToUncertainPairedData();

            double[] expectedXFromIFdaFunction = new double[func.Coordinates.Count];
            double[] expectedXFromUncertainPD = upd.xs();

            Statistics.IDistribution[] expectedYFromIFdaFunciton = new Statistics.IDistribution[func.Coordinates.Count];
            Statistics.IDistribution[] expectedYFromUncertainPD = upd.ys();

            for(int i = 0; i< func.Coordinates.Count; i++)
            {
                expectedXFromIFdaFunction[i] = func.Coordinates[i].X.Value();
                var temp = (IDistributedOrdinate)func.Coordinates[i].Y;
                expectedYFromIFdaFunciton[i] = temp.Dist;      
            }
            Assert.Equal(expectedXFromUncertainPD, listx.ToArray());
            Assert.Equal(expectedXFromIFdaFunction, listx.ToArray());
            Assert.Equal(expectedYFromUncertainPD, listy);
            Assert.Equal(expectedYFromIFdaFunciton, listy);
        }
    }
}
