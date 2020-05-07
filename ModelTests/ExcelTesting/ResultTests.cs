using Functions;
using Model;
using Model.Inputs.Functions.ImpactAreaFunctions;
using Model.Outputs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace ModelTests.ExcelTesting
{
    public class ResultTests
    {
        private const string _TestDataRelativePath = "ExcelTesting\\ExcelData\\ResultTestData.xlsx";

        [Theory]
        [ResultData(_TestDataRelativePath, new int[] { 1})]
        public void Excel_Compute_Tests(
            List<double> flowFreqProbs, ICoordinatesFunction flowFreqFunc,
            List<double> inOutProbs, ICoordinatesFunction inflowOutflowFunc,
            List<double> ratingProbs, ICoordinatesFunction ratingFunc,
            List<double> extIntProbs, ICoordinatesFunction exteriorInteriorFunc,
            List<double> failurProbs, ICoordinatesFunction failureFunc,
            List<double> stageDamageProbs, ICoordinatesFunction stageDamageFunc,
            List<string> thresholdTypes, List<double> thresholdValues,
            List<double> expectedDamages, List<double> expectedInteriorStage, List<double> expectedExteriorStage,
            int worksheetNumber, int rowToWriteTo, int columnToWriteTo)
        {
            //get the metrics
            List<MetricEnum> metricThresholdTypes = new List<MetricEnum>();
            foreach (string thresholdType in thresholdTypes)
            {
                metricThresholdTypes.Add(ConvertStringToMetricEnum(thresholdType));
            }
            List<IMetric> metrics = CreateMetrics(metricThresholdTypes, thresholdValues);

            //create the transform functions
            List<ITransformFunction> transformFunctions = new List<ITransformFunction>();
            if (inflowOutflowFunc != null)
            {
                transformFunctions.Add( ImpactAreaFunctionFactory.FactoryTransform(inflowOutflowFunc, ImpactAreaFunctionEnum.InflowFrequency));
            }
            if (ratingFunc != null)
            {
                transformFunctions.Add(ImpactAreaFunctionFactory.FactoryTransform(ratingFunc, ImpactAreaFunctionEnum.Rating));
            }
            if (exteriorInteriorFunc != null)
            {
                transformFunctions.Add(ImpactAreaFunctionFactory.FactoryTransform(exteriorInteriorFunc, ImpactAreaFunctionEnum.ExteriorInteriorStage));
            }
            if (failureFunc != null)
            {
                transformFunctions.Add(ImpactAreaFunctionFactory.FactoryTransform(failureFunc, ImpactAreaFunctionEnum.LeveeFailure));
            }
            if (stageDamageFunc != null)
            {
                transformFunctions.Add(ImpactAreaFunctionFactory.FactoryTransform(stageDamageFunc, ImpactAreaFunctionEnum.InteriorStageDamage));
            }

            //create the frequency function
            IFrequencyFunction inflowFrequencyFunction = ImpactAreaFunctionFactory.FactoryFrequency(flowFreqFunc, ImpactAreaFunctionEnum.InflowFrequency);

            ICondition condition = ConditionFactory.Factory("testName", 1987, inflowFrequencyFunction, transformFunctions, metrics);

            Result result = new Result(condition);
            List<List<double>> probabilites = CreateProbabilities(flowFreqProbs, inOutProbs, ratingProbs, extIntProbs, failurProbs, stageDamageProbs);
            result.Compute(probabilites);

            System.Collections.Concurrent.ConcurrentDictionary<int, IDictionary<IMetric, double>> computeResults = result.Realizations;


            

            //bool passedTest = DidTestPass(results, metrics, expectedResults);
            //ExcelDataAttributeBase.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, CreateDataTable(results, metrics), passedTest);
            Assert.True(false);

        }

        private List<List<double>> CreateProbabilities(List<double> flowFreqProbs, List<double> inOutProbs, List<double> ratingProbs, List<double>extIntProbs, List<double>failureProbs, List<double>stageDamageProbs)
        {
            List<List<double>> retval = new List<List<double>>();
            if(flowFreqProbs != null && flowFreqProbs.Count >0)
            {
                retval.Add(flowFreqProbs);
            }
            if (inOutProbs != null && inOutProbs.Count > 0)
            {
                retval.Add(inOutProbs);
            }
            if (ratingProbs != null && ratingProbs.Count > 0)
            {
                retval.Add(ratingProbs);
            }
            if (extIntProbs != null && extIntProbs.Count > 0)
            {
                retval.Add(extIntProbs);
            }
            if (failureProbs != null && failureProbs.Count > 0)
            {
                retval.Add(failureProbs);
            }
            if (stageDamageProbs != null && stageDamageProbs.Count > 0)
            {
                retval.Add(stageDamageProbs);
            }

            //check that all the lists with values has the same number
            int num = retval[0].Count;
            foreach(List<double> probs in retval)
            {
                if(probs.Count != num)
                {
                    throw new Exception("Different number of probabilites in test.");
                }
            }
            return retval;
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

        private MetricEnum ConvertStringToMetricEnum(string metric)
        {
            if (metric.ToUpper().Equals("EXTERIORSTAGE") || metric.ToUpper().Equals("EXTERIOR STAGE"))
            {
                return MetricEnum.ExteriorStage;
            }
            else if (metric.ToUpper().Equals("INTERIORSTAGE") || metric.ToUpper().Equals("INTERIOR STAGE"))
            {
                return MetricEnum.InteriorStage;
            }
            else if (metric.ToUpper().Equals("DAMAGES"))
            {
                return MetricEnum.Damages;
            }
            else if (metric.ToUpper().Equals("EXPECTEDANNUALDAMAGE") || metric.ToUpper().Equals("EXPECTED ANNUAL DAMAGE"))
            {
                return MetricEnum.ExpectedAnnualDamage;
            }
            else
            {
                throw new InvalidOperationException("Could not convert: " + metric + " to a metric.");
            }

        }

        private DataTable CreateDataTable(IDictionary<IMetric, double> results, List<IMetric> metrics)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            for (int i = 0; i < metrics.Count; i++)
            {
                IMetric metric = metrics[i];
                if (results.ContainsKey(metric))
                {
                    double actual = results[metric];
                    dt.Rows.Add(metrics[i].Type.ToString() + " = " + actual.ToString());
                }
                else
                {
                    dt.Rows.Add("Metric: " + metric.Type.ToString() + " was not in the condition's results");
                }
            }
            return dt;
        }
        private DataTable CreateDataTable(string message)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Rows.Add(message);
            return dt;
        }

        private bool DidTestPass(IDictionary<IMetric, double> results, List<IMetric> metrics, List<double> expectedResults)
        {
            bool passedTest = true;


            for (int i = 0; i < expectedResults.Count; i++)
            {
                IMetric metric = metrics[i];
                if (results.ContainsKey(metric))
                {
                    double metricResult = results[metric];

                    double expectedResult = expectedResults[i];

                    if (!HasMinimalDifference(metricResult, expectedResult))
                    {
                        return false;
                    }
                }
                else
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

        private bool HasMinimalDifference(double value1, double value2)
        {
            double diff = Math.Abs(value1 - value2);
            return diff < .0001;
        }

    }
}
