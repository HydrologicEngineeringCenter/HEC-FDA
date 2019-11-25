using Functions;
using Functions.Ordinates;
using Model;
using Model.Condition.ComputePoint.ImpactAreaFunctions;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace ModelTests.Condition.ComputePoint.ImpactAreaFunctions
{
    [ExcludeFromCodeCoverage]

    public class ImpactAreaFunctionsSerializationTests
    {
        /// <summary>
        /// Creates an infow frequency curve. Writes it to xml then creates an inflow frequency curve 
        /// from that xml. Verifies that the curves have the same values.
        /// </summary>
        [Fact]
        public void WriteAndReadConstantCurve()
        {
            List<double> xs = new List<double>()
            {
                1,2,3,4,5
            };
            List<double> ys = new List<double>()
            {
                3,4,5,6,7
            };
            IFrequencyFunction inflowFrequency = ComputeTestData.CreateInflowFrequencyFunction(xs, ys);

            //write the function to xml
            XElement xml = inflowFrequency.WriteToXML();
            //create a function from the xml
            Model.IFdaFunction func = ImpactAreaFunctionFactory.Factory(xml.ToString(), ImpactAreaFunctionEnum.InflowFrequency);
            
            //test that they two functions are equal
            Assert.True(func.GetType().Equals(typeof(InflowFrequency)));
            Assert.True(func.Function.F(new Constant(1)).Value() == 3);
            Assert.True(func.Function.F(new Constant(2)).Value() == 4);
            Assert.True(func.Function.F(new Constant(3)).Value() == 5);
            Assert.True(func.Function.F(new Constant(4)).Value() == 6);


        }

        /// <summary>
        /// Creates an infow frequency curve. Writes it to xml then creates an inflow frequency curve 
        /// from that xml. Verifies that the curves have the same values.
        /// </summary>
        [Fact]
        public void WriteAndReadDistributedCurve_Normal()
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
            InflowFrequency inflowFrequency = new InflowFrequency(function);

            //write the function to xml
            XElement xml = inflowFrequency.WriteToXML();
            //create a function from the xml
            Model.IFdaFunction func = ImpactAreaFunctionFactory.Factory(xml.ToString(), ImpactAreaFunctionEnum.InflowFrequency);

            //test that they two functions are equal
            Assert.True(func.GetType().Equals(typeof(InflowFrequency)));
            Assert.True(IsCoordinatesEqual(xs, inflowFrequency, func));


        }

        /// <summary>
        /// Creates an infow frequency curve. Writes it to xml then creates an inflow frequency curve 
        /// from that xml. Verifies that the curves have the same values.
        /// </summary>
        [Fact]
        public void WriteAndReadDistributedCurve_Triangular()
        {
            List<double> xs = new List<double>()
            {
                1,2,3,4,5
            };
            List<IDistributedValue> ys = new List<IDistributedValue>();
            ys.Add(DistributedValueFactory.Factory(new Triangular(.1,1,2)));
            ys.Add(DistributedValueFactory.Factory(new Triangular(.2, 1, 3)));
            ys.Add(DistributedValueFactory.Factory(new Triangular(.3, 1, 4)));
            ys.Add(DistributedValueFactory.Factory(new Triangular(.4, 1, 5)));
            ys.Add(DistributedValueFactory.Factory(new Triangular(.5, 1, 6)));


            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            InflowFrequency inflowFrequency = new InflowFrequency(function);

            //write the function to xml
            XElement xml = inflowFrequency.WriteToXML();
            //create a function from the xml
            Model.IFdaFunction func = ImpactAreaFunctionFactory.Factory(xml.ToString(), ImpactAreaFunctionEnum.InflowFrequency);

            //test that they two functions are equal
            Assert.True(func.GetType().Equals(typeof(InflowFrequency)));
            Assert.True(IsCoordinatesEqual(xs, inflowFrequency, func));


        }

        /// <summary>
        /// Creates an infow frequency curve. Writes it to xml then creates an inflow frequency curve 
        /// from that xml. Verifies that the curves have the same values.
        /// </summary>
        [Fact]
        public void WriteAndReadDistributedCurve_Uniform()
        {
            List<double> xs = new List<double>()
            {
                1,2,3,4,5
            };
            List<IDistributedValue> ys = new List<IDistributedValue>();
            ys.Add(DistributedValueFactory.Factory(new Uniform(.1, 2)));
            ys.Add(DistributedValueFactory.Factory(new Uniform(.2,  3)));
            ys.Add(DistributedValueFactory.Factory(new Uniform(.3,  4)));
            ys.Add(DistributedValueFactory.Factory(new Uniform(.4,  5)));
            ys.Add(DistributedValueFactory.Factory(new Uniform(.5,  6)));


            ICoordinatesFunction function = ICoordinatesFunctionsFactory.Factory(xs, ys);
            InflowFrequency inflowFrequency = new InflowFrequency(function);

            //write the function to xml
            XElement xml = inflowFrequency.WriteToXML();
            //create a function from the xml
            Model.IFdaFunction func = ImpactAreaFunctionFactory.Factory(xml.ToString(), ImpactAreaFunctionEnum.InflowFrequency);

            //test that they two functions are equal
            Assert.True(func.GetType().Equals(typeof(InflowFrequency)));
            Assert.True(IsCoordinatesEqual(xs, inflowFrequency, func));


        }

        private bool IsCoordinatesEqual(List<double> xs, IFdaFunction originalFunction, IFdaFunction functionToTest)
        {
            bool retval = true;
            foreach(double x in xs)
            {
                Constant ord = new Constant(x);
                retval = IsOrdinatesEqual(originalFunction.Function.F(ord), functionToTest.Function.F(ord));
                if(retval == false)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsOrdinatesEqual(IOrdinate ord1, IOrdinate ord2)
        {
            bool retval = true;
            if(!ord1.GetType().Equals(ord2.GetType()))
            {
                retval = false;
            }
            else if(Math.Abs(ord1.Value() - ord2.Value()) > .0001)
            {
                retval = false;
            }
            return retval;
        }


        //private string GetTestConstantXML()
        //{
        //    return "<Functions Type='NotLinked'><Function Interpolator = 'NoInterpolation' >" +
        //        "<Coordinate>" +
        //            "<Ordinate Value = '1' Type = 'Constant'/>  <Ordinate Value = '3' Type = 'Constant'/>  " +
        //        "</Coordinate> " +
        //        "<Coordinate> " +
        //            "<Ordinate Value = '2' Type = 'Constant'/>" +
        //            "<Ordinate Value = '4' Type = 'Constant'/> " +
        //        "</Coordinate> " +
        //        "</Function></Functions> ";


        //    //return "<Functions Type='Single'><Function Interpolator = 'NoInterpolation' >" +
        //    //    "<Coordinate ><Ordinate Value = '1' Type = 'Constant' />  <Ordinate Value = '3' Type = 'Constant' />  " +
        //    //    "</Coordinate >  <Coordinate> <Ordinate Value = '2' Type = 'Constant'/>" +
        //    //    " <Ordinate Value = '4' Type = 'Constant' /> </ Coordinate > < Coordinate >" +
        //    //    "< Ordinate Value = '3' Type = 'Constant' />< Ordinate Value = '5' Type = 'Constant' /> " +
        //    //    "</ Coordinate >  < Coordinate >< Ordinate Value = '4' Type = 'Constant' /> < Ordinate Value = '6' Type = 'Constant' />" +
        //    //    "  </ Coordinate >  < Coordinate > < Ordinate Value = '5' Type = 'Constant' /> " +
        //    //    "< Ordinate Value = '7' Type = 'Constant' /> </ Coordinate > </ Function ></ Functions > ";
        //}

        //private string GetTestDistributedXML()
        //{
        //    return "<Functions Type = 'NotLinked'> <Function Interpolator = 'NoInterpolation'><Coordinate>" +
        //        "<Ordinate Value = '1' Type = 'Constant'/> <Ordinate Type = 'Normal' Value = '1, 0, 2147483647'/> " +
        //        "</Coordinate><Coordinate> <Ordinate Value = '2' Type = 'Constant'/> " +
        //        "<Ordinate Type = 'Normal' Value = '1, 0.1, 2147483647'/> </Coordinate>  </Function>  </Functions>";
        //}

        //private string GetTestDistributedXML2()
        //{
        //    return "<Functions Type = 'NotLinked' > <Function Interpolator = 'NoInterpolation' > " +
        //         "<Coordinate >  " +
        //         "<Ordinate Type = 'Constant' > <Constant Value = '1' /></Ordinate > <Ordinate Type = 'Distribution' >" +
        //         " <Normal Mean = '1' StandardDeviation = '0' SampleSize = '2147483647' /></Ordinate > </Coordinate > <Coordinate >" +
        //         "   <Ordinate Type = 'Constant' > <Constant Value = '2' /> </Ordinate > <Ordinate Type = 'Distribution' > " +
        //         "<Normal Mean = '1' StandardDeviation = '0.1' SampleSize = '2147483647' /> </Ordinate >  </Coordinate ></Function>  </Functions>";
        //}
    }
}
