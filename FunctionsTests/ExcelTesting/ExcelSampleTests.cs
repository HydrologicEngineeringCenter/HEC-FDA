using Functions;
using Functions.CoordinatesFunctions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]
    public class ExcelSampleTests
    {
        private  const string _TestDataRelativePath = "ExcelTesting\\ExcelData\\ComposeTestData.xlsx";
        
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
        [ExcelData(_TestDataRelativePath, 1)]
        public void ExcelXlsTests(List<double> xs1, List<double> ys1, List<double> xs2, List<double> ys2,
           List<double> expectedxs, List<double> expectedys, int rowToWriteTo, int columnToWriteTo )
        //List<double> expectedxs, List<double> expectedys, IXLWorkbook workBook)
        {
            IFunction func1 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs1, ys1, InterpolationEnum.Linear);
            IFunction func2 = (IFunction)ICoordinatesFunctionsFactory.Factory(xs2, ys2, InterpolationEnum.Linear);
            ICoordinatesFunction expectedFunc = ICoordinatesFunctionsFactory.Factory(expectedxs,expectedys, InterpolationEnum.Linear);

            ICoordinatesFunction actualFunc = func1.Compose(func2);
            bool testPassed = expectedFunc.Equals(actualFunc);
            ExcelDataAttribute.SaveData(_TestDataRelativePath, 1, rowToWriteTo, columnToWriteTo, CreateDataTable(actualFunc), testPassed);
            
            Assert.True(testPassed);

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
    }
}
