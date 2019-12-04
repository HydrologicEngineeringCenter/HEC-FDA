using Functions;
using Functions.CoordinatesFunctions;
using FunctionsTests.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.Utilities.Samplers
{
    [ExcludeFromCodeCoverage]
    public class LinkedFunctionsSamplerTests: CoordinateFunctionsTestData
    {

        #region Can Sample

        /// <summary> Tests that with a linked function, the "CanSample()" returns true </summary>
        [Fact]
        public void LinkedCoordinatesFunction_CanSample_Returns_True()
        {
            LinkedFunctionsSampler linkedSampler = new LinkedFunctionsSampler();

            List<ICoordinatesFunction> functions = Create_3Constant_StrictlyIncreasing_NonOverlappingXs_OrdinateFunctions();

            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
            interpolators.Add(InterpolationEnum.Linear);
            interpolators.Add(InterpolationEnum.Linear);

            CoordinatesFunctionLinked func = new CoordinatesFunctionLinked(functions, interpolators);
            Assert.True( linkedSampler.CanSample(func));
            
        }

        #endregion

        #region Constant Functions

        /// <summary> Tests that a linked function composed of constant functions samples correctly.</summary>
        [Fact]
        public void LinkedCoordinatesFunction_Sample_Returns_IFunction()
        {
            LinkedFunctionsSampler linkedSampler = new LinkedFunctionsSampler();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 9, 10, 11, 12 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 13, 14, 15, 16 };
            //create a constant func
            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
            CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);


            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>() { const1Func, const2Func, const3Func };

            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
            interpolators.Add(InterpolationEnum.Linear);
            interpolators.Add(InterpolationEnum.Linear);

            CoordinatesFunctionLinked func = new CoordinatesFunctionLinked(functions, interpolators);
            IFunction sampledFunc = linkedSampler.Sample(func, .5);
            //spot check a value.
            ICoordinate coord = sampledFunc.Coordinates[5];
            Assert.True(coord.X.Value() == 5 && coord.Y.Value() == 10);

        }

        #endregion

    }
}
