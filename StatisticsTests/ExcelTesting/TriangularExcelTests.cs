using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace StatisticsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]
    public class TriangularExcelTests
    {

        private const string TEST_DATA_RELATIVE_PATH = "ExcelTesting\\ExcelData\\Triangular.xlsx";
        private const int MIN_WORKSHEET = 1;
        private const int MAX_WORKSHEET = 2;
        private const int MODE_WORKSHEET = 3;
        private const int MEAN_WORKSHEET = 4;
        private const int MEDIAN_WORKSHEET = 5;
        private const int VARIANCE_WORKSHEET = 6;
        private const int STDEV_WORKSHEET = 7;
        private const int SKEWNESS_WORKSHEET = 8;
        private const int SAMPLESIZE_WORKSHEET = 9;
        private const int PDF_WORKSHEET = 10;
        private const int CDF_WORKSHEET = 11;
        private const int INVERSE_CDF_WORKSHEET = 12;
        private const int PRINT_WORKSHEET = 13;
        private const int EQUALS_WORKSHEET = 14;


        private DataTable CreateDataTable(double value)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("Actual");
            dt.Rows.Add(value);
            return dt;
        }


        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, MIN_WORKSHEET)]
        public void MinTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.Range.Min;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, MIN_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, MAX_WORKSHEET)]
        public void MaxTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.Range.Max;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, MAX_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, MODE_WORKSHEET)]
        public void ModeTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.Mode;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, MODE_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, MEAN_WORKSHEET)]
        public void MeanTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.Mean;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, MEAN_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, MEDIAN_WORKSHEET)]
        public void MedianTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.Median;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, MEDIAN_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, VARIANCE_WORKSHEET)]
        public void VarianceTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.Variance;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, VARIANCE_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, STDEV_WORKSHEET)]
        public void StandardDeviationTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.StandardDeviation;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, STDEV_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, SKEWNESS_WORKSHEET)]
        public void SkewnessTests(double mode, double min, double max, double other, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.Skewness;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, SKEWNESS_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, SAMPLESIZE_WORKSHEET)]
        public void SampleSizeTests(double mode, double min, double max, double sampleSize, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = null;
            if (sampleSize == -9999)
            {
                tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            }
            else
            {

                tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max, (int)sampleSize);
            }
            double actualValue = tri.SampleSize;
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, SAMPLESIZE_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, PDF_WORKSHEET)]
        public void PDFTests(double mode, double min, double max, double x, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.PDF(x);
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, PDF_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }

        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, CDF_WORKSHEET)]
        public void CDFTests(double mode, double min, double max, double x, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.CDF(x);
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, CDF_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }
        [Theory]
        [ExcelData(TEST_DATA_RELATIVE_PATH, INVERSE_CDF_WORKSHEET)]
        public void InverseCDFTests(double mode, double min, double max, double p, double expectedValue, int rowToWriteTo, int columnToWriteTo)
        {
            Triangular tri = (Triangular)IDistributionFactory.FactoryTriangular(min, mode, max);
            double actualValue = tri.InverseCDF(p);
            bool testPassed = (actualValue == expectedValue);
            ExcelDataAttribute.SaveData(TEST_DATA_RELATIVE_PATH, INVERSE_CDF_WORKSHEET, rowToWriteTo, columnToWriteTo, CreateDataTable(actualValue), testPassed);
            Assert.True(testPassed);
        }
    }
}
