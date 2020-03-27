using ClosedXML.Excel;
using Statistics;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Sdk;

namespace StatisticsTests.ExcelTesting.ExcelData
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class IDistributionExcelData : DataAttribute
    {
        #region Fields
        /* The next field is retrieved by
         * (1) Getting the location of the code that is running (from assembly)
         * (2) Getting the directory of the location from step 1
         * (3) Turning the directory into a file path string
         */
        public static string ApplicationRoot => new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)").Match(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).Value;       
        #endregion

        #region Properties
        public string ExcelFilePath { get; }
        public int ExcelWorksheetNumber { get; }
        public IDistributionTestEnum Test { get; }
        public Statistics.IDistributionEnum Distribution { get; }
        #endregion

        #region Constructor
        public IDistributionExcelData(string relativePath, IDistributionTestEnum test, IDistributionEnum distribution)
        {
            ExcelFilePath = ApplicationRoot + "\\" + relativePath;
            ExcelWorksheetNumber = SetExcelWorksheetIndex(distribution, test);
            Test = test;
            Distribution = distribution;
        }
        private int SetExcelWorksheetIndex(IDistributionEnum distribution, IDistributionTestEnum test)
        {
            switch (distribution)
            {
                case IDistributionEnum.Beta4Parameters:
                    switch (test) 
                    {
                        case IDistributionTestEnum.Alpha: 
                            return 1;
                        case IDistributionTestEnum.Beta: 
                            return 2;
                        case IDistributionTestEnum.Mode: 
                            return 3;
                        case IDistributionTestEnum.Mean: 
                            return 4;
                        case IDistributionTestEnum.Median: 
                            return 5;
                        case IDistributionTestEnum.Variance: 
                            return 6;
                        case IDistributionTestEnum.StandardDevaiation:
                            return 7;
                        case IDistributionTestEnum.Skewness:
                            return 8;
                        case IDistributionTestEnum.Kurtosis:
                            return 9;
                        case IDistributionTestEnum.SampleSize:
                            return 10;
                        case IDistributionTestEnum.PDF:
                            return 11;
                        case IDistributionTestEnum.CDF:
                            return 12;
                        case IDistributionTestEnum.InverseCDF:
                            return 13;
                        case IDistributionTestEnum.Print:
                            return 14;
                        case IDistributionTestEnum.Equals:
                            return 15;
                        default:
                            throw new NotImplementedException();
                    }
                case IDistributionEnum.LogNormal:
                case IDistributionEnum.LogPearsonIII: 
                    switch (test)
                    {
                        case IDistributionTestEnum.Min:
                            return 1;
                        case IDistributionTestEnum.Max:
                            return 2;
                        case IDistributionTestEnum.Mode:
                            return 3;
                        case IDistributionTestEnum.Mean:
                            return 4;
                        case IDistributionTestEnum.Median:
                            return 5;
                        case IDistributionTestEnum.Variance:
                            return 6;
                        case IDistributionTestEnum.StandardDevaiation:
                            return 7;
                        case IDistributionTestEnum.Skewness:
                            return 8;
                        case IDistributionTestEnum.Kurtosis:
                            return 9;
                        case IDistributionTestEnum.SampleSize:
                            return 10;
                        case IDistributionTestEnum.PDF:
                            return 11;
                        case IDistributionTestEnum.CDF:
                            return 12;
                        case IDistributionTestEnum.InverseCDF:
                            return 13;
                        case IDistributionTestEnum.Print:
                            return 14;
                        case IDistributionTestEnum.Equals:
                            return 15;
                        default:
                            throw new NotImplementedException();
                    }
                case IDistributionEnum.Normal:
                case IDistributionEnum.Triangular:
                case IDistributionEnum.Uniform:
                    switch (test)
                    {
                        case IDistributionTestEnum.Min:
                            return 1;
                        case IDistributionTestEnum.Max:
                            return 2;
                        case IDistributionTestEnum.Mode:
                            return 3;
                        case IDistributionTestEnum.Mean:
                            return 4;
                        case IDistributionTestEnum.Median:
                            return 5;
                        case IDistributionTestEnum.Variance:
                            return 6;
                        case IDistributionTestEnum.StandardDevaiation:
                            return 7;
                        case IDistributionTestEnum.Skewness:
                            return 8;
                        case IDistributionTestEnum.SampleSize:
                            return 9;
                        case IDistributionTestEnum.PDF:
                            return 10;
                        case IDistributionTestEnum.CDF:
                            return 11;
                        case IDistributionTestEnum.InverseCDF:
                            return 12;
                        case IDistributionTestEnum.Print:
                            return 13;
                        case IDistributionTestEnum.Equals:
                            return 14;
                        default:
                            throw new NotImplementedException();
                    }
                default:
                    throw new NotImplementedException();
            }

        }
        #endregion

        #region Functions
        #region DataAttribute Functions
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) throw new ArgumentNullException(nameof(testMethod));
            ParameterInfo[] parameters = testMethod.GetParameters();
            IXLWorksheet worksheet = new XLWorkbook(ExcelFilePath).Worksheet(ExcelWorksheetNumber);
            var tests = GatherTestsData(worksheet, Test, Distribution);
            return tests;
        }
        private IEnumerable<object[]> GatherTestsData(IXLWorksheet worksheet, IDistributionTestEnum test, IDistributionEnum distribution)
        {
            int testRow = 0;
            List<object[]> tests = new List<object[]>();
            while (testRow > -1)
            {
                testRow = FindNextTest(worksheet, testRow, distribution);
                if (testRow > -1) tests.Add(GatherTestParameters(worksheet, testRow, distribution));  
            }
            return tests;
        }
        private int FindNextTest(IXLWorksheet worksheet, int startRow, IDistributionEnum distribution)
        {
            /* This method finds the row index of the next test...
             *      STARTS looking in below the last test row (last test row + 1) 
             *      OR ROW 1 (NOTE: Excel indexes are 1 - not 0 based).
             */
            for (int i = startRow + 1; i < startRow + 10; i++)
            {
                /* Look in first column of i_th row and test if value is double
                 *      IF true THEN use row index to 
                 *      ELSE keep looking over next 10 rows
                 */
                string val = worksheet.Row(i).Cell(1).Value.ToString();
                if (Double.TryParse(worksheet.Row(i).Cell(1).Value.ToString(), out double x)) return i;
            }
            return -1;
        }
        private object[] GatherTestParameters(IXLWorksheet worksheet, int rowIndex, IDistributionEnum distribution)
        {

            //List<object> parameters = new List<object>();
            switch (distribution) 
            {
                
                case IDistributionEnum.Beta4Parameters:
                    /* 6 Parameters (+3 result columns):
                     *  0 - alpha
                     *  1 - beta
                     *  2 - min
                     *  3 - max
                     *  4 - sample size or function parameter (e.g. p or x)
                     *  5 - expected result
                     *  -------------------
                     *  6 - row index of test
                     *  7 - column index of results = row 7 (excel rows are not 0 based they start at 1) 
                     */
                    int betaResultsColumnIndex = 7;
                    List<object> betaparameters = new List<object>();
                    for (int i = 0; i < betaResultsColumnIndex - 1; i++)
                    { 
                        betaparameters.Add(CellValue(worksheet, rowIndex, i + 1));
                    }
                    betaparameters.Add(rowIndex);
                    betaparameters.Add(betaResultsColumnIndex);
                    return betaparameters.ToArray();
                case IDistributionEnum.LogPearsonIII:
                    /* 5 Parameters (+3 result columns):
                     *  0 - mean
                     *  1 - standard deviation
                     *  2 - skew
                     *  3 - sample size or function parameter (e.g. p or x)
                     *  4 - expected result
                     *  -------------------
                     *  5 - row index of test
                     *  6 - column index of results = row 11 (excel rows are not 0 based they start at 1 & 5 extra rows are reserved for an alternative parameterizaiton of the lp3 distribution) 
                     */
                    int lp3ResultsColumnIndex = 6;
                    List<object> lp3parameters = new List<object>();
                    for (int i = 0; i < lp3ResultsColumnIndex - 1; i++)
                    {
                        lp3parameters.Add(CellValue(worksheet, rowIndex, i + 1));
                    }
                    lp3parameters.Add(rowIndex);
                    lp3parameters.Add(lp3ResultsColumnIndex);
                    return lp3parameters.ToArray();
                case IDistributionEnum.Triangular:
                    /* 5 Parameters (+3 result columns):
                     *  0 - mode
                     *  1 - min
                     *  2 - max
                     *  3 - sample size or function parameter (e.g. p or x)
                     *  4 - expected result
                     *  -------------------
                     *  5 - row index of test
                     *  6 - column index of results row 6 (excel rows are not 0 based they start at 1) 
                     */
                    int triResultsColumnIndex = 6;
                    List<object> triparameters = new List<object>();
                    for (int i = 0; i < triResultsColumnIndex - 1; i++)
                    {
                        triparameters.Add(CellValue(worksheet, rowIndex, i + 1));
                    }
                    triparameters.Add(rowIndex);
                    triparameters.Add(triResultsColumnIndex);
                    return triparameters.ToArray();
                case IDistributionEnum.Normal:
                    /* 4 Parameters:
                     *  0 - mean
                     *  1 - standard deviation
                     *  2 - sample size or function parameter (e.g. p or x)
                     *  3 - expected result
                     *  -------------------
                     *  results begin on row 5 (excel rows are not 0 based they start at 1) 
                     */
                    int normResultsColumnIndex = 5;
                    List<object> normparameters = new List<object>();
                    for (int i = 0; i < normResultsColumnIndex - 1; i++)
                    {
                        normparameters.Add(CellValue(worksheet, rowIndex, i + 1));
                    }
                    normparameters.Add(rowIndex);
                    normparameters.Add(normResultsColumnIndex);
                    return normparameters.ToArray();
                case IDistributionEnum.LogNormal:
                    /* 4 Parameters:
                     *  0 - mean
                     *  1 - standard deviation
                     *  2 - sample size or function parameter (e.g. p or x)
                     *  3 - expected result*  
                     *  -------------------
                     *  results begin on row 5 (excel rows are not 0 based they start at 1) 
                     */
                    int lognormResultsColumnIndex = 5;
                    List<object> lognormparameters = new List<object>(); 
                    for (int i = 0; i < lognormResultsColumnIndex - 1; i++)
                    {
                        lognormparameters.Add(CellValue(worksheet, rowIndex, i + 1));
                    }
                    lognormparameters.Add(rowIndex);
                    lognormparameters.Add(lognormResultsColumnIndex);
                    return lognormparameters.ToArray();
                case IDistributionEnum.Uniform:
                    /* 4 Parameters:
                     * 0 - minimum
                     * 1 - maximum
                     * 2 - sample size or function parameter
                     * 3 - expected result
                     * -------------------------------------
                     */

                default:
                    throw new NotSupportedException($"The unsupported {distribution} type was specified causing an error.");
            }
        }
        private double CellValue(IXLWorksheet worksheet, int rowIndex, int colIndex)
        {
            object value = worksheet.Row(rowIndex).Cell(colIndex).Value;
            return value == "" ? double.NaN : Convert.ToDouble(value);
        }
        #endregion

        public static void SaveData(string relativePath, int worksheetIndex, int rowIndex, int colIndex, DataTable results)
        {
            IXLWorkbook book = new XLWorkbook(ApplicationRoot + "\\" + relativePath);
            IXLWorksheet worksheet = book.Worksheet(worksheetIndex);
            for (int i = 0; i < results.Columns.Count; i++) worksheet.Cell(rowIndex, colIndex + i).Value = results.Rows[0].ItemArray[i];
            worksheet.Cell(rowIndex, colIndex + 1).Style.Fill.BackgroundColor = (string)worksheet.Cell(rowIndex, colIndex + 1).Value == "Passed" ? XLColor.Green : XLColor.Red;
            book.Save();
        }
        #endregion

    }
}
