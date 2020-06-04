using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StatisticsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public class ExcelTriangularDataAttribute:ExcelDataAttributeBase
    {
        protected override List<Type> ColumnTypes { get; set; }
        protected override List<DataLength> ColumnDataLengths { get; set; }
        protected override List<int> ColumnIndices { get; set; }
        protected override int ColumnToWriteResults { get; set; }

        public ExcelTriangularDataAttribute(string fileName, int worksheetNumber) : base(fileName, worksheetNumber)
        {
            ColumnTypes = new List<Type>() { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };
            ColumnDataLengths = new List<DataLength> { DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly };
            ColumnIndices = new List<int>() { 1, 2, 3, 4, 5 };
            ColumnToWriteResults = 6;
        }

    }
}
