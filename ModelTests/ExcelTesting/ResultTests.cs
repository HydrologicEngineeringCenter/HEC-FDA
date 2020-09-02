using Functions;
using Model;
using System;
using System.Collections.Concurrent;
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
        public void Excel_Result_Tests(
            List<double> flowFreqProbs, IFdaFunction flowFreqFunc,
            List<double> inOutProbs, IFdaFunction inflowOutflowFunc,
            List<double> ratingProbs, IFdaFunction ratingFunc,
            List<double> extIntProbs, IFdaFunction exteriorInteriorFunc,
            List<double> failurProbs, IFdaFunction failureFunc,
            List<double> stageDamageProbs, IFdaFunction stageDamageFunc,
            List<string> thresholdTypes, List<double> thresholdValues,
            List<double> expectedDamages, List<double> expectedInteriorStage, List<double> expectedExteriorStage,
            int worksheetNumber, int rowToWriteTo, int columnToWriteTo)
        {
            //get the metrics
            List<IMetricEnum> metricThresholdTypes = new List<IMetricEnum>();
            foreach (string thresholdType in thresholdTypes)
            {
                metricThresholdTypes.Add(ConvertStringToMetricEnum(thresholdType));
            }
            List<IMetric> metrics = CreateMetrics(metricThresholdTypes, thresholdValues);

            //create the transform functions
            List<ITransformFunction> transformFunctions = new List<ITransformFunction>();
            if (inflowOutflowFunc != null)
            {
                transformFunctions.Add((ITransformFunction)inflowOutflowFunc);// IFdaFunctionFactory.Factory(inflowOutflowFunc, IParameterEnum.InflowFrequency));
            }
            if (ratingFunc != null)
            {
                transformFunctions.Add((ITransformFunction)ratingFunc);// ImpactAreaFunctionFactory.FactoryTransform(ratingFunc, ImpactAreaFunctionEnum.Rating));
            }
            if (exteriorInteriorFunc != null)
            {
                transformFunctions.Add((ITransformFunction)exteriorInteriorFunc);// ImpactAreaFunctionFactory.FactoryTransform(exteriorInteriorFunc, ImpactAreaFunctionEnum.ExteriorInteriorStage));
            }
            if (failureFunc != null)
            {
                transformFunctions.Add((ITransformFunction)failureFunc);// ImpactAreaFunctionFactory.FactoryTransform(failureFunc, ImpactAreaFunctionEnum.LeveeFailure));
            }
            if (stageDamageFunc != null)
            {
                transformFunctions.Add((ITransformFunction)stageDamageFunc);// ImpactAreaFunctionFactory.FactoryTransform(stageDamageFunc, ImpactAreaFunctionEnum.InteriorStageDamage));
            }

            //create the frequency function
            IFrequencyFunction inflowFrequencyFunction = (IFrequencyFunction)flowFreqFunc;// ImpactAreaFunctionFactory.FactoryFrequency(flowFreqFunc, ImpactAreaFunctionEnum.InflowFrequency);

            ICondition condition = ConditionFactory.Factory("testName", 1987, inflowFrequencyFunction, transformFunctions, metrics);

            //register samplers
            Sampler.RegisterSampler(new ConstantSampler());

            Result result = new Result(condition);
            List<List<double>> probabilites = CreateProbabilities(flowFreqProbs, inOutProbs, ratingProbs, extIntProbs, failurProbs, stageDamageProbs);
            result.Compute(probabilites);

            ConcurrentDictionary<int, IDictionary<IMetric, double>> computeResults = result.Realizations;

            bool didTestsPass = DidTestsPass(computeResults, metrics, expectedDamages, expectedInteriorStage, expectedExteriorStage);


            DataTable dt = CreateDataTable(computeResults, metrics);
            ExcelDataAttributeBase.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, dt, didTestsPass);
            Assert.True(didTestsPass);

        }

        private bool DidTestsPass(ConcurrentDictionary<int, IDictionary<IMetric, double>> results, List<IMetric> metrics, List<double> expectedDamages, List<double> expectedInterior, List<double> expectedExterior  )
        {
            bool DidAllIterationsPass = true;
            for(int i = 0;i<results.Count;i++)
            {
                List<double> expectedResults = new List<double>() { expectedDamages[i], expectedInterior[i], expectedExterior[i] };
                bool didIterationPass = DidTestPass(results[i], metrics, expectedResults);
                if(!didIterationPass)
                {
                    return false;
                }
            }
            return DidAllIterationsPass;
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

            //i did this the wrong way. I need to switch the vertical and horizontal
            List<List<double>> probabilities = new List<List<double>>();
            for(int i = 0; i<num;i++)
            {
                List<double> singleRow = new List<double>();
                foreach(List<double> oldRow in retval)
                {
                    singleRow.Add(oldRow[i]);
                }
                probabilities.Add(singleRow);

            }

            return probabilities;
        }

        internal List<IMetric> CreateMetrics(List<IMetricEnum> types, List<double> values)
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

        private IMetricEnum ConvertStringToMetricEnum(string metric)
        {
            if (metric.ToUpper().Equals("EXTERIORSTAGE") || metric.ToUpper().Equals("EXTERIOR STAGE"))
            {
                return IMetricEnum.ExteriorStage;
            }
            else if (metric.ToUpper().Equals("INTERIORSTAGE") || metric.ToUpper().Equals("INTERIOR STAGE"))
            {
                return IMetricEnum.InteriorStage;
            }
            else if (metric.ToUpper().Equals("DAMAGES"))
            {
                return IMetricEnum.Damages;
            }
            else if (metric.ToUpper().Equals("EXPECTEDANNUALDAMAGE") || metric.ToUpper().Equals("EXPECTED ANNUAL DAMAGE"))
            {
                return IMetricEnum.ExpectedAnnualDamage;
            }
            else
            {
                throw new InvalidOperationException("Could not convert: " + metric + " to a metric.");
            }

        }

        private DataTable CreateDataTable(ConcurrentDictionary<int, IDictionary<IMetric, double>> results, List<IMetric> metrics)
        {
            //the user might not have put in all three metrics.
            //use the list of metrics to just write out the ones the user defined.
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Damages AEP");
            dt.Columns.Add("Interior Stage AEP");
            dt.Columns.Add("Exterior Stage AEP");

            for(int i= 0;i<results.Count;i++)
            {
                IDictionary<IMetric, double> singleRowResult = results[i];
                List<object> actualRowValues = GetRowOfActualValues(singleRowResult, metrics);
                DataRow dr = dt.NewRow();
                dr["Damages AEP"] = actualRowValues[0];
                dr["Interior Stage AEP"] = actualRowValues[1];
                dr["Exterior Stage AEP"] = actualRowValues[2];
                dt.Rows.Add(dr);
            }
            return dt;

        }

        private List<object> GetRowOfActualValues(IDictionary<IMetric, double> singleRowResult, List<IMetric> metrics)
        {
            bool hasDamages = false;
            bool hasInterior = false;
            bool hasExterior = false;

            double damageVal = 0;
            double intVal = 0;
            double extVal = 0;

            for (int j = 0; j < metrics.Count; j++)
            {
                IMetric met = metrics[j];
                IMetricEnum metType = met.ParameterType;
                if (singleRowResult.ContainsKey(met))
                {
                    switch (met.ParameterType)// == IMetricEnum.Damages)
                    {
                        case IMetricEnum.Damages:
                            {
                                 damageVal = singleRowResult[met];
                                hasDamages = true;
                                break;
                            }
                        case IMetricEnum.InteriorStage:
                            {
                                 intVal = singleRowResult[met];
                                hasInterior = true;
                                break;
                            }
                        case IMetricEnum.ExteriorStage:
                            {
                                 extVal = singleRowResult[met];
                                hasExterior = true;
                                break;
                            }
                    }

                }
            }

            //need to return in this order: damages, interior stage, exterior stage
            List<object> retval = new List<object>();
            if(hasDamages)
            {
                retval.Add(damageVal);
            }
            else
            {
                retval.Add("N/A");
            }

            if(hasInterior)
            {
                retval.Add(intVal);
            }
            else
            {
                retval.Add("N/A");
            }
            if (hasExterior)
            {
                retval.Add(extVal);
            }
            else
            {
                retval.Add("N/A");
            }
            return retval;
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
                    dt.Rows.Add(metrics[i].ParameterType.ToString() + " = " + actual.ToString());
                }
                else
                {
                    dt.Rows.Add("Metric: " + metric.ParameterType.ToString() + " was not in the condition's results");
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
