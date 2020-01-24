using Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionsTests.ExcelTesting
{
    public class ExcelSampleTests
    {
        [Theory]
        [ExcelData("ExcelTesting\\ExcelData\\ComposeTestData.xlsx", 1)]
        public void ExcelXlsTests(ICoordinatesFunction func1, ICoordinatesFunction func2,
           ICoordinatesFunction expectedFunc, ICoordinatesFunction actualFunc)
        //List<double> expectedxs, List<double> expectedys, IXLWorkbook workBook)
        {
            //ExcelDataAttribute.SaveData("ExcelTesting\\ExcelData\\ComposeTestData.xlsx");
            Assert.True(expectedFunc.Equals(actualFunc));
            int test = 0;
            //testing how to write to the excel file
            // DataTable dt = new DataTable("TestTable");
            // dt.Columns.Add("XValues");
            // dt.Columns.Add("YValues");
            // for(int i = 0;i<expectedxs.Count;i++)
            // {
            //     dt.Rows.Add(xs[i], ys[i]);
            // }

            // ExcelDataAttribute.SaveDataTableToNewFile(workBook,1,)
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
