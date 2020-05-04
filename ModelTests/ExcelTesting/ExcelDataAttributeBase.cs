using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace ModelTests.ExcelTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]

    public abstract class ExcelDataAttributeBase : DataAttribute
    {

        protected enum DataLength
        {
            VariableLength,
            FirstLineOnly
        }

        protected abstract List<Type> ColumnTypes
        {
            get;
            set;
        }
        /// <summary>
        /// I have to know if this is a single value column or a column that can be a list. I can't
        /// decide that on the fly because you might be expecting a list of x values and a list of y values
        /// for a given test and only get a single x and a single y. This should still be a valid test but those
        /// single values would need to be put into a list because that is what the unit test is expecting.
        /// </summary>
        protected abstract List<DataLength> ColumnDataLengths { get; set; }
        protected abstract List<int> ColumnIndices { get; set; }
        protected abstract int ColumnToWriteResults { get; set; }

        private bool MultipleWorksheets { get; set; }
        private int[] Worksheets { get; set; }

        public string FileName { get; private set; }
        public int WorksheetNumber { get; }
        public ExcelDataAttributeBase(string fileName, int worksheetNumber)
        {
            FileName = fileName;
            WorksheetNumber = worksheetNumber;
            MultipleWorksheets = false;
        }

        public ExcelDataAttributeBase(string fileName, int[] worksheetNumbers)
        {
            MultipleWorksheets = true;
            FileName = fileName;
            Worksheets = worksheetNumbers;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException("testMethod");

            ParameterInfo[] pars = testMethod.GetParameters();
            return DataSource(FileName);
        }

        private object[] ReadTestData(int dataStartIndex, IXLWorksheet ws, int wsNumber, out int lastIndex)
        {
            int rowToWriteResultsTo = dataStartIndex - 1;
            int highestNumberOfRowsInTest = 1;
            //these are the parameters that will get passed into the test method
            object[] test1 = new object[ColumnIndices.Count + 3];
            if (!MultipleWorksheets)
            {
                //doesnt need the worksheet number added to test
                test1 = new object[ColumnIndices.Count + 2];
            }

            int i = 0;
            for (i = 0; i < ColumnIndices.Count; i++)
            {
                int colNumber = ColumnIndices[i];
                if (ColumnTypes[i] == typeof(double))
                {
                    switch (ColumnDataLengths[i])
                    {
                        case DataLength.VariableLength:
                            {
                                List<double> doubleValues = GetDoubleValuesVariableLength(dataStartIndex, colNumber, ws);
                                test1[i] = doubleValues;
                                if (doubleValues.Count > highestNumberOfRowsInTest)
                                {
                                    highestNumberOfRowsInTest = doubleValues.Count;
                                }
                                break;
                            }
                        case DataLength.FirstLineOnly:
                            {
                                double dbl = GetDoubleValueSingleRow(dataStartIndex, colNumber, ws);
                                test1[i] = dbl;
                                break;
                            }
                    }

                }
                else if (ColumnTypes[i] == typeof(int))
                {
                    switch (ColumnDataLengths[i])
                    {
                        case DataLength.VariableLength:
                            {
                                List<int> intValues = GetIntValuesVariableLength(dataStartIndex, colNumber, ws);
                                test1[i] = intValues;
                                if (intValues.Count > highestNumberOfRowsInTest)
                                {
                                    highestNumberOfRowsInTest = intValues.Count;
                                }
                                break;
                            }
                        case DataLength.FirstLineOnly:
                            {
                                int intValue = GetIntValueSingleRow(dataStartIndex, colNumber, ws);
                                test1[i] = intValue;
                                break;
                            }
                    }
                }
                else if (ColumnTypes[i] == typeof(string))
                {
                    switch (ColumnDataLengths[i])
                    {
                        case DataLength.VariableLength:
                            {
                                List<string> stringVals = GetStringValuesVariableLength(dataStartIndex, colNumber, ws);
                                test1[i] = stringVals;
                                if (stringVals.Count > highestNumberOfRowsInTest)
                                {
                                    highestNumberOfRowsInTest = stringVals.Count;
                                }
                                break;
                            }
                        case DataLength.FirstLineOnly:
                            {
                                string s = GetStringValueSingleRow(dataStartIndex, colNumber, ws);
                                test1[i] = s;
                                break;
                            }
                    }
                }
            }

            if (MultipleWorksheets)
            {
                test1[i] = wsNumber;
                test1[i + 1] = rowToWriteResultsTo;
                test1[i + 2] = ColumnToWriteResults;
            }
            else
            {
                test1[i] = rowToWriteResultsTo;
                test1[i + 1] = ColumnToWriteResults;
            }

            lastIndex = dataStartIndex + highestNumberOfRowsInTest;
            return test1;
        }

        public IEnumerable<object[]> DataSource(string fileName)
        {
            string filePath = GetApplicationRoot() + "\\" + fileName;
            var workbook = new XLWorkbook(filePath);
            //the worksheets and the rows are not 0 based. They start on 1.
            //IXLWorksheet ws = workbook.Worksheet(WorksheetNumber);

            //int dataStartIndex = FindNextTestIndex(ws, 1);

            //List<object[]> listOfTests = new List<object[]>();

            //bool moreExcelTestsToRun = true;
            //while (moreExcelTestsToRun)
            //{
            //    int lastRowOfTest = -1;
            //    object[] nextTest = ReadTestData(dataStartIndex, ws, out lastRowOfTest);
            //    listOfTests.Add(nextTest);

            //    int nextTestIndex = FindNextTestIndex(ws, lastRowOfTest);
            //    if (nextTestIndex == -1)
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        dataStartIndex = nextTestIndex;
            //    }
            //}

            //return listOfTests;
            List<object[]> tests = new List<object[]>();
            if (MultipleWorksheets)
            {
                foreach (int wsNumber in Worksheets)
                {
                    tests.AddRange(GetTestsForWorksheet(workbook, wsNumber));
                }
            }
            else
            {
                tests.AddRange(GetTestsForWorksheet(workbook, WorksheetNumber));
            }
            return tests;
        }

        private IEnumerable<object[]> GetTestsForWorksheet(XLWorkbook workbook, int worksheetNumber)
        {
            //the worksheets and the rows are not 0 based. They start on 1.
            IXLWorksheet ws = workbook.Worksheet(worksheetNumber);

            int dataStartIndex = FindNextTestIndex(ws, 1);

            List<object[]> listOfTests = new List<object[]>();

            bool moreExcelTestsToRun = true;
            while (moreExcelTestsToRun)
            {
                int lastRowOfTest = -1;
                object[] nextTest = ReadTestData(dataStartIndex, ws, worksheetNumber, out lastRowOfTest);
                listOfTests.Add(nextTest);

                int nextTestIndex = FindNextTestIndex(ws, lastRowOfTest);
                if (nextTestIndex == -1)
                {
                    break;
                }
                else
                {
                    dataStartIndex = nextTestIndex;
                }
            }

            return listOfTests;
        }

        protected static int FindNextTestIndex(IXLWorksheet ws, int startLookingAtRow)
        {
            for (int i = startLookingAtRow; i < startLookingAtRow + 15; i++)
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

        //protected static int FindNextEmptyRowIndex(int currentTestStartIndex, List<double> xs1, List<double> ys1, List<double> xs2, List<double> ys2)
        //{
        //    //get the longest list so that we can start looking for other tests after that.
        //    List<int> listOfCount = new List<int>() { xs1.Count, ys1.Count, xs2.Count, ys2.Count };
        //    int currentTestNumberOfRows = listOfCount.Max();

        //    return currentTestNumberOfRows + currentTestStartIndex;
        //}
        protected static string GetStringValueSingleRow(int startRow, int startCol, IXLWorksheet ws)
        {
            object value = ws.Row(startRow).Cell(startCol).Value;
            return value.ToString();

        }
        protected static List<string> GetStringValuesVariableLength(int startRow, int startCol, IXLWorksheet ws)
        {
            List<string> values = new List<string>();
            object value = ws.Row(startRow).Cell(startCol).Value;
            while (value != "")
            {
                values.Add(Convert.ToString(value));
                startRow++;
                value = ws.Row(startRow).Cell(startCol).Value;
            }
            return values;
        }
        protected static List<int> GetIntValuesVariableLength(int startRow, int startCol, IXLWorksheet ws)
        {
            List<int> values = new List<int>();
            object value = ws.Row(startRow).Cell(startCol).Value;
            while (value != "")
            {
                values.Add(Convert.ToInt32(value));
                startRow++;
                value = ws.Row(startRow).Cell(startCol).Value;
            }
            return values;
        }
        protected static int GetIntValueSingleRow(int startRow, int startCol, IXLWorksheet ws)
        {
            object value = ws.Row(startRow).Cell(startCol).Value;
            return Convert.ToInt32(value);

        }

        protected static List<double> GetDoubleValuesVariableLength(int startRow, int startCol, IXLWorksheet ws)
        {
            List<double> values = new List<double>();
            object value = ws.Row(startRow).Cell(startCol).Value;
            while (value != "")
            {
                values.Add(Convert.ToDouble(value));
                startRow++;
                value = ws.Row(startRow).Cell(startCol).Value;
            }
            return values;
        }
        protected static double GetDoubleValueSingleRow(int startRow, int startCol, IXLWorksheet ws)
        {
            object value = ws.Row(startRow).Cell(startCol).Value;
            return Convert.ToDouble(value);

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
