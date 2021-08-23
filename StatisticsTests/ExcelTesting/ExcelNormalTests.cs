using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace StatisticsTests.ExcelTesting
{
    public class ExcelNormalTests
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
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_MIN)]
        public void Normal_Min_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.Range.Min;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_MIN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_MAX)]
        public void Normal_Max_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.Range.Max;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_MAX, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_MODE)]
        public void Normal_Mode_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.Mode;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_MODE, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_MEAN)]
        public void Normal_Mean_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.Mean;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_MEAN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_MEDIAN)]
        public void Normal_Median_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.Median;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_MEDIAN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_VARIANCE)]
        public void Normal_Variance_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.Variance;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_VARIANCE, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_STDEV)]
        public void Normal_StDev_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.StandardDeviation;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_STDEV, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_SKEWNESS)]
        public void Normal_Skewness_Tests(double mean, double stDev, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.Skewness;
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_SKEWNESS, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_PDF)]
        public void Normal_PDF_Tests(double mean, double stDev, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.PDF(xVal);
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_PDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_CDF)]
        public void Normal_CDF_Tests(double mean, double stDev, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.CDF(xVal);
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_CDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelNormalData(TestDataRelativePath + "Normal.xlsx", WS_INVERSE_CDF)]
        public void Normal_InverseCDF_Tests(double mean, double stDev, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, stDev);
            double actual = testObj.InverseCDF(xVal);
            bool passed = DidTestPass(actual, expected);
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Normal.xlsx", WS_INVERSE_CDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
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
