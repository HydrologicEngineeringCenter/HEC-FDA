using Functions;
using Functions.CoordinatesFunctions;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace ModelTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public class ExcelComputeTests
    {

        private const double INVALID_OPERATION_EXCEPTION = -9999;
        private const double ARGUMENT_OUT_OF_RANGE_EXCEPTION = -8888;
        private const string _TestDataRelativePath = "ExcelTesting\\ExcelData\\ComputeTestData.xlsx";

        [Theory]
        [ExcelComputeData(_TestDataRelativePath, 1)]
        public void Excel_Compute_Tests(
            List<double> xs1, List<double> ys1, string interpolation1,
            List<double> xs2, List<double> ys2, string interpolation2,
            List<double> xs3, List<double> ys3, string interpolation3,
            List<double> xs4, List<double> ys4, string interpolation4,
            List<double> xs5, List<double> ys5, string interpolation5,
            List<string> thresholdTypes, List<double> thresholdValues, List<double> expectedResults,
            int rowToWriteTo, int columnToWriteTo)
        {
            Sampler.RegisterSampler(new ConstantSampler());

            IFrequencyFunction inflowFreq = null;
            if (xs1.Count > 0)
            {
                InterpolationEnum interp1 = ConvertToInterpolationEnum(interpolation1);
                IFunction func1 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs1, ys1, interp1);
                inflowFreq = ImpactAreaFunctionFactory.FactoryFrequency(func1, ImpactAreaFunctionEnum.InflowFrequency);
            }

            List<ITransformFunction> transformFunctions = GetTransformFunctions(xs2, ys2, interpolation2, xs3, ys3, interpolation3, xs4, ys4, interpolation4, xs5, ys5, interpolation5);

            List<MetricEnum> metricThresholdTypes = new List<MetricEnum>();// { ConvertStringToMetricEnum(thresholdType) };
            foreach(string thresholdType in thresholdTypes)
            {
                metricThresholdTypes.Add(ConvertStringToMetricEnum(thresholdType));
            }

            List<IMetric> metrics = CreateMetrics(metricThresholdTypes,  thresholdValues);
            ICondition condition = ConditionFactory.Factory("testName", 1987, inflowFreq, transformFunctions, metrics);

            int randomPacketSize = transformFunctions.Count + 1;
            IDictionary<IMetric, double> results = condition.Compute(GetRandomNumbers(randomPacketSize));

            bool passedTest = DidTestPass(results, metrics, expectedResults);
            ExcelDataAttributeBase.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, CreateDataTable(results, metrics), passedTest);
            //Assert.True(passedTest);
            Assert.True(passedTest);


        }

        private List<double> GetRandomNumbers(int numberOfRandomNumbers)
        {
            int Seed = 1;
            List<double> randomNumbers = new List<double>();

            Random randomNumberGenerator = new Random(Seed);
            for (int k = 0; k < numberOfRandomNumbers; k++)
            {
                randomNumbers.Add(randomNumberGenerator.NextDouble());
            }
            return randomNumbers;
        }

        private MetricEnum ConvertStringToMetricEnum(string metric)
        {
            if(metric.ToUpper().Equals("EXTERIORSTAGE"))
            {
                return MetricEnum.ExteriorStage;
            }
            else if (metric.ToUpper().Equals("INTERIORSTAGE"))
            {
                return MetricEnum.InteriorStage;
            }
            else if (metric.ToUpper().Equals("DAMAGES"))
            {
                return MetricEnum.Damages;
            }
            else if (metric.ToUpper().Equals("EXPECTEDANNUALDAMAGE"))
            {
                return MetricEnum.ExpectedAnnualDamage;
            }
            else
            {
                throw new InvalidOperationException("Could not convert: " + metric + " to a metric.");
            }

        }

        //public enum MetricEnum
        //{
        //    NotSet = 0,
        //    ExteriorStage = 1,
        //    InteriorStage = 2,
        //    Damages = 3,
        //    ExpectedAnnualDamage = 4,
        //}

        private List<ITransformFunction> GetTransformFunctions(List<double> xs2, List<double> ys2, string interpolation2,
            List<double> xs3, List<double> ys3, string interpolation3,
            List<double> xs4, List<double> ys4, string interpolation4,
            List<double> xs5, List<double> ys5, string interpolation5)
        {
            ITransformFunction inflowOutflow = null;
            if (xs2.Count > 0)
            {
                InterpolationEnum interp2 = ConvertToInterpolationEnum(interpolation2);
                IFunction func2 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs2, ys2, interp2);
                inflowOutflow = ImpactAreaFunctionFactory.FactoryTransform(func2, ImpactAreaFunctionEnum.InflowOutflow);
            }

            ITransformFunction rating = null;
            if (xs3.Count > 0)
            {
                InterpolationEnum interp3 = ConvertToInterpolationEnum(interpolation3);
                IFunction func3 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs3, ys3, interp3);
                rating = ImpactAreaFunctionFactory.FactoryTransform(func3, ImpactAreaFunctionEnum.Rating);
            }

            ITransformFunction exteriorInterior = null;
            if (xs4.Count > 0)
            {
                InterpolationEnum interp4 = ConvertToInterpolationEnum(interpolation4);
                IFunction func4 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs4, ys4, interp4);
                exteriorInterior = ImpactAreaFunctionFactory.FactoryTransform(func4, ImpactAreaFunctionEnum.ExteriorInteriorStage);

            }

            ITransformFunction stageDamage = null;
            if (xs5.Count > 0)
            {
                InterpolationEnum interp5 = ConvertToInterpolationEnum(interpolation5);
                IFunction func5 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs5, ys5, interp5);
                stageDamage = ImpactAreaFunctionFactory.FactoryTransform(func5, ImpactAreaFunctionEnum.InteriorStageDamage);
            }
            List<ITransformFunction> transformFunctions = new List<ITransformFunction>();
            if (inflowOutflow != null)
            {
                transformFunctions.Add(inflowOutflow);
            }
            if (rating != null)
            {
                transformFunctions.Add(rating);
            }
            if (exteriorInterior != null)
            {
                transformFunctions.Add(exteriorInterior);
            }
            if (stageDamage != null)
            {
                transformFunctions.Add(stageDamage);
            }
            return transformFunctions;
        }


        internal List<IMetric> CreateMetrics(List<MetricEnum> types, List<double> values)
        {
            if (types.Count != values.Count)
            {
                throw new Exception("Metric types and values were different lengths.");
            }
            List<IMetric> metrics = new List<IMetric>();
            for (int i = 0; i < types.Count; i++)
            {
                metrics.Add(new Metric(types[i], values[i]));
            }
            return metrics;
        }


        private DataTable CreateDataTable(IDictionary<IMetric, double> results, List<IMetric> metrics)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            for(int i = 0;i<metrics.Count;i++)
            {
                double actual = results[metrics[i]];
                dt.Rows.Add(metrics[i].Type.ToString() + " = " + actual.ToString());
            }
            return dt;
        }

        
        private bool DidTestPass(IDictionary<IMetric, double> results, List<IMetric> metrics, List<double> expectedResults)
        {
            bool passedTest = true;

       
            for (int i = 0; i < expectedResults.Count; i++)
            {
                double metricResult = results[metrics[i]];
                double expectedResult = expectedResults[i];

                if (!HasMinimalDifference(metricResult, expectedResult))
                {
                    return false;
                }
            }
            return passedTest;
        }


        private bool DidTestPass(List<object> actualResults, List<double> expectedResults)
        {
            bool passedTest = true;
            for (int i = 0; i < expectedResults.Count; i++)
            {
                object result = actualResults[i];
                if (result is double)
                {
                    if (!HasMinimalDifference((double)result, expectedResults[i]))//, 1))
                    {
                        passedTest = false;
                    }
                }
                else
                {
                    passedTest = false;
                }
            }
            return passedTest;
        }

        private InterpolationEnum ConvertToInterpolationEnum(string interp)
        {

            if (interp.ToUpper().Equals("LINEAR"))
            {
                return InterpolationEnum.Linear;
            }
            else if (interp.ToUpper().Equals("PIECEWISE"))
            {
                return InterpolationEnum.Piecewise;
            }
            else if (interp.ToUpper().Equals("NONE"))
            {
                return InterpolationEnum.None;
            }
            else if (interp.ToUpper().Equals("NATURALCUBICSPLINE"))
            {
                return InterpolationEnum.NaturalCubicSpline;
            }
            else throw new ArgumentException("could not convert '" + interp + "'.");
        }

        private DataTable CreateDataTable(List<object> actualYs, OrderedSetEnum linkedFunctionOrder)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Columns.Add("LinkedFunctionOrder");
            for (int i = 0; i < actualYs.Count; i++)
            {
                if (i == 0)
                {
                    dt.Rows.Add(actualYs[i], linkedFunctionOrder.ToString());
                }
                else
                {
                    dt.Rows.Add(actualYs[i]);
                }
            }

            return dt;
        }

        private bool HasMinimalDifference(double value1, double value2)
        {
            double diff = Math.Abs(value1 - value2);
            return diff < .0001;
        }

    }
}
