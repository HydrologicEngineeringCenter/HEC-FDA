using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StatisticsTests.ExcelTesting
{
    [ExcludeFromCodeCoverage]

    public class ExcelUniformDataAttribute : ExcelDataAttributeBase
    {
        protected override List<Type> ColumnTypes { get; set; }
        protected override List<DataLength> ColumnDataLengths { get; set; }
        protected override List<int> ColumnIndices { get; set; }
        protected override int ColumnToWriteResults { get; set; }

        public ExcelUniformDataAttribute(string fileName, int worksheetNumber) : base(fileName, worksheetNumber)
        {
            ColumnTypes = new List<Type>() { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };
            ColumnDataLengths = new List<DataLength> { DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly };
            ColumnIndices = new List<int>() { 1, 2, 3, 4, 5 };
            ColumnToWriteResults = 6;
        }

        public ExcelUniformDataAttribute(string fileName, int[] worksheetNumbers) : base(fileName, worksheetNumbers)
        {
            ColumnTypes = new List<Type>() { typeof(double), typeof(double), typeof(double), typeof(double), typeof(double) };
            ColumnDataLengths = new List<DataLength> { DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly, DataLength.FirstLineOnly };
            ColumnIndices = new List<int>() { 1, 2, 3, 4, 5 };
            ColumnToWriteResults = 6;
        }


    }
}
