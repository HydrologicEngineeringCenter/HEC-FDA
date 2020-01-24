using ClosedXML.Excel;
using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace FunctionsTests.ExcelTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ExcelDataAttribute: DataAttribute
    {
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
                object[] test1 = new object[4];

                //x and y values from Excel in columns 1 and 2
                List<double> xs1 = GetValues(dataStartIndex, 1, ws);
                List<double> ys1 = GetValues(dataStartIndex, 2, ws);
                ICoordinatesFunction func1 = ICoordinatesFunctionsFactory.Factory(xs1, ys1, InterpolationEnum.Linear);
                test1[0] = func1;
                //x and y values from Excel in columns 4 and 5
                List<double> xs2 = GetValues(dataStartIndex, 4, ws);
                List<double> ys2 = GetValues(dataStartIndex, 5, ws);
                ICoordinatesFunction func2 = ICoordinatesFunctionsFactory.Factory(xs2, ys2, InterpolationEnum.Linear);
                test1[1] = func2;

                //x and y values from Excel in columns 7 and 8
                List<double> xs3 = GetValues(dataStartIndex, 7, ws);
                List<double> ys3 = GetValues(dataStartIndex, 8, ws);
                ICoordinatesFunction expectedFunc = ICoordinatesFunctionsFactory.Factory(xs3, ys3, InterpolationEnum.Linear);
                test1[2] = expectedFunc;

                IFunction actualFunc = ((IFunction)func1).Compose((IFunction)func2);
                test1[3] = actualFunc;

                listOfTests.Add(test1);

                //write out the actual results to the excel file
                SaveData(workbook, ws, actualFunc, dataStartIndex, 10, expectedFunc.Equals(actualFunc));

                //find the next test
                int firstEmptyRow = FindNextEmptyRowIndex(dataStartIndex, func1, func2);
                int nextTestIndex = FindNextTestIndex(ws, firstEmptyRow);
                if(nextTestIndex == -1)
                {
                    break;
                }
                else
                {
                    dataStartIndex = nextTestIndex;
                }
                //get the longest length from xs and ys from func1 and func2
                //search the next 10 rows
            }

            return listOfTests;

        }

        private static int FindNextTestIndex(IXLWorksheet ws, int startLookingAtRow)
        {
            for(int i = startLookingAtRow; i< startLookingAtRow + 10; i++)
            {
                object value = ws.Row(i).Cell(1).Value;
                double dbl;
                bool isDouble = Double.TryParse(value.ToString(),out dbl);
                if(isDouble)
                {
                    return i;
                }
            }
            return -1;
        }
        
        private static int FindNextEmptyRowIndex(int currentTestStartIndex, ICoordinatesFunction func1, ICoordinatesFunction func2)
        {
            int currentTestNumberOfRows = func1.Coordinates.Count;
            if(func2.Coordinates.Count>func1.Coordinates.Count)
            {
                currentTestNumberOfRows = func2.Coordinates.Count;
            }
            return currentTestNumberOfRows + currentTestStartIndex;
        }

        private static List<double> GetValues(int startRow, int startCol, IXLWorksheet ws)
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

        private static void SaveData(IXLWorkbook wb, IXLWorksheet ws, ICoordinatesFunction function, int rowIndex, int colIndex, bool passedTest)
        {
            DataTable dt = new DataTable("TestTable");
            dt.Columns.Add("XValues");
            dt.Columns.Add("YValues");
            for (int i = 0; i < function.Coordinates.Count; i++)
            {
                double x = function.Coordinates[i].X.Value();
                double y = function.Coordinates[i].Y.Value();
                dt.Rows.Add(x, y);
            }
            ws.Cell(rowIndex - 1, colIndex).Value = "x";
            ws.Cell(rowIndex - 1, colIndex+1).Value = "y";
            //write out if the test passed
            if (passedTest)
            {
                ws.Cell(rowIndex - 1, colIndex+2).Style.Fill.BackgroundColor = XLColor.Green;
                ws.Cell(rowIndex - 1, colIndex + 2).Value = "Passed";
            }
            else
            {
                ws.Cell(rowIndex - 1, colIndex + 2).Style.Fill.BackgroundColor = XLColor.Red;
                ws.Cell(rowIndex - 1, colIndex + 2).Value = "Failed";
            }

            //write out the date of the test
            ws.Cell(rowIndex - 1, colIndex + 3).Value = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
            ws.Column(colIndex+3).AdjustToContents();

            ws.Cell(rowIndex, colIndex).InsertData(dt);
            wb.Save();
        }

        //public static void SaveData(string fileName)
        //{
        //    string filePath = GetApplicationRoot() + "\\" + fileName;
        //    var workbook = new XLWorkbook(filePath);
        //    IXLWorksheet Worksheet = workbook.Worksheet(1);
        //    Worksheet.Cell(10, 2).InsertData(new List<double>() { 777, 88, 99 });
        //    int NumberOfLastRow = Worksheet.LastRowUsed().RowNumber();
        //    IXLCell CellForNewData = Worksheet.Cell(NumberOfLastRow + 1, 1);

        //    //CellForNewData.InsertData("cody test");
        //    workbook.Save();
        //}

        /// <summary>
        /// This is going to save the file in the bin\debug of ExcelDataTesting project
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        public static void SaveDataTableToNewFile(IXLWorkbook workbook, int worksheetNumber, int startRow, int startCol, DataTable dt, string fileName)
        {

        }
    }
}
