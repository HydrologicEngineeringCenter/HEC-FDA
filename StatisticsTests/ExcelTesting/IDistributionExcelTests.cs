using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace StatisticsTests.ExcelTesting
{
    public class IDistributionExcelTests
    {
        private const string TestDataRelativePath = "ExcelTesting\\ExcelData\\";

        private DataTable CreateTestResultTable(double actual, bool passed)
        {
            DataTable dt = new DataTable("TestResult");
            dt.Columns.Add("Actual", typeof(double));
            dt.Columns.Add("Pass/Fail", typeof(string));
            dt.Columns.Add("Date Of Result", typeof(string));
            dt.Rows.Add(new object[] { actual,  passed ? "Passed" : "Failed", DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt") });
            return dt;
        }

        #region Min Tests
        //[Theory]
        //[IDistributionExcelData(TestDataRelativePath + "Beta4Parameters.xlsx", IDistributionTestEnum.Min, Statistics.IDistributionEnum.Beta4Parameters)]
        //public void Beta4Parameters_MinimumTests(double alpha, double beta, double min, double max, int n,  double expected, int testRowIndex, int resultsColumnIndex)
        //{
        //    Statistics.Distributions.Beta4Parameters testObj = new Statistics.Distributions.Beta4Parameters(alpha, beta, min, max - min);
        //    double actual = testObj.Range.Min;
        //    bool passed = actual == expected;
        //    DataTable results = CreateTestResultTable(actual, passed);
        //    IDistributionExcelData.SaveData(TestDataRelativePath + "Beta4Parameters.xlsx", (int)IDistributionTestEnum.Min, testRowIndex, resultsColumnIndex, results);
        //}
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "LogNormal.xlsx", IDistributionTestEnum.Min, Statistics.IDistributionEnum.LogNormal)]
        public void LogNormal_MinimumTests(double mean, double sd, double n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.LogNormal testObj = new Statistics.Distributions.LogNormal(mean, sd);
            double actual = testObj.Range.Min;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(TestDataRelativePath + "LogNormal.xlsx", (int)IDistributionTestEnum.Min, testRowIndex, resultsColumnIndex, results);
        }
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "LogPearsonIII.xlsx", IDistributionTestEnum.Min, Statistics.IDistributionEnum.LogPearsonIII)]
        public void LogPearsonIII_MinimumTests(double mean, double sd, double skew, int n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.LogPearsonIII testObj = new Statistics.Distributions.LogPearsonIII(mean, sd, skew);
            double actual = testObj.Range.Min;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(TestDataRelativePath + "LogPearsonIII.xlsx", (int)IDistributionTestEnum.Min, testRowIndex, resultsColumnIndex, results);
        }
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "Normal.xlsx", IDistributionTestEnum.Min, Statistics.IDistributionEnum.Normal)]
        public void Normal_MinimumTests(double mean, double sd, double n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, sd);
            double actual = testObj.Range.Min;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(TestDataRelativePath + "Normal.xlsx", (int)IDistributionTestEnum.Min, testRowIndex, resultsColumnIndex, results);
        }
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "Triangular.xlsx", IDistributionTestEnum.Min, Statistics.IDistributionEnum.Triangular)]
        public void Triangular_MinimumTests(double mode, double min, double max, int n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Triangular testObj = new Statistics.Distributions.Triangular(min, mode, max);
            double actual = testObj.Range.Min;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(TestDataRelativePath + "Triangular.xlsx", (int)IDistributionTestEnum.Min, testRowIndex, resultsColumnIndex, results);
        }
        //[Theory]
        //[IDistributionExcelData(TestDataRelativePath + "Uniform.xlsx", IDistributionTestEnum.Min, Statistics.IDistributionEnum.Uniform)]
        //public void Uniform_MinimumTests(double min, double max, int n, double expected, int testRowIndex, int resultsColumnIndex)
        //{
        //    Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
        //    double actual = testObj.Range.Min;
        //    bool passed = actual == expected;
        //    DataTable results = CreateTestResultTable(actual, passed);
        //    IDistributionExcelData.SaveData(TestDataRelativePath + "Uniform.xlsx", (int)IDistributionTestEnum.Min, testRowIndex, resultsColumnIndex, results);
        //}
        #endregion

        #region Max Tests
        //[Theory]
        //[IDistributionExcelData(TestDataRelativePath + "Beta4Parameters.xlsx", IDistributionTestEnum.Max, Statistics.IDistributionEnum.Beta4Parameters)]
        //public void Beta4Parameters_MaximumTests(double alpha, double beta, double min, double max, int n, double expected, int testRowIndex, int resultsColumnIndex)
        //{
        //    Statistics.Distributions.Beta4Parameters testObj = new Statistics.Distributions.Beta4Parameters(alpha, beta, min, max - min);
        //    double actual = testObj.Range.Max;
        //    bool passed = actual == expected;
        //    DataTable results = CreateTestResultTable(actual, passed);
        //    IDistributionExcelData.SaveData(
        //        TestDataRelativePath + "Beta4Parameters.xlsx", 
        //        (int)IDistributionTestEnum.Max, testRowIndex, resultsColumnIndex, results);
        //}
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "LogNormal.xlsx", IDistributionTestEnum.Max, Statistics.IDistributionEnum.LogNormal)]
        public void LogNormal_MaximumTests(double mean, double sd, int n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.LogNormal testObj = new Statistics.Distributions.LogNormal(mean, sd);
            double actual = testObj.Range.Max;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(
                TestDataRelativePath + "LogNormal.xlsx", 
                (int)IDistributionTestEnum.Max, testRowIndex, resultsColumnIndex, results);
        }
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "LogPearsonIII.xlsx", IDistributionTestEnum.Max, Statistics.IDistributionEnum.LogPearsonIII)]
        public void LogPearsonIII_MaximumTests(double mean, double sd, double skew, int n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.LogPearsonIII testObj = new Statistics.Distributions.LogPearsonIII(mean, sd, skew);
            double actual = testObj.Range.Max;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(
                TestDataRelativePath + "LogPearsonIII.xlsx", 
                (int)IDistributionTestEnum.Max, testRowIndex, resultsColumnIndex, results);
        }
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "Normal.xlsx", IDistributionTestEnum.Max, Statistics.IDistributionEnum.Normal)]
        public void Normal_MaximumTests(double mean, double sd, int n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Normal testObj = new Statistics.Distributions.Normal(mean, sd);
            double actual = testObj.Range.Max;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(
                TestDataRelativePath + "Normal.xlsx", 
                (int)IDistributionTestEnum.Max, testRowIndex, resultsColumnIndex, results);
        }
        [Theory]
        [IDistributionExcelData(TestDataRelativePath + "Triangular.xlsx", IDistributionTestEnum.Max, Statistics.IDistributionEnum.Triangular)]
        public void Triangular_MaximumTests(double mode, double min, double max, int n, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Triangular testObj = new Statistics.Distributions.Triangular(min, mode, max);
            double actual = testObj.Range.Max;
            bool passed = actual == expected;
            DataTable results = CreateTestResultTable(actual, passed);
            IDistributionExcelData.SaveData(
                TestDataRelativePath + "Triangular.xlsx", 
                (int)IDistributionTestEnum.Max, testRowIndex, resultsColumnIndex, results);


        }

        #endregion


        #region Uniform Tests
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
        public void Uniform_Min_Tests(double mode, double min, double max,double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Range.Min;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MIN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MAX)]
        public void Uniform_Max_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Range.Max;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MAX, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MODE)]
        public void Uniform_Mode_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Mode;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MODE, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MEAN)]
        public void Uniform_Mean_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Mean;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MEAN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_MEDIAN)]
        public void Uniform_Median_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Median;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_MEDIAN, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_VARIANCE)]
        public void Uniform_Variance_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Variance;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_VARIANCE, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_STDEV)]
        public void Uniform_StDev_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.StandardDeviation;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_STDEV, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_SKEWNESS)]
        public void Uniform_Skewness_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.Skewness;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_SKEWNESS, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_SAMPLESIZE)]
        public void Uniform_SampleSize_Tests(double mode, double min, double max, double sampleSize, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.SampleSize;
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_SAMPLESIZE, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_PDF)]
        public void Uniform_PDF_Tests(double mode, double min, double max, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.PDF(xVal);
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_PDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_CDF)]
        public void Uniform_CDF_Tests(double mode, double min, double max, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.CDF(xVal);
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_CDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        [Theory]
        [ExcelUniformData(TestDataRelativePath + "Uniform.xlsx", WS_INVERSE_CDF)]
        public void Uniform_Inverse_CDF_Tests(double mode, double min, double max, double xVal, double expected, int testRowIndex, int resultsColumnIndex)
        {
            Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
            double actual = testObj.InverseCDF(xVal);
            bool passed = actual == expected;
            ExcelDataAttributeBase.SaveData(TestDataRelativePath + "Uniform.xlsx", WS_INVERSE_CDF, testRowIndex, resultsColumnIndex, CreateDataTable(actual), passed);
            Assert.True(passed);
        }

        #endregion


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

    }
}
