//using System;
//using Model.Inputs.Functions;
//using Model.Inputs.Functions.ImpactAreaFunctions;
//using System.Collections.Generic;
//using Xunit;
//using Functions;
//using System.Diagnostics.CodeAnalysis;

//namespace ModelTests.InputsTests.FunctionsTests.ImpactAreaFunctionsTests
//{
//    [ExcludeFromCodeCoverage]

//    public class DamageFrequencyTests
//    {
//        #region Properties Tests
//        [TestMethod()]
//        public void TypeProperty_AlwaysEqualToDamageFrequency()
//        {
//            //var mockFunction = new Mock<IFunctionBase>();
//            //DamageFrequency testObject = new DamageFrequency(mockFunction.Object);
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 0, 1 }, new double[2] { 0, 10 }, true, false));
//            DamageFrequency testObject = new DamageFrequency(testFunction);
//            Assert.AreEqual(ImpactAreaFunctionEnum.DamageFrequency, testObject.Type);
//        }
//        #endregion

//        #region Validate() Tests
//        [TestMethod()]
//        public void Validate_GoodDataReturnsTrue()
//        {
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 0, 1 }, new double[2] { 0, 10 }, true, false));
//            DamageFrequency testObject = new DamageFrequency(testFunction);
//            Assert.IsTrue(testObject.IsValid);
//        }
//        [TestMethod()]
//        public void Validate_NegativeFrequencyOrdinateReturnsFalse()
//        {
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { -0.1, 1 }, new double[2] { 0, 10 }, true, false));
//            DamageFrequency testObject = new DamageFrequency(testFunction);
//            Assert.IsFalse(testObject.IsValid);
//        }
//        [TestMethod()]
//        public void Validate_GreaterThan1FrequencyOrdinateReturnsFalse()
//        {
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 0, 1.1 }, new double[2] { 0, 10 }, true, false));
//            DamageFrequency testObject = new DamageFrequency(testFunction);
//            Assert.IsFalse(testObject.IsValid);
//        }
//        [TestMethod()]
//        public void Validate_NegativeDamageOrdinateReturnsFalse()
//        {
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 0, 1 }, new double[2] { -1, 10 }, true, false));
//            DamageFrequency testObject = new DamageFrequency(testFunction);
//            Assert.IsFalse(testObject.IsValid);
//        }
//        [TestMethod()]
//        public void Validate_InvalidStatisticsCurveIncreasingReturnsFalse()
//        {
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 1, 0 }, new double[2] { 0, 0 }, true, false));
//            DamageFrequency testObject = new DamageFrequency(testFunction);
//            Assert.IsFalse(testObject.IsValid);
//        }
//        [TestMethod()]
//        public void Validate_InvalidStatisticsUncertainCurveIncreasingReturnsFalse()
//        {
//            IFunctionBase testFunction = new UncertainOrdinatesFunction(new Statistics.UncertainCurveIncreasing(new double[2] { 1, 0 }, new Statistics.ContinuousDistribution[2] { new Statistics.Normal(0, 1), new Statistics.Normal(1, 1) }, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal));
            
//            Assert.IsFalse(new DamageFrequency(testFunction).IsValid);
//        }
//        #endregion

//        #region ReportValidationErrors() Tests
//        [TestMethod()]
//        public void ReportValidationErrors_SingleNegativeFrequencyOrdinatesErrorReturnsSingleErrorMessage()
//        {
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { -0.1, 1 }, new double[2] { 0, 10 }, true, false));

//            IList<string> errors = (List<string>)new DamageFrequency(testFunction).ReportValidationErrors();
//            Assert.AreEqual(1, errors.Count);
//        }
//        [TestMethod()]
//        public void ReportValidationErrors_NegativeFrequencyOrdinatesReported()
//        {           
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { -0.1, 1 }, new double[2] { 0, 10 }, true, false));

//            bool passed = false;
//            IEnumerable<string> messages = new DamageFrequency(testFunction).ReportValidationErrors();
//            foreach (string s in messages) if (s.Contains("-0.1")) passed = true;
//            Assert.IsTrue(passed);
//        }
//        [TestMethod()]
//        public void ReportValidationErrors_StatisticsCurveIncreasingErrorsReportedVerbatim()
//        {
//            IFunctionBase testFunction = new OrdinatesFunction(new Statistics.CurveIncreasing(new double[2] { 1, 0 }, new double[2] { 0, 0 }, true, false));
//            List<string> actual = (List<string>)new DamageFrequency(testFunction).ReportValidationErrors();
//            CollectionAssert.AreEqual((List<string>)testFunction.ReportValidationErrors(), actual);
//        }
//        [TestMethod()]
//        public void ReportValidationErrors_UncertainCurveIncreasingErrorsReportedVerbatim()
//        {
//            IFunctionBase testFunction = new UncertainOrdinatesFunction(new Statistics.UncertainCurveIncreasing(new double[2] { 0, 1 }, new Statistics.Normal[2] { new Statistics.Normal(0, 1), new Statistics.Normal(1, 1) }, true, false, Statistics.UncertainCurveDataCollection.DistributionsEnum.Normal));
//            List<string> actual = (List<string>)new DamageFrequency(testFunction).ReportValidationErrors();
//            CollectionAssert.AreEqual((List<string>)testFunction.ReportValidationErrors(), actual);
//        }
//        #endregion


//        [Fact]
//        public void Sampler_Testing_ConstantFunctions_NoInterpolator()
//        {

//            Sampler.RegisterSampler(new ConstantSampler());

//            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
//            List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

//            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
//            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

//            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

//            xs = new List<double>() { 0, 25, 50, 75, 100 };
//            ys = new List<double>() { 0, 25, 50, 75, 100 };
//            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
//            type = ImpactAreaFunctionEnum.InflowOutflow;

//            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

//            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

//            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

//            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
//            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
//            Assert.True(composedFunction.F(new Constant(0)).Value() == 0);
//            Assert.True(composedFunction.F(new Constant(1)).Value() == 100);
//        }

//        [Fact]
//        public void Sampler_Testing_ConstantFunctions_LinearInterpolator()
//        {

//            Sampler.RegisterSampler(new ConstantSampler());

//            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
//            List<double> ys = new List<double>() { 0, 100, 1000, 2000, 3000 };

//            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
//            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

//            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

//            xs = new List<double>() { 0, 50, 150, 275, 1000 };
//            ys = new List<double>() { 0, 10, 110, 750, 10000 };
//            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.Linear);
//            type = ImpactAreaFunctionEnum.InflowOutflow;

//            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

//            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

//            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

//            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);

//            //x  |  y
//            //0    0
//            //.25   60

//            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
//            Assert.True(composedFunction.F(new Constant(0)).Value() == 0);
//            Assert.True(composedFunction.F(new Constant(.25)).Value() == 60);
//        }

//        /// <summary>
//        /// This one is failing because the coordinatesFunctionConstants.IsZOffOverlap() is returning true
//        /// during the compute. I think these should be valid values. It might be because there is no interpolator. 
//        /// Try this one with an interpolator.
//        /// </summary>
//        [Fact]
//        public void Sampler_Testing_OneDistributedFunction_OneConstantFunction_1_NoInterpolator()
//        {
//            Sampler.RegisterSampler(new ConstantSampler());
//            Sampler.RegisterSampler(new DistributionSampler());

//            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
//            List<IDistributedValue> ys = new List<IDistributedValue>()
//                   {
//                       DistributedValueFactory.Factory( new Normal(1,2)),
//                       DistributedValueFactory.Factory(new Triangular(3,4,5)),
//                       DistributedValueFactory.Factory(new Uniform(5,6)),
//                       DistributedValueFactory.Factory(new Normal(6,1)),
//                       DistributedValueFactory.Factory(new Uniform(7,8)),

//                   };

//            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
//            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

//            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

//            xs = new List<double>() { 0, 25, 50, 75, 100 };
//            List<double> ys2 = new List<double>() { 0, 25, 50, 75, 100 };
//            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys2);
//            type = ImpactAreaFunctionEnum.InflowOutflow;

//            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

//            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

//            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

//            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
//            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
//            Assert.True(composedFunction.F(new Constant(0)).Value() == 0);
//            Assert.True(composedFunction.F(new Constant(1)).Value() == 100);
//        }

//        [Fact]
//        public void Sampler_Testing_OneDistributedFunction_OneConstantFunction_2_NoInterpolator()
//        {
//            Sampler.RegisterSampler(new ConstantSampler());
//            Sampler.RegisterSampler(new DistributionSampler());

//            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
//            List<IDistributedValue> ys = new List<IDistributedValue>()
//                   {
//                       DistributedValueFactory.Factory( new Normal(1,2)),
//                       DistributedValueFactory.Factory(new Triangular(3,4,5)),
//                       DistributedValueFactory.Factory(new Uniform(5,6)),
//                       DistributedValueFactory.Factory(new Normal(6,1)),
//                       DistributedValueFactory.Factory(new Uniform(7,8)),

//                   };

//            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
//            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

//            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

//            xs = new List<double>() { 0, 1, 2, 3, 4 };
//            List<double> ys2 = new List<double>() { 0, 25, 50, 75, 100 };
//            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys2);
//            type = ImpactAreaFunctionEnum.InflowOutflow;

//            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

//            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

//            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

//            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
//            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
//            Assert.True(composedFunction.F(new Constant(0)).Value() == 25);
//            Assert.True(composedFunction.F(new Constant(.25)).Value() == 100);
//        }

//        [Fact]
//        public void Sampler_Testing_TwoDistributedFunctions_NoInterpolator()
//        {
//            Sampler.RegisterSampler(new ConstantSampler());
//            Sampler.RegisterSampler(new DistributionSampler());

//            List<double> xs = new List<double>() { 0, .25, .5, .75, 1 };
//            List<IDistributedValue> ys = new List<IDistributedValue>()
//                   {
//                       DistributedValueFactory.Factory( new Normal(1,2)),
//                       DistributedValueFactory.Factory(new Triangular(3,4,5)),
//                       DistributedValueFactory.Factory(new Uniform(5,6)),
//                       DistributedValueFactory.Factory(new Normal(6,1)),
//                       DistributedValueFactory.Factory(new Uniform(7,8)),

//                   };

//            ICoordinatesFunction coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys);
//            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

//            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

//            xs = new List<double>() { 0, 4, 5, 6, 7 };
//            List<double> ys2 = new List<double>() { 0, 25, 50, 75, 100 };
//            coordFunction = ICoordinatesFunctionsFactory.Factory(xs, ys2);
//            type = ImpactAreaFunctionEnum.InflowOutflow;

//            IFdaFunction inflowOutflow = ImpactAreaFunctionFactory.Factory(coordFunction, type);
//            IFunction computeableInflowOutflow = Sampler.Sample(inflowOutflow.Function, .5);

//            //IComputableTransformFunction computableInflowOutflow = inflowOutflow.Sample(.5);

//            //IComputableFrequencyFunction composedFunction = computableInflowFreq.Compose(computableInflowOutflow);

//            IFunction composedFunction = computableInflowFreq.Compose(computeableInflowOutflow);
//            // Assert.True(composedFunction.Type == ImpactAreaFunctionEnum.OutflowFrequency);
//            Assert.True(composedFunction.F(new Constant(.25)).Value() == 25);
//            Assert.True(composedFunction.F(new Constant(.75)).Value() == 75);
//        }

//        [Fact]
//        public void TestingLinkedFunction_ConstantFunctions_NoInterpolator()
//        {

//            Sampler.RegisterSampler(new LinkedFunctionsSampler());


//            List<double> xs = new List<double>() { 0, .1, .2, .3, .4 };
//            List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

//            ICoordinatesFunction coordFunction1 = ICoordinatesFunctionsFactory.Factory(xs, ys);

//            List<double> xs2 = new List<double>() { .5, .6, .7, .8, .9 };
//            List<double> ys2 = new List<double>() { 150, 250, 500, 750, 1000 };

//            ICoordinatesFunction coordFunction2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2);

//            List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>() { coordFunction1, coordFunction2 };
//            List<InterpolationEnum> enumsBetweenFuncs = new List<InterpolationEnum>() { InterpolationEnum.Linear };
//            ICoordinatesFunction linkedFunction = ICoordinatesFunctionsFactory.Factory(funcs, enumsBetweenFuncs);

//            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

//            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.Factory(linkedFunction, type);
//            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

//            Assert.True(true);

//        }

//        /// <summary>
//        /// This one has an infinite loop
//        /// </summary>
//        [Fact]
//        public void TestingLinkedFunction_ConstantAndDistributedFunctions_NoInterpolator()
//        {

//            Sampler.RegisterSampler(new LinkedFunctionsSampler());


//            List<double> xs = new List<double>() { 0, .1, .2, .3, .4 };
//            List<double> ys = new List<double>() { 0, 25, 50, 75, 100 };

//            ICoordinatesFunction coordFunction1 = ICoordinatesFunctionsFactory.Factory(xs, ys);

//            List<double> xs2 = new List<double>() { .5, .6, .7, .8, .9 };
//            List<IDistributedValue> ys2 = new List<IDistributedValue>()
//                   {
//                       DistributedValueFactory.Factory( new Normal(1,2)),
//                       DistributedValueFactory.Factory(new Triangular(3,4,5)),
//                       DistributedValueFactory.Factory(new Uniform(5,6)),
//                       DistributedValueFactory.Factory(new Normal(6,1)),
//                       DistributedValueFactory.Factory(new Uniform(7,8)),

//                   };

//            ICoordinatesFunction coordFunction2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2);

//            List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>() { coordFunction1, coordFunction2 };
//            List<InterpolationEnum> enumsBetweenFuncs = new List<InterpolationEnum>() { InterpolationEnum.Linear };
//            ICoordinatesFunction linkedFunction = ICoordinatesFunctionsFactory.Factory(funcs, enumsBetweenFuncs);

//            ImpactAreaFunctionEnum type = ImpactAreaFunctionEnum.InflowFrequency;

//            IFdaFunction inflowFrequency = ImpactAreaFunctionFactory.Factory(linkedFunction, type);
//            IFunction computableInflowFreq = Sampler.Sample(inflowFrequency.Function, .5);

//            Assert.True(true);

//        }
//    }
//}
