
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
