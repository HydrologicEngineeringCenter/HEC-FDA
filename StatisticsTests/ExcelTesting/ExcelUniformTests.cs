using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace StatisticsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]
    public class ExcelUniformTests
    {

        private const string TestDataRelativePath = "ExcelTesting\\ExcelData\\";

        private const int WS_MIN = 1;
        private const int WS_MAX = 2;
        private const int WS_MODE = 3;
        private const int WS_MEAN = 4;
        private const int WS_MEDIAN = 5;
        private const int WS_VARIANCE = 6;
        private const int WS_STDEV = 7;
        private const int WS_SKEWNESS = 8;
        private const int WS_SAMPLESIZE = 9;
        private const int WS_PDF = 10;
        private const int WS_CDF = 11;
        private const int WS_INVERSE_CDF = 12;



        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MIN)]
        public void Uniform_Min_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Range.Min;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MIN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MAX)]
        public void Uniform_Max_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Range.Max;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MAX, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }


        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MEAN)]
        public void Uniform_Mean_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Mean;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MEAN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MEDIAN)]
        public void Uniform_Median_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Median;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MEDIAN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_VARIANCE)]
        public void Uniform_Variance_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Variance;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_VARIANCE, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_STDEV)]
        public void Uniform_StDev_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.StandardDeviation;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_STDEV, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_SKEWNESS)]
        public void Uniform_Skewness_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Skewness;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_SKEWNESS, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_SAMPLESIZE)]
        public void Uniform_SampleSize_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max, (int)sampleSize);
            double actual = testObj.SampleSize;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_SAMPLESIZE, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }


        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_CDF)]
        public void Uniform_CDF_Tests(double mode, double min, double max, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.CDF(xVal);
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_CDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_INVERSE_CDF)]
        public void Uniform_Inverse_CDF_Tests(double mode, double min, double max, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.InverseCDF(xVal);
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_INVERSE_CDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }


        private DataTable CreateDataTable(List<object> actualYs)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            for (int i = 0; i < actualYs.Count; i++)
            {
                dt.Rows.Add(actualYs[i]);
            }

            return dt;
        }

        private DataTable CreateDataTable(double result)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Rows.Add(result);
            return dt;
        }
        private DataTable CreateDataTable(string result)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Rows.Add(result);
            return dt;
        }

        private bool DidTestPass(double actual, double expected)
        {
            if (!HasMinimalDifference((double)actual, expected))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool HasMinimalDifference(double value1, double value2)
        {
            double diff = Math.Abs(value1 - value2);
            return diff < .0001;
        }
    }
}
