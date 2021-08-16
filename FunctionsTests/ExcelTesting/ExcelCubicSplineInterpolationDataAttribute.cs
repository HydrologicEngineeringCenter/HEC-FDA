using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FunctionsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public sealed class ExcelCubicSplineInterpolationDataAttribute : ExcelDataAttributeBase
    {
        protected override List<Type> ColumnTypes {   get;    set; }
        protected override List<DataLength> ColumnDataLengths { get; set; }
        protected override List<int> ColumnIndices { get; set; }
        protected override int ColumnToWriteResults { get; set; }

        public ExcelCubicSplineInterpolationDataAttribute(string fileName, int worksheetNumber) : base(fileName, worksheetNumber)
        {
            ColumnTypes = new List<Type>() { typeof(double), typeof(double), typeof(string), typeof(double), typeof(double) };
            ColumnDataLengths = new List<DataLength> { DataLength.VariableLength, DataLength.VariableLength, DataLength.FirstLineOnly, DataLength.VariableLength, DataLength.VariableLength };
            ColumnIndices = new List<int>() { 1, 2, 3, 4, 6 };
            ColumnToWriteResults = 7;
        }
    }
}
