using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace StatisticsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExcelDataAttribute : DataAttribute
    {
        private const int MODE_COL = 1;
        private const int MIN_COL = 2;
        private const int MAX_COL = 3;
        private const int OTHER_COL = 4;
        private const int EXPECTED_COL = 5;
        private const int ACTUAL_COL = 6;


        public string FileName { get; private set; }
        public int WorksheetNumber { get; }

        public ExcelDataAttribute(string fileName, int worksheetNumber)
        {
            FileName = fileName;
            WorksheetNumber = worksheetNumber;
        }



        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");

            ParameterInfo[] pars = testMethod.GetParameters();
            return DataSource(FileName);
        }

        IEnumerable<object[]> DataSource(string fileName)
        {
            string filePath = GetApplicationRoot() + "\\" + fileName;
            var workbook = new XLWorkbook(filePath);
            //the worksheets and the rows are not 0 based. They start on 1.
            var ws = workbook.Worksheet(WorksheetNumber);
            int dataStartIndex = FindNextTestIndex(ws, 1);

            List<object[]> listOfTests = new List<object[]>();

            bool moreExcelTestsToRun = true;
            while (moreExcelTestsToRun)
            {
                //these are the parameters that will get passed into the test method
                object[] test = new object[7];

                //x and y values from Excel in columns 1 and 2
                double mode = GetValue(dataStartIndex, MODE_COL, ws);
                double min = GetValue(dataStartIndex, MIN_COL, ws);
                double max = GetValue(dataStartIndex, MAX_COL, ws);

                double other = -9999;
                if(IsThereAValue(dataStartIndex, OTHER_COL, ws))
                {
                    other = GetValue(dataStartIndex, OTHER_COL, ws);
                }

                double expected = GetValue(dataStartIndex, EXPECTED_COL, ws);

                test[0] = mode;
                test[1] = min;
                test[2] = max;
                test[3] = other;
                test[4] = expected;

                test[5] = dataStartIndex - 1;
                test[6] = ACTUAL_COL;

                //add the test to the list and look for another
                listOfTests.Add(test);

                //find the next test
                //int firstEmptyRow = FindNextEmptyRowIndex(dataStartIndex, xs1, ys1, xs2, ys2);
                int nextTestIndex = FindNextTestIndex(ws, dataStartIndex + 1);
                if (nextTestIndex == -1)
                {
                    //no more tests, break out of the loop
                    break;
                }
                else
                {
                    dataStartIndex = nextTestIndex;
                }
            }

            return listOfTests;

        }

        private static int FindNextTestIndex(IXLWorksheet ws, int startLookingAtRow)
        {
            for (int i = startLookingAtRow; i < startLookingAtRow + 10; i++)
            {
                object value = ws.Row(i).Cell(1).Value;
                double dbl;
                bool isDouble = Double.TryParse(value.ToString(), out dbl);
                if (isDouble)
                {
                    return i;
                }
            }
            return -1;
        }

        //private static int FindNextEmptyRowIndex(int currentTestStartIndex, List<double> xs1, List<double> ys1, List<double> xs2, List<double> ys2)
        //{
        //    //get the longest list so that we can start looking for other tests after that.
        //    List<int> listOfCount = new List<int>() { xs1.Count, ys1.Count, xs2.Count, ys2.Count };
        //    int currentTestNumberOfRows = listOfCount.Max();

        //    return currentTestNumberOfRows + currentTestStartIndex;
        //}

        private static bool IsThereAValue(int startRow, int startCol, IXLWorksheet ws)
        {
            bool retval = false;
            object value = ws.Row(startRow).Cell(startCol).Value;
            if (value != "")
            {
                retval = true;
            }
            return retval;
        }

        private static double GetValue(int startRow, int startCol, IXLWorksheet ws)
        {
            double retval = -1;
            object value = ws.Row(startRow).Cell(startCol).Value;
            if (value != "")
            {
                retval = Convert.ToDouble(value);
            }
            return retval;
        }


        private static object[] GetRowValues(IXLRow row)
        {
            object[] retval = new object[3];
            retval[0] = Convert.ToInt32(row.Cell(1).Value);
            retval[1] = (string)row.Cell(2).Value;
            retval[2] = (string)row.Cell(3).Value;
            return retval;
        }

        static string GetFullFilename(string filename)
        {
            string d = GetApplicationRoot();
            string dir = Directory.GetCurrentDirectory();
            string executable = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(executable), filename));
        }
        public static string GetFullDirectoryPath()
        {
            string executable = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return Path.GetDirectoryName(executable);
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static void SaveData(string relativePath, int worksheetNum, int row, int col, DataTable data, bool testPassed)
        {
            //The column headers, the test result, and the timestamp will start at row. The data in the table will start at row + 1.
            string filePath = GetApplicationRoot() + "\\" + relativePath;
            var workbook = new XLWorkbook(filePath);
            var ws = workbook.Worksheet(worksheetNum);

            //write out the data table columns
            for (int i = 0; i < data.Columns.Count; i++)
            {

                ws.Cell(row, col + i).Value = data.Columns[i].ColumnName;

            }
            //write out the data table
            ws.Cell(row + 1, col).InsertData(data);

            //write out if the test passed
            int colForTestResult = col + data.Columns.Count;
            if (testPassed)
            {
                ws.Cell(row, colForTestResult).Value = "Passed";
                ws.Cell(row, colForTestResult).Style.Fill.BackgroundColor = XLColor.Green;
            }
            else
            {
                ws.Cell(row, colForTestResult).Value = "Failed";
                ws.Cell(row, colForTestResult).Style.Fill.BackgroundColor = XLColor.Red;
            }

            //write out the timestamp
            ws.Cell(row, colForTestResult + 1).Value = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");


            workbook.Save();
        }
    }
}
