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
        //[Theory]
        //[IDistributionExcelData(TestDataRelativePath + "Uniform.xlsx", IDistributionTestEnum.Max, Statistics.IDistributionEnum.Uniform)]
        //public void Uniform_MaximumTests(double min, double max, int n, double expected, int testRowIndex, int resultsColumnIndex)
        //{
        //    Statistics.Distributions.Uniform testObj = new Statistics.Distributions.Uniform(min, max);
        //    double actual = testObj.Range.Max;
        //    bool passed = actual == expected;
        //    DataTable results = CreateTestResultTable(actual, passed);
        //    IDistributionExcelData.SaveData(
        //        TestDataRelativePath + "Uniform.xlsx", 
        //        (int)IDistributionTestEnum.Max, testRowIndex, resultsColumnIndex, results);
        //}
        #endregion
    }
}
