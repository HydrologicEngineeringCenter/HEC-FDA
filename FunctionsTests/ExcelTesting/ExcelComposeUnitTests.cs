using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]
    public class ExcelComposeUnitTests
    {
        private readonly ITestOutputHelper output;

        private const string _TestDataRelativePath = "ExcelTesting\\ExcelData\\ComposeTestData.xlsx";
        private const double _InvalidOperationException = -9999;

        public ExcelComposeUnitTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private DataTable CreateDataTable(ICoordinatesFunction actualFunction)
        {
            DataTable dt = new DataTable("DataTable");
            dt.Columns.Add("XValues");
            dt.Columns.Add("YValues");
            for (int i = 0; i < actualFunction.Coordinates.Count; i++)
            {
                ICoordinate coord = actualFunction.Coordinates[i];
                dt.Rows.Add(coord.X.Value(), coord.Y.Value());
            }

            return dt;
        }

        [Theory]
        [ExcelDataCompose(_TestDataRelativePath, 1)]
        public void ExcelComposeLinearInterpTests(List<double> xs1, List<double> ys1, List<double> xs2, List<double> ys2,
           List<double> expectedxs, List<double> expectedys, int rowToWriteTo, int columnToWriteTo )
        //List<double> expectedxs, List<double> expectedys, IXLWorkbook workBook)
        {
            IFunction func1 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs1, ys1, InterpolationEnum.Linear);
            //i am switching the xs and ys because chris hamilton switched those columns around.
            IFunction func2 = (IFunction)ICoordinatesFunctionsFactory.Factory( ys2,xs2, InterpolationEnum.Linear);
            ICoordinatesFunction expectedFunc = ICoordinatesFunctionsFactory.Factory(expectedxs,expectedys, InterpolationEnum.Linear);

            if (expectedFunc.Coordinates[0].X.Value() == _InvalidOperationException)
            {
                Assert.Throws<InvalidOperationException>(() => func1.Compose(func2));
                DataTable dt = new DataTable();
                dt.Columns.Add("XValues");
                dt.Columns.Add("YValues");
                
                dt.Rows.Add("InvalidOperationException", "");
                
                ExcelDataComposeAttribute.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, dt, true);

            }
            else
            {
                ICoordinatesFunction actualFunc = func1.Compose(func2);
                bool testPassed = expectedFunc.Equals(actualFunc);
                ExcelDataComposeAttribute.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, CreateDataTable(actualFunc), testPassed);

                Assert.True(testPassed);


            }

            //testing how to write to the excel file
            // DataTable dt = new DataTable("TestTable");
            // dt.Columns.Add("XValues");
            // dt.Columns.Add("YValues");
            // for(int i = 0;i<expectedxs.Count;i++)
            // {
            //     dt.Rows.Add(xs[i], ys[i]);
            // }

            // var ws = workBook.Worksheet(1);

            //// ws.Cell(4, 10).Value = dt.AsEnumerable();
            // var rangeWithData = ws.Cell(4, 10).InsertData(dt.AsEnumerable());
            // string directory = ExcelDataAttribute.GetFullDirectoryPath();
            // string newFilePath = directory + "\\ComposeTestDataResults.xlsx";
            // workBook.SaveAs(newFilePath);
            // int j = 0;
        }

        [Theory]
        [ExcelDataCompose(_TestDataRelativePath, 2)]
        public void ExcelComposeSplineInterpTests(List<double> xs1, List<double> ys1, List<double> xs2, List<double> ys2,
           List<double> expectedxs, List<double> expectedys, int rowToWriteTo, int columnToWriteTo)
        //List<double> expectedxs, List<double> expectedys, IXLWorkbook workBook)
        {
            IFunction func1 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs1, ys1, InterpolationEnum.NaturalCubicSpline);
            //i am switching the xs and ys because chris hamilton switched those columns around.
            IFunction func2 = (IFunction)ICoordinatesFunctionsFactory.Factory( ys2,xs2, InterpolationEnum.NaturalCubicSpline);
            ICoordinatesFunction expectedFunc = ICoordinatesFunctionsFactory.Factory(expectedxs, expectedys, InterpolationEnum.NaturalCubicSpline);

            WriteFunctionToOutput(func1, "Function1");
            WriteFunctionToOutput(func2, "Function2");

            if (expectedFunc.Coordinates[0].X.Value() == _InvalidOperationException)
            {
                Assert.Throws<InvalidOperationException>(() => func1.Compose(func2));

                DataTable dt = new DataTable();
                dt.Columns.Add("XValues");
                dt.Columns.Add("YValues");

                dt.Rows.Add("InvalidOperationException", "");

                ExcelDataComposeAttribute.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, dt, true);

            }
            else
            {
                ICoordinatesFunction actualFunc = func1.Compose(func2);
                WriteFunctionToOutput(actualFunc, "Composed Function");

                bool testPassed = expectedFunc.Equals(actualFunc);
                ExcelDataComposeAttribute.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, CreateDataTable(actualFunc), testPassed);

                Assert.True(testPassed);


            }

            //testing how to write to the excel file
            // DataTable dt = new DataTable("TestTable");
            // dt.Columns.Add("XValues");
            // dt.Columns.Add("YValues");
            // for(int i = 0;i<expectedxs.Count;i++)
            // {
            //     dt.Rows.Add(xs[i], ys[i]);
            // }

            // var ws = workBook.Worksheet(1);

            //// ws.Cell(4, 10).Value = dt.AsEnumerable();
            // var rangeWithData = ws.Cell(4, 10).InsertData(dt.AsEnumerable());
            // string directory = ExcelDataAttribute.GetFullDirectoryPath();
            // string newFilePath = directory + "\\ComposeTestDataResults.xlsx";
            // workBook.SaveAs(newFilePath);
            // int j = 0;
        }


        private void WriteFunctionToOutput(ICoordinatesFunction function, string name)
        {
            output.WriteLine("");
            output.WriteLine("Function: " + name);
            foreach (ICoordinate coord in function.Coordinates)
            {
                output.WriteLine( coord.X.Value() + ", " + coord.Y.Value());
            }

        }

    }
}
