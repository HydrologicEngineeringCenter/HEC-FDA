using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public sealed class ExcelOrderDataAttribute : ExcelDataAttributeBase
    {
        protected override List<Type> ColumnTypes { get; set; }     
        protected override List<DataLength> ColumnDataLengths { get; set; }
        protected override List<int> ColumnIndices { get; set; }
        protected override int ColumnToWriteResults { get; set; }

        public ExcelOrderDataAttribute(string fileName, int worksheetNumber) : base(fileName, worksheetNumber)
        {
            ColumnTypes = new List<Type>() { typeof(double), typeof(double), typeof(string), typeof(string) };
            ColumnDataLengths = new List<DataLength> { DataLength.VariableLength, DataLength.VariableLength, DataLength.FirstLineOnly, DataLength.FirstLineOnly };
            ColumnIndices = new List<int>() { 1, 2, 3, 5 };
            ColumnToWriteResults = 6;
        }
    }
}
